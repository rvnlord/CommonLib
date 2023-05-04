using System;
using System.Drawing;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Utils.UtilClasses;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CommonLib.Source.Common.Extensions
{
    public static class IWebElementExtensions
    {
        public static void ScrollDown(this IWebElement element, int pixels)
        {
            var driver = element.GetDriver();
            var js = (IJavaScriptExecutor) driver;
            long getScrollTop() => js.ExecuteScript("return arguments[0].scrollTop;", element).ToLong();
            var scrollTop = getScrollTop();
            js.ExecuteScript("arguments[0].scrollTop += arguments[1];", element, pixels);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            try
            {
                wait.Until(d =>
                {
                    var scrolledTo = getScrollTop();
                    return Math.Abs(scrolledTo - scrollTop - pixels) <= 2;
                });
            }
            catch (Exception)
            {
                var scrolledTo = getScrollTop();
                var exactScrolledTo = (double) js.ExecuteScript("return arguments[0].scrollTop;", element);
                Logger.For(typeof(IWebElementExtensions)).Warn($" Math.Abs(scrolledTo - scrollTop - pixels) = {Math.Abs(scrolledTo - scrollTop - pixels)}: scrolledTo = {scrolledTo}, scrollTop = {scrollTop}, pixels = {pixels}, exactScrolledTo = {exactScrolledTo:0.0000}");
                js.ExecuteScript("arguments[0].scrollTop += arguments[1];", element, Math.Abs(scrolledTo - scrollTop - pixels));
            }
        }

        private static IWebDriver GetDriver(this IWebElement element)
        {
            var driver = ((IWrapsDriver)element).WrappedDriver;
            return driver;
        }
    }
}
