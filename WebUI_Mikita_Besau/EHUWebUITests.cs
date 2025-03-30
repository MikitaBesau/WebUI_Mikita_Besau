using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace WebUI_Mikita_Besau
{
    [TestFixture]
    public class EHUWebUITests
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private const string BaseUrl = "https://en.ehu.lt/";

        [SetUp]
        public void Setup()
        {
            driver = new FirefoxDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        //content on the real page is different from the content in the task
        [Test]
        public void VerifyNavigationToAboutPage()
        {
            driver.Navigate().GoToUrl(BaseUrl);
            IWebElement aboutLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("About")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", aboutLink);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", aboutLink);
            wait.Until(ExpectedConditions.UrlToBe("https://en.ehu.lt/about/"));
            Assert.That(driver.Url, Is.EqualTo("https://en.ehu.lt/about/"));
            Assert.That(driver.Title, Is.EqualTo("About"));
            var header = driver.FindElement(By.TagName("h1"));
            Assert.That(header.Text, Does.Contain("About"));
        }

        [Test]
        public void VerifySearchFunctionality()
        {
            driver.Navigate().GoToUrl(BaseUrl);
            var searchButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".header-search")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", searchButton);
            searchButton.Click();
            var searchBox = wait.Until(ExpectedConditions.ElementToBeClickable(By.Name("s")));
            searchBox.SendKeys("study programs");
            searchBox.SendKeys(Keys.Enter);
            wait.Until(ExpectedConditions.UrlContains("/?s=study+programs"));
            Assert.That(driver.Url, Does.Contain("/?s=study+programs"));
            Assert.That(driver.PageSource, Does.Contain("study programs"));
        }

        [Test]
        public void VerifyLanguageChange()
        {
            driver.Navigate().GoToUrl(BaseUrl);
            var mainLanguageSwitcher = wait.Until(
                ExpectedConditions.ElementIsVisible(By.CssSelector("ul.language-switcher > li > a"))
            );
            new Actions(driver).MoveToElement(mainLanguageSwitcher).Perform();
            var ltLink = wait.Until(
                ExpectedConditions.ElementToBeClickable(By.XPath("//ul[@class='language-switcher']//a[contains(@href, 'lt.ehu.lt')]"))
            );
            new Actions(driver).MoveToElement(ltLink).Click().Perform();
            wait.Until(ExpectedConditions.UrlContains("lt.ehu.lt"));
            Assert.That(driver.Url, Is.EqualTo("https://lt.ehu.lt/"));
        }

        [Test]
        public void VerifyContactForm()
        {
            driver.Navigate().GoToUrl("https://en.ehu.lt/contact/");
            Assert.That(driver.PageSource, Does.Contain("franciskscarynacr@gmail.com"));
            Assert.That(driver.PageSource, Does.Contain("+370 68 771365"));
            Assert.That(driver.PageSource, Does.Contain("+375 29 5781488"));
            Assert.That(driver.PageSource, Does.Contain("Facebook"));
            Assert.That(driver.PageSource, Does.Contain("Telegram"));
            Assert.That(driver.PageSource, Does.Contain("VK"));
        }
    }
}
