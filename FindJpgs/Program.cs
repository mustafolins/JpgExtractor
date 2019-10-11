using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FindJpgs
{
    class Program
    {
        private static int CurrentCount = 1;

        static void Main(string[] args)
        {
            var file = "/home/mustafolins/Documents/Classes/CSIS 3700/GoblinsV2.dd";
            using (var fs = new FileStream(file, FileMode.Open))
            {
                byte[] header = { 0xFF, 0xD8, 0xFF };   // leaving off the last byte of the header
                                                        // in case it's a different version
                byte[] footer = { 0xFF, 0xD9 };

                bool startOfFile = false;

                List<byte> readBytes = new List<byte>();
                for (int i = 0; i < fs.Length; i++)
                {
                    readBytes.Add((byte)fs.ReadByte());
                    if (!startOfFile && EndsWithArray(readBytes, header))
                    {
                        readBytes.Clear();
                        readBytes.AddRange(header);
                        startOfFile = true;
                    }
                    else if (EndsWithArray(readBytes, footer))
                    {
                        WriteFile(readBytes);
                        readBytes.Clear();
                        startOfFile = false;
                    }
                }
            }
        }

        private static void WriteFile(List<byte> readBytes)
        {
            using (var fs = new FileStream("Goblin" + CurrentCount + ".jpg", FileMode.OpenOrCreate))
            {
                using (var bw = new BinaryWriter(fs))
                {
                    for (int i = 0; i < readBytes.Count; i++)
                    {
                        bw.Write(readBytes[i]);
                    }
                }
            }
            CurrentCount++;
        }

        private static bool EndsWithArray(List<byte> bytes, byte[] byteArray)
        {
            if (bytes.Count < byteArray.Length)
                return false;

            for (int i = 0; i < byteArray.Length; i++)
            {
                if (bytes[bytes.Count - byteArray.Length + i] != byteArray[i])
                    return false;
            }

            return true;
        }
    }
}
