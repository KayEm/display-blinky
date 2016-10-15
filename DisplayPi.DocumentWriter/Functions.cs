using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;

namespace DisplayPi.DocumentWriter
{
    // https://azure.microsoft.com/en-us/documentation/articles/documentdb-get-started/

    public static class Functions
    {
        private static DocumentClient _client;
        private static string EndpointUrl => ConfigurationManager.ConnectionStrings["DocumentDBUrl"].ConnectionString;
        private static string AuthorizationKey => ConfigurationManager.ConnectionStrings["DocumentDBToken"].ConnectionString;

        private const string DbName = "DisplayPiDB";
        private const string CollectionName = "DisplayPiResponseMessages";

        public static DocumentClient Client => _client ?? (_client = new DocumentClient(new Uri(EndpointUrl),
            AuthorizationKey));


        public static async Task CreateDatabase()
        {
            try
            {
                await Client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DbName));
            }
            catch (DocumentClientException de)
            {
                // If the database does not exist, create a new database
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await Client.CreateDatabaseAsync(new Database { Id = DbName });
                }
                else
                {
                    throw;
                }
            }
        }

        public static async Task CreateCollection()
        {
            try
            {
                await Client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DbName, CollectionName));
            }
            catch (DocumentClientException de)
            {
                // If the document collection does not exist, create a new collection
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    // DocumentDB collections can be reserved with throughput specified in request units/second. 1 RU is a normalized request equivalent to the read
                    // of a 1KB document.  Here we create a collection with 400 RU/s.
                    await Client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DbName),
                        new DocumentCollection { Id = CollectionName },
                        new RequestOptions { OfferThroughput = 400 });
                }
                else
                {
                    throw;
                }
            }
        }

        // This function will get triggered/executed when a new message is written in the servicebus output queue
        public static void ProcessQueueMessage([ServiceBusTrigger("outputqueue", AccessRights.Listen)] BrokeredMessage message, TextWriter log)
        {
            try
            {
                var stream = message.GetBody<Stream>();
                var document = JsonSerializable.LoadFrom<Document>(stream);
                Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DbName, CollectionName), document).Wait();
            }
            catch (Exception e)
            {
                log.WriteLine(e.Message);
            }
        }
    }
}
