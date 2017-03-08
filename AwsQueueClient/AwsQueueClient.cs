using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using AwsQueue.Data;
using AwsQueue.Interface;
using AwsQueue.Model;
using Newtonsoft.Json;

namespace AwsQueue
{
    public class AwsQueueClient
    {
        private static AmazonSQSClient _client;
        private static string _queueUrl;
        private static readonly Lazy<AwsQueueClient> QueueClient = new Lazy<AwsQueueClient>(() => new AwsQueueClient());

        #region Constructors
        private AwsQueueClient()
        {
            if (_client == null) _client = new AmazonSQSClient(new StoredProfileAWSCredentials("developer"), RegionEndpoint.USEast1);
            GetQueueStatus();
        }

        static AwsQueueClient() { }
        #endregion

        #region Private Static Functions
        public static AwsQueueClient GetInstance => QueueClient.Value;

        private static void GetQueueStatus()
        {
            var response = _client.GetQueueUrl(Global.Queue.QueueName);
            _queueUrl = string.IsNullOrEmpty(response.QueueUrl)
                ? CreateQueue
                : _queueUrl = response.QueueUrl;

        }

        private static string CreateQueue =>
            _client.CreateQueue(new CreateQueueRequest
            {
                QueueName = Global.Queue.QueueName
            }).QueueUrl;
        #endregion

        #region Public Functions
        public async void Enqueue(IAwsArgs args)
        {
            //Serialize and send message
            var message = new SendMessageRequest(_queueUrl, JsonConvert.SerializeObject(args));
            await _client.SendMessageAsync(message);
        }

        public async Task<IEnumerable<IAwsArgs>> Dequeue()
        {
            //Retrieve message
            var recieveRequest = new ReceiveMessageRequest(_queueUrl);
            var response = await _client.ReceiveMessageAsync(recieveRequest);

            //Initialize return collections
            var returnList = response.Messages.Select(msg => JsonConvert.DeserializeObject<AwsEventArgs>(msg.Body)).Cast<IAwsArgs>().ToList();

            //Delete messages
            var deleteList = response.Messages.Select(msg => new DeleteMessageBatchRequestEntry(msg.MessageId, msg.ReceiptHandle)).ToList();
            var result = await _client.DeleteMessageBatchAsync(_queueUrl, deleteList);

            //Return messages
            return returnList;
        }
        #endregion
    }
}
