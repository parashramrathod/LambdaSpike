using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace LambdaWithSQS
{
    public class LambdaFunction
    {
        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public LambdaFunction()
        {

        }


        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
        /// to respond to SQS messages.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Handler(SQSEvent evnt, ILambdaContext context)
        {
            var recordCount = evnt.Records.Count;
            var processId = Guid.NewGuid();
            foreach (var message in evnt.Records)
            {
                context.Logger.LogLine($"Processing message {recordCount} - Id {processId}");
                await ProcessLambdaMessageAsync(message, context);
            }
        }

        private async Task ProcessLambdaMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
        {
            if (message.Body.Contains("Error:")) throw new Exception();

            context.Logger.LogLine($"Processed message {message.Body}");
            context.Logger.LogLine("Attributes");
            context.Logger.LogLine("----------");
            context.Logger.LogLine(string.Join(",", message.Attributes.Select(s => $"{s.Key}: {s.Value}")));

            await Task.CompletedTask;
        }
    }
}