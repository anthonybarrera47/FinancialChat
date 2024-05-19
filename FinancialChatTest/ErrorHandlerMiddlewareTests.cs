using FinancialChat.Middleware;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FinancialChatUnitTest
{
    public class ErrorHandlerMiddlewareTests
    {
        [Fact]
        public async Task Invoke_Should_Return_InternalServerError_When_Exception_Is_Thrown()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var responseStream = new MemoryStream();
            context.Response.Body = responseStream;
            var middleware = new ErrorHandlerMiddleware(next: (innerHttpContext) => throw new Exception("Test exception"));

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = new StreamReader(context.Response.Body).ReadToEnd();
            Assert.Contains("Test exception", responseBody);
        }

        [Fact]
        public async Task Invoke_Should_Continue_Pipeline_When_No_Exception_Is_Thrown()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var responseStream = new MemoryStream();
            context.Response.Body = responseStream;
            var middleware = new ErrorHandlerMiddleware(next: (innerHttpContext) => Task.CompletedTask);

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
        }
    }
}
