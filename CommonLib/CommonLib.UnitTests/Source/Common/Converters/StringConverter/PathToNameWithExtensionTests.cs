using System.Linq;
using CommonLib.Source.Common.Converters;
using NUnit.Framework;

namespace CommonLib.UnitTests.Source.Common.Converters.StringConverter
{
    [TestFixture]
    public class PathToNameWithExtensionTests
    {
        public static string Path1 { get; set; }  = "IMG-342435.png";
        public static string Path2 { get; set; }  = "/home/user/test.t";
        public static string Path3 { get; set; }  = "C:\\Users\\Desktop\\My Awesome Things\\test.aww";
        public static string Path4 { get; set; }  = "C:\\Users\\Desktop\\My Awesome Things\\test";
        public static string Path5 { get; set; }  = "C:\\Users\\Desktop\\My Awesome Things\\.htacceess";
        public static string Path6 { get; set; }  = "";
        public static string Path7 { get; set; } = "il_1588xN.3668094915_o9q5.jpg";
            
        [Test]
        public static void PathToNameWithExtension_GetCorrectNameWithExtension()
        {
            Assert.AreEqual(Path1.PathToNameWithExtension(), "IMG-342435.png");
            Assert.AreEqual(Path2.PathToNameWithExtension(), "test.t");
            Assert.AreEqual(Path3.PathToNameWithExtension(), "test.aww");
            Assert.AreEqual(Path4.PathToNameWithExtension(), "test");
            Assert.AreEqual(Path5.PathToNameWithExtension(), ".htacceess");
            Assert.AreEqual(Path6.PathToNameWithExtension(), "");
            Assert.AreEqual(Path7.PathToNameWithExtension(), "il_1588xN.3668094915_o9q5.jpg");
        }
    }
}