using System.Linq;
using CommonLib.Source.Common.Converters;
using NUnit.Framework;

namespace CommonLib.UnitTests.Source.Common.Converters.BitConverter
{
    [TestFixture]
    public class ToVarIntTests
    {
        [Test]
        public static void ToVarInt_CreateCorrectVarInt()
        {
            Assert.AreEqual(7.ToVarInt(), "01000111".BitArrayStringToBitArray());
            Assert.AreEqual(1000.ToVarInt(), "100100001011111".BitArrayStringToBitArray());
            Assert.AreEqual(4095.ToVarInt(), "11010111111111111".BitArrayStringToBitArray());
            Assert.AreEqual(8100.ToVarInt(), "001100010010111111".BitArrayStringToBitArray());
            Assert.AreEqual(8100.ToVarInt(4), "00110010010111111".BitArrayStringToBitArray());
            Assert.AreEqual(8100.ToVarInt(4, 3), "11110010010111111".BitArrayStringToBitArray());
            Assert.AreEqual(Enumerable.Repeat(false, 7).Concat(8100.ToVarInt(4, 3)), "000000011110010010111111".BitArrayStringToBitArray());
        }

        [Test]
        public static void GetFirstVarInt_ConvertByteArrayToVarInt()
        {
            Assert.AreEqual("01000111".BitArrayToByteArray().GetFirstVarInt(), 7);
            Assert.AreEqual("100100001011111".BitArrayToByteArray().GetFirstVarInt(), 1000);
            Assert.AreEqual("11010111111111111".BitArrayToByteArray().GetFirstVarInt(), 4095);
            Assert.AreEqual("001100010010111111".BitArrayToByteArray().GetFirstVarInt(), 8100);
            Assert.AreEqual("00110010010111111".BitArrayToByteArray().GetFirstVarInt(0, 4), 8100);
            Assert.AreEqual("11110010010111111".BitArrayToByteArray().GetFirstVarInt(0, 4, 3), 8100);
            Assert.AreEqual("000000011110010010111111".BitArrayToByteArray().GetFirstVarInt(7, 4, 3), 8100);
        }

        [Test]
        public static void GetFirstVarIntLength_GetCorrectVarIntLength()
        {
            Assert.AreEqual("01000111".BitArrayToByteArray().GetFirstVarIntLength(), 8);
            Assert.AreEqual("100100001011111".BitArrayToByteArray().GetFirstVarIntLength(), 15);
            Assert.AreEqual("11010111111111111".BitArrayToByteArray().GetFirstVarIntLength(), 17);
            Assert.AreEqual("001100010010111111".BitArrayToByteArray().GetFirstVarIntLength(), 18);
            Assert.AreEqual("00110010010111111".BitArrayToByteArray().GetFirstVarIntLength(0, 4), 17);
            Assert.AreEqual("11110010010111111".BitArrayToByteArray().GetFirstVarIntLength(0, 4, 3), 17);
            Assert.AreEqual("000000011110010010111111".BitArrayToByteArray().GetFirstVarIntLength(7, 4, 3), 17);
        }
    }
}
