using HP.Omnicept;
using HP.Omnicept.Messaging.Messages;
using System;
using System.Threading;

namespace OmiceptToOSC.Sensors
{
    public class SensorType : DisplayableProperty
    {
        private int _UpdateInterval;
        private long _LastUpdate;
        private bool _IsEnabled;
        public uint MessageType { get; }
        public string MessageTypeName => MessageTypes.GetName(MessageType);
        public string SensorName { get; }

        public int UpdateInterval { 
            get { return _UpdateInterval; } 
            set { _UpdateInterval = value; OnPropertyChanged(nameof(UpdateInterval)); }
        } //In ms

        public long LastUpdate
        { 
            get { return _LastUpdate; } 
            set { _LastUpdate = value; OnPropertyChanged(nameof(LastUpdate)); }
        } //In ms


        public Action<SensorType>? HasBeenEnabled;
        public CancellationTokenSource? IsRunning;
        public bool IsEnabled
        { 
            get { return _IsEnabled; } 
            set { _IsEnabled = value; OnPropertyChanged(nameof(IsEnabled)); if (HasBeenEnabled != null) HasBeenEnabled.Invoke(this); }
        }
        public bool IsDisabled => !IsEnabled;



        //public Action ValuesUpdated;
        public SensorType(uint messageType, string sensorName, int updateInterval)
        {
            MessageType = messageType;
            SensorName = sensorName;
            UpdateInterval = updateInterval;
            IsEnabled = false;
        }
        virtual public void FetchData(ref GliaLastValueCacheCustom gliaLastValueCache) { }
        virtual public MessageVersionSemantic GetMessageVersionSemantic() { return new MessageVersionSemantic("1.0.0"); }

    }
}
