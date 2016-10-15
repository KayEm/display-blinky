using System;
using Newtonsoft.Json;

namespace DisplayPi.Common.Interfaces
{
    public sealed class DisplayPiResponseMessage
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        public DisplayPiInputMessage InputMessage { get; set; }

        public DateTime AcknowledgementTimeStamp { get; set; }

        public string MorseCode { get; set; }

        public string EncodedImage { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, InputMessage: {InputMessage}, AcknowledgementTimeStamp: {AcknowledgementTimeStamp}, EncodedImage: {EncodedImage}";
        }
    }
}
