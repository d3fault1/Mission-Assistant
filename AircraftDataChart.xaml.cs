using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MissionAssistant
{
    /// <summary>
    /// Interaction logic for AircraftDataChart.xaml
    /// </summary>
    public partial class AircraftDataChart : Window
    {
        private PerformanceData pdata;
        private FuelStartData fsdata;
        private FuelReduceData frdata;
        private string performanceTable = "Performance Data", fuelTable = "Fuel Data", speedUnitTable = "Speed and Unit Data", aircraftName, opMode;
        private List<string> nameList;
        private bool isUnitsFixed = false;
        private string cache;
        public AircraftDataChart(List<string> param, string mode)
        {
            InitializeComponent();
            opMode = mode;
            if (mode == "edit") aircraftName = param[0];
            else nameList = param;
            pdata = new PerformanceData();
            fsdata = new FuelStartData();
            frdata = new FuelReduceData();
            performanceChart.Items.SortDescriptions.Add(new SortDescription(performanceChart.Columns[0].SortMemberPath, ListSortDirection.Ascending));
            performanceChart.Items.IsLiveSorting = true;
            performanceChart.DataContext = pdata;
            performanceChart.ItemsSource = pdata.performanceDatas;
            startFuelChart.DataContext = fsdata;
            startFuelChart.ItemsSource = fsdata.fuelStartDatas;
            reductionFuelChart.DataContext = frdata;
            reductionFuelChart.ItemsSource = frdata.fuelReduceDatas;
            if (!File.Exists(@".\test.db"))
            {
                using (IDbConnection conn = new SQLiteConnection(@"Data Source=.\test.db;Version=3"))
                {
                    conn.Execute($"CREATE TABLE '{performanceTable}' ('Aircraft' TEXT, 'ALT' REAL, 'ClimbTime' REAL, 'ClimbDistance' REAL, 'ClimbFuel' REAL, 'DescendTime' REAL, 'DescendDistance' REAL, 'DescendFuel' REAL, 'Speed01' REAL, 'Speed02' REAL, 'Speed03' REAL, 'Speed04' REAL, 'Speed05' REAL)", new DynamicParameters());
                    conn.Execute($"CREATE TABLE '{fuelTable}' ('Aircraft' TEXT, 'Type' TEXT, 'Label' TEXT, 'Value' REAL)", new DynamicParameters());
                    conn.Execute($"CREATE TABLE '{speedUnitTable}' ('Aircraft' TEXT, 'SpeedID' INTEGER NOT NULL UNIQUE, 'Value' REAL, 'Unit' TEXT, PRIMARY KEY(SpeedID AUTOINCREMENT))", new DynamicParameters());
                }
            }
            if (mode == "edit")
            {
                aircraftNameBx.Text = aircraftName;
                aircraftNameBx.IsReadOnly = true;
                using (IDbConnection conn = new SQLiteConnection(@"Data Source=.\test.db;Version=3"))
                {
                    var uOutput = conn.Query<String>($"SELECT Unit FROM '{speedUnitTable}' WHERE Aircraft='{aircraftName}' ORDER BY SpeedID", new DynamicParameters());
                    String[] units = uOutput.ToArray();
                    alt_units.SelectedValue = units[0];
                    dist_units.SelectedValue = units[1];
                    speed_units.SelectedValue = units[2];
                    fuel_units.SelectedValue = units[3];
                    lfft_units.SelectedValue = units[4];
                    var pOutput = conn.Query<PerformanceData>($"SELECT * FROM '{performanceTable}' WHERE Aircraft='{aircraftName}' ORDER BY ALT", new DynamicParameters());
                    pdata.performanceDatas.Clear();
                    foreach (PerformanceData info in pOutput)
                    {
                        pdata.performanceDatas.Add(info);
                    }
                    var fsOutput = conn.Query<FuelStartData>($"SELECT Label, Value FROM '{fuelTable}' WHERE Aircraft='{aircraftName}' AND Type='Starting' ORDER BY Label", new DynamicParameters());
                    fsdata.fuelStartDatas.Clear();
                    foreach (FuelStartData info in fsOutput)
                    {
                        fsdata.fuelStartDatas.Add(info);
                    }
                    var frOutput = conn.Query<FuelReduceData>($"SELECT Label, Value FROM '{fuelTable}' WHERE Aircraft='{aircraftName}' AND Type='Reduction' ORDER BY Label", new DynamicParameters());
                    frdata.fuelReduceDatas.Clear();
                    foreach (FuelReduceData info in frOutput)
                    {
                        frdata.fuelReduceDatas.Add(info);
                    }
                    var dOutput = conn.Query<Double>($"SELECT Value FROM '{speedUnitTable}' WHERE Aircraft='{aircraftName}' ORDER BY SpeedID", new DynamicParameters());
                    Double[] columnNames = dOutput.ToArray();
                    if (columnNames[0] != 0) dataspd1.Text = columnNames[0].ToString();
                    if (columnNames[1] != 0) dataspd2.Text = columnNames[1].ToString();
                    if (columnNames[2] != 0) dataspd3.Text = columnNames[2].ToString();
                    if (columnNames[3] != 0) dataspd4.Text = columnNames[3].ToString();
                    if (columnNames[4] != 0) dataspd5.Text = columnNames[4].ToString();
                }
            }
        }

        private void changeUnits(object sender, SelectionChangedEventArgs e)
        {
            if (performanceChart != null && startFuelChart != null && reductionFuelChart != null)
            {
                isUnitsFixed = false;
                default_unitsBtn.Content = "Fix Units";
                default_unitsBtn.IsEnabled = true;
            }
            switch ((sender as ComboBox).Name)
            {
                case "alt_units":
                    if (performanceChart != null && startFuelChart != null && reductionFuelChart != null)
                    {
                        foreach (PerformanceData param in pdata.performanceDatas)
                        {
                            param.alt = DataConverters.LengthUnits(param.alt, alt_units.Text, alt_units.SelectedValue.ToString());
                        }
                        performanceChart.Items.Refresh();
                    }
                    break;
                case "dist_units":
                    if (performanceChart != null && startFuelChart != null && reductionFuelChart != null)
                    {
                        foreach (PerformanceData param in pdata.performanceDatas)
                        {
                            param.climbdist = DataConverters.LengthUnits(param.climbdist, dist_units.Text, dist_units.SelectedValue.ToString());
                            param.descenddist = DataConverters.LengthUnits(param.descenddist, dist_units.Text, dist_units.SelectedValue.ToString());
                        }
                        performanceChart.Items.Refresh();
                    }
                    break;
                case "speed_units":
                    if (performanceChart != null && startFuelChart != null && reductionFuelChart != null)
                    {
                        if (!String.IsNullOrEmpty(dataspd1.Text)) dataspd1.Text = DataConverters.SpeedUnits(Convert.ToDouble(dataspd1.Text), speed_units.Text.Split('-')[1], speed_units.SelectedValue.ToString().Split('-')[1]).ToString();
                        if (!String.IsNullOrEmpty(dataspd2.Text)) dataspd2.Text = DataConverters.SpeedUnits(Convert.ToDouble(dataspd2.Text), speed_units.Text.Split('-')[1], speed_units.SelectedValue.ToString().Split('-')[1]).ToString();
                        if (!String.IsNullOrEmpty(dataspd3.Text)) dataspd3.Text = DataConverters.SpeedUnits(Convert.ToDouble(dataspd3.Text), speed_units.Text.Split('-')[1], speed_units.SelectedValue.ToString().Split('-')[1]).ToString();
                        if (!String.IsNullOrEmpty(dataspd4.Text)) dataspd4.Text = DataConverters.SpeedUnits(Convert.ToDouble(dataspd4.Text), speed_units.Text.Split('-')[1], speed_units.SelectedValue.ToString().Split('-')[1]).ToString();
                        if (!String.IsNullOrEmpty(dataspd5.Text)) dataspd5.Text = DataConverters.SpeedUnits(Convert.ToDouble(dataspd5.Text), speed_units.Text.Split('-')[1], speed_units.SelectedValue.ToString().Split('-')[1]).ToString();
                    }
                    break;
                case "fuel_units":
                    if (performanceChart != null && startFuelChart != null && reductionFuelChart != null)
                    {
                        foreach (PerformanceData param in pdata.performanceDatas)
                        {
                            param.climbfuel = DataConverters.MassUnits(param.climbfuel, fuel_units.Text, fuel_units.SelectedValue.ToString());
                            param.descendfuel = DataConverters.MassUnits(param.descendfuel, fuel_units.Text, fuel_units.SelectedValue.ToString());
                        }
                        foreach (FuelStartData param in fsdata.fuelStartDatas)
                        {
                            param.startval = DataConverters.MassUnits(param.startval, fuel_units.Text, fuel_units.SelectedValue.ToString());
                        }
                        foreach (FuelReduceData param in frdata.fuelReduceDatas)
                        {
                            param.reductionval = DataConverters.MassUnits(param.reductionval, fuel_units.Text, fuel_units.SelectedValue.ToString());
                        }
                        performanceChart.Items.Refresh();
                        startFuelChart.Items.Refresh();
                        reductionFuelChart.Items.Refresh();
                    }
                    break;
                case "lfft_units":
                    if (performanceChart != null && startFuelChart != null && reductionFuelChart != null)
                    {
                        if (pdata.performanceDatas.Count != 0)
                        {
                            if (MessageBox.Show($"Changing This Unit Will Result in Resetting All the Existing {Environment.NewLine} Value Corresponding to This Unit {Environment.NewLine} {Environment.NewLine} Are you sure you want to proceed?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                            {
                                lfft_units.SelectionChanged -= changeUnits;
                                lfft_units.SelectedValue = lfft_units.Text;
                                lfft_units.SelectionChanged += changeUnits;
                                return;
                            }
                        }
                        foreach (PerformanceData param in pdata.performanceDatas)
                        {
                            param.spd1 = DataConverters.ConsumptionUnits(param.spd1, lfft_units.Text.Replace(" ", String.Empty), lfft_units.SelectedValue.ToString().Replace(" ", String.Empty));
                            param.spd2 = DataConverters.ConsumptionUnits(param.spd2, lfft_units.Text.Replace(" ", String.Empty), lfft_units.SelectedValue.ToString().Replace(" ", String.Empty));
                            param.spd3 = DataConverters.ConsumptionUnits(param.spd3, lfft_units.Text.Replace(" ", String.Empty), lfft_units.SelectedValue.ToString().Replace(" ", String.Empty));
                            param.spd4 = DataConverters.ConsumptionUnits(param.spd4, lfft_units.Text.Replace(" ", String.Empty), lfft_units.SelectedValue.ToString().Replace(" ", String.Empty));
                            param.spd5 = DataConverters.ConsumptionUnits(param.spd5, lfft_units.Text.Replace(" ", String.Empty), lfft_units.SelectedValue.ToString().Replace(" ", String.Empty));
                        }
                        performanceChart.Items.Refresh();
                    }
                    break;
                default:
                    return;
            }
        }

        private void numericFilter(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.OemPeriod || e.Key == Key.Decimal) && (sender as TextBox).Text.IndexOf(".") != -1) e.Handled = true;
            else if ((e.Key < Key.D0 || e.Key > Key.D9) && (e.Key < Key.NumPad0 || e.Key > Key.NumPad9) && (e.Key != Key.OemPeriod && e.Key != Key.Decimal)) e.Handled = true;
        }

        private void lockControls(object sender, DataGridBeginningEditEventArgs e)
        {
            if ((sender as DataGrid).CurrentColumn.GetCellContent(e.Row) is TextBlock)
            {
                TextBlock t = (sender as DataGrid).CurrentColumn.GetCellContent(e.Row) as TextBlock;
                cache = t.Text;
            }
            alt_units.IsEnabled = false;
            dist_units.IsEnabled = false;
            speed_units.IsEnabled = false;
            fuel_units.IsEnabled = false;
            lfft_units.IsEnabled = false;
            saveBtn.IsEnabled = false;
        }

        private void fixUnits(object sender, RoutedEventArgs e)
        {
            isUnitsFixed = true;
            default_unitsBtn.Content = "Units Fixed";
            default_unitsBtn.IsEnabled = false;
        }

        private bool isValid(DataGrid parent)
        {
            if (Validation.GetHasError(parent)) return false;
            else
            {
                for (int i = 0; i < parent.ItemContainerGenerator.Items.Count; i++)
                {
                    DataGridRow row = (DataGridRow)parent.ItemContainerGenerator.ContainerFromIndex(i);
                    if (row != null && Validation.GetHasError(row)) return false;
                }
                return true;
            }
        }

        private void applyDataValidation(object sender, DataGridCellEditEndingEventArgs e)
        {
            (sender as DataGrid).CellEditEnding -= applyDataValidation;
            (sender as DataGrid).CommitEdit();
            alt_units.IsEnabled = true;
            dist_units.IsEnabled = true;
            speed_units.IsEnabled = true;
            fuel_units.IsEnabled = true;
            lfft_units.IsEnabled = true;
            if (!isValid(sender as DataGrid))
            {
                if (e.EditingElement is TextBox)
                {
                    (e.EditingElement as TextBox).Text = cache;
                }
            }
            else if (e.EditingElement is TextBox)
            {
                if (String.IsNullOrEmpty((e.EditingElement as TextBox).Text))
                {
                    if (sender == startFuelChart)
                    {
                        (e.Row.Item as FuelStartData).startlabel = "New";
                    }
                    else if (sender == reductionFuelChart)
                    {
                        (e.Row.Item as FuelReduceData).reductionlabel = "New";
                    }
                }
            }
            if (isValid(performanceChart) && isValid(startFuelChart) && isValid(reductionFuelChart)) saveBtn.IsEnabled = true;
            else saveBtn.IsEnabled = false;
            (sender as DataGrid).CellEditEnding += applyDataValidation;
        }

        private void saveData(object sender, RoutedEventArgs e)
        {
            if (!isUnitsFixed)
            {
                MessageBox.Show("Please Set Default Units for Aircraft Data Parameters.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (pdata.performanceDatas.Count == 0)
            {
                MessageBox.Show("Performance Data Chart is Empty. Please Fill.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (fsdata.fuelStartDatas.Count == 0)
            {
                MessageBox.Show("Starting Fuel Chart is Empty. Please Fill.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (frdata.fuelReduceDatas.Count == 0)
            {
                MessageBox.Show("Reduction Fuel Chart is Empty. Please Fill.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (opMode == "add")
            {
                if (aircraftNameBx.Text == "")
                {
                    MessageBox.Show("Aircraft Name Invalid!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (nameList.Contains(aircraftNameBx.Text))
                {
                    MessageBox.Show("Aircraft Already Exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                aircraftName = aircraftNameBx.Text;
            }
            using (IDbConnection conn = new SQLiteConnection(@"Data Source=.\test.db;Version=3"))
            {
                conn.Execute($"DELETE FROM '{performanceTable}' WHERE Aircraft='{aircraftName}'", new DynamicParameters());
                conn.Execute($"DELETE FROM '{fuelTable}' WHERE Aircraft='{aircraftName}'", new DynamicParameters());
                conn.Execute($"DELETE FROM '{speedUnitTable}' WHERE Aircraft='{aircraftName}'", new DynamicParameters());
                foreach (PerformanceData input in pdata.performanceDatas)
                {
                    conn.Execute($"INSERT INTO '{performanceTable}' ('Aircraft', 'Alt', 'ClimbTime', 'ClimbDistance', 'ClimbFuel', 'DescendTime', 'DescendDistance', 'DescendFuel', 'Speed01', 'Speed02', 'Speed03', 'Speed04', 'Speed05') VALUES('{aircraftName}', @alt, @climbtime, @climbdist , @climbfuel, @descendtime, @descenddist, @descendfuel, @spd1, @spd2, @spd3, @spd4, @spd5)", input);
                }
                foreach (FuelStartData input in fsdata.fuelStartDatas)
                {
                    conn.Execute($"INSERT INTO '{fuelTable}' ('Aircraft', 'Type', 'Label', 'Value') VALUES ('{aircraftName}', 'Starting', @startlabel, @startval)", input);
                }
                foreach (FuelReduceData input in frdata.fuelReduceDatas)
                {
                    conn.Execute($"INSERT INTO '{fuelTable}' ('Aircraft', 'Type', 'Label', 'Value') VALUES ('{aircraftName}', 'Reduction', @reductionlabel, @reductionval)", input);
                }
                conn.Execute($"INSERT INTO '{speedUnitTable}' ('Aircraft', 'Value', 'Unit') VALUES ('{aircraftName}', '{Convert.ToDouble(String.IsNullOrEmpty(dataspd1.Text) ? "0" : dataspd1.Text)}', '{alt_units.SelectedValue.ToString()}')", new DynamicParameters());
                conn.Execute($"INSERT INTO '{speedUnitTable}' ('Aircraft', 'Value', 'Unit') VALUES ('{aircraftName}', '{Convert.ToDouble(String.IsNullOrEmpty(dataspd2.Text) ? "0" : dataspd2.Text)}', '{dist_units.SelectedValue.ToString()}')", new DynamicParameters());
                conn.Execute($"INSERT INTO '{speedUnitTable}' ('Aircraft', 'Value', 'Unit') VALUES ('{aircraftName}', '{Convert.ToDouble(String.IsNullOrEmpty(dataspd3.Text) ? "0" : dataspd3.Text)}', '{speed_units.SelectedValue.ToString()}')", new DynamicParameters());
                conn.Execute($"INSERT INTO '{speedUnitTable}' ('Aircraft', 'Value', 'Unit') VALUES ('{aircraftName}', '{Convert.ToDouble(String.IsNullOrEmpty(dataspd4.Text) ? "0" : dataspd4.Text)}', '{fuel_units.SelectedValue.ToString()}')", new DynamicParameters());
                conn.Execute($"INSERT INTO '{speedUnitTable}' ('Aircraft', 'Value', 'Unit') VALUES ('{aircraftName}', '{Convert.ToDouble(String.IsNullOrEmpty(dataspd5.Text) ? "0" : dataspd5.Text)}', '{lfft_units.SelectedValue.ToString()}')", new DynamicParameters());
            }
            DialogResult = true;
            Close();
        }
    }

    public class StringValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (String.IsNullOrEmpty(value.ToString()) || String.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(false, "Value Cannot be Null or Empty");
            }
            else return new ValidationResult(true, "Valid");
        }
    }
}
