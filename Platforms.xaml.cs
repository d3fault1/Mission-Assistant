using Dapper;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Mission_Assistant
{
    /// <summary>
    /// Interaction logic for PlatformsStreams.xaml
    /// </summary>
    public partial class Platforms : Window
    {
        bool dbOk = false;
        public Platforms()
        {
            InitializeComponent();
            if (!File.Exists(@".\test.db")) dbOk = false;
            else dbOk = true;
            platformListbx.Items.Add("Platform");
        }

        private void btnOperations(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Name)
            {
                case "addPlatforms":
                    break;
                case "modifyData":
                    AircraftDataChart editData = new AircraftDataChart(aircraftListbx.SelectedItems.OfType<string>().ToList(), "edit");
                    editData.ShowDialog();
                    break;
                case "addData":
                    AircraftDataChart addData = new AircraftDataChart(aircraftListbx.Items.OfType<string>().ToList(), "add");
                    addData.Closed += refreshList;
                    addData.ShowDialog();
                    break;
                default:
                    return;
            }
        }

        private void refreshList(object sender, EventArgs e)
        {
            using (IDbConnection cnn = new SQLiteConnection(@"Data Source=.\test.db;Version=3"))
            {
                var output = cnn.Query<string>(@"SELECT DISTINCT Aircraft FROM 'Performance Data' ORDER BY Aircraft", new DynamicParameters());
                aircraftListbx.ItemsSource = output.ToList();
            }
        }

        private void selectItem(object sender, SelectionChangedEventArgs e)
        {
            switch ((sender as ListBox).Name)
            {
                case "platformListbx":
                    streamListbx.Items.Add("Stream");
                    break;
                case "streamListbx":
                    if (!dbOk)
                    {
                        addData.IsHitTestVisible = true;
                        return;
                    }
                    using (IDbConnection cnn = new SQLiteConnection(@"Data Source=.\test.db;Version=3"))
                    {
                        var output = cnn.Query<string>(@"SELECT DISTINCT Aircraft FROM 'Performance Data' ORDER BY Aircraft", new DynamicParameters());
                        aircraftListbx.ItemsSource = output.ToList();
                    }
                    addData.IsHitTestVisible = true;
                    break;
                case "aircraftListbx":
                    modifyData.IsHitTestVisible = true;
                    break;
                default:
                    return;
            }
        }
    }
}
