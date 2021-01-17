//https://elanderson.net/2019/12/log-requests-and-responses-in-asp-net-core-3/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;

namespace Moolah.Monzo.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TelemetryClient _telemetryClient;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public RequestResponseLoggingMiddleware(RequestDelegate next, TelemetryClient telemetryClient)
        {
            _next = next;
            _telemetryClient = telemetryClient;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext context)
        {
            await LogRequest(context);
            await LogResponse(context);
        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();

            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            _telemetryClient.TrackTrace($"Http Request Detail:{Environment.NewLine}" +
                                        $"Schema:{context.Request.Scheme} " +
                                        $"Host: {context.Request.Host} " +
                                        $"Path: {context.Request.Path} " +
                                        $"QueryString: {context.Request.QueryString} " +
                                        $"Request Body: {ReadStreamInChunks(requestStream)}",
                SeverityLevel.Information,
                new Dictionary<string, string>
                {
                    { "HttpBody", "Request" },
                    { "Schema", context.Request.Scheme },
                    { "Host", context.Request.Host.ToString() },
                    { "Path", context.Request.Path }
                });
            context.Request.Body.Position = 0;
        }

        private async Task LogResponse(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _telemetryClient.TrackTrace($"Http Response Detail:{Environment.NewLine}" +
                                        $"Schema:{context.Request.Scheme} " +
                                        $"Host: {context.Request.Host} " +
                                        $"Path: {context.Request.Path} " +
                                        $"QueryString: {context.Request.QueryString} " +
                                        $"Response Body: {text}",
                SeverityLevel.Information,
                new Dictionary<string, string>
                {
                    { "HttpBody", "Response" },
                    { "Schema", context.Request.Scheme },
                    { "Host", context.Request.Host.ToString() },
                    { "Path", context.Request.Path }
                });

            await responseBody.CopyToAsync(originalBodyStream);
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;

            stream.Seek(0, SeekOrigin.Begin);

            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;

            do
            {
                readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            return textWriter.ToString();
        }
    }
}
