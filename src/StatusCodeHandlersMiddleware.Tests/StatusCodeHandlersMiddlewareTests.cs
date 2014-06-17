namespace StatusCodeHandlersMiddleware
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Owin;
    using Microsoft.Owin.Testing;
    using Owin;
    using Xunit;

    public class StatusCodeHandlersMiddlewareTests
    {
        [Fact]
        public async Task When_middleware_writes_body_then_custom_handler_for_status_code_is_not_invoked()
        {
            const string custom404Message = "Custom 404";
            Action<IAppBuilder> configuration = app => app
                .Use(StatusCodeHandlersMiddleware.UseStatusCodeHandlers(opts =>
                    opts.WithHandler(404, async env =>
                    {
                        await new OwinResponse(env).WriteAsync(custom404Message);
                    })))
                .Use(async (context, next) =>
                {
                    context.Response.StatusCode = 404;
                    byte[] bytes = Encoding.UTF8.GetBytes("404 Body");
                    await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                });

            using (var server = TestServer.Create(configuration))
            {
                HttpResponseMessage response = await server.CreateRequest("/").GetAsync();
                string content = await response.Content.ReadAsStringAsync();

                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
                content.Should().NotEndWith(custom404Message);
            }
        }

        [Fact]
        public async Task When_middleware_does_not_write_body_then_custom_handler_should_be_invoked()
        {
            const string custom404Message = "Custom 404";
            Action<IAppBuilder> configuration = app => app
                .Use(StatusCodeHandlersMiddleware.UseStatusCodeHandlers(opts =>
                    opts.WithHandler(404, async env =>
                    {
                        await new OwinResponse(env).WriteAsync(custom404Message);
                    })))
                .Use((context, next) =>
                {
                    context.Response.StatusCode = 404;
                    return Task.FromResult(0);
                });

            using (var server = TestServer.Create(configuration))
            {
                HttpResponseMessage response = await server.CreateRequest("/").GetAsync();
                string content = await response.Content.ReadAsStringAsync();

                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
                content.Should().Be(custom404Message);
            }
        }

        [Fact]
        public async Task With_custom_exception_handler_that_doesnt_write_a_body_then_should_use_custom_handler()
        {
            Action<IAppBuilder> configuration = app => app
                .Use(StatusCodeHandlersMiddleware.UseStatusCodeHandlers(opts =>
                    opts.WithHandler(500, async env =>
                    {
                        await new OwinResponse(env).WriteAsync("Custom 500");
                    })))
                .Use(async (context, next) =>
                {
                    try
                    {
                        await next();
                    }
                    catch
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ReasonPhrase = "Internal Server Error";
                    }
                })
                .Use((context, next) =>
                {
                    throw new InvalidOperationException("Error");
                });

            using (var server = TestServer.Create(configuration))
            {
                HttpResponseMessage response = await server.CreateRequest("/").GetAsync();
                string content = await response.Content.ReadAsStringAsync();

                response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
                content.Should().Be("Custom 500");
            }
        }

        [Fact]
        public async Task With_custom_exception_handler_that_writes_a_body_then_should_not_use_custom_handler()
        {
            Action<IAppBuilder> configuration = app => app
                .Use(StatusCodeHandlersMiddleware.UseStatusCodeHandlers(opts =>
                    opts.WithHandler(500, async env =>
                    {
                        await new OwinResponse(env).WriteAsync("Custom 500");
                    })))
                .Use(async (context, next) =>
                {
                    try
                    {
                        await next();
                    }
                    catch (Exception ex)
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ReasonPhrase = "Internal Server Error";
                        byte[] bytes = Encoding.UTF8.GetBytes(ex.Message);
                        context.Response.Body.Write(bytes, 0, bytes.Length);
                    }
                })
                .Use((context, next) =>
                {
                    throw new InvalidOperationException("Error");
                });

            using (var server = TestServer.Create(configuration))
            {
                HttpResponseMessage response = await server.CreateRequest("/").GetAsync();
                string content = await response.Content.ReadAsStringAsync();

                response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
                content.Should().Be("Error");
            }
        }
    }
}