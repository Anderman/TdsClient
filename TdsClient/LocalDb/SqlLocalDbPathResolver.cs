using System;
using Microsoft.Win32;

namespace Medella.TdsClient.LocalDb
{
    public static class SqlLocalDbPathResolver
    {
        private const string LocalDbInstalledVersionRegistryKey = "SOFTWARE\\Microsoft\\Microsoft SQL Server Local DB\\Installed Versions\\";

        private const string InstanceApiPathValueName = "InstanceAPIPath";

        public static string GetDllPath()
        {
            using var key = Registry.LocalMachine.OpenSubKey(LocalDbInstalledVersionRegistryKey);
            if (key == null)
                throw new InvalidOperationException("<sc.SNI.LocalDB.Windows.GetUserInstanceDllPath |SNI|ERR > not installed. Error state ={0}.");

            var zeroVersion = new Version();

            var latestVersion = zeroVersion;

            foreach (var subKey in key.GetSubKeyNames())
                if (Version.TryParse(subKey, out var currentKeyVersion) && latestVersion.CompareTo(currentKeyVersion) < 0)
                    latestVersion = currentKeyVersion;

            // If no valid versions are found, then error out
            if (latestVersion.Equals(zeroVersion))
                throw new InvalidOperationException("<sc.SNI.LocalDB.Windows.GetUserInstanceDllPath |SNI|ERR > Invalid Configuration. state ={0}.");

            // Use the latest version to get the DLL path
            using var latestVersionKey = key.OpenSubKey(latestVersion.ToString());
            var instanceApiPathRegistryObject = latestVersionKey!.GetValue(InstanceApiPathValueName);
            if (instanceApiPathRegistryObject == null)
                throw new InvalidOperationException("<sc.SNI.LocalDB.Windows.GetUserInstanceDllPath |SNI|ERR > No SQL user instance DLL. Instance API Path Registry Object Error. state ={0}.");

            var valueKind = latestVersionKey.GetValueKind(InstanceApiPathValueName);
            if (valueKind != RegistryValueKind.String)
                throw new InvalidOperationException("<sc.SNI.LocalDB.Windows.GetUserInstanceDllPath |SNI|ERR > No SQL user instance DLL. state ={0}. Registry value kind error.");

            var dllPath = (string)instanceApiPathRegistryObject;

            return dllPath;
        }
    }
}