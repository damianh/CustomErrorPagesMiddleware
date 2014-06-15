namespace CustomErrorPagesMiddleware
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

    public static class CustomErrorPagesMiddleware
    {
        private const string ResponseStatusCodeKey = "owin.ResponseStatusCode";
        private const string ResponseBodyKey = "owin.ResponseBody";
        private const string ServerOnSendingHeaders = "server.OnSendingHeaders";

        public static MidFunc UseCustomErrorPages(Action<CustomErrorPagesOptions> options)
        {
            var errorPagesOptions = new CustomErrorPagesOptions();
            options(errorPagesOptions);
            return UseCustomErrorPages(errorPagesOptions);
        }

        public static MidFunc UseCustomErrorPages(CustomErrorPagesOptions errorPagesOptions)
        {
            return
                next =>
                    async env =>
                    {
                        var onSendingHeaders = env.Get<Action<Action<object>, object>>(ServerOnSendingHeaders);
                        if (onSendingHeaders != null)
                        {
                            var responseBody = env.Get<Stream>(ResponseBodyKey);
                            var streamWrapper = new StreamWrapper(responseBody, async () =>
                            {
                                var statusCode = env.Get<int?>(ResponseStatusCodeKey) ?? 200;
                                AppFunc errorPagehandler = errorPagesOptions.GetHandler(statusCode);
                                if (errorPagehandler != null)
                                {
                                    env[ResponseBodyKey] = responseBody;
                                    await errorPagehandler(env);
                                    return false;
                                }
                                return true;
                            });
                            env[ResponseBodyKey] = streamWrapper;
                        }

                        await next(env);
                    };
        }

        private class StreamWrapper : Stream
        {
            private readonly Stream _innerStream;
            private readonly Func<Task<bool>> _shouldWrite;

            public StreamWrapper(Stream innerStream, Func<Task<bool>> onFirstWrite)
            {
                _innerStream = innerStream;

                bool? shouldWrite = null;

                _shouldWrite = (async () =>
                {
                    if (!shouldWrite.HasValue)
                    {
                        shouldWrite = await onFirstWrite();
                    }
                    return shouldWrite.Value;
                });
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
                if (_shouldWrite().Result)
                {
                    _innerStream.Write(buffer, offset, count);
                }
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
                if (await _shouldWrite())
                {
                    await _innerStream.WriteAsync(buffer, offset, count, cancellationToken);
                }
            }
        }
    }
}
