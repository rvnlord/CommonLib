using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;

namespace CommonLib.Source.Common.Utils.TypeUtils
{
    public static class AssertUtils
    {
        public static void ThrowsExceptionWithMessage(Action func, string message, TestFrameworkType testFramework = TestFrameworkType.NUnit)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var exceptionThrown = false;
            var actualMessage = "";

            try
            {
                func.Invoke();
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                actualMessage = ex.Message;
            }

            if (!exceptionThrown)
            {
                if (testFramework == TestFrameworkType.NUnit)
                    throw new AssertionException($"An exception with message '{message}' was expected, but not thrown");
                throw new AssertFailedException($"An exception with message '{message}' was expected, but not thrown");
            }

            if (actualMessage != message)
            {
                if (testFramework == TestFrameworkType.NUnit)
                    throw new AssertionException($"An exception with message '{message}' was expected, but the message says '{actualMessage}' instead");
                throw new AssertFailedException($"An exception with message '{message}' was expected, but the message says '{actualMessage}' instead");
            }
        }
    }

    public enum TestFrameworkType
    {
        MSTest,
        NUnit
    }
}
