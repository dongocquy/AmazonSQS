using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonSQS
{
    class Program
    {
        static void Main(string[] args)
        {
            string accessKey = "your_accesskey";
            string secretAccessKey = "your_secretaccesskey";
            string _Url = "https://sqs.------------.amazonaws.com";
            string _Queue = "your_Queue";
            string _MessageBody = "John Doe customer information.";
            string _AmazonUserID = "your_id";
            var client = new SQSClient(accessKey, secretAccessKey);
            var _MessageAttributes = new Dictionary<string, MessageAttributeValue>
                  {
                    {
                      "MyNameAttribute", new MessageAttributeValue
                        { DataType = "String", StringValue = "John Doe" }
                    },
                    {
                      "MyAddressAttribute", new MessageAttributeValue
                        { DataType = "String", StringValue = "123 Main St." }
                    },
                    {
                      "MyRegionAttribute", new MessageAttributeValue
                        { DataType = "String", StringValue = "Any Town, United States" }
                    }
                  };
            string Uid = client.sendMessage(_MessageAttributes, _MessageBody, _Url, _Queue, _AmazonUserID);
            Console.WriteLine(Uid);
            Console.ReadKey();
        }
    }
    public class SQSClient
    {
        protected string AccesskeyID;
        protected string Secretaccesskey;
        public SQSClient(string _AccesskeyID, string _Secretaccesskey)
        {

            AccesskeyID = _AccesskeyID;
            Secretaccesskey = _Secretaccesskey;
            
        }
        public string sendMessage(Dictionary<string, MessageAttributeValue> _MessageAttributeValue, string _MessageBody, string _Url, string _Queue, string _AmazonUserID)
        {
            //https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/SQS/TSendMessageRequest.html
            try
            {
                var sqsConfig = new AmazonSQSConfig();
                sqsConfig.ServiceURL = _Url;
                var client = new AmazonSQSClient(AccesskeyID, Secretaccesskey, sqsConfig);
                var request = new SendMessageRequest
                {
                    DelaySeconds = (int)TimeSpan.FromSeconds(5).TotalSeconds,
                    MessageAttributes = _MessageAttributeValue,
                    MessageBody = _MessageBody,
                    QueueUrl = _Url + "/" + _AmazonUserID + "/" + _Queue
                };
                var response = client.SendMessage(request);
                return response.MessageId;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public void receiveMessage(string _Url, string _Queue, string _AmazonUserID)
        {
            //https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/SQS/TReceiveMessageRequest.html
            var sqsConfig = new AmazonSQSConfig();
            sqsConfig.ServiceURL = _Url;
            var client = new AmazonSQSClient(AccesskeyID, Secretaccesskey, sqsConfig);
            var request = new ReceiveMessageRequest
            {
                AttributeNames = new List<string>() { "All" },
                MaxNumberOfMessages = 5,
                QueueUrl = _Url + "/" + _AmazonUserID + "/" + _Queue,
                VisibilityTimeout = (int)TimeSpan.FromMinutes(10).TotalSeconds,
                WaitTimeSeconds = (int)TimeSpan.FromSeconds(5).TotalSeconds
            };

            var response = client.ReceiveMessage(request);

            if (response.Messages.Count > 0)
            {
                foreach (var message in response.Messages)
                {
                    Console.WriteLine("For message ID '" + message.MessageId + "':");
                    Console.WriteLine("  Body: " + message.Body);
                    Console.WriteLine("  Receipt handle: " + message.ReceiptHandle);
                    Console.WriteLine("  MD5 of body: " + message.MD5OfBody);
                    Console.WriteLine("  MD5 of message attributes: " +
                      message.MD5OfMessageAttributes);
                    Console.WriteLine("  Attributes:");

                    foreach (var attr in message.Attributes)
                    {
                        Console.WriteLine("    " + attr.Key + ": " + attr.Value);
                    }
                }
            }
            else
            {
                Console.WriteLine("No messages received.");
            }
        }
    }
}
