using HP.Omnicept.Messaging.Messages;
using System;
using System.Diagnostics;

namespace OmiceptToOSC.Sensors
{
    public class HeartRateOsc : SensorType
    {
        public uint HeartRate { get; set; } = 0;
        public string HeartRateOscOutput { get; set; }
        public HeartRateOsc(uint messageType, string sensorName, string heartRateOscOutput) : base(messageType, sensorName, 5000)
        {
            HeartRateOscOutput = heartRateOscOutput;
        }

        override public void FetchData(ref GliaLastValueCacheCustom gliaLastValueCache)
        {
            var SensorData = gliaLastValueCache.ReturnLastMessageOfType<HeartRate>(MessageType);
            if (SensorData != null && HeartRate != SensorData.Rate)
            {
                HeartRate = SensorData.Rate;
                Debug.WriteLine(SensorData.ToString());
                if (ValuesUpdated != null)
                {
                    ValuesUpdated.Invoke();
                }
            }
        }
    }
}
