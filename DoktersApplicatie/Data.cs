using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public Data()
        {
            clients = new ObservableCollection<Client>();
            messages = new ObservableCollection<Message>();

            clients.Add(new Client { Name = "Kapil Malhotra", Age = 30, BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, AccWatt = 400, SessionTime = 100, Resistance = 40 });
            clients.Add(new Client { Name = "Raj Kundra", Age = 34, BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, AccWatt = 400, SessionTime = 100, Resistance = 40 });
            clients.Add(new Client { Name = "Amitabh Bachan", Age = 80, BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, AccWatt = 400, SessionTime = 100, Resistance = 40 });
            clients.Add(new Client { Name = "Deepak Khanna", Age = 72, BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, AccWatt = 400, SessionTime = 100, Resistance = 40 });

            WattChart wattChart = new WattChart();
        }

    }

    public class WattChart
    {
        public Func<double, string> Formatter { get; set; }
        public SeriesCollection Series { get; set; }

        public WattChart()
        {
                var dayConfig =  new CartesianMapper<DateModel>()
                .X(dayModel => (double) dayModel.DateTime.Ticks/TimeSpan.FromHours(1).Ticks)
                .Y(dayModel => dayModel.Value);

            Series = new SeriesCollection(dayConfig);
            Series.Add(new LineSeries(new DateModel(7)));

        }

        public void add()
        {

        }

            public class DateModel
    {
        public System.DateTime DateTime { get; set; }
        public double Value { get; set; }

        public DateModel(double value)
        {
            DateTime = System.DateTime.Now;
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
