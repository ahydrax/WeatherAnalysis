using System;
using System.Runtime.Serialization;

namespace WeatherAnalysis.Core.Exceptions
{
    public class WeatherRecordNotFoundException : WeatherAnalysisException
    {
        public WeatherRecordNotFoundException() { }

        public WeatherRecordNotFoundException(string message)
            : base(message) { }

        public WeatherRecordNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }

        protected WeatherRecordNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
