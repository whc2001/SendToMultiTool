using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWshRuntimeLibrary;

namespace SendToMultiTool
{
    static class AddToSendTo
    {
        public static void CreateShortcut(string name, string cmd, string arg)
        {
            object shAppData = (object)"AppData";
            WshShell shell = new WshShell();
            string shortcutAddress = (string)shell.SpecialFolders.Item(ref shAppData) + $@"\Microsoft\Windows\SendTo\{name}.lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.TargetPath = cmd;
            shortcut.Arguments = arg;
            shortcut.Save();
        }

        public static void Create()
        {
            string appPath = Application.ExecutablePath;
            CreateShortcut("Rename with CRC32 LE", $"{appPath}", "CRC32LE");
            CreateShortcut("Rename with CRC32 BE", $"{appPath}", "CRC32BE");
            CreateShortcut("Rename with MD5 LE", $"{appPath}", "MD5LE");
            CreateShortcut("Rename with MD5 BE", $"{appPath}", "MD5BE"); 
            CreateShortcut("Rename with SHA1 LE", $"{appPath}", "SHA1LE");
            CreateShortcut("Rename with SHA1 BE", $"{appPath}", "SHA1BE"); 
            CreateShortcut("Rename with SHA256 LE", $"{appPath}", "SHA256LE");
            CreateShortcut("Rename with SHA256 BE", $"{appPath}", "SHA256BE");
            CreateShortcut("Fixed Key Table AES Encrypt", $"{appPath}", "AESENC");
            CreateShortcut("Fixed Key Table AES Decrypt", $"{appPath}", "AESDEC");
            CreateShortcut("Wipe Content", $"{appPath}", "WIPE");
        }
    }
}
