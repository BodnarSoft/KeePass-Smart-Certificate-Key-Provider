using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("SmartCertificateKeyProviderPlugin")]
[assembly: AssemblyDescription("This plugin uses X.509 certificate from your personal certificate storage to encrypt KeePass database with generated SHA1 hash from certificate's signature. It also allow to use certificates that are on your Smart Card. It is possible to use Secure Desktop feature (needs to be enabled in Options - Security tab) to avoid Keyloggers.\nDON'T LOOSE YOUR CERTIFICATE OR YOU WILL NOT BE ABLE TO OPEN YOUR DATABASE.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("BodnarSoft")]
[assembly: AssemblyProduct("KeePass Plugin")]
[assembly: AssemblyCopyright("Copyright 2018, BodnarSoft")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("1fbc0e45-0c76-4346-967b-10d681fb712e")]