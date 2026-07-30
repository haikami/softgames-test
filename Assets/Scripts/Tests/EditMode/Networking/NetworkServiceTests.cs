using System.Text;
using System.Threading.Tasks;
using Core.Networking;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace Core.Tests.EditMode.Networking
{
    /// <summary>
    /// NetworkService tests using a fake web requester
    /// </summary>
    public class NetworkServiceTests
    {
        private const string TestUrl = "https://example.com/data";
        private const string Owner = nameof(NetworkServiceTests);

        private class Payload
        {
            public string name;
        }

        private static byte[] Json(string json) => Encoding.UTF8.GetBytes(json);

        [Test]
        public async Task GetJson_ReturnsSuccess_WhenServerRespondsWithValidJson()
        {
            var requester = new FakeWebRequester(RawResponse.FromHttp(200, Json("{\"name\":\"Dev1\"}")));
            var service = new NetworkService(requester);

            var result = await service.GetJson<Payload>(TestUrl, Owner).AsTask();

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Dev1", result.Value.name);
            Assert.AreEqual(1, requester.CallCount);
        }

        [Test]
        public async Task GetJson_ReturnsParseFailure_WhenBodyIsNotValidJson()
        {
            var requester = new FakeWebRequester(RawResponse.FromHttp(200, Json("not json")));
            var service = new NetworkService(requester);

            var result = await service.GetJson<Payload>(TestUrl, Owner).AsTask();

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(NetworkErrorType.ParseFailure, result.Error.Type);
        }

        [Test]
        public async Task GetJson_RetriesOnServerError_ThenSucceeds()
        {
            var requester = new FakeWebRequester(
                RawResponse.FromHttp(500, null),
                RawResponse.FromHttp(500, null),
                RawResponse.FromHttp(200, Json("{\"name\":\"Dev2\"}")));
            var service = new NetworkService(requester);
            var options = new NetworkRequestOptions(maxRetries: 2, retryDelaySeconds: 0f, timeoutSeconds: 5);

            var result = await service.GetJson<Payload>(TestUrl, Owner, options).AsTask();

            Assert.IsTrue(result.IsSuccess);
            // 1 initial attempt + 2 retries
            Assert.AreEqual(3, requester.CallCount); 
        }

        [Test]
        public async Task GetJson_DoesNotRetry_OnClientError()
        {
            var requester = new FakeWebRequester(RawResponse.FromHttp(404, Json("{\"message\":\"not found\"}")));
            var service = new NetworkService(requester);
            var options = new NetworkRequestOptions(maxRetries: 2, retryDelaySeconds: 0f, timeoutSeconds: 5);

            var result = await service.GetJson<Payload>(TestUrl, Owner, options).AsTask();

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(NetworkErrorType.Http, result.Error.Type);
            Assert.AreEqual(404, result.Error.HttpStatusCode);
            Assert.AreEqual("not found", result.Error.Message);
            // 4xx are not retried because they would return the same value
            Assert.AreEqual(1, requester.CallCount); 
            
        }

        [Test]
        public async Task GetJson_ReturnsUnreachable_AfterExhaustingRetriesOnTransportFailure()
        {
            var requester = new FakeWebRequester(
                RawResponse.TransportFailure("dns failure"),
                RawResponse.TransportFailure("dns failure"));
            var service = new NetworkService(requester);
            var options = new NetworkRequestOptions(maxRetries: 1, retryDelaySeconds: 0f, timeoutSeconds: 5);

            var result = await service.GetJson<Payload>(TestUrl, Owner, options).AsTask();

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(NetworkErrorType.Unreachable, result.Error.Type);
            // 1 initial attempt + 1 retry, then give up
            Assert.AreEqual(2, requester.CallCount);
        }

        [Test]
        public async Task CancelAll_CancelsInFlightRequest_ForMatchingOwner()
        {
            var requester = new FakeWebRequester();
            requester.HangNextCall();
            var service = new NetworkService(requester);

            var pending = service.GetJson<Payload>(TestUrl, Owner).AsTask();

            service.CancelAll(Owner);
            var result = await pending;

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(NetworkErrorType.Cancelled, result.Error.Type);
        }
    }
}
