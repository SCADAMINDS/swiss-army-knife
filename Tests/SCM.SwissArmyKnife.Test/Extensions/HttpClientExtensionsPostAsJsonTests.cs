using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCM.SwissArmyKnife.Extensions;
using Xunit;

namespace SCM.SwissArmyKnife.Test.Extensions
{

    public class HttpClientExtensionsPostAsJsonTests
    {
        [Fact]
        public async Task PostAsJson_ShouldReturnAsJson()
        {
            // Arrange
            var originalDictionary = new Dictionary<string, string>
            {
                {"foo", "bar"}
            };
            var json = JsonConvert.SerializeObject(originalDictionary);

            var handlerMock = HttpMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json)
            });

            var client = new HttpClient(handlerMock.Object);

            // Act
            var dictionaryFromClient = await client.PostAsJsonAsync<Dictionary<string, string>>("http://doesntmatter.com");

            // Assert
            dictionaryFromClient.Should().BeEquivalentTo(originalDictionary);
        }

        [Fact]
        public async Task PostAsJson_ShouldThrowError_WithBodyInIt_OnNonSuccessfulHttpCode()
        {
            // Arrange
            var errorResponse = "some error response";

            var handlerMock = HttpMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent(errorResponse)
            });

            var client = new HttpClient(handlerMock.Object);

            // Act
            Func<Task> action = async () => await client.PostAsJsonAsync<Dictionary<string, string>>("http://doesntmatter.com");

            // Assert
            await action.Should().ThrowAsync<HttpRequestException>().WithMessage($"*{errorResponse}*");
        }

        [Fact]
        public async Task PostAsJson_ShouldThrowError_WithBodyInIt_OnJsonParseException()
        {
            // Arrange
            var serverResponse = "{invalid json}";

            var handlerMock = HttpMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(serverResponse)
            });

            var client = new HttpClient(handlerMock.Object);

            // Act
            Func<Task> action = async () => await client.PostAsJsonAsync<Dictionary<string, string>>("http://doesntmatter.com");

            // Assert
            await action.Should().ThrowAsync<JsonException>().WithMessage($"*{serverResponse}*");
        }

        [Fact]
        public async Task PostAsJson_ShouldThrowError_WithTruncatedBodyInIt_OnNonSuccessfulHttpCode()
        {
            // Arrange
            var errorResponse = "123456789";

            var handlerMock = HttpMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent(errorResponse)
            });

            var client = new HttpClient(handlerMock.Object);

            // Act
            Func<Task> action = async () => await client.PostAsJsonAsync<Dictionary<string, string>>("http://doesntmatter.com", null, 5);

            // Assert
            // Error body only contains 12345 and then a quote '
            await action.Should().ThrowAsync<HttpRequestException>().WithMessage($"*12345...*");
        }

        // This test relies on internet connection which isn't optimal
        [Fact]
        public async Task PostAsJson_ShouldRespectBasePath_WhenCalledWithStringAsUrl()
        {
            // Arrange
            var client = new HttpClient()
            {
                BaseAddress = new Uri("https://postman-echo.com/")
            };

            // Act & Assert
            // This should work without any issues, as the relative url provided is a string
            var response = await client.PostAsJsonAsync<Dictionary<string, object>>("post?foo1=bar1");

            response.Should().ContainKey("args").WhichValue.Should().BeEquivalentTo(new Dictionary<string, JValue>
            {
                {"foo1", new JValue("bar1")}
            });
        }

        // This test relies on internet connection which isn't optimal
        [Fact]
        public async Task PostAsJson_ShouldThrowError_WhenCalledWithRelativeUri()
        {
            // Arrange
            var client = new HttpClient()
            {
                BaseAddress = new Uri("https://postman-echo.com/")
            };

            // Act & Assert
            // This should throw an error because a relative Uri is specified, which is not allowed.
            // It's technically throwing in the "new Uri" part, so it's not the best test.
            Func<Task> actionThatThrows = async () => await client.PostAsJsonAsync<Dictionary<string, object>>(new Uri("/post?foo1=bar1&foo2=bar2"));

            // Throws either ArgumentException or UriFormatException depending on platform
            await actionThatThrows.Should().ThrowAsync<SystemException>()
                .WithMessage("*scheme*"); // error message varies across platform
        }

        // This test relies on internet connection which isn't optimal
        [Fact]
        public async Task PostAsJson_ShouldIgnoreBasePath_WhenCalledWithFullyQualifiedUriInsteadOfString()
        {
            // Arrange
            var client = new HttpClient()
            {
                BaseAddress = new Uri("https://some-not-real-address.com/")
            };

            // Act & Assert
            // This should work as the Uri provided is fully qualified
            var response = await client.PostAsJsonAsync<Dictionary<string, object>>(new Uri("https://postman-echo.com/post?foo1=bar1"));

            response.Should().ContainKey("args").WhichValue.Should().BeEquivalentTo(new Dictionary<string, JValue>
            {
                {"foo1", new JValue("bar1")}
            });
        }

        // This test relies on internet connection which isn't optimal
        [Fact]
        public async Task PostAsJson_ShouldIgnoreBasePath_WhenCalledWithFullyQualifiedUrl()
        {
            // Arrange
            var client = new HttpClient()
            {
                BaseAddress = new Uri("https://some-not-real-address.com/")
            };

            // Act & Assert
            // This should work as the Uri provided is fully qualified
            var response = await client.PostAsJsonAsync<Dictionary<string, object>>("https://postman-echo.com/post?foo1=bar1");

            response.Should().ContainKey("args").WhichValue.Should().BeEquivalentTo(new Dictionary<string, JValue>
            {
                {"foo1", new JValue("bar1")}
            });
        }

        // from https://gingter.org/2018/07/26/how-to-mock-httpclient-in-your-net-c-unit-tests/
        private static Mock<HttpMessageHandler> HttpMessageHandlerMock(HttpResponseMessage response)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    //TODO here we do no validation that the posted content is correct
                    //But we probably don't need to.
                    ItExpr.Is<HttpRequestMessage>(message =>
                        message.Method == HttpMethod.Post
                    ),
                    ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
                .ReturnsAsync(response)
                .Verifiable();
            return handlerMock;
        }
    }
}
