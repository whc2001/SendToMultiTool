using DamienG.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendToMultiTool
{
    internal class KData
    {
        public static byte[] Encode(byte[] payload)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write(Encoding.ASCII.GetBytes("CNDK"));
                bw.Write(Crc32.Compute(payload));
                bw.Write(payload.Length);
                bw.Write(payload.Length);
                bw.Write(payload);
                return ms.ToArray();
            }
        }

        public static byte[] Decode(byte[] source)
        {
            using (MemoryStream ms = new MemoryStream(source))
            using (BinaryReader br = new BinaryReader(ms))
            {
                byte[] head = br.ReadBytes(4);
                if (!(head[2] == 'D' && head[3] == 'K'))
                    throw new InvalidDataException($"Header Error: {Encoding.ASCII.GetString(head)}");
                br.ReadUInt32();
                uint len1 = br.ReadUInt32();
                uint len2 = br.ReadUInt32();
                byte[] payload = br.ReadBytes((int)Math.Min(len1, len2));
                return payload;
            }
        }
    }
}
