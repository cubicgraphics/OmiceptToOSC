using HP.Omnicept.Messaging.Messages;
using OmiceptToOSC.OscParameter;
using System.Diagnostics;

namespace OmiceptToOSC.Sensors
{
    public class HeartRateVariabilityOsc : SensorType
    {
        public OscParameter<float> Rmssd { get; }
        public OscParameter<float> Sdnn { get; }
        public HeartRateVariabilityOsc(uint messageType, string sensorName, string rmssdOscOutput, string sdnnOscOutput) : base(messageType, sensorName, 1000*60)
        {
            Rmssd = new(ParamType.floatingPoint, rmssdOscOutput, 0);
            Sdnn = new(ParamType.floatingPoint, sdnnOscOutput, 0);
        }

        override public void FetchData(ref GliaLastValueCacheCustom gliaLastValueCache)
        {
            var SensorData = gliaLastValueCache.ReturnLastMessageOfType<HeartRateVariability>(MessageType);
            if (SensorData != null && (Rmssd.Value != SensorData.Rmssd || Sdnn.Value != SensorData.Sdnn))
            {
                Rmssd.Value = SensorData.Rmssd;
                Sdnn.Value = SensorData.Sdnn;
                Debug.WriteLine(SensorData.ToString());
/*                if (ValuesUpdated != null)
                {
                    ValuesUpdated.Invoke();
                }*/
            }
        }
    }
}
