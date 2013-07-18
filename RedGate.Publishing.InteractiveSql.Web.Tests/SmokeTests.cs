using System;
using RedGate.Publishing.InteractiveSql.SelfHosted;
using RedGate.Publishing.InteractiveSql.Web.Tests.Browsing;
using Xunit;
using Xunit.Sdk;

namespace RedGate.Publishing.InteractiveSql.Web.Tests
{
    public class SmokeTests
    {
        private const int Port = 59911;

        [Fact]
        public void EnteringIncorrectQueryShowsError()
        {
            using (var app = new SelfHostedWebApp(Port))
            {
                app.Start();
                using (var browser = AppBrowser.Open(app.HostAddress))
                {
                    browser.SubmitQuery("Kablam!");
                    Assert.Equal(
                        "There was an error in your SQL query: Incorrect syntax near '!'.",
                        browser.ReadError()
                    );
                    Assert.Equal(false, browser.IsCorrectAnswer());
                }
            }
        }

        [Fact]
        public void CorrectQueryShowsCorrectQueryMessage()
        {
            using (var app = new SelfHostedWebApp(Port))
            {
                app.Start();
                using (var browser = AppBrowser.Open(app.HostAddress))
                {
                    browser.SubmitQuery("SELECT model FROM cars");
                    Assert.Equal("", browser.ReadError());
                    Assert.Equal(true, browser.IsCorrectAnswer());
                }
            }
        }
    }
}
