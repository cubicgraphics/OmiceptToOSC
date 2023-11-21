using OmiceptToOSC.Sensors;
using System.Windows;


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
            sensorProvider = new OmniceptSensorProvider();
            sensorProvider.UseableMessages[0].ValuesUpdated += HandleHeartRateChange;
        }

        public void HandleHeartRateChange()
        {
            HeartRateLabel.Content = ((HeartRateOsc)sensorProvider.UseableMessages[0]).HeartRate;
        }

        private void StartCollectingFromOmnicept_Click(object sender, RoutedEventArgs e)
        {
            if (sensorProvider.IsConnected)
            {
                sensorProvider.Teardown();
            }
            else
            {
                sensorProvider.Initialise();
            }
        }
    }
}
