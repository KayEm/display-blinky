using System;
using Newtonsoft.Json;

namespace DisplayPi.Common.Interfaces
{
    public sealed class DisplayPiInputMessage
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        public string Message { get; set; }

        public string Author { get; set; }

        public DateTime TimeStamp { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Message: {Message}, Author: {Author}, TimeStamp: {TimeStamp}";
        }
    }
}
