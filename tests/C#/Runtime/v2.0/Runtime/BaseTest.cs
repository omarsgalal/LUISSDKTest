namespace LUIS.Runtime.Tests
{
    using System;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
    using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
    using Microsoft.Azure.Test.HttpRecorder;
    using Microsoft.Rest.ClientRuntime.Azure.TestFramework;

    public abstract class BaseTest
    {
        private const HttpRecorderMode mode = HttpRecorderMode.Record;

        protected const string appId = "0894d430-8f00-4bcd-8153-45e06a1feca1";
        protected const string subscriptionKey = "62b834e0069943d9b035f32abd230359";

        private string ClassName => GetType().FullName;

        private ILUISRuntimeClient GetClient(DelegatingHandler handler, string subscriptionKey = subscriptionKey)
        {
            var client = new LUISRuntimeClient(new ApiKeyServiceClientCredentials(subscriptionKey), handlers: handler);
            client.Endpoint = "https://westus.api.cognitive.microsoft.com";
            return client;
        }

        protected async void UseClientFor(Func<ILUISRuntimeClient, Task> doTest, string className = null, [CallerMemberName] string methodName = "")
        {
            using (MockContext context = MockContext.Start(className ?? ClassName, methodName))
            {
                HttpMockServer.Initialize(className ?? ClassName, methodName, mode);
                ILUISRuntimeClient client = GetClient(HttpMockServer.CreateInstance());
                await doTest(client);
                context.Stop();
            }
        }
    }
}
