using DamienG.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SendToMultiTool
{
    static class Program
    {        public static void RenameFile(FileInfo file, string mode)
        {
            try
            {
                byte[] content = File.ReadAllBytes(file.FullName);
                string newHash = "";
                byte[] newContent = null;
                switch (mode)
                {
                    case "CRC32LE":
                        newHash = Utils.ReverseBytes(Crc32.Compute(content)).ToString("X8");
                        break;
                    case "CRC32BE":
                        newHash = Crc32.Compute(content).ToString("X8");
                        break;
                    case "MD5LE":
                        newHash = Utils.ByteArrayToHexString(Utils.ReverseBytes(new MD5CryptoServiceProvider().ComputeHash(content)));
                        break;
                    case "MD5BE":
                        newHash = Utils.ByteArrayToHexString(new MD5CryptoServiceProvider().ComputeHash(content));
                        break;
                    case "SHA1LE":
                        newHash = Utils.ByteArrayToHexString(Utils.ReverseBytes(new SHA1CryptoServiceProvider().ComputeHash(content)));
                        break;
                    case "SHA1BE":
                        newHash = Utils.ByteArrayToHexString(new SHA1CryptoServiceProvider().ComputeHash(content));
                        break;
                    case "SHA256LE":
                        newHash = Utils.ByteArrayToHexString(Utils.ReverseBytes(new SHA256CryptoServiceProvider().ComputeHash(content)));
                        break;
                    case "SHA256BE":
                        newHash = Utils.ByteArrayToHexString(new SHA256CryptoServiceProvider().ComputeHash(content));
                        break;
                    case "AESENC":
                        newContent = FixedKeyAes.Encrypt(content);
                        break;
                    case "AESDEC":
                        newContent = FixedKeyAes.Decrypt(content);
                        break;
                    case "WIPE":
                        newContent = new byte[0];
                        break;
                    default:
                        throw new InvalidOperationException($"不支持的方法: {mode}");
                }
                if (newContent != null)
                    File.WriteAllBytes(file.FullName, newContent);
                if(!string.IsNullOrEmpty(newHash))
                    file.MoveTo($"{file.Directory.FullName}\\{newHash}{file.Extension}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"重命名错误: {ex.Message}{Environment.NewLine}{Environment.NewLine}{ex.StackTrace}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void RenameDirectoryRecursively(DirectoryInfo dir, string mode)
        {
            foreach (FileInfo file in dir.EnumerateFiles())
                RenameFile(file, mode);
            foreach (DirectoryInfo subDir in dir.EnumerateDirectories())
                RenameDirectoryRecursively(subDir, mode);
        }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                AddToSendTo.Create();
            }
            else
            {
                try
                {
                    Queue<string> param = new Queue<string>(args);
                    string mode = param.Dequeue().ToUpper();
                    if (mode.Contains("AES"))
                        FixedKeyAes.InitKeys();
                    while (param.Count > 0)
                    {
                        string item = param.Dequeue();
                        if (Utils.IsDirectory(item))
                            RenameDirectoryRecursively(new DirectoryInfo(item), mode);
                        else
                            RenameFile(new FileInfo(item), mode);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"错误: {ex.Message}{Environment.NewLine}{Environment.NewLine}{ex.StackTrace}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
