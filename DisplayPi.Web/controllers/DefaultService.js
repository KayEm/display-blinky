'use strict';
var azure = require('azure');
var uuid = require('node-uuid');
var config = require('../config.js');

var DocumentDBClient = require('documentdb').DocumentClient;
var serviceBusService = azure.createServiceBusService(config.commandQueueConnectionString);

var docDbClient = new DocumentDBClient(config.documentDbUrl, {
    masterKey: config.documentDbSecret
});

var databaseUrl = `dbs/${config.documentDbUrl}`;
var collectionUrl = `${databaseUrl}/colls/DisplayPiResponseMessages`;

exports.getMessages = function(args, res, next) {
    /**
     * parameters expected in the args:
    * size (int)
    **/
     var size = args.size.value;
     if (size === undefined) {
         size = 10;
     }
    
     var querySpec = {
                        query: 'SELECT TOP @size * FROM DisplayPiResponseMessages m ORDER BY m._ts DESC',
                        parameters: [
                            {
                                name: '@size',
                                value: size
                            }
                        ]
                    };

    docDbClient.queryDocuments('/dbs/PuwWAA==/colls/PuwWAKoAAAA=/', querySpec).toArray(function (err, results)
    {
        if (err) {
            res.end(JSON.stringify({error: "Error occured"}));
        } else {
               var message = {};
                message['application/json'] = results;
                    
                var replacer = function (key, value) {
                    if (key.startsWith('_')) {
                     return undefined;
                    }
                    
                    return value;
                }

                if (Object.keys(message).length > 0) {
                    res.setHeader('Content-Type', 'application/json');
                res.end(JSON.stringify(message[Object.keys(message)[0]] || {}, replacer, 2));
                }
                else {
                    res.end();
                }        
            }
    });
}

exports.sendMessage = function(args, res, next) {
    /**
     * parameters expected in the args:
    * message (NewMessage)
    **/
    // no response value expected for this operation
    var queueMessage = {
        "id": uuid.v4(),
        "Message": args.message.value.Message,
        "Author": args.message.value.Author,
        "TimeStamp": new Date()
    };
    var brokeredMessage = {
        body: JSON.stringify(queueMessage),
    };
    
    serviceBusService.sendQueueMessage('commandqueue', brokeredMessage,  function(error)
    {
        if(!error) {
            console.log("Send message called with id " + queueMessage.id + " " + queueMessage.Author + " " + queueMessage.Message + "" + queueMessage.TimeStamp);
        } else {
        console.log("Error ocurred");
    }});
    
    res.end();
      
}