using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace RedGate.Publishing.InteractiveSql.Web.Tests.Browsing
{
    public class AppBrowser : IDisposable
    {
        public static AppBrowser Open(string hostAddress)
        {
            var driver = WebDriver();
            driver.Url = hostAddress;
            var browser = new AppBrowser(driver);
            try
            {
                browser.WaitForPageLoad();
                return browser;
            }
            catch (Exception)
            {
                browser.Dispose();
                throw;
            }
        }

        private void WaitForPageLoad()
        {
            Wait().Until(_ => !ReadError().Contains("{{error}}"));
        }

        private static FirefoxDriver WebDriver()
        {
            return new FirefoxDriver();
        }

        private readonly FirefoxDriver m_Driver;

        private AppBrowser(FirefoxDriver driver)
        {
            m_Driver = driver;
        }

        public void Dispose()
        {
            m_Driver.Dispose();
        }

        public void SubmitQuery(string query)
        {
            m_Driver.FindElement(By.ClassName("query-input")).SendKeys(query);
            m_Driver.FindElement(ByButtonText("Run it")).Click();

            Wait().Until(_ => ExecutedQuery() == query);
        }

        private string ExecutedQuery()
        {
            return m_Driver.FindElement(By.ClassName("executed-query")).Text;
        }

        private WebDriverWait Wait()
        {
            return new WebDriverWait(m_Driver, TimeSpan.FromSeconds(10));
        }

        public string ReadError()
        {
            return m_Driver.FindElement(By.ClassName("error")).Text;
        }

        public bool IsCorrectAnswer()
        {
            var resultsWidgetText = m_Driver.FindElement(By.ClassName("results-widget")).Text;
            return resultsWidgetText.Contains("Correct answer");
        }

        private By ByButtonText(string text)
        {
            return By.XPath(string.Format(".//button[text()={0}]", XPaths.XPathLiteral(text)));
        }
    }
}
