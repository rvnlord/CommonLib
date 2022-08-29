using System;
using System.Linq;
using System.Xml.Schema;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions;
using CommonLib.Source.Common.Extensions.Collections;
using CommonLib.Source.Common.Utils;

namespace CommonLib.Console
{
    public class Program
    {
        public static void Main()
        {
            var vi1 = 7.ToVarInt();
            var vi2 = 1000.ToVarInt();
            var vi3 = 4095.ToVarInt();
            var vi4 = 8100.ToVarInt();
            var vi5 = 8100.ToVarInt(4);
            var vi6 = 8100.ToVarInt(4, 3);
            var vi7 = Enumerable.Repeat(false, 7).Concat(8100.ToVarInt(4, 3));

            var vis1 = vi1.ToBitArrayString();
            var vis2 = vi2.ToBitArrayString();
            var vis3 = vi3.ToBitArrayString();
            var vis4 = vi4.ToBitArrayString();
            var vis5 = vi5.ToBitArrayString();
            var vis6 = vi6.ToBitArrayString();
            var vis7 = vi7.ToBitArrayString();

            var v = vi5.BitArrayToByteArray().GetFirstVarIntLength(0, 4);

            var path = @"G:\My Files\Programming\CSharp\Projects\MyProgram\MyProgram\obj\Host\bin";
            var substring = @"\oBj\HoSt\bIN";
            var before = path.BeforeFirstOrWholeIgnoreCase(substring);

            //var obj = new Node {
            //    Name = "one",
            //    Child = new Node {
            //        Name = "two",
            //        Child = new Node {
            //            Name = "three"
            //        }
            //    }
            //};
            //var json = new { e1 = new { e2 = new { e3 = "last" }, e21 = "test21" } };
            //var json2 = new { e1 = new { e2 = new { e3 = "last" } } };


            //var txt2 = obj.JsonSerialize(2);
            //var txt3 = json.JsonSerialize(2);
            //var txt4 = json2.JsonSerialize(2);
            //var o2 = obj.ToJToken(1);

            //var t = txt2.JsonDeserialize();
            //var t2 = txt2.ToJToken();

            //System.Console.WriteLine("Test completed");

            //var pt = "Test string to encrypt.";
            //var senderKeyPair = CryptoUtils.GenerateECKeyPair();
            //var receiverKeyPair = CryptoUtils.GenerateECKeyPair();

            //var cipheredText = pt.UTF8ToByteArray()
            //    .EncryptECC(senderKeyPair.Private.ToECPrivateKeyByteArray(), receiverKeyPair.Public.ToECPublicKeyByteArray())
            //    .ToBase58String();

            //var decryptedText = cipheredText.Base58ToByteArray()
            //    .DecryptECC(receiverKeyPair.Private.ToECPrivateKeyByteArray(), senderKeyPair.Public.ToECPublicKeyByteArray())
            //    .ToUTF8String();
            
            var pt = "Long plain text";
            var keyPair = CryptoUtils.GenerateECCKeyPair();

            var cipheredText = pt.UTF8ToByteArray()
                .CompressGZip()
                .EncryptECC(keyPair.Person1Private)
                .ToBase58String();

            var decryptedText = cipheredText.Base58ToByteArray()
                .DecryptECC(keyPair.Person2Private)
                .DecompressGZip()
                .ToUTF8String();

            var beforeOrWhole = "?keepPrompt=true".BeforeFirstOrWhole("?");

            //System.Console.WriteLine($"1st priv key: {keyPair.Person1Private.ToBase58String()}");
            //System.Console.WriteLine($"2nd priv key: {keyPair.Person2Private.ToBase58String()}");
            //System.Console.WriteLine($"plain text: {pt}");
            //System.Console.WriteLine($"compressed plain text: {pt.UTF8ToByteArray().CompressGZip().ToBase58String()}");
            //System.Console.WriteLine($"encrypted text: {cipheredText}");
            //System.Console.WriteLine($"decrypted text: {decryptedText}");
        }
    }

    public class Node
    {
        public string Name { get; set; }
        public Node Child { get; set; }
    }
}
