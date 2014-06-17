namespace StatusCodeHandlersMiddleware
{
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Owin;
    using Microsoft.Owin.Testing;
    using Owin;
    using Xunit;

    public class AppBuilderExtensionsTests
    {
        [Fact]
        public async Task When_added_to_app_builder_pipeline_should_invoke_handler()
        {
            const string custom404 = "Custom 404";
            using (var server = TestServer.Create(app => app.UseStatusCodeHandlers(opts => opts.WithErrorPage(404,
                env =>
                {
                    var bytes = Encoding.UTF8.GetBytes(custom404);
                    new OwinResponse(env).Body.Write(bytes, 0, bytes.Length);
                    return Task.FromResult(0);
                }))))
            {
                var response = await server.CreateRequest("/").GetAsync();
                var body = await response.Content.ReadAsStringAsync();

                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
                body.Should().Be(custom404);
            }
        }
    }
}