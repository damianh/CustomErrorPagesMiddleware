namespace StatusCodeHandlersMiddleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class StatusCodeHandlersOptions
    {
        private readonly Dictionary<int, AppFunc> _statusCodeHandlers = new Dictionary<int, AppFunc>();

        public StatusCodeHandlersOptions WithErrorPage(int statusCode, AppFunc handler)
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