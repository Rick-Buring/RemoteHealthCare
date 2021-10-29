using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunicationObjects.DataObjects;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;

namespace DoktersApplicatie
{
    class HistoryData : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public double ElapsedTime { get; set; }
        public int DistanceTraveled { get; set; }
        public int AccWatt { get; set; }


        public HistoryValueTimeChart WattHistoryChart { get; set; }
        public HistoryValueTimeChart RpmHistoryChart { get; set; }
        public HistoryValueTimeChart BpmHistoryChart { get; set; }
        public HistoryValueTimeChart KmhHistoryChart { get; set; }

        public HistoryData(History history) => FillChartsAndData(GetHealthHistory(history));


        private void FillChartsAndData(List<HealthData> healthData)
        {
            List<HistoryValueTimeChart.ValueTime> watts = new List<HistoryValueTimeChart.ValueTime>();
            List<HistoryValueTimeChart.ValueTime> rpms = new List<HistoryValueTimeChart.ValueTime>();
            List<HistoryValueTimeChart.ValueTime> bpms = new List<HistoryValueTimeChart.ValueTime>();
            List<HistoryValueTimeChart.ValueTime> speeds = new List<HistoryValueTimeChart.ValueTime>();

            foreach (HealthData h in healthData)
            {
                watts.Add(new HistoryValueTimeChart.ValueTime(h.CurWatt, h.ElapsedTime));
                rpms.Add(new HistoryValueTimeChart.ValueTime(h.RPM, h.ElapsedTime));
                bpms.Add(new HistoryValueTimeChart.ValueTime(h.Heartbeat, h.ElapsedTime));
                speeds.Add(new HistoryValueTimeChart.ValueTime(h.Speed, h.ElapsedTime));
            }

            this.ElapsedTime = healthData[healthData.Count - 1].ElapsedTime;
            this.DistanceTraveled = healthData[healthData.Count - 1].DistanceTraveled;
            this.AccWatt = healthData[healthData.Count - 1].AccWatt;

            this.WattHistoryChart = new HistoryValueTimeChart(watts);
            this.RpmHistoryChart = new HistoryValueTimeChart(rpms);
            this.BpmHistoryChart = new HistoryValueTimeChart(bpms);
            this.KmhHistoryChart = new HistoryValueTimeChart(speeds);

        }

        private List<HealthData> GetHealthHistory(History history)
        {
            List<HealthData> HistoryData = new List<HealthData>();

            string[] array = history.clientHistory.Split("\n");

            for (int i = 0; i < array.Length - 1; i++)
            {
                string[] data = array[i].Split(".");
                int hb = int.Parse(data[0]);
                int rpm = int.Parse(data[1]);
                double speed = double.Parse(data[2]);
                int currWatt = int.Parse(data[3]);
                int accWatt = int.Parse(data[4]);
                double time = double.Parse(data[5]);
                int distance = 0;
                string d = data[6];
                if (d.Contains("\r"))
                {
                    distance = int.Parse(d.Substring(0, d.IndexOf("\r")));
                }
                else
                {
                    distance = int.Parse(d);
                }

                HealthData healthData = new HealthData()
                {
                    Heartbeat = hb,
                    RPM = rpm,
                    Speed = speed,
                    CurWatt = currWatt,
                    AccWatt = accWatt,
                    ElapsedTime = time,
                    DistanceTraveled = distance
                };
                HistoryData.Add(healthData);

            }

            return HistoryData;
        }

        public class HistoryValueTimeChart : INotifyPropertyChanged
        {


            public Func<double, string> Formatter { get; set; }
            public ChartValues<ValueTime> Values { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            public double From { get; set; }
            public double To { get; set; }

            private double maxValue { get; set; }
            private double minValue { get; set; }

            public int Step { get; set; }


            public HistoryValueTimeChart(List<ValueTime> chart)
            {
                var dayConfig = new CartesianMapper<ValueTime>()
                    .X(dayModel => dayModel.SecondsSinceStart)
                    .Y(dayModel => dayModel.Value);
                Charting.For<ValueTime>(dayConfig);
                this.Values = new ChartValues<ValueTime>(chart);

                this.minValue = chart[0].SecondsSinceStart;
                this.From = this.minValue;

                double lastTimeValue = chart[^1].SecondsSinceStart;
                if (30 < lastTimeValue)
                {
                    this.To = 30;
                }
                else
                {

                    this.To = lastTimeValue;
                }

                updateStep();

                this.maxValue = this.Values[^1].SecondsSinceStart;

                Formatter = value => TimeSpan.FromSeconds(value).ToString(@"mm\:ss");

            }

            private void updateStep()
            {
                this.Step = (int)(this.To - this.From) / 3;
            }


            public void OnMouseWheelScroll(MouseWheelEventArgs e)
            {
                Debug.WriteLine(e.Delta);
                double changeValue = e.Delta / 20.00;
                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    double ToChangeValue = this.To - changeValue / 10.0;
                    if (ToChangeValue < maxValue && ToChangeValue > 0 && ToChangeValue - this.From > 1)
                    {
                        this.To -= changeValue / 10.0;
                    }
                    else if (ToChangeValue < maxValue)
                    {
                        this.To = maxValue;
                    }
                    this.updateStep();
                }
                else
                {
                    double range = this.To - this.From;
                    if (this.To - changeValue < maxValue)
                    {
                        this.To -= changeValue;
                    }
                    else
                    {

                        this.To = maxValue;
                        this.From = maxValue - range;
                        return;

                    }

                    if (this.From - changeValue > 0)
                    {
                        this.From -= changeValue;
                    }
                    else
                    {
                        this.From = this.minValue;
                        this.To = this.minValue + range;
                    }

                }
            }


            public class ValueTime
            {
                public double SecondsSinceStart { get; set; }
                public double Value { get; set; }

                public ValueTime(double value, double elapsedTime)
                {
                    SecondsSinceStart = elapsedTime;
                    //Debug.WriteLine(SecondsSinceStart);
                    Value = value;
                }
            }
        }
    }
}
