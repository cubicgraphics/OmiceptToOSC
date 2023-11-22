using HP.Omnicept.Messaging.Messages;
using HP.Omnicept;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using OmiceptToOSC.Sensors;

namespace OmiceptToOSC
{
    public class OmniceptSensorProvider
    {
        private Glia? _gliaClient;

        private GliaLastValueCacheCustom gliaLastValueCacheCustom;

        private bool isConnected;
        private CancellationTokenSource IsRunningCollector;
        public bool IsConnected => isConnected;


        //TODO - alter the subscribed data when something is enabled/disabled so we are not reciving more than needed
        //TODO - make the initialise funtion not yeet my pc into freeze land for 5 min (pop some awaits in there)

/*        private readonly uint[] Messages = {
            MessageTypes.ABI_MESSAGE_HEART_RATE,
            MessageTypes.ABI_MESSAGE_HEART_RATE_VARIABILITY,
            MessageTypes.ABI_MESSAGE_EYE_TRACKING,
            MessageTypes.ABI_MESSAGE_EYE_TRACKING_FRAME,
            MessageTypes.ABI_MESSAGE_BYTE_MESSAGE,
            MessageTypes.ABI_MESSAGE_COGNITIVE_LOAD,
            MessageTypes.ABI_MESSAGE_COGNITIVE_LOAD_INPUT_FEATURE,
            MessageTypes.ABI_MESSAGE_FACIAL_EMG,
            MessageTypes.ABI_MESSAGE_FACIAL_EMG_FRAME,
            MessageTypes.ABI_MESSAGE_VSYNC,
            MessageTypes.ABI_MESSAGE_SCENE_COLOR,
            MessageTypes.ABI_MESSAGE_SCENE_COLOR_FRAME,
            MessageTypes.ABI_MESSAGE_CONNECTION_STATUS_SIGNAL,
            MessageTypes.ABI_MESSAGE_CAMERA_IMAGE,
            MessageTypes.ABI_MESSAGE_IMU,
            MessageTypes.ABI_MESSAGE_IMU_FRAME,
            MessageTypes.ABI_MESSAGE_AUDIO,
            MessageTypes.ABI_MESSAGE_AUDIO_FRAME,
            MessageTypes.ABI_MESSAGE_SUBSCRIPTION_RESULT_LIST
        };*/

        public readonly SensorType[] UseableMessages;

        public OmniceptSensorProvider(SensorType[] SensorTypes)
        {
            UseableMessages = SensorTypes;
            IsRunningCollector = new();
        }
        public async void Initialise()
        {
            if (!IsRunningCollector.IsCancellationRequested)
            {
                IsRunningCollector.Cancel();
            }
            IsRunningCollector.Dispose();
            IsRunningCollector = new();
            if (StartGlia())
            {
                foreach(var sensorMessage in UseableMessages)
                {
                    sensorMessage.HasBeenEnabled += StartStopFetcher;
                    StartStopFetcher(sensorMessage);
                }
            }
            Debug.WriteLine("Initialised");
        }
        private void StartStopFetcher(SensorType sensorToStart) //Will start or stop a fetcher when its IsEnabled bool is changed
        {
            if (sensorToStart.IsEnabled)
            {
                sensorToStart.IsRunning ??= new();
                if (sensorToStart.IsRunning.IsCancellationRequested)
                {
                    sensorToStart.IsRunning.Dispose();
                    sensorToStart.IsRunning = new();
                }
                else
                {
                    sensorToStart.IsRunning.Cancel();
                    sensorToStart.IsRunning.Dispose();
                    sensorToStart.IsRunning = new();
                }
                CancellationTokenSource IsCollectorRunning = CancellationTokenSource.CreateLinkedTokenSource(IsRunningCollector.Token, sensorToStart.IsRunning.Token);
                FetchData(sensorToStart, IsCollectorRunning); //Runs the collectors on the same thread
            }
            else
            {
                if(sensorToStart.IsRunning != null)
                {
                    if (sensorToStart.IsRunning.IsCancellationRequested)
                    {
                        sensorToStart.IsRunning.Dispose();
                    }
                    else
                    {
                        sensorToStart.IsRunning.Cancel();
                        sensorToStart.IsRunning.Dispose();
                    }
                }
            }
        }
        public void Teardown()
        {
            if (!IsRunningCollector.IsCancellationRequested)
            {
                IsRunningCollector.Cancel();
            }
            IsRunningCollector.Dispose();
            foreach (var sensorMessage in UseableMessages)
            {
                sensorMessage.HasBeenEnabled -= StartStopFetcher;
            }
            StopGlia();
            Debug.WriteLine("Teardown complete");
            return;
        }

        private void CollectorException(Exception e)
        {
            Debug.WriteLine("Reading Glia cache error: " + e.Message);
        }


        private bool StartGlia()
        {
            // Verify Glia is Disposed
            StopGlia();

            // Start Glia
            try
            {
                _gliaClient = new Glia("OSCOmniceptModule", new SessionLicense("36eff722-dad0-4e06-bd59-0bff6b7bfe10", "wkzScX2H6mB9FP-0ThuK8ggx-VUKQ8dCuzQPhsSOBdvEbw6fTGT6ZWmB6imEvLgq", LicensingModel.Rev_Share, false));
                gliaLastValueCacheCustom = new GliaLastValueCacheCustom(_gliaClient.Connection);
                gliaLastValueCacheCustom.TransportException += CollectorException;

                SubscriptionList sl = new();
                foreach (var item in UseableMessages)
                {
                    sl.Subscriptions.Add(new Subscription(item.MessageType, String.Empty, String.Empty, String.Empty, String.Empty, item.GetMessageVersionSemantic()));
                }

                _gliaClient.setSubscriptions(sl);
                isConnected = true;
            }
            catch (Exception e)
            {
                isConnected = false;
                //Log fail
            }
            Debug.WriteLine("Started GLIA");
            return isConnected;
        }

        private void StopGlia()
        {
            // Verify Glia is Disposed
            isConnected = false;
            if (gliaLastValueCacheCustom != null)
            {
                gliaLastValueCacheCustom.TransportException -= CollectorException;
                gliaLastValueCacheCustom?.Stop();
            }
            if (_gliaClient != null)
                _gliaClient?.Dispose();
            gliaLastValueCacheCustom = null!;
            _gliaClient = null;
            isConnected = false;
            Glia.cleanupNetMQConfig();
        }


        async void FetchData(SensorType sensorInput, CancellationTokenSource IsRunning)
        {
            Debug.WriteLine("Message collect loop started for " + sensorInput.SensorName);
            while (!IsRunning.IsCancellationRequested && sensorInput.IsEnabled)
            {
                sensorInput.FetchData(ref gliaLastValueCacheCustom);
                sensorInput.LastUpdate = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
                try
                {
                    await Task.Delay(sensorInput.UpdateInterval, IsRunning.Token);
                }
                catch (TaskCanceledException) { Debug.WriteLine("Message collect loop ended for " + sensorInput.SensorName); return;/* module.LogDebug("Omnicept Heart rate data fetching loop await task ended");*/ }
                catch (ObjectDisposedException) { Debug.WriteLine("Message collect loop ended for " + sensorInput.SensorName); return;/* module.LogDebug("Omnicept Heart rate Fetch data loop await task ended unexpectedly"); */}
            }
            IsRunning.Dispose();
            Debug.WriteLine("Message collect loop ended for " + sensorInput.SensorName);
        }
    }
}