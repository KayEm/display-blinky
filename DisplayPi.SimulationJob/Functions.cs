﻿using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using DisplayPi.Common.Helpers;
using DisplayPi.Common.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace DisplayPi.SimulationJob
{
    public class Functions
    {

        const string Image =
            "data:image/png;base64,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";




        // This function will get triggered/executed when a new message is written
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([ServiceBusTrigger("commandqueue", AccessRights.Listen)] BrokeredMessage message, TextWriter log)
        {
            try
            {
                var messageBody = new StreamReader(message.GetBody<Stream>(), Encoding.UTF8).ReadToEnd();
                var inputMessage = JsonConvert.DeserializeObject<DisplayPiInputMessage>(messageBody);
                Debug.WriteLine(inputMessage);

                SendAcknowledgement(inputMessage);
            }
            catch (Exception e)
            {
                log.WriteLine(e.Message);
            }

            //log.WriteLine(message);
        }

        private static QueueClient _sender;

        private static QueueClient Sender
        {
            get
            {
                if (_sender == null)
                {
                    _sender =
                    QueueClient.CreateFromConnectionString(
                    ConfigurationManager.ConnectionStrings["OutputQueue"].ConnectionString,
                    "outputqueue");
                }

                return _sender;
            }
        }

        private static void SendAcknowledgement(DisplayPiInputMessage message)
        {
            var acknowledgementMessage = new DisplayPiResponseMessage
            {
                Id = Guid.NewGuid(),
                InputMessage = message,
                AcknowledgementTimeStamp = DateTime.Now,
                EncodedImage = Image,
                MorseCode = message.Message.ConvertToMorse()
            };

            try
            {

                var encodedMessage = JsonConvert.SerializeObject(acknowledgementMessage);
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(encodedMessage)))
                {
                    Sender.Send(new BrokeredMessage(stream));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + " " + ex.InnerException);
            }

            Debug.WriteLine(acknowledgementMessage);
        }
    }
}