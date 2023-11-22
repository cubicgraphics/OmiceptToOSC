using HP.Omnicept.Messaging.Messages;
using OmiceptToOSC.OscParameter;
using OmiceptToOSC.Sensors;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace OmiceptToOSC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly OmniceptSensorProvider sensorProvider;
        public MainWindow()
        {
            InitializeComponent();

            SensorType[] UseableMessages =
            {
                new HeartRateOsc(MessageTypes.ABI_MESSAGE_HEART_RATE, "Heart rate", "OSC thing"),
                new HeartRateVariabilityOsc(MessageTypes.ABI_MESSAGE_HEART_RATE_VARIABILITY, "Heart rate variability", "OSC thing1", "OSC thing1"),
                new CognitiveLoadOsc(MessageTypes.ABI_MESSAGE_COGNITIVE_LOAD, "Cognitive load", "OSC thing1", "OSC thing2"),
            };

            sensorProvider = new OmniceptSensorProvider(UseableMessages);

            SensorOutListBox.Items.Insert(0, CreateNewGroupBoxForHeartRate((HeartRateOsc)sensorProvider.UseableMessages[0]));
            SensorOutListBox.Items.Insert(1, CreateNewGroupBoxForHeartRateVariability((HeartRateVariabilityOsc)sensorProvider.UseableMessages[1]));
            SensorOutListBox.Items.Insert(2, CreateNewGroupBoxForCognitiveLoad((CognitiveLoadOsc)sensorProvider.UseableMessages[2]));
            CreateToggles(sensorProvider.UseableMessages);
        }

        private void CreateToggles(SensorType[] Sensors)
        {
            for (int i = 0; i < Sensors.Length; i++)
            {
                TogglesGrid.Children.Add(CreateToggle(Sensors[i], i));
            }
        }

        private CheckBox CreateToggle(SensorType sensor, int Index)
        {
            CheckBox Toggle = new()
            {
                Content = sensor.SensorName,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 16 * Index, 0, 0),
            };

            Binding CheckBoxBinding = new()
            {
                Source = sensor,
                Path = new PropertyPath("IsEnabled"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(Toggle, CheckBox.IsCheckedProperty, CheckBoxBinding);
            return Toggle;
        }


        private Grid CreateOscEntry(string EntryName, IOscParameter param, int RowIndex)
        {
            //120 80 50 and then param box
            
            var ParamName = new Label()
            {
                Content = EntryName,
                Height = 20,
                Width = 120,
                HorizontalAlignment = HorizontalAlignment.Left,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0, 0, 0, 0),
            };
            var paramType = new Label()
            {
                Content = param.GetParameterType().ToString(),
                Height = 20,
                Width = 80,
                Margin = new Thickness(120, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0, 0, 0, 0),
            };
            var ValueToUpdate = new TextBox
            {
                Height = 20,
                Width = 50,
                Margin = new Thickness(200, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0, 0, 0, 0),
            };
            var paramOscEndpoint = new TextBox()
            {
                Height = 20,
                Width = 340,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(250, 0, 0, 0),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            var grid = new Grid()
            {
                Margin = new Thickness(0, RowIndex*20, 0, 0),
            };

            Binding EndpointBinding = new()
            {
                Source = param,
                Path = new PropertyPath("OscEndpoint"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(paramOscEndpoint, TextBox.TextProperty, EndpointBinding);

            Binding ParamBinding = new()
            {
                Source = param,
                Path = new PropertyPath("Value"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(ValueToUpdate, TextBox.TextProperty, ParamBinding);

            grid.Children.Add(ParamName);
            grid.Children.Add(paramType);
            grid.Children.Add(ValueToUpdate);
            grid.Children.Add(paramOscEndpoint);


            return grid;
        }

        private GroupBox CreateNewGroupBoxForHeartRate(HeartRateOsc sensor)
        {
            var grid = new Grid();
            var box = CreateGroupBox(50, sensor);
            box.Content = grid;
            grid.Children.Add(CreateOscEntry("Heart Rate", sensor.HeartRate, 0));
            return box;
        }
        private GroupBox CreateNewGroupBoxForHeartRateVariability(HeartRateVariabilityOsc sensor)
        {
            var grid = new Grid();
            var box = CreateGroupBox(75, sensor);
            box.Content = grid;
            grid.Children.Add(CreateOscEntry("Rmssd", sensor.Rmssd, 0));
            grid.Children.Add(CreateOscEntry("Sdnn", sensor.Sdnn, 1));
            return box;
        }
        private GroupBox CreateNewGroupBoxForCognitiveLoad(CognitiveLoadOsc sensor)
        {
            var grid = new Grid();
            var box = CreateGroupBox(75, sensor);
            box.Content = grid;
            grid.Children.Add(CreateOscEntry("CognitiveLoadValue", sensor.CognitiveLoadValue, 0));
            grid.Children.Add(CreateOscEntry("StandardDeviation", sensor.StandardDeviation, 1));
            return box;
        }

        private GroupBox CreateGroupBox(int Height, SensorType sensor)
        {
            var box = new GroupBox()
            {
                Header = sensor.SensorName,
                Height = Height,
                Width = 600,
                Visibility = Visibility.Collapsed,
            };

            var VisibilityCallback = () =>
            {
                if (ShowDisabledSensorsCheckBox.IsChecked != true)
                {
                    box.IsEnabled = true;
                    box.Visibility = sensor.IsEnabled ? Visibility.Visible : Visibility.Collapsed;
                    return;
                }
                box.Visibility = Visibility.Visible;
                box.IsEnabled = sensor.IsEnabled;
            };
            sensor.HasBeenEnabled += (_) => VisibilityCallback.Invoke();
            RefreshShownSensors += VisibilityCallback;

            return box;
        }

        private void StartCollectingFromOmnicept_Click(object sender, RoutedEventArgs e)
        {
            if (sensorProvider.IsConnected)
            {
                sensorProvider.Teardown();
                StartCollectingFromOmnicept.Content = "Start collecting from Omnicept";
            }
            else
            {
                sensorProvider.Initialise();
                StartCollectingFromOmnicept.Content = "Stop collecting from Omnicept";
            }
        }

        private Action RefreshShownSensors;
        private void ShowDisabledSensorsCheckBoxCallback(object sender, RoutedEventArgs e)
        {
            if(RefreshShownSensors != null)
            {
                RefreshShownSensors.Invoke();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            sensorProvider.Teardown();
        }
    }
}
