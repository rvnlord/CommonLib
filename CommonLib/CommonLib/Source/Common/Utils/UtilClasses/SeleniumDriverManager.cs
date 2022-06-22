using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using CommonLib.Source.Common.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class SeleniumDriverManager : IDisposable
    {
        private static string _previousPage;

        private static readonly List<ChromeDriver> _drivers = new List<ChromeDriver>();
        private ChromeDriver _driver;
        private WebDriverWait _wait;
        private static ChromeDriverService _chromeService;

        public WebDriverWait Wait
        {
            get => _wait ??= new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            set => _wait = value;
        }

        public string Address => _driver.Url;

        private static string FindDriverDirectory()
        {
            const string chromeDriver = "chromedriver.exe";
            var searchDir = AppDomain.CurrentDomain.BaseDirectory.SkipLastWhile(c => c == '\\');
            string driverDir = null;
            var depth = 0;
            while (driverDir == null && depth++ < 4)
            {
                driverDir = Directory.GetFiles(searchDir, chromeDriver, SearchOption.AllDirectories).OrderBy(p => p.Length).FirstOrDefault()?.BeforeLastIgnoreCase(chromeDriver).SkipLastWhile(c => c == '\\');
                searchDir = Directory.GetParent(searchDir).FullName;
            }
            if (driverDir == null)
                throw new ArgumentNullException($"Can't find '{chromeDriver}' directory");
            return driverDir;
        }

        private static string FindBrowserExe()
        {
            const string browserName = "chrome.exe";
            //const string portableBrowserName = "GoogleChromePortable.exe";
            //var searchDir = AppDomain.CurrentDomain.BaseDirectory.SkipLastWhile(c => c == '\\');
            //string browserDir = null;
            //var depth = 0;
            //while (browserDir == null && depth++ < 4)
            //{
            //    browserDir = Directory.GetFiles(searchDir, portableBrowserName, SearchOption.AllDirectories).OrderBy(p => p.Length).FirstOrDefault()?.BeforeLastIgnoreCase(portableBrowserName).SkipLastWhile(c => c == '\\');
            //    searchDir = Directory.GetParent(searchDir).FullName;
            //}

            //if (browserDir != null)
            //    return $@"{browserDir}\{portableBrowserName}";

            var defaultExeLocation = $@"C:\Program Files (x86)\Google\Chrome\Application\{browserName}";

            if (File.Exists(defaultExeLocation))
                return defaultExeLocation;

            throw new ArgumentNullException($"Can't find '{browserName}' in any lcoation");
        }

        public SeleniumDriverManager OpenOrReuseDriver(bool headlessMode = false, bool reuse = true)
        {
            if (_driver.WindowHandles.Count > 0) return this;
            if (reuse && _drivers.Any() && _drivers.Last().WindowHandles.Count > 0)
                _driver = _drivers.Last();
            else
            {
                var driverDir = FindDriverDirectory();
                var binaryLocation = FindBrowserExe();

                if (_chromeService == null)
                    _chromeService = ChromeDriverService.CreateDefaultService(driverDir); // Directory.GetCurrentDirectory() // AppDomain.CurrentDomain.BaseDirectory // Directory.GetCurrentDirectory()}\wwwroot
                var chromeOptions = new ChromeOptions { BinaryLocation = binaryLocation };
                if (headlessMode)
                {
                    chromeOptions.AddArguments(new List<string> 
                    {
                        "no-sandbox",
                        "--silent-launch",
                        "--no-startup-window",
                        "headless",
                        "--disable-dev-shm-usage"
                    });
                    _chromeService.HideCommandPromptWindow = true;
                }
                else
                {
                    chromeOptions.AddArguments(new List<string>
                    {
                        "no-sandbox",
                        //"--disable-dev-shm-usage"
                    });
                }
                    
                _driver = new ChromeDriver(_chromeService, chromeOptions);
                var size = new Size(1240, 720);
                _driver.Manage().Window.Size = size;
                //_driver.Manage().Window.Position = PointUtils.CenteredWindowTopLeft(size); // TODO: figure out a way how to make it cross platform
                _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                _drivers.Add(_driver);
            }

            return this;
        }

        public void DisableWaitingForElements()
        {
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
        }

        public void EnableWaitingForElements()
        {
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
        }

        public void NavigateTo(string address)
        {
            _previousPage = _driver.Url;
            _driver.Navigate().GoToUrl(new Uri(address));
        }

        public void NavigateToAndStopWaitingForUrlAfter(string url, int dontWaitAfter) //, Action actionBeforeLoaded = null
        {
            _previousPage = _driver.Url;
            _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(dontWaitAfter);
            try
            {
                _driver.Navigate().GoToUrl(new Uri(url));
            }
            catch (WebDriverTimeoutException)
            { }

            _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
            Wait.Until(d => d.Url != _previousPage); // czeka na Url automatycznie
        }

        public void NavigateToAndStopWaitingForUrlAfter(Uri uri, int dontWaitAfter)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            NavigateToAndStopWaitingForUrlAfter(uri.ToString(), dontWaitAfter);
        }

        public void ClickAndWaitForUrl(IWebElement webElement)
        {
            if (webElement == null)
                throw new ArgumentNullException(nameof(webElement));

            _previousPage = _driver.Url;
            webElement.Click();
            Wait.Until(d => d.Url != _previousPage);
        }

        public void CloseDriver()
        {
            if (_driver?.SessionId != null)
            {
                _drivers.Remove(_driver);
                _driver.Quit();
                _driver = null;
            }
        }

        public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xPath)
        {
            return _driver.FindElements(By.XPath(xPath));
        }

        public IWebElement FindElementByXPath(string xPath)
        {
            return _driver.FindElement(By.XPath(xPath));
        }

        public IWebElement FindElementByName(string name)
        {
            return _driver.FindElement(By.Name(name));
        }

        public IWebElement FindElementById(string id)
        {
            return _driver.FindElement(By.Id(id));
        }

        public IWebElement FindElementByClassName(string className)
        {
            return _driver.FindElement(By.ClassName(className));
        }

        public IWebElement FindElementByTagName(string tag)
        {
            return _driver.FindElement(By.TagName(tag));
        }

        public IWebElement FindElement(By by)
        {
            return _driver.FindElement(by);
        }

        public bool WaitUntilOrTimeout(Func<IWebDriver, bool> waitUntil)
        {
            try
            {
                Wait.Until(waitUntil);
                return false;
            }
            catch (WebDriverTimeoutException)
            {
                return true;
            }
        }

        public void TryUntilElementAttachedToPage(Action action, bool dontWait = false, int throwOnCatchNum = 10)
        {
            if (action == null) 
                throw new ArgumentNullException(nameof(action));
            if (dontWait)
                DisableWaitingForElements();

            var isExCaught = true;
            var catchCount = 0;
            while (isExCaught)
            {
                try
                {
                    action();
                    isExCaught = false;
                }
                catch (StaleElementReferenceException)
                {
                    isExCaught = true;
                    catchCount++;
                    if (catchCount >= throwOnCatchNum)
                        throw;
                }
            }

            if (dontWait)
                EnableWaitingForElements();
        }

        public void WithoutWaitingForElements(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            DisableWaitingForElements();
            action();
            EnableWaitingForElements();
        }

        public object ExecuteScript(string script, params object[] args)
        {
            return ((IJavaScriptExecutor) _driver).ExecuteScript(script, args);
        }

        public void HideElement(By by)
        {
            var element = _driver.FindElement(by);
            ((IJavaScriptExecutor) _driver).ExecuteScript("arguments[0].style.visibility='hidden'", element);
        }

        public static void CloseAllDrivers()
        {
            try
            {
                foreach (var d in _drivers.Where(d => d?.SessionId != null))
                {
                    d.Quit();
                    d.Dispose();
                }
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                _drivers.Clear();
                _chromeService?.Dispose();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) 
                return;
            if (_driver == null) 
                return;

            _driver.Dispose();
            _driver = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
