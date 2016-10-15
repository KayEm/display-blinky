using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using DisplayPi.Common.Helpers;
using DisplayPi.Common.Interfaces;

using DisplayPi.LCD1602;
using Newtonsoft.Json;
using ppatierno.AzureSBLite.Messaging;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace DisplayPi
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const int RS = 27;
        private const int E = 22;
        private readonly int[] DATA_PIN = { 18, 23, 24, 25 };
        public LcdDisplay Lcd { get; private set; }
        //private readonly int[] DATA_PIN = { 25, 24, 23, 18 };

        

        public void Run(IBackgroundTaskInstance taskInstance)
        {

            this.Client = QueueClient.CreateFromConnectionString("Endpoint=***REMOVED***;SharedAccessKeyName=RaspberryPiCommandAccess;SharedAccessKey=***REMOVED***", "commandqueue");
            this.Sender =
                QueueClient.CreateFromConnectionString(
                    "Endpoint=***REMOVED***;SharedAccessKeyName=RaspberryPiOutputAccess;SharedAccessKey=***REMOVED***",
                    "outputqueue");
           
           // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //

            Lcd = new LcdDisplay();
            Lcd.InitGpio(rs:RS, e:E, data:DATA_PIN);


            while (true)
            {
      
                try
                {
                    var message = Client.Receive();
                    message.Complete();
                    var body = Encoding.UTF8.GetString(message.GetBytes());
                    var inputMessage = JsonConvert.DeserializeObject<DisplayPiInputMessage>(body);
                   
                    DisplayInputMessage(inputMessage);
                    SendAcknowledgement(inputMessage);

                    // Process message from queue.
                    Debug.WriteLine("Body: " + body);
                    
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + " " + ex.InnerException);
                }
            }
        }

        private QueueClient Sender { get; set; }

        private QueueClient Client { get; set; }


        private void DisplayInputMessage(DisplayPiInputMessage message, TimeSpan? endDelay=null)
        {
            Lcd.Clear();
            Lcd.SendMessage($"{message.Author}\n{message.Message}");
            Debug.WriteLine(message.ToString());
            Task.Delay(endDelay ?? TimeSpan.FromSeconds(3)).Wait();
        }

        private void SendAcknowledgement(DisplayPiInputMessage message)
        {
            var acknowledgementMessage = new DisplayPiResponseMessage
            {
                Id = Guid.NewGuid(),
                InputMessage = message,
                AcknowledgementTimeStamp = DateTime.Now,
                EncodedImage = string.Empty,
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
