using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloatDataConverter
{
    class Program
    {
        private static string path = "";

        /// <summary>
        /// Main Program Float Data Converter
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // TODO: read from configuration file
            if (path == "")
                path = Directory.GetCurrentDirectory();

            Console.WriteLine("Data Converter Program. Press enter to continue...");
            Console.ReadKey();
            ExtractFiles(path);
            ConvertFiles(path);
            Console.WriteLine("Done. Press enter to continue...");
            Console.ReadKey();
        }

        static void ConvertFiles(string path)
        {
            // Convert Files
            DirectoryInfo directorySelected = new DirectoryInfo(path);
            foreach (FileInfo fileToConvert in directorySelected.GetFiles("*.dat"))
            {
                Convert(fileToConvert);
            }
        }

        private static void Convert(FileInfo fileToConvert)
        {
            MapExtensions.ConvertFile(fileToConvert);
        }

        static void ExtractFiles(string path)
        {
            // Decompress
            DirectoryInfo directorySelected = new DirectoryInfo(path);
            foreach (FileInfo fileToDecompress in directorySelected.GetFiles("*.gz"))
            {
                Decompress(fileToDecompress);
            }
        }

        static void Decompress(FileInfo fileToDecompress)
        {
            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            {
                string currentFileName = fileToDecompress.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                        Console.WriteLine("Decompressed: {0}", fileToDecompress.Name);
                    }
                }
            }
        }
    }
}
