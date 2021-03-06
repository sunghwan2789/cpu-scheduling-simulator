﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SchedulerSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string dataFilename { get; set;}
        private Scheduler scheduler = new FirstComeFirstServed();
        private Data data = new Data();

        public MainWindow()
        {
            InitializeComponent();
            editor.ItemsSource = data.Processes;
            txtTimeQuantum.Text = data.TimeQuantum.ToString();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                dataFilename = dlg.FileName;
                var dp = new DataParser(dataFilename);
                data = dp.Parse();

                editor.ItemsSource = data.Processes;
                txtTimeQuantum.Text = data.TimeQuantum.ToString();

                RunSimulator();
            }

        }

        private void RunSimulator()
        {
            data.TimeQuantum = int.Parse(txtTimeQuantum.Text);
            var tab = (TabItem)tabControl.SelectedItem;
            switch (tab.Name)
            {
                case "fcfs":
                    scheduler = new FirstComeFirstServed();
                    break;
                case "sjf":
                    scheduler = new ShortestJobFirst();
                    break;
                case "srt":
                    scheduler = new ShortestRemainingTimeFirst();
                    break;
                case "npp":
                    scheduler = new NonPreemptivePriority();
                    break;
                case "pp":
                    scheduler = new PreemptivePriority();
                    break;
                case "hrn":
                    scheduler = new HighestResponseRatioNext();
                    break;
                case "rr":
                    scheduler = new RoundRobin
                    {
                        TimeQuantum = data.TimeQuantum,
                    };
                    break;
            }

            foreach (var p in data.Processes.OrderBy(i => i.ArrivalTime))
            {
                scheduler.Push(p);
            }

            canvaaas.DataContext = scheduler.GetResult();
            listView.ItemsSource = scheduler.GetFinalResult();
            lblAverage.Text = $@"평균 대기 시간: {scheduler.GetAverageWaitingTime()}
평균 반환 시간: {scheduler.GetAverageTurnaroundTime()}
평균 응답 시간: {scheduler.GetAverageResponseTime()}";
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RunSimulator();
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            RunSimulator();
        }
    }
}
