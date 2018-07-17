using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloatDataConverter
{
    public static class MapExtensions
    {
        // Init default parameters
        private static string inputFilePath = @"input.dat";
        private static string outputCsvPath = @"output.csv";
        private static string outputCropCsvPath = @"output_cropped.csv";
        private static int numDataRow = 1200;
        private static int numDataColumn = 3600;
        private static double gridSize = 0.1;

        // Initial upper and leftmost data: 59.95°N, 0.05°E
        private static double firstDataLat = 59.95;
        private static double firstDataLon = 0.05;

        // South Sumatera area 1.5°S - 5°S and 102°E - 106°E
        private static double croptopLatDeg = -1.5;
        private static double cropbottomLatDeg = -5;
        private static double croprightLongDeg = 106;
        private static double cropleftLongDeg = 102;


        public static void ConvertData()
        {
            
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

            string input;
            Console.WriteLine(" Gunakan default value? (Y/N)");
            input = Console.ReadLine();
            if (input.Equals("n", StringComparison.OrdinalIgnoreCase))
            {
                Console.Write("File input: "); inputFilePath = Console.ReadLine();
                Console.Write("File output: "); outputCsvPath = Console.ReadLine();
                Console.Write("File cropped output: "); outputCropCsvPath = Console.ReadLine();
                Console.Write("Ukuran Grid Horizontal: "); numDataColumn = Convert.ToInt32(Console.ReadLine());
                Console.Write("Ukuran Grid Vertikal: "); numDataRow = Convert.ToInt32(Console.ReadLine());
                Console.Write("Grid Size: "); gridSize = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine("Gunakan aturan di bawah untuk pengisian Latitude dan Longitude");
                Console.WriteLine("Lat: > 0 untuk North/Utara, < 0 untuk South/Selatan");
                Console.WriteLine("Lon: > 0 untuk East/Bujur Timur, < 0 untuk West/Bujur Barat");
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

        public static void ConvertFile(FileInfo file)
        {
            // Map input data and export to CSV
            var map = new Map(numDataRow, numDataColumn, gridSize, firstDataLat, firstDataLon.InterpolateLon());
            map.ImportData(file.FullName);

            // Crop Map and export to CSV
            var currentFilePath = file.FullName;
            var outputFilePath = currentFilePath.Remove(currentFilePath.Length - file.Extension.Length) + "_crop.csv";
            var newMap = map.CropMap(croptopLatDeg, cropbottomLatDeg, cropleftLongDeg.InterpolateLon(), croprightLongDeg.InterpolateLon());
            map.SaveToFile(outputFilePath, newMap.PrintDataCsvFormat());
            Console.WriteLine("Converted: {0}", file);
        }

        public static string ToLatString(this double lat)
        {
            return lat >= 0 ? string.Format("{0}°N", lat) : string.Format("{0}°S", Math.Abs(lat));
        }
        public static string ToLonString(this double lon)
        {
            return lon < 0 ? string.Format("{0}°W", lon) : string.Format("{0}°E", Math.Abs(lon));
        }

        /// <summary>
        /// Interpolate Longitude untuk map dengan Data menggunakan 0 BT 180 BT 180BB 0 BB 
        /// sedangkan program menggunakan koordinat kartesian 180 0 180
        /// contoh: 102 BT direpresentasikan dlm 102 diinterpolasi menjadi -78
        /// contoh: 3 BB direpresentasikan dlm -3 diinterpolasi menjadi 177
        /// </summary>
        /// <returns>Longitude interpolated based on program requires</returns>
        public static double InterpolateLon(this double lonDeg)
        {
            return lonDeg < 0 ? 180 + lonDeg : -1 * (180 - lonDeg);
        }

        /// <summary>
        /// Return interpolated longitude (-180 0 180) to (0E 180E 180W 0W)
        /// </summary>
        /// <param name="lon"></param>
        /// <returns></returns>
        public static double DeInterpolateLon(this double lon)
        {
            return lon < 0 ? 180 + lon : -1 * (180 - lon);
        }
    }
}
