namespace CustomErrorPagesMiddlewareTests
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using CustomErrorPagesMiddleware;
    using global::CustomErrorPagesMiddlewareTests.Annotations;
    using FluentAssertions;
    using Microsoft.Owin;
    using Microsoft.Owin.Testing;
    using Owin;
    using Xunit;

    public class CustomErrorPagesMiddlewareTests
    {
        [Fact]
        public async Task Can_sepecify_custom_handler_for_status_code()
        {
            using (var server = TestServer.Create<Startup>())
            {
                HttpResponseMessage response = await server.CreateRequest("/").GetAsync();
                string content = await response.Content.ReadAsStringAsync();

                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
                content.Should().Be("Custom 404");
            }
        }

        [UsedImplicitly]
        public class Startup
        {
            [UsedImplicitly]
            public void Configuration(IAppBuilder app)
            {
                app
                    .Use(CustomErrorPagesMiddleware.UseCustomErrorPages(opts => 
                        opts.WithErrorPage(404, async env =>
                        {
                            await new OwinResponse(env).WriteAsync("Custom 404");
                        })))
                    .UseNancy();
            }
        }
    }
}