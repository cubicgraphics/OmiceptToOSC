using HP.Omnicept;
using HP.Omnicept.Messaging.Messages;
using System.Diagnostics;
using System;

namespace OmiceptToOSC.Sensors
{
    public class CognitiveLoadOsc : SensorType
    {
        public float CognitiveLoadValue { get; set; } = 0;
        public float StandardDeviation { get; set; } = 0;
        public string DataState { get; set; } = "";
        public string CognitiveLoadValueOscOutput { get; set; }
        public string StandardDeviationOscOutput { get; set; }
        public CognitiveLoadOsc(uint messageType, string sensorName, string cognitiveLoadValueOscOutput, string standardDeviationOscOutput) : base(messageType, sensorName, 1000)
        {
            CognitiveLoadValueOscOutput = cognitiveLoadValueOscOutput;
            StandardDeviationOscOutput = standardDeviationOscOutput;
        }

        override public void FetchData(ref GliaLastValueCacheCustom gliaLastValueCache)
        {
            var SensorData = gliaLastValueCache.ReturnLastMessageOfType<CognitiveLoad>(MessageType);
            if (SensorData != null && (CognitiveLoadValue != SensorData!.CognitiveLoadValue || StandardDeviation != SensorData.StandardDeviation || DataState != SensorData.DataState) )
            {
                CognitiveLoadValue = SensorData.CognitiveLoadValue;
                StandardDeviation = SensorData.StandardDeviation;
                DataState = SensorData.DataState;
                Debug.WriteLine(SensorData.ToString());
                if(ValuesUpdated != null)
                {
                    ValuesUpdated.Invoke();
                }
            }
        }

        override public MessageVersionSemantic GetMessageVersionSemantic() { return new MessageVersionSemantic(CognitiveLoad.LatestMessageVersion.VersionString); }
        public override string ToString()
        {
            return "CognitiveLoadValue: " + CognitiveLoadValue + "  StandardDeviation: " + StandardDeviation + " DataState" + DataState;
        }
    }
}
