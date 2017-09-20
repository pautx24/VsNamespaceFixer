using System;

namespace NamespaceFixer
{
    internal static class Statics
    {
        public const string ProjectFileExtension = "csproj";
    }

    internal static class Guids
    {
        public static readonly Guid NamespaceFixerCmdSet = new Guid("{19492BCB-32B3-4EC3-8826-D67CD5526653}");
        public const string NamespaceFixerPackage = "3C7C5ABE-82AC-4A37-B077-0FF60E8B1FD3";
    }

    internal static class Ids
    {
        public const int CmdIdAdjustNamespace = 0x2001;
    }
}