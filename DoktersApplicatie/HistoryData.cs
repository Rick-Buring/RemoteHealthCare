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
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;

namespace DoktersApplicatie
{
    class HistoryData 
    {
        public HistoryValueTimeChart WattHistoryChart { get; set; }
        public HistoryValueTimeChart RpmHistoryChart { get; set; }
        public HistoryValueTimeChart BpmHistoryChart { get; set; }
        public HistoryValueTimeChart KmhHistoryChart { get; set; }

        public HistoryData()
        {
            this.WattHistoryChart = new HistoryValueTimeChart();
            this.RpmHistoryChart = new HistoryValueTimeChart();
            this.BpmHistoryChart = new HistoryValueTimeChart();
            this.KmhHistoryChart = new HistoryValueTimeChart();
        }

        public class HistoryValueTimeChart : INotifyPropertyChanged
        {
         
            
                public Func<double, string> Formatter { get; set; }
                public ChartValues<ValueTime> Values { get; set; }
                public DateTime time { get; set; } = DateTime.Now;

                public event PropertyChangedEventHandler PropertyChanged;

                public double From { get; set; }
                public double To { get; set; }

                private double maxValue { get; set; }

                public int Step { get; set; }


                public HistoryValueTimeChart()
                {
                    var dayConfig = new CartesianMapper<ValueTime>()
                        .X(dayModel => dayModel.SecondsSinceStart)
                        .Y(dayModel => dayModel.Value);
                    this.Values = new ChartValues<ValueTime>();
                    Charting.For<ValueTime>(dayConfig);
                    this.From = 0;
                    this.To = 30;

                    this.Step = 10;

                    this.maxValue = this.Values[^1].SecondsSinceStart;

                    Random random = new Random();

                    new Thread(async () =>
                    {
                        while (true)
                        {
                            ValueTime dayModel = new ValueTime(random.NextDouble() * 400, time);
                            Values.Add(dayModel);

                            await Task.Delay(500);
                        }
                    }).Start();

                    Formatter = value => new System.DateTime((long)(value * TimeSpan.FromSeconds(1).Ticks)).ToString("mm:ss");

                }

                private void updateStep()
                {
                    this.Step = (int) (this.To - this.From) / 3;
                }


                public void OnMouseWheelScroll(MouseWheelEventArgs e)
                {
                    Debug.WriteLine(e.Delta);
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        this.To -= e.Delta / 20.00;
                        this.updateStep();
                    }
                    else
                    {

                    double changeValue = e.Delta / 20.00;
                    
                    if (this.To - changeValue > 0 ) {
                        this.To -= e.Delta / 20.00;
                    } else {
                        this.To = 0;
                    }

                    if (this.From - changeValue < maxValue)
                    {
                        this.From -= e.Delta / 20.00;
                    } else {
                        this.From = maxValue;
                    }

                    }
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
