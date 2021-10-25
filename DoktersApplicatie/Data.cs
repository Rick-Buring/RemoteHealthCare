﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CommunicationObjects.DataObjects;
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

            clients.Add(new Client { Name = "Kapil Malhotra", BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, Distance = 2.5, AccWatt = 400, SessionTime = 100, Resistance = 50 });
            clients.Add(new Client { Name = "Raj Kundra", BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, Distance = 2.5, AccWatt = 400, SessionTime = 100, Resistance = 20 });
            clients.Add(new Client { Name = "Amitabh Bachan", BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, Distance = 2.5, AccWatt = 400, SessionTime = 100, Resistance = 30 });
            clients.Add(new Client { Name = "Deepak Khanna", BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, Distance = 2.5, AccWatt = 400, SessionTime = 100, Resistance = 40 });
        }

    }



    public class Client : INotifyPropertyChanged
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

        public class ValueTimeChart : INotifyPropertyChanged
        {
            public Func<double, string> Formatter { get; set; }
            public ChartValues<ValueTime> Values { get; set; }
            public DateTime time { get; set; } = DateTime.Now;

            public event PropertyChangedEventHandler PropertyChanged;

            public double MinValue { get; set; }
            public double MaxValue { get; set; }


            public ValueTimeChart()
            {
                var dayConfig = new CartesianMapper<ValueTime>()
                .X(dayModel => dayModel.SecondsSinceStart)
                .Y(dayModel => dayModel.Value);

                this.MaxValue = 30.00;
                this.Values = new ChartValues<ValueTime>();
                Charting.For<ValueTime>(dayConfig);

                Random random = new Random();

                new Thread(async () =>
                {
                    while (true)
                    {
                        ValueTime dayModel = new ValueTime(random.NextDouble() * 400, time);
                        add(dayModel);

                        await Task.Delay(500);
                    }
                }).Start();

                Formatter = value => new System.DateTime((long)(value * TimeSpan.FromSeconds(1).Ticks)).ToString("mm:ss");

            }

            public void add(ValueTime valueTime)
            {
                if (valueTime.SecondsSinceStart > 30)
                {
                    this.MinValue = valueTime.SecondsSinceStart - 30;
                    this.MaxValue = valueTime.SecondsSinceStart;
                }
                if (Values.Count > 65) Values.RemoveAt(0);

                this.Values.Add(valueTime);

            }

            public class ValueTime
            {
                public double SecondsSinceStart { get; set; }
                public double Value { get; set; }

                public ValueTime(double value, DateTime startTime)
                {
                    SecondsSinceStart = ((double)DateTime.Now.Ticks - startTime.Ticks) / TimeSpan.TicksPerSecond;
                    //Debug.WriteLine(SecondsSinceStart);
                    Value = value;
                }
            }
        }
    }
}
