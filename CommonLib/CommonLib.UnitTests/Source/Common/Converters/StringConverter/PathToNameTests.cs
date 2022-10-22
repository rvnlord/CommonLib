using CommonLib.Source.Common.Converters;
using NUnit.Framework;
using static CommonLib.UnitTests.Source.Common.Converters.StringConverter.PathToNameWithExtensionTests;

namespace CommonLib.UnitTests.Source.Common.Converters.StringConverter
{
    [TestFixture]
    public class PathToNameTests
    {
        [Test]
        public static void PathToName_GetCorrectName()
        {
            Assert.AreEqual(Path1.PathToName(), "IMG-342435");
            Assert.AreEqual(Path2.PathToName(), "test");
            Assert.AreEqual(Path3.PathToName(), "test");
            Assert.AreEqual(Path4.PathToName(), "test");
            Assert.AreEqual(Path5.PathToName(), ".htacceess");
            Assert.AreEqual(Path6.PathToName(), "");
            Assert.AreEqual(Path7.PathToName(), "il_1588xN.3668094915_o9q5");
        }
    }
}
