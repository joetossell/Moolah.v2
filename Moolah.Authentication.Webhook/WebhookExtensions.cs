// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Moolah.Authentication.Webhook;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WebhookExtensions
    {
        public static AuthenticationBuilder AddWebhook<TWebhookManager>(this AuthenticationBuilder builder)
            where TWebhookManager : class, IWebhookManager
            => builder.AddWebhook<TWebhookManager>(WebhookAuthenticationDefaults.AuthenticationScheme);

        public static AuthenticationBuilder AddWebhook<TWebhookManager>(this AuthenticationBuilder builder, string authenticationScheme)
            where TWebhookManager : class, IWebhookManager
            => builder.AddWebhook<TWebhookManager>(authenticationScheme, configureOptions: (Action<WebhookAuthenticationOptions>)null);

        public static AuthenticationBuilder AddWebhook<TWebhookManager>(this AuthenticationBuilder builder, Action<WebhookAuthenticationOptions> configureOptions)
            where TWebhookManager : class, IWebhookManager
            => builder.AddWebhook<TWebhookManager>(WebhookAuthenticationDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddWebhook<TWebhookManager>(this AuthenticationBuilder builder, string authenticationScheme, Action<WebhookAuthenticationOptions> configureOptions)
            where TWebhookManager : class, IWebhookManager
            => builder.AddWebhook<TWebhookManager>(authenticationScheme, displayName: null, configureOptions: configureOptions);

        public static AuthenticationBuilder AddWebhook<TWebhookManager>(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<WebhookAuthenticationOptions> configureOptions)
            where TWebhookManager : class, IWebhookManager
        {
            builder.Services.AddFactory<IWebhookManager, TWebhookManager>();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<WebhookAuthenticationOptions>, PostConfigureWebhookAuthenticationOptions>());
            builder.Services.AddOptions<WebhookAuthenticationOptions>(authenticationScheme);
            return builder.AddScheme<WebhookAuthenticationOptions, WebhookAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }

        private static void AddFactory<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddTransient<TService, TImplementation>();
            services.AddSingleton<Func<TService>>(x => () => x.GetService<TService>());
        }
    }
}
