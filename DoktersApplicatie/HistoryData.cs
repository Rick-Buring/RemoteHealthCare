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
        public HistoryValueTimeChart HistoryChart { get; set; }

        public HistoryData()
        {
            this.HistoryChart = new HistoryValueTimeChart();
        }

        public class HistoryValueTimeChart : INotifyPropertyChanged
        {
         
            
                public Func<double, string> Formatter { get; set; }
                public ChartValues<ValueTime> Values { get; set; }
                public DateTime time { get; set; } = DateTime.Now;

                public event PropertyChangedEventHandler PropertyChanged;

                public double MinValue { get; set; }
                public double MaxValue { get; set; }

                public double From { get; set; }
                public double To { get; set; }


                public HistoryValueTimeChart()
                {
                    var dayConfig = new CartesianMapper<ValueTime>()
                        .X(dayModel => dayModel.SecondsSinceStart)
                        .Y(dayModel => dayModel.Value);
                    this.Values = new ChartValues<ValueTime>();
                    Charting.For<ValueTime>(dayConfig);

                    this.MinValue = 0;
                    this.MaxValue = 200;
                    this.From = 0;
                    this.To = 30;

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

                private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
                {
                    var chart = (LiveCharts.Wpf.CartesianChart) sender;
                    var mouseCoordinate = e.GetPosition(chart);
                    var p = chart.ConvertToChartValues(mouseCoordinate);
                    Debug.WriteLine(p);
                }

                public void OnMouseWheelScroll(MouseWheelEventArgs e)
                {
                    Debug.WriteLine(e.Delta);
                    this.MaxValue -= e.Delta / 5.00;
                } 

                public void OnDragOver(Point mousePoint, DragEventArgs e)
                {
                    this.From += mousePoint.Y;
                    this.To += mousePoint.Y;

                    Debug.WriteLine(mousePoint.Y);

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
