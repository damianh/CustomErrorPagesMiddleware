namespace Owin
{
    using System;
    using StatusCodeHandlersMiddleware;

    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseStatusCodeHandlers(this IAppBuilder appBuilder, StatusCodeHandlersOptions options)
        {
            Guard.EnsureNotNull(appBuilder, "appBuilder");
            Guard.EnsureNotNull(options, "options");
            
            appBuilder.Use(StatusCodeHandlersMiddleware.UseCustomErrorPages(options));
            return appBuilder;
        }

        public static IAppBuilder UseStatusCodeHandlers(this IAppBuilder appBuilder, Action<StatusCodeHandlersOptions> configureOptions)
        {
            Guard.EnsureNotNull(appBuilder, "appBuilder");
            Guard.EnsureNotNull(configureOptions, "options");

            var options = new StatusCodeHandlersOptions();
            configureOptions(options);
            UseStatusCodeHandlers(appBuilder, options);
            return appBuilder;
        }
    }
}