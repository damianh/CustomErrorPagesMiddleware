namespace StatusCodeHandlersMiddleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    /// <summary>
    /// Represents options to configure the <see cref="StatusCodeHandlers"/>.
    /// </summary>
    public class StatusCodeHandlersOptions
    {
        private readonly Dictionary<int, AppFunc> _statusCodeHandlers = new Dictionary<int, AppFunc>();

        /// <summary>
        /// Add the handler for the specified HTTP status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>The current <see cref="StatusCodeHandlersOptions"/></returns>
        public StatusCodeHandlersOptions WithHandler(int statusCode, AppFunc handler)
        {
            _statusCodeHandlers.Add(statusCode, handler);
            return this;
        }

        internal AppFunc GetHandler(int statusCode)
        {
            Func<IDictionary<string, object>, Task> appFunc;
            return _statusCodeHandlers.TryGetValue(statusCode, out appFunc) ? appFunc : null;
        }
    }
}