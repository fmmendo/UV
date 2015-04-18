using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace UV
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string uvLevel = "";
        public string UvLevel { get { return uvLevel; } set { uvLevel = value; OnPropertyChanged(); } }
        public int MinIndex { get; set; }
        public int MaxIndex { get; set; }
        private string description { get; set; }
        public string Description { get { return description; } set { description = value; OnPropertyChanged(); } }
        public ObservableCollection<IntKeyValue> SkinTypes { get; set; }
        public IntKeyValue SelectedSkinType { get; set; }
        public int SkinTimeInSun { get; set; }
        public double FinalMinTimeInSun { get; set; }
        public double FinalMaxTimeInSun { get; set; }
        public int SunscreenValue { get; set; }
        public ObservableCollection<StringKeyValue> LocationTypes { get; set; }
        public string SPF { get; set; }

        private const string MESSAGE_NONE = "No UV detected.";
        private const string MESSAGE_LOW = "Apply SPF 15 sunscreen. {0} {1}";
        private const string MESSAGE_MEDIUM = "Apply SPF 15 sunscreen every 2 hours; wear a hat. {0} {1}";
        private const string MESSAGE_HIGH = "Apply SPF 15 to 30 sunscreen every 2 hours; wear a hat and sunglasses. {0} {1}";
        private const string MESSAGE_VERYHIGH = "Apply SPF 30 sunscreen every 2 hours; wear a hat, sunglasses, and protective clothing. {0} {1}";
        private const string MESSAGE_EXTREME = "Apply SPF 30 sunscreen often; wear a hat, sunglasses, and protective clothing. Environmental factors may have greatly increased UV intensity, be extra careful. {0} {1}";
        private const string MESSAGE_EXTRA = "Reduce or avoid exposure during solar noon (between 11AM and 3PM).";
        private const string MESSAGE_BURN_TIME = "At current UV intensity it takes between {0:0} and {1:0} minutes to burn.";

        public List<StringKeyValue> SelectedLocationType = new List<StringKeyValue>();

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private CoreDispatcher Dispatcher;
        /// <summary> 
        /// Event handler for the PropertyChanged event. 
        /// </summary> 
        /// <param name="propertyName">The name of the property that has changed.</param> 
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = PropertyChanged;
            if (eventHandler != null)
            {
                try
                {
                    eventHandler(this, new PropertyChangedEventArgs(propertyName));
                }
                catch { }
            }
        }
        #endregion

        public MainViewModel()
        {
            SunscreenValue = 1;
            SkinTypes = new ObservableCollection<IntKeyValue>()
            { 
                new IntKeyValue {Key = 1, Value = 67},
                new IntKeyValue {Key = 2, Value = 100},
                new IntKeyValue {Key = 3, Value = 200},
                new IntKeyValue {Key = 4, Value = 300},
                new IntKeyValue {Key = 5, Value = 400},
                new IntKeyValue {Key = 6, Value = 500},
            };
            SelectedSkinType = SkinTypes[0];
            LocationTypes = new ObservableCollection<StringKeyValue>() {
                new StringKeyValue {Key = "Mountain (3000m)", Value = 0.5},
                new StringKeyValue {Key = "River", Value = 0.25},
                new StringKeyValue {Key = "Sand/Grass", Value = 0.2},
                new StringKeyValue {Key = "City", Value = 0.15},
                new StringKeyValue {Key = "Ocean", Value = 0.5},
                new StringKeyValue {Key = "Snow", Value = 0.85},
            };
        }

        public MainViewModel(CoreDispatcher Dispatcher)
            : this()
        {
            this.Dispatcher = Dispatcher;
        }

        public async void CalculateModifiedUvIndex()
        {
            if (MinIndex <= 0 || MaxIndex <= 0)
                return;

            if (SelectedSkinType == null || SelectedSkinType.Key < 1 || SelectedSkinType.Key > 6)
                return;

            if (SunscreenValue <= 0)
                SunscreenValue = 1;

            double modMinIndex = MinIndex;
            modMaxIndex = MaxIndex;
            foreach (var item in SelectedLocationType)
            {
                modMinIndex += modMinIndex * item.Value;
                modMaxIndex += modMaxIndex * item.Value;
            }

            FinalMinTimeInSun = SelectedSkinType.Value / modMinIndex;
            FinalMaxTimeInSun = SelectedSkinType.Value / modMaxIndex;

            FinalMinTimeInSun = FinalMinTimeInSun * SunscreenValue;
            FinalMaxTimeInSun = FinalMaxTimeInSun * SunscreenValue;


            //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High,() => 
            ////await this.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            //{

            //    GetDescription(modMaxIndex, burn);
            //});

            //OnPropertyChanged("UvLevel");
            //OnPropertyChanged("Description");
        }

        public string GetDescription()
        {

            string burn = String.Format(MESSAGE_BURN_TIME, FinalMaxTimeInSun, FinalMinTimeInSun);
            var d = string.Empty;
            if (modMaxIndex <= 0)
            {
                d = String.Format(MESSAGE_NONE);
                UvLevel = "None";
            }
            else if (modMaxIndex <= 2)
            {
                d = String.Format(MESSAGE_LOW, MESSAGE_EXTRA, burn);
                UvLevel = "Low";
            }
            else if (modMaxIndex <= 5)
            {
                d = String.Format(MESSAGE_MEDIUM, MESSAGE_EXTRA, burn);
                UvLevel = "Medium";
            }
            else if (modMaxIndex <= 7)
            {
                d = String.Format(MESSAGE_HIGH, MESSAGE_EXTRA, burn);
                UvLevel = "High";
            }
            else if (modMaxIndex <= 10)
            {
                d = String.Format(MESSAGE_VERYHIGH, MESSAGE_EXTRA, burn);
                UvLevel = "Very High";
            }
            else
            {
                d = String.Format(MESSAGE_EXTREME, MESSAGE_EXTRA, burn);
                UvLevel = "Extreme";
            }

            return d;
        }

        public double modMaxIndex { get; private set; }
    }


    public class StringKeyValue
    {
        public string Key { get; set; }
        public double Value { get; set; }
    }
    public class IntKeyValue
    {
        public int Key { get; set; }
        public int Value { get; set; }
    }
}
