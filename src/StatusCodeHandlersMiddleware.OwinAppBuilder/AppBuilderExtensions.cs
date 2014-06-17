namespace Owin
{
    using System;
    using StatusCodeHandlersMiddleware;

    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Use the status code handlers middleware in an OWIN pipeline.
        /// </summary>
        /// <param name="appBuilder">An IAppBuilder instance.</param>
        /// <param name="options">The middleware options.</param>
        /// <returns>The IAppBuilder instance.</returns>
        public static IAppBuilder UseStatusCodeHandlers(this IAppBuilder appBuilder, StatusCodeHandlersOptions options)
        {
            Guard.EnsureNotNull(appBuilder, "appBuilder");
            Guard.EnsureNotNull(options, "options");
            
            appBuilder.Use(StatusCodeHandlersMiddleware.UseStatusCodeHandlers(options));
            return appBuilder;
        }

        /// <summary>
        /// Use the status code handlers middleware in an OWIN pipeline that is built using IAppBuilder.
        /// </summary>
        /// <param name="appBuilder">An IAppBuilder instance.</param>
        /// <param name="configureOptions">A delegate to configure the middleware options.</param>
        /// <returns>The IAppBuilder instance.</returns>
        public static IAppBuilder UseStatusCodeHandlers(this IAppBuilder appBuilder, Action<StatusCodeHandlersOptions> configureOptions)
        {
            Guard.EnsureNotNull(appBuilder, "appBuilder");
            Guard.EnsureNotNull(configureOptions, "configureOptions");

            var options = new StatusCodeHandlersOptions();
            configureOptions(options);
            return UseStatusCodeHandlers(appBuilder, options);
        }
    }
}