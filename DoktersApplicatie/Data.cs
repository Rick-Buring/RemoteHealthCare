using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommunicationObjects.DataObjects;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Prism.Mvvm;

namespace DoktersApplicatie
{
    public class Data : BindableBase, INotifyPropertyChanged
    {

        public ObservableCollection<Client> clients { get; set; }
        public ObservableCollection<Message> messages { get; set; }

        private Dictionary<string, Client> clientsDictionary { get; }

        public Func<double, string> Formatter { get; set; }
        public SeriesCollection Series { get; set; }

        private Dispatcher dispatcher;

        public Data()
        {
            clients = new ObservableCollection<Client>();
            messages = new ObservableCollection<Message>();
            this.dispatcher = Dispatcher.CurrentDispatcher;
            this.clientsDictionary = new Dictionary<string, Client>();
            //clients.Add(new Client { Name = "Kapil Malhotra", BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, Distance = 2.5, AccWatt = 400, SessionTime = 100, Resistance = 50 });
            //clients.Add(new Client { Name = "Raj Kundra", BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, Distance = 2.5, AccWatt = 400, SessionTime = 100, Resistance = 20 });
            //clients.Add(new Client { Name = "Amitabh Bachan", BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, Distance = 2.5, AccWatt = 400, SessionTime = 100, Resistance = 30 });
            //clients.Add(new Client ("Henk") { BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, Distance = 2.5, AccWatt = 400, SessionTime = 100, Resistance = 40 });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void AddClients(List<Client> client)
        {
            
            
            foreach (Client c in client)
			{

                if (!clients.Contains(c))
				{
                    this.clientsDictionary.Add(c.Name, c);

                    this.dispatcher.Invoke(() =>
                    {
                        this.clients.Add(c);
                        Debug.WriteLine($"Adding new client: {c.Name}");
                    });
                }
			}

            foreach(Client c in clients)
			{
                if (!client.Contains(c))
				{
                    this.clientsDictionary.Remove(c.Name);
                    this.dispatcher.Invoke(() =>
                    {
                        this.clients.Remove(c);
                        Debug.WriteLine($"Removing client: {c.Name}");
                    });
				}
			}

           
        }


        public void UpdateClient(string client, HealthData healthData) {

            clientsDictionary[client].Update(healthData);
        }

    }

    public class Client : IComparable<string>, INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int BPM { get; set; }
        public int RPM { get; set; }
        public double KMH { get; set; }
        public int CurrWatt { get; set; }
        public double Distance { get; set; }
        public int AccWatt { get; set; }
        public int SessionTime { get; set; }
        public int Resistance { get; set; }

        public int TempResistance { get; set; } = 50;

        public ValueTimeChart WattChart { get; set; } = new ValueTimeChart();
        public ValueTimeChart BpmChart { get; set; } = new ValueTimeChart();
        public ValueTimeChart RpmChart { get; set; } = new ValueTimeChart();
        public ValueTimeChart KmhChart { get; set; } = new ValueTimeChart();

        public event PropertyChangedEventHandler PropertyChanged;

        public Client(string name) => this.Name = name;

        public void Update (HealthData healthData) {
            BPM = healthData.Heartbeat;
            RPM = healthData.RPM;
            KMH = Math.Round(healthData.Speed, 2);
            CurrWatt = healthData.CurWatt;
            AccWatt = healthData.AccWatt;
            Distance = healthData.DistanceTraveled;
            SessionTime = healthData.ElapsedTime;
            if (SessionTime == 0)
            {
                WattChart.resetChart();
                BpmChart.resetChart();
                RpmChart.resetChart();
                KmhChart.resetChart();
            }

            WattChart.add(CurrWatt, SessionTime);
            BpmChart.add(BPM, SessionTime);
            RpmChart.add(RPM, SessionTime);
            KmhChart.add(KMH, SessionTime);
        }

        public int CompareTo(string other)
        {
            return this.Name.CompareTo(other);
        }

        public class ValueTimeChart : INotifyPropertyChanged
        {
            public Func<double, string> Formatter { get; set; }
            public ChartValues<ValueTime> Values { get; set; }
            public DateTime time { get; set; } 

            public event PropertyChangedEventHandler PropertyChanged;

            public double MinValue { get; set; }
            public double MaxValue { get; set; }


            public ValueTimeChart()
            {
                time = DateTime.Now;
                var dayConfig = new CartesianMapper<ValueTime>()
                .X(dayModel => dayModel.SecondsSinceStart)
                .Y(dayModel => dayModel.Value);

                this.MaxValue = 30.00;
                this.Values = new ChartValues<ValueTime>();
                Charting.For<ValueTime>(dayConfig);

                Formatter = value => new System.DateTime((long)(value * TimeSpan.FromSeconds(1).Ticks)).ToString("mm:ss");

            }

            public void resetChart()
            {
                this.Values.Clear();
                time = DateTime.Now;
            }

            public void add(double value, int elapsedTime)
            {
                ValueTime valueTime = new ValueTime(value, elapsedTime);
                if (elapsedTime > 30)
                {
                    this.MinValue = elapsedTime - 30;
                    this.MaxValue = elapsedTime;
                }
                if (Values.Count > 305) Values.RemoveAt(0);

                this.Values.Add(valueTime);

            }

            public class ValueTime
            {
                public double SecondsSinceStart { get; set; }
                public double Value { get; set; }

                public ValueTime(double value, int startTime)
                {
                    SecondsSinceStart = (double) startTime;
                    //Debug.WriteLine(SecondsSinceStart);
                    Value = value;
                }
            }
        }
    }
}
