using System;
using System.Runtime.Serialization;

namespace WeatherAnalysis.Core.Exceptions
{
    public class WeatherServiceException : WeatherAnalysisException
    {
        public WeatherServiceException() { }

        public WeatherServiceException(string message) : base(message) { }

        public WeatherServiceException(string message, Exception innerException) : base(message, innerException) { }

        protected WeatherServiceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
