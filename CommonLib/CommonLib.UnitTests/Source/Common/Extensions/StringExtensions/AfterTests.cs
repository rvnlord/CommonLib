using System;
using CommonLib.Source.Common.Extensions;
using CommonLib.Source.Common.Utils.TypeUtils;
using NUnit.Framework;

namespace CommonLib.UnitTests.Source.Common.Extensions.StringExtensions
{
    [TestFixture]
    public class AfterTests
    {
        [Test]
        public void After_ReturnSubstring()
        {
            const string phrase = "Blue foxes are Cute too";

            Assert.AreEqual(phrase.After("foxes"), " are Cute too");
            Assert.AreEqual(phrase.After("Cute "), "too");
            Assert.AreEqual(phrase.After("Blue"), " foxes are Cute too");
        }

        [Test]
        public void After_ReturnCorrectOccurance()
        {
            const string phrase = "Fox jumped over another fox and there were foxes all around!";
            const string phrase2 = "spmddlTipstersContextMenu";

            Assert.AreEqual(phrase.After("fox"), " and there were foxes all around!");
            Assert.AreEqual(phrase.After("fox", 2), "es all around!");
            Assert.AreEqual(phrase2.After("sp"), "mddlTipstersContextMenu");
        }

        [Test]
        public void After_ReturnCorrectOccuranceFromEnd()
        {
            const string phrase = "Fox jumped over another fox and there were foxes all around!";

            Assert.AreEqual(phrase.After("fox", -1), "es all around!");
            Assert.AreEqual(phrase.After("fox", -2), " and there were foxes all around!");
        }

        [Test]
        public void After_ThrowIfStringIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((string) null).After("fox"));
        }

        [Test]
        public void After_ThrowIfSubstringIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => "fox".After(null));
        }

        [Test]
        public void After_ThrowIfStringDoesntContainSubstring()
        {
            AssertUtils.ThrowsExceptionWithMessage(() => "my house".After("fox"), "String doesn't contain substring");
        }

        [Test]
        public void After_ThrowIfOccuranceIsZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => "fox".After("fox", 0));
        }

        [Test]
        public void After_ThrowIfReturnValueIsEmpty()
        {
            Assert.Throws<NullReferenceException>(() => "hedgehog fox".After("fox"));
        }
        
        [Test]
        public void After_ThrowIfStringAndSubstringAreTheSame()
        {
            AssertUtils.ThrowsExceptionWithMessage(() => "fox".After("fox"), "String equals substring");
        }
    }
}
