using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace CommonLib.Source.Common.Extensions
{
    public static class RemoteWebDriverExtensions
    {
        public static bool IsClosed(this RemoteWebDriver driver)
        {
            if (driver?.SessionId == null)
                return true;
            
            try
            {
                var url = driver.Url; // if has url than is reachable
            }
            catch (WebDriverException)
            {
                return true;
            }

            return false;
        }

        public static bool IsOpen(this RemoteWebDriver driver)
        {
            return !driver.IsClosed();
        }

    }
}
