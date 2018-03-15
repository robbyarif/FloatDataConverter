using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloatDataConverter
{
    class Program
    {
        /// <summary>
        /// Main Program Float Data Converter
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Init default parameters
            string inputFilePath = @"input.dat";
            string outputTxtPath = @"output.txt";
            string outputCsvPath = @"output.csv";
            string outputCropTxtPath = @"output_cropped.txt";
            string outputCropCsvPath = @"output_cropped.csv";
            int numDataRow = 480;
            int numDataColumn = 1440;
            double gridSize = 0.25;

            // Initial upper and leftmost data: 59.95°N, 0.05°E
            double firstDataLat = 59.95;
            double firstDataLon = -0.05;

            // South Sumatera area 2.5°S - 5°S and 102°E - 106°E
            double croptopLatDeg = -2.5;
            double cropbottomLatDeg = -5;
            double croprightLongDeg = -106;
            double cropleftLongDeg = -102;

            Console.WriteLine("4-byte Float Binary Data Converter");
            Console.WriteLine("===================================");
            Console.WriteLine("Program akan mengkonversi file {0} menjadi dua file: {1} dan {2}", inputFilePath, outputCsvPath, outputCropCsvPath);
            Console.WriteLine("Saat ini input data diasumsikan");
            Console.WriteLine("- Ukuran Grid: {0} (horizontal) x {1} (vertikal)", numDataColumn, numDataRow);
            Console.WriteLine("- Grid Size: {0}", gridSize);
            Console.WriteLine("- Data dipetakan dari kiri - kanan lalu atas - bawah.");
            Console.WriteLine("- Data pertama pada koordinat {0}, {1}.", firstDataLat.ToLatString(), firstDataLon.ToLonString());
            Console.WriteLine("- Koordinat Crop Data (Indonesia): {0} - {1}, {2} - {3}",
                croptopLatDeg.ToLatString(), cropbottomLatDeg.ToLatString(), cropleftLongDeg.ToLonString(), croprightLongDeg.ToLonString());

            // TODO: create input method
            string input;
            Console.WriteLine(" Gunakan default value? (Y/N)");
            input = Console.ReadLine();
            if (input.Equals("n", StringComparison.OrdinalIgnoreCase))
            {
                Console.Write("File input: "); inputFilePath = Console.ReadLine();
                Console.Write("File output: "); outputCsvPath = Console.ReadLine();
                Console.Write("File cropped output: "); outputCropCsvPath= Console.ReadLine();
                Console.Write("Ukuran Grid Horizontal: "); numDataColumn = Convert.ToInt32(Console.ReadLine());
                Console.Write("Ukuran Grid Vertikal: "); numDataRow = Convert.ToInt32(Console.ReadLine());
                Console.Write("Grid Size: "); gridSize = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine("Gunakan aturan di bawah untuk pengisian Latitude dan Longitude");
                Console.WriteLine("Lat: > 0 untuk North/Utara, < 0 untuk South/Selatan");
                Console.WriteLine("Lon: < 0 untuk East/Bujur Timur, > 0 untuk West/Bujur Barat");
                Console.Write("Data pertama terletak pada Lat: "); firstDataLat = Convert.ToDouble(Console.ReadLine());
                Console.Write("Data pertama terletak pada Lon: "); firstDataLon = Convert.ToDouble(Console.ReadLine());
                Console.Write("Top Crop Lat: "); croptopLatDeg = Convert.ToDouble(Console.ReadLine());
                Console.Write("Bottom Crop Lat: "); cropbottomLatDeg = Convert.ToDouble(Console.ReadLine());
                Console.Write("Left Crop Lon: "); cropleftLongDeg = Convert.ToDouble(Console.ReadLine());
                Console.Write("Right Crop Lon: "); croprightLongDeg = Convert.ToDouble(Console.ReadLine());
            }

            try
            {
                // Map input data and export to CSV
                Console.WriteLine("Importing file {0}...", inputFilePath);
                var map = new Map(numDataRow, numDataColumn, gridSize, firstDataLat, firstDataLon.InterpolateLon());
                Console.WriteLine("Export initial data to file {0}...", outputCsvPath);
                map.ImportData(inputFilePath);
                map.SaveToFile(outputCsvPath, map.PrintDataCsvFormat());

                // Crop Map and export to CSV
                Console.WriteLine("Cropping spatial data...");
                var newMap = map.CropMap(croptopLatDeg, cropbottomLatDeg, cropleftLongDeg.InterpolateLon(), croprightLongDeg.InterpolateLon());
                Console.WriteLine("Export cropped data to file {0}...", outputCropCsvPath);
                map.SaveToFile(outputCropCsvPath, newMap.PrintDataCsvFormat());

                Console.WriteLine("Berhasil mengkonversi data. Cek file {0} dan {1}...", outputCsvPath, outputCropCsvPath);
                Console.ReadKey();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                Console.ReadKey();
            }
        }
    }
}
