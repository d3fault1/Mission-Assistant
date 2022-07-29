using Dapper;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows;

namespace MissionAssistant
{
    /// <summary>
    /// Interaction logic for MissionPicker.xaml
    /// </summary>
    public partial class MissionPicker : Window
    {
        public MissionPicker()
        {
            InitializeComponent();
            if (File.Exists(@".\test.db"))
            {
                using (IDbConnection cnn = new SQLiteConnection(@"Data Source=.\test.db;Version=3"))
                {
                    var output = cnn.Query<string>(@"SELECT DISTINCT Aircraft FROM 'Performance Data' ORDER BY Aircraft", new DynamicParameters());
                    aircraftListbx.ItemsSource = output.ToList();
                }
            }
        }

        private void aircraftListbx_Selectionchanged(object sender, RoutedEventArgs e)
        {
            selectAc.IsHitTestVisible = true;
        }

        private void selectAc_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
