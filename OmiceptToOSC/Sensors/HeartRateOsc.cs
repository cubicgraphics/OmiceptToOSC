using HP.Omnicept.Messaging.Messages;
using OmiceptToOSC.OscParameter;
using System.Diagnostics;

namespace OmiceptToOSC.Sensors
{
    public class HeartRateOsc : SensorType
    {
        public OscParameter<int> HeartRate { get; }
        public HeartRateOsc(uint messageType, string sensorName, string heartRateOscOutput) : base(messageType, sensorName, 5000)
        {
            HeartRate = new(ParamType.interger, heartRateOscOutput, 5);
        }

        override public void FetchData(ref GliaLastValueCacheCustom gliaLastValueCache)
        {
            var SensorData = gliaLastValueCache.ReturnLastMessageOfType<HeartRate>(MessageType);
            if (SensorData != null && HeartRate.Value != SensorData.Rate)
            {
                HeartRate.Value = (int)SensorData.Rate;
                Debug.WriteLine(SensorData.ToString());
/*                if (ValuesUpdated != null)
                {
                    ValuesUpdated.Invoke();
                }*/
            }
        }
    }
}
