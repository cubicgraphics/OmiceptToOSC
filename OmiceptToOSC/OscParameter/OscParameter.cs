
using System;
using System.ComponentModel;

namespace OmiceptToOSC.OscParameter
{
    public class OscParameter<T> : DisplayableProperty, IOscParameter
    {
        private readonly ParamType paramType;
        private string _OscEndpoint;
        private T _Value;
        public OscParameter(ParamType parameterType, string oscEndpoint, T value)
        {
            paramType = parameterType;
            _OscEndpoint = oscEndpoint;
            _Value = value;
        }
        public ParamType GetParameterType()
        {
            return paramType;
        }

        public string OscEndpoint
        {
            get { return _OscEndpoint; }
            set { _OscEndpoint = value; OnPropertyChanged(nameof(OscEndpoint)); }
        }
        public T Value
        {
            get { return _Value; }
            set { _Value = value; OnPropertyChanged(nameof(Value)); }
        }
    }
}
