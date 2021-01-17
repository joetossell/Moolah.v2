using System;
using Microsoft.Extensions.Logging;

namespace Moolah.Authentication.Webhook
{
    internal static class LoggingExtensions
    {
        private static readonly Action<ILogger, string, Exception> _authenticationSchemeSignedIn;
        private static readonly Action<ILogger, string, Exception> _authenticationSchemeSignedOut;

        static LoggingExtensions()
        {
            _authenticationSchemeSignedIn = LoggerMessage.Define<string>(
                eventId: new EventId(10, "AuthenticationSchemeSignedIn"),
                logLevel: LogLevel.Information,
                formatString: "AuthenticationScheme: {AuthenticationScheme} signed in.");
            _authenticationSchemeSignedOut = LoggerMessage.Define<string>(
                eventId: new EventId(11, "AuthenticationSchemeSignedOut"),
                logLevel: LogLevel.Information,
                formatString: "AuthenticationScheme: {AuthenticationScheme} signed out.");
        }

        public static void AuthenticationSchemeSignedIn(this ILogger logger, string authenticationScheme)
        {
            _authenticationSchemeSignedIn(logger, authenticationScheme, null);
        }

        public static void AuthenticationSchemeSignedOut(this ILogger logger, string authenticationScheme)
        {
            _authenticationSchemeSignedOut(logger, authenticationScheme, null);
        }
    }
}