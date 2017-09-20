using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Asp.Web.IntegrationTests
{
    public class UsersControllerTests : IClassFixture<TestServerFixture>
    {
        private readonly TestServerFixture _fixture;

        public UsersControllerTests(TestServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Login_Get_RenderLoginForm()
        {
            // Arrange & Act
            var response = await _fixture.Client.GetAsync("/Account/Login");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Sign In", responseString);
        }

        [Fact]
        public async Task Login_Post_MissingDomainUsernameAndPassword_NotAcceptPosted()
        {
            // Arrange & Act
            HttpResponseMessage initialResponse = await _fixture.Client.GetAsync("/Account/Login");
            var antiForgeryValues = await _fixture.ExtractAntiForgeryValues(initialResponse);

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Login");

            postRequest.Headers.Add("Cookie", new CookieHeaderValue(TestServerFixture.AntiForgeryCookieName,
                antiForgeryValues.cookieValue).ToString());

            var formData = new Dictionary<string, string>
            {
                {TestServerFixture.AntiForgeryFieldName, antiForgeryValues.fieldValue}
            };

            postRequest.Content = new FormUrlEncodedContent(formData);

            // Assert
            var postResponse = await _fixture.Client.SendAsync(postRequest);
            postResponse.EnsureSuccessStatusCode();
            var responseString = await postResponse.Content.ReadAsStringAsync();

            Assert.Contains("field-validation-error", responseString);
            var regex = new Regex("field-validation-error");
            Assert.Equal(3, regex.Matches(responseString).Count);
        }

        [Fact]
        public async Task Login_Post_ValidCredentials_RedirectToDashboard()
        {
            // Arrange & Act
            HttpResponseMessage initialResponse = await _fixture.Client.GetAsync("/Account/Login");
            var antiForgeryValues = await _fixture.ExtractAntiForgeryValues(initialResponse);

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Login");

            postRequest.Headers.Add("Cookie", new CookieHeaderValue(TestServerFixture.AntiForgeryCookieName,
                antiForgeryValues.cookieValue).ToString());

            var formData = new Dictionary<string, string>
            {
                {TestServerFixture.AntiForgeryFieldName, antiForgeryValues.fieldValue},
                {"Domain", "domain2.com"},
                {"UserName", "johndoe"},
                {"Password", "Sw@*V9Rk"},
            };

            postRequest.Content = new FormUrlEncodedContent(formData);

            // Assert
            var postResponse = await _fixture.Client.SendAsync(postRequest);
            Assert.Equal(HttpStatusCode.Found, postResponse.StatusCode);
            Assert.Equal("/Administration", postResponse.Headers.Location.ToString());
        }
    }
}