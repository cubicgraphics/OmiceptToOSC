using HP.Omnicept.Messaging.Messages;
using System;
using System.Diagnostics;

namespace OmiceptToOSC.Sensors
{
    public class HeartRateVariabilityOsc : SensorType
    {
        public float Rmssd { get; set; } = 0;
        public float Sdnn { get; set; } = 0;
        public string RmssdOscOutput { get; set; }
        public string SdnnOscOutput { get; set; }
        public HeartRateVariabilityOsc(uint messageType, string sensorName, string rmssdOscOutput, string sdnnOscOutput) : base(messageType, sensorName, 1000*60)
        {
            RmssdOscOutput = rmssdOscOutput;
            SdnnOscOutput = sdnnOscOutput;
        }

        override public void FetchData(ref GliaLastValueCacheCustom gliaLastValueCache)
        {
            var SensorData = gliaLastValueCache.ReturnLastMessageOfType<HeartRateVariability>(MessageType);
            if (SensorData != null && (Rmssd != SensorData.Rmssd || Sdnn != SensorData.Sdnn))
            {
                Rmssd = SensorData.Rmssd;
                Sdnn = SensorData.Sdnn;
                Debug.WriteLine(SensorData.ToString());
                if (ValuesUpdated != null)
                {
                    ValuesUpdated.Invoke();
                }
            }
        }
    }
}
