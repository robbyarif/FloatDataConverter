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
            string outputFilePath = @"output.csv";
            string outputCropFilePath = @"output_cropped.csv";
            int numDataRow = 480;//8; //480
            int numDataColumn = 1440;//4; //1440
            double gridSize = 0.25;//1;//0.25;

            double firstDataLat = 0.05;
            double firstDataLon = 59.95;

            double topLatDeg = 60;// 4;//60; //north
            double bottomLatDeg = -60;//-4;//-60; //south
            double rightLongDeg = 180; // 2;//180; //west
            double leftLongDeg = -180;//-2;//-180; //east

            double croptopLatDeg = -2.5;//3.5;//60;
            double cropbottomLatDeg = -5;//-4;//0;
            double croprightLongDeg = -106;//-74;//2;//180;
            double cropleftLongDeg = -102;//-78;//-2;//0;

            Console.WriteLine("4-byte Float Binary Data Converter");
            Console.WriteLine("===================================");
            Console.WriteLine("Program akan mengkonversi file {0} menjadi dua file: {1} dan {2}", inputFilePath, outputFilePath, outputCropFilePath );
            Console.WriteLine("Saat ini input data diasumsikan");
            Console.WriteLine("- Ukuran Grid: {0} (horizontal) x {1} (vertikal)", numDataColumn, numDataRow);
            Console.WriteLine("- Resolusi Grid: {0}", gridSize);
            //Console.WriteLine("- Koordinat Data: 60°N - 60°S, 0°E - 180°E - 180°W - 0°W");
            Console.WriteLine("- Data dipetakan dari kiri - kanan lalu atas - bawah.");
            Console.WriteLine("- Data pertama pada koordinat 0.05°E, 59.95°N.", firstDataLon, firstDataLat);
            Console.WriteLine("- Koordinat Crop Data: 2.5°S - 5°S, 102°E - 106°E");
            Console.WriteLine(" Tekan enter untuk melanjutkan.");
            Console.ReadKey();

            // TODO: create input method

            // Map input data and export to CSV
            var map = new Map(
                numDataRow, numDataColumn, gridSize, firstDataLat, firstDataLon,
                topLatDeg, bottomLatDeg, rightLongDeg, leftLongDeg);
            map.ImportData(inputFilePath);
            map.SaveToFile(outputFilePath, map.PrintData());

            // Crop Map and export to CSV
            var newCropLeftLongDeg = InterpolateLon(cropleftLongDeg);
            var newCropRightLongDeg = InterpolateLon(croprightLongDeg);
            var newMap = map.CropMap(croptopLatDeg, cropbottomLatDeg, newCropRightLongDeg, newCropLeftLongDeg);
            //File.WriteAllText(outputCropFilePath, newMap.PrintData());
            map.SaveToFile(outputCropFilePath, map.PrintDataCsvFormat());

            Console.WriteLine("Berhasil mengkonversi data. Cek file output.txt dan output_cropped.txt");
            Console.ReadKey();
        }

        /// <summary>
        /// Interpolate Longitude untuk map dengan Data menggunakan 0 BT 180 BT 180BB 0 BB 
        /// sedangkan program menggunakan 180BB 0 180BT
        /// contoh: 102 (BT) direpresentasikan dlm -102 diinterpolasi menjadi -78
        /// contoh: 3 BB direpresentasikan dlm 3 diinterpolasi menjadi 177
        /// </summary>
        /// <returns>Longitude interpolated based on program requires</returns>
        private static double InterpolateLon(double cropLongDeg)
        {
            return cropLongDeg < 0 ? -1 * (180 + cropLongDeg) : 180 - cropLongDeg;
        }
    }
}
