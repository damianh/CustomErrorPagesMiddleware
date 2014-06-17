namespace StatusCodeHandlersMiddleware
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
    using MidFunc = System.Func<
        System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>,
        System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>
        >;

    /// <summary>
    ///     OWIN middleware to allow a custom handler for specified status codes, when chile middleware has set a code but NOT written a body.
    /// </summary>
    /// <remarks>
    ///     A handler is only invoked if child middleware has set a status code but not written anything to the response body stream.
    /// </remarks>
    public static class StatusCodeHandlersMiddleware
    {
        private const string ResponseStatusCodeKey = "owin.ResponseStatusCode";
        private const string ResponseBodyKey = "owin.ResponseBody";

        /// <summary>
        /// Use the status code handlers middleware in an OWIN pipeline.
        /// </summary>
        /// <param name="configureOptions">A delegate to configure the middleware options.</param>
        /// <returns>An OWIN middleware function.</returns>
        public static MidFunc UseStatusCodeHandlers(Action<StatusCodeHandlersOptions> configureOptions)
        {
            var options = new StatusCodeHandlersOptions();
            configureOptions(options);
            return UseStatusCodeHandlers(options);
        }

        /// <summary>
        /// Use the status code handlers middleware in an OWIN pipeline.
        /// </summary>
        /// <param name="options">The middleware options.</param>
        /// <returns>An OWIN middleware function.</returns>
        public static MidFunc UseStatusCodeHandlers(StatusCodeHandlersOptions options)
        {
            return
                next =>
                    async env =>
                    {
                        var responseBody = env.Get<Stream>(ResponseBodyKey);
                        var streamWrapper = new StreamWrapper(responseBody);
                        env[ResponseBodyKey] = streamWrapper;

                        await next(env);

                        if (!streamWrapper.WriteOccured)
                        {
                            var statusCode = env.Get<int?>(ResponseStatusCodeKey) ?? 200;
                            AppFunc handler = options.GetHandler(statusCode);
                            if (handler != null)
                            {
                                await handler(env);
                            }
                        }
                    };
        }

        private class StreamWrapper : Stream
        {
            private readonly Stream _innerStream;
            private bool _writeOccured;

            public bool WriteOccured
            {
                get { return _writeOccured; }
            }

            public StreamWrapper(Stream innerStream)
            {
                _innerStream = innerStream;
            }

            public override void Flush()
            {
                _innerStream.Flush();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _innerStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _innerStream.SetLength(value);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _innerStream.Read(buffer, offset, count);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _writeOccured = true;
                _innerStream.Write(buffer, offset, count);
            }

            public override bool CanRead
            {
                get { return _innerStream.CanRead; }
            }

            public override bool CanSeek
            {
                get { return _innerStream.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return _innerStream.CanWrite; }
            }

            public override long Length
            {
                get { return _innerStream.Length; }
            }

            public override long Position
            {
                get { return _innerStream.Position; }
                set { _innerStream.Position = value; }
            }

            public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                _writeOccured = true;
                await _innerStream.WriteAsync(buffer, offset, count, cancellationToken);
            }
        }
    }
}
