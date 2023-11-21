using HP.Omnicept;
using HP.Omnicept.Messaging.Messages;
using System;

namespace OmiceptToOSC.Sensors
{
    public class SensorType
    {
        public uint MessageType { get; }
        public string MessageTypeName => MessageTypes.GetName(MessageType);
        public string SensorName { get; }
        public int UpdateInterval { get; } //In ms
        public long LastUpdate { get; set; } //In ms
        public Action ValuesUpdated;
        public SensorType(uint messageType, string sensorName, int updateInterval)
        {
            MessageType = messageType;
            SensorName = sensorName;
            UpdateInterval = updateInterval;
        }
        virtual public void FetchData(ref GliaLastValueCacheCustom gliaLastValueCache) { }
        virtual public MessageVersionSemantic GetMessageVersionSemantic() { return new MessageVersionSemantic("1.0.0"); }

    }
}
