# KeePass Smart Certificate Key Provider
This [KeePass 2.x](http://www.keepass.info) plugin is used to protect a KeePass database with [X.509](http://en.wikipedia.org/wiki/X.509) certificate that is: 
* installed in _Windows **My User** account Certificate Store_
* stored on a [Smart Card](http://en.wikipedia.org/wiki/Smart_card).

## Features
What the plugin can do:
* allows to use X.509 certificate installed in Windows
* allows to use X.509 certificate installed on Smart Card
* remembers last used certificate for a particular KeePass database, so you don't have to choose it during each unlock of the database

## Compatibility
It should be compatible with any Smart Card, but it was tested only with: 
* [YUBIKEY NEO](https://www.yubico.com/products/yubikey-hardware/yubikey-neo/)
* [Gold Key Tokens](http://www.goldkey.com/goldkey-security-token/)

## How it works with Smart Cards
After inserting USB token into PC, Windows will automatically install drivers (_if not, please refer to USB token manufacturer pages to download additional drivers_) and registers available certificates into yours Windows from Smart Card.  
This registration is more like a link between the Smart Card and Windows, because of the security. 

The certificate will never leave Smart Card, but Windows will mark it as a "link" and when you would like to use the certificate you would need to enter a PIN of the Smart Card to access selected certificate.

## Why to use Smart Cards
With Smart Cards you add an additional level of security, ensuring that the **KeePass Key file** would not be compromised or stolen by some virus or malware.  
Also if you lose your Smart Card, your certificates are protected by PIN and after entering 3 times wrong PIN, the Smart Card will be locked.

## How the plugin works
It is pretty simple. The plugin will use X.509 certificate to digitally sign some predefined phrase, that is specified in the plugin, with private key of the certificate and uses the output of the signature as "secret key" for the KeePass database.

The plugin doesn't work with private key of the certificate directly, just uses API to generate hashed / encrypted digital signature. 

```C#
rsa.SignData("some text ...", HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
```

>Also if you use Smart Card, you can't access the private key of the certificate by standard API, because you are not allowed to!

You can check for more details about digital RSA signatures on internet like http://www.paradigm.ac.uk/workbook/metadata/authenticity-signatures.html

## Plugin installation
1. download lates plugin from [Releases](../../releases/latest)
2. close running KeePass application
3. copy SmartCertificateKeyProviderPlugin.dll into KeePass directory (*by default C:\Program Files (x86)\KeePass Password Safe 2*)
4. start KeePass application
5. in Open database dialog you will see Key File dropdown, where you can select **Smart Certificate Key Provider**. This also applies for creating or updating KeePass database

>Plugin uses cache that stores information about selected certificate to particular opened database, so you don't have to select same certicate again after database lock. 
>This cache is only in protected memory so after closing KeePass apllication, the cache is lost.

## Development requirements 
The plugin is written in Visual Studio 2017 with C# and Microsoft.NET Framework 4.7.1.  
The KeePass application is included in repository **Dependencies** folder so the project can be build without installation of the KeePass.

## License
This plugin is under [MIT](LICENSE) license.