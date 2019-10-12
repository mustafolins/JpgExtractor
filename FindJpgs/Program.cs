using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FindJpgs
{
    class Program
    {
        public static int CurrentCount { get; set; } = 1;
        public static string FileName { get; set; } = "";
        public static int WritingToFileCount { get; set; } = 0;

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine($"Usage: FindJpgs InFile OutputFileName");
                return;
            }

            var file = args[0];
            FileName = args?[1] ?? "";
            using (var fs = new FileStream(file, FileMode.Open))
            {
                // the header of the type of file being looked for doesn't have to be
                // jpgs it should work for other file types as well.
                byte[] header = { 0xFF, 0xD8, 0xFF };   // leaving off the last byte of the header
                                                        // in case it's a different version
                                                        // the footer of the type of file to be looked for.
                byte[] footer = { 0xFF, 0xD9 };

                // indicates that the start of the file has been reached
                bool startOfFile = false;

                // initialize list for temporary storage of bytes
                List<byte> readBytes = new List<byte>();
                // loop through entire file to find all potential files
                for (int i = 0; i < fs.Length; i++)
                {
                    // read a byte
                    readBytes.Add((byte)fs.ReadByte());
                    // start of file reached
                    if (!startOfFile && EndsWithArray(readBytes, header))
                    {
                        readBytes.Clear();
                        readBytes.AddRange(header);
                        startOfFile = true;
                    }
                    // foor of file reached
                    else if (EndsWithArray(readBytes, footer))
                    {
                        _ = WriteFile(readBytes.ToArray());
                        readBytes.Clear();
                        startOfFile = false;
                    }
                }

                while (WritingToFileCount > 0)
                {
                    Thread.Sleep(1);
                }
            }
        }

        ///<summary>Writes a file with the given <paramref name="readBytes"/></summary>
        ///<param name="readBytes">The bytes to write to file.</param>
        private static async Task WriteFile(byte[] readBytes)
        {
            WritingToFileCount++;
            await Task.Delay(1);
            using (var fs = new FileStream(FileName + CurrentCount + ".jpg", FileMode.OpenOrCreate))
            {
                using (var bw = new BinaryWriter(fs))
                {
                    for (int i = 0; i < readBytes.Length; i++)
                    {
                        bw.Write(readBytes[i]);
                    }
                }
            }
            CurrentCount++;
            WritingToFileCount--;
        }

        ///<summary>Returns true if the given bytes list ends with the given byteArray.</summary>
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
