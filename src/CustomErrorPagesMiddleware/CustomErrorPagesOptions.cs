namespace CustomErrorPagesMiddleware
{
    using System.Collections.Generic;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class CustomErrorPagesOptions
    {
        private readonly Dictionary<int, AppFunc> _statusCodeHandlers = new Dictionary<int, AppFunc>();

        public CustomErrorPagesOptions WithErrorPage(int statusCode, AppFunc handler)
        {
            _statusCodeHandlers.Add(statusCode, handler);
            return this;
        }

        internal AppFunc GetHandler(int statusCode)
        {
            AppFunc appFunc;
            return _statusCodeHandlers.TryGetValue(statusCode, out appFunc) ? appFunc : null;
        }
    }
}