## CommonLib (WIP) 

This is a cross-platform, multi-purpose library containing various extension, utility and converter methods. It is designed to complement existing .NET convenience improvements. I tend to reference this library in all my new projects. It changed over the years and it constantly evolves to include new functionality. For production purposes I often extract what I need from this repo. I publish it in case you wanted to compile one of my other projects that requires it - if it does, it will be explicitly stated in `README` file.
   
### Examples:

While I don't have time to properly document and test everything, here are a few examples to give you a general idea:

```
var path = @"G:\My Files\Programming\CSharp\Projects\MyProgram\MyProgram\obj\Host\bin";
var substring = @"\oBj\HoSt\bIN";
var before = path.BeforeFirstOrWholeIgnoreCase(substring);
```

```
var cipheredText = pt.UTF8ToByteArray()
    .CompressGZip()
    .EncryptECC(keyPair.Person1Private)
    .ToBase58String();

var decryptedText = cipheredText.Base58ToByteArray()
    .DecryptECC(keyPair.Person2Private)
    .DecompressGZip()
    .ToUTF8String();
```

![1](/Images/2022-06-22_165926.png?raw=true)

