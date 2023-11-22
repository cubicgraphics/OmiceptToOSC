using HP.Omnicept;
using HP.Omnicept.Messaging.Messages;
using System.Diagnostics;
using System;
using OmiceptToOSC.OscParameter;

namespace OmiceptToOSC.Sensors
{
    public class CognitiveLoadOsc : SensorType
    {
        public OscParameter<float> CognitiveLoadValue { get; }
        public OscParameter<float> StandardDeviation { get; }
        public string DataState { get; set; } = "";
        public CognitiveLoadOsc(uint messageType, string sensorName, string cognitiveLoadValueOscOutput, string standardDeviationOscOutput) : base(messageType, sensorName, 1000)
        {
            CognitiveLoadValue = new(ParamType.floatingPoint, cognitiveLoadValueOscOutput, 0);
            StandardDeviation = new(ParamType.floatingPoint, standardDeviationOscOutput, 0);
        }

        override public void FetchData(ref GliaLastValueCacheCustom gliaLastValueCache)
        {
            var SensorData = gliaLastValueCache.ReturnLastMessageOfType<CognitiveLoad>(MessageType);
            if (SensorData != null && (CognitiveLoadValue.Value != SensorData!.CognitiveLoadValue || StandardDeviation.Value != SensorData.StandardDeviation || DataState != SensorData.DataState) )
            {
                CognitiveLoadValue.Value = SensorData.CognitiveLoadValue;
                StandardDeviation.Value = SensorData.StandardDeviation;
                DataState = SensorData.DataState;
                Debug.WriteLine(SensorData.ToString());
/*                if(ValuesUpdated != null)
                {
                    ValuesUpdated.Invoke();
                }*/
            }
        }

        override public MessageVersionSemantic GetMessageVersionSemantic() { return new MessageVersionSemantic(CognitiveLoad.LatestMessageVersion.VersionString); }
        public override string ToString()
        {
            return "CognitiveLoadValue: " + CognitiveLoadValue + "  StandardDeviation: " + StandardDeviation + " DataState" + DataState;
        }
    }
}
