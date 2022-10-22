using CommonLib.Source.Common.Converters;
using NUnit.Framework;
using static CommonLib.UnitTests.Source.Common.Converters.StringConverter.PathToNameWithExtensionTests;

namespace CommonLib.UnitTests.Source.Common.Converters.StringConverter
{
    [TestFixture]
    public class PathToExtensionTests
    {
        [Test]
        public static void PathToExtension_GetCorrectExtension()
        {
            Assert.AreEqual(Path1.PathToExtension(), "png");
            Assert.AreEqual(Path2.PathToExtension(), "t");
            Assert.AreEqual(Path3.PathToExtension(), "aww");
            Assert.AreEqual(Path4.PathToExtension(), "");
            Assert.AreEqual(Path5.PathToExtension(), "");
            Assert.AreEqual(Path6.PathToExtension(), "");
            Assert.AreEqual(Path7.PathToExtension(), "jpg");
        }
    }
}