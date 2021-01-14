using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices.ComTypes;

namespace Mission_Assistant
{
    public class PerformanceData : INotifyPropertyChanged
    {
        [Column(name: "ALT")]
        public double alt { get; set; }
        [Column(name: "ClimbTime")]
        public double climbtime { get; set; }
        [Column(name: "ClimbDistance")]
        public double climbdist { get; set; }
        [Column(name: "ClimbFuel")]
        public double climbfuel { get; set; }
        [Column(name: "DescendTime")]
        public double descendtime { get; set; }
        [Column(name: "DescendDistance")]
        public double descenddist { get; set; }
        [Column(name: "DescendFuel")]
        public double descendfuel { get; set; }
        [Column(name: "Speed01")]
        public double spd1 { get; set; }
        [Column(name: "Speed02")]
        public double spd2 { get; set; }
        [Column(name: "Speed03")]
        public double spd3 { get; set; }
        [Column(name: "Speed04")]
        public double spd4 { get; set; }
        [Column(name: "Speed05")]
        public double spd5 { get; set; }
        [Column(name: "Aircraft")]
        public string aircraft { get; set; }

        public TimeSpan maxVal { get; } = TimeSpan.FromMinutes(3599);
        public TimeSpan? cTimeval
        {
            get { return TimeSpan.FromMinutes(climbtime); }
            set
            {
                climbtime = (value as TimeSpan?).Value.TotalMinutes;
            }
        }
        public TimeSpan? dTimeval
        {
            get { return TimeSpan.FromMinutes(descendtime); }
            set
            {
                descendtime = (value as TimeSpan?).Value.TotalMinutes;
            }
        }

        public PerformanceData Zero()
        {
            return new PerformanceData
            {
                alt = 0,
                climbtime = 0,
                climbdist = 0,
                climbfuel = 0,
                descendtime = 0,
                descenddist = 0,
                descendfuel = 0,
                spd1 = 0,
                spd2 = 0,
                spd3 = 0,
                spd4 = 0,
                spd5 = 0,
                aircraft = this.aircraft
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<PerformanceData> _performancedatas = new ObservableCollection<PerformanceData>();
        public ObservableCollection<PerformanceData> performanceDatas
        {
            get { return _performancedatas; }
            set
            {
                _performancedatas = value;
                PropertyChanged(this, new PropertyChangedEventArgs("performanceDatas"));
            }
        }
    }

    public class FuelStartData : INotifyPropertyChanged
    {
        [Column(name: "Label")]
        public string startlabel { get; set; }
        [Column(name: "Value")]
        public double startval { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<FuelStartData> _fuelstartdatas = new ObservableCollection<FuelStartData>();
        public ObservableCollection<FuelStartData> fuelStartDatas
        {
            get { return _fuelstartdatas; }
            set
            {
                _fuelstartdatas = value;
                PropertyChanged(this, new PropertyChangedEventArgs("fuelStartDatas"));
            }
        }
    }

    public class FuelReduceData : INotifyPropertyChanged
    {
        [Column(name: "Label")]
        public string reductionlabel { get; set; }
        [Column(name: "Value")]
        public double reductionval { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<FuelReduceData> _fuelreducedatas = new ObservableCollection<FuelReduceData>();
        public ObservableCollection<FuelReduceData> fuelReduceDatas
        {
            get { return _fuelreducedatas; }
            set
            {
                _fuelreducedatas = value;
                PropertyChanged(this, new PropertyChangedEventArgs("fuelReduceDatas"));
            }
        }
    }
}
