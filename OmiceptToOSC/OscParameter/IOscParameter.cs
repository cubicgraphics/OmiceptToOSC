using System;

namespace OmiceptToOSC.OscParameter
{
    public enum ParamType
    {
        boolean,
        floatingPoint,
        interger,
    }

    public interface IOscParameter
    {
        public string OscEndpoint { get; set; }
        public ParamType GetParameterType();
    }
}
