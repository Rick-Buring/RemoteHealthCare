using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;

namespace DoktersApplicatie
{
    class Data
    {

        public ObservableCollection<Client> clients { get; set; }
        public ObservableCollection<Message> messages { get; set; }

        public Func<double, string> Formatter { get; set; }
        public SeriesCollection Series { get; set; }
        private static WattChart wattChart { get; set; }

        public Data()
        {
            clients = new ObservableCollection<Client>();
            messages = new ObservableCollection<Message>();

            clients.Add(new Client { Name = "Kapil Malhotra", Age = 30, BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, AccWatt = 400, SessionTime = 100, Resistance = 40 });
            clients.Add(new Client { Name = "Raj Kundra", Age = 34, BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, AccWatt = 400, SessionTime = 100, Resistance = 40 });
            clients.Add(new Client { Name = "Amitabh Bachan", Age = 80, BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, AccWatt = 400, SessionTime = 100, Resistance = 40 });
            clients.Add(new Client { Name = "Deepak Khanna", Age = 72, BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, AccWatt = 400, SessionTime = 100, Resistance = 40 });

         
                wattChart = new WattChart();
            
        }

    }

    public class WattChart : INotifyPropertyChanged
    {
        public Func<double, string> Formatter { get; set; }
        public ChartValues<DateModel> Values { get; set; }
        public DateTime time { get; set; } = DateTime.Now;

        public event PropertyChangedEventHandler PropertyChanged;
        
        
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        
        public double AxisUnit { get; set; }

        public WattChart()
        {
            var dayConfig = new CartesianMapper<DateModel>()
            .X(dayModel => dayModel.SecondsSinceStart)
            .Y(dayModel => dayModel.Value);

            this.MaxValue = 30.00;
            this.Values = new ChartValues<DateModel>();
            LiveCharts.Charting.For<DateModel>(dayConfig);

            AxisUnit = TimeSpan.TicksPerSecond;

            Random random = new Random();

            new Thread(async () => 
            {
                while (true)
                {
                    DateModel dayModel = new DateModel(random.NextDouble() * 400, time);
                    add(dayModel);

                    await Task.Delay(500);
                }
            }).Start();
            

            Formatter = value => new System.DateTime((long)(value * TimeSpan.FromSeconds(1).Ticks)).ToString("mm:ss");

        }

        public void add(DateModel dateModel)
        {
            if (dateModel.SecondsSinceStart > 30)
            {
                this.MinValue = dateModel.SecondsSinceStart - 30;
                this.MaxValue = dateModel.SecondsSinceStart;
            }
            if (Values.Count > 100) Values.RemoveAt(0);

            this.Values.Add(dateModel);

        }

     


        public class DateModel
        {
            public double SecondsSinceStart { get; set; }
            public double Value { get; set; }

            public DateModel(double value, DateTime startTime)
            {
                SecondsSinceStart = ((double)DateTime.Now.Ticks - startTime.Ticks) / TimeSpan.TicksPerSecond;
                Debug.WriteLine(SecondsSinceStart);
                Value = value;
            }
        }



    }

    public class Client
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public int BPM { get; set; }
        public int RPM { get; set; }
        public double KMH { get; set; }
        public int CurrWatt { get; set; }
        public double Distance { get; set; }
        public int AccWatt { get; set; }
        public int SessionTime { get; set; }
        public int Resistance { get; set; }

        public string AccWattText
        {
            get
            {
                return "Accumulated Watt: " + AccWatt;
            }
        }
        public string DistanceText
        {
            get
            {
                return "Distance: " + Distance;
            }
        }
        public string SessionTimeText
        {
            get
            {
                return "Session Time: " + SessionTime;
            }
        }
        public string ResistanceText
        {
            get
            {
                return "Resistance: " + Resistance;
            }
        }

    }




    public class Message
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Text { get; set; }
    }

}
