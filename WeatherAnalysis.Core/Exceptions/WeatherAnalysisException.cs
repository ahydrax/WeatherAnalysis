using System;
using System.Runtime.Serialization;

namespace WeatherAnalysis.Core.Exceptions
{
    public class WeatherAnalysisException : Exception
    {
        public WeatherAnalysisException() { }

        public WeatherAnalysisException(string message)
            : base(message) { }

        public WeatherAnalysisException(string message, Exception innerException)
            : base(message, innerException) { }

        protected WeatherAnalysisException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
