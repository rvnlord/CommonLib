using CommonLib.Source.Common.Extensions;
using NUnit.Framework;

namespace CommonLib.UnitTests.Source.Common.Extensions.StringExtensions
{
    [TestFixture]
    public class AfterIgnoreCaseTests
    {
        [Test]
        public static void AfterIgnoreCase_ReturnSubstring()
        {
            const string phrase = "I used to RULE the WOrlD Seas WOUld rise wHeN I gave the Word";

            Assert.AreEqual(phrase.AfterIgnoreCase("WOrlD"), " Seas WOUld rise wHeN I gave the Word");
            Assert.AreEqual(phrase.AfterIgnoreCase("world"), " Seas WOUld rise wHeN I gave the Word");
            Assert.AreEqual(phrase.AfterIgnoreCase("WOrlD"), " Seas WOUld rise wHeN I gave the Word");
            Assert.AreEqual(phrase.AfterIgnoreCase("WORLD"), " Seas WOUld rise wHeN I gave the Word");
        }
        
        [Test]
        public void AfterIgnoreCase_ReturnCorrectOccurance()
        {
            const string phrase = "Fox jumped over another fOx and there were FOXes all around!";

            Assert.AreEqual(phrase.AfterIgnoreCase("fOX"), " jumped over another fOx and there were FOXes all around!");
            Assert.AreEqual(phrase.AfterIgnoreCase("FoX", 2), " and there were FOXes all around!");
            Assert.AreEqual(phrase.AfterIgnoreCase("fox", 3), "es all around!");
        }

        [Test]
        public void AfterIgnoreCase_ReturnCorrectOccuranceFromEnd()
        {
            const string phrase = "Fox jumped over another fOx and there were FOXes all around!";

            Assert.AreEqual(phrase.AfterIgnoreCase("fOX", -3), " jumped over another fOx and there were FOXes all around!");
            Assert.AreEqual(phrase.AfterIgnoreCase("FoX", -2), " and there were FOXes all around!");
            Assert.AreEqual(phrase.AfterIgnoreCase("fox", -1), "es all around!");
        }
    }
}
