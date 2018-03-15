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
            string inputFilePath = @"C:\Users\robby\Documents\Visual Studio 2013\Projects\FloatDataConverter\FloatDataConverter\input.dat";
            string outputTxtPath = @"C:\Users\robby\Documents\Visual Studio 2013\Projects\FloatDataConverter\FloatDataConverter\output.txt";
            string outputCsvPath = @"C:\Users\robby\Documents\Visual Studio 2013\Projects\FloatDataConverter\FloatDataConverter\output.csv";
            string outputCropTxtPath = @"C:\Users\robby\Documents\Visual Studio 2013\Projects\FloatDataConverter\FloatDataConverter\output_cropped.txt";
            string outputCropCsvPath = @"C:\Users\robby\Documents\Visual Studio 2013\Projects\FloatDataConverter\FloatDataConverter\output_cropped.csv";
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
            Console.WriteLine("- Resolusi Grid: {0}", gridSize);
            Console.WriteLine("- Data dipetakan dari kiri - kanan lalu atas - bawah.");
            Console.WriteLine("- Data pertama pada koordinat {0}, {1}.", firstDataLat.ToLatString(), firstDataLon.ToLonString());
            Console.WriteLine("- Koordinat Crop Data (Indonesia): 2.5°S - 5°S, 102°E - 106°E",
                croptopLatDeg.ToLatString(), cropbottomLatDeg.ToLatString(), cropleftLongDeg.ToLonString(), croprightLongDeg.ToLonString());
            Console.WriteLine(" Tekan enter untuk melanjutkan.");
            Console.ReadKey();

            // TODO: create input method
            try
            {
                // Map input data and export to CSV
                var map = new Map(numDataRow, numDataColumn, gridSize, firstDataLat, firstDataLon.InterpolateLon());
                map.ImportData(inputFilePath);
                map.SaveToFile(outputTxtPath, map.PrintData());
                map.SaveToFile(outputCsvPath, map.PrintDataCsvFormat());

                // Crop Map and export to CSV
                var newMap = map.CropMap(croptopLatDeg, cropbottomLatDeg, cropleftLongDeg.InterpolateLon(), croprightLongDeg.InterpolateLon());
                map.SaveToFile(outputCropTxtPath, newMap.PrintData());
                map.SaveToFile(outputCropCsvPath, newMap.PrintDataCsvFormat());

                Console.WriteLine("Berhasil mengkonversi data. Cek file output.txt dan output_cropped.txt");
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
