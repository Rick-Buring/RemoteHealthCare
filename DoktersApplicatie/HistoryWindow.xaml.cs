using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DoktersApplicatie
{
    public partial class HistoryWindow : Window
    {
        public HistoryWindow()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var chart = (LiveCharts.Wpf.CartesianChart) sender;
            HistoryData.HistoryValueTimeChart currentChart = (HistoryData.HistoryValueTimeChart) chart.DataContext;

            currentChart.OnMouseWheelScroll(e);
        }


    }
}
