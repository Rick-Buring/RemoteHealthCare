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
using DoktersApplicatie.ViewModels;
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

        private HomeVM.RemovedClients removeClients;

        public Data(HomeVM.RemovedClients removeClients)
        {
            this.removeClients = removeClients;
            clients = new ObservableCollection<Client>();
            messages = new ObservableCollection<Message>();
            this.dispatcher = Dispatcher.CurrentDispatcher;
            this.clientsDictionary = new Dictionary<string, Client>();
            }

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
            List<Client> removedClients = new List<Client>();

            foreach(Client c in clients)
            {
                if (!client.Contains(c))
                {
                    this.clientsDictionary.Remove(c.Name);
                    removedClients.Add(c);

                }
            }
            this.dispatcher.Invoke(() =>
            {
                foreach (Client c in removedClients)
                {
                    this.clients.Remove(c);
                    Debug.WriteLine($"Removing client: {c.Name}");
                }
            });
            if (this.removeClients != null)
            {
                this.removeClients(removedClients);
            }
        }


        public void UpdateClient(string client, HealthData healthData) {

            clientsDictionary[client].Update(healthData);
        }

    }

    public class Client : IComparable<string>, INotifyPropertyChanged, IEquatable<Client>
    {
        public string Name { get; set; }
        public int BPM { get; set; }
        public int RPM { get; set; }
        public double KMH { get; set; }
        public int CurrWatt { get; set; }
        public double Distance { get; set; }
        public int AccWatt { get; set; }
        public double SessionTime { get; set; }
        public int Resistance { get; set; }
        public bool InSession { get; set; }
        public string StartStopSessionText { get; set; } = "Start Session";

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

        public void StartStopSession()
        {

            if (StartStopSessionText.Equals("Start Session"))
            {

                StartStopSessionText = "Stop Session";
                this.InSession = true;

            }
            else
            {
                StartStopSessionText = "Start Session";
                this.InSession = false;
            }

        }

        public bool Equals(Client other)
        {
            if (this.Name == other.Name)
            {
                return true;
            }
            else
            {
                return false;
            }
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
                this.MinValue = 0;
                this.MaxValue = 30;
            }

            public void add(double value, double elapsedTime)
            {
                ValueTime valueTime = new ValueTime(value, elapsedTime);
                if (elapsedTime > 30)
                {
                    this.MinValue = elapsedTime - 30;
                    this.MaxValue = elapsedTime;
                }
                if (Values.Count > 35) Values.RemoveAt(0);

                this.Values.Add(valueTime);

            }

            public class ValueTime
            {
                public double SecondsSinceStart { get; set; }
                public double Value { get; set; }

                public ValueTime(double value, double startTime)
                {
                    SecondsSinceStart = startTime;
                    Value = value;
                }
            }
        }
    }
}
