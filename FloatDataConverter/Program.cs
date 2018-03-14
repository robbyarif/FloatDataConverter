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

        static void Main(string[] args)
        {
            string inputFilePath = @"input.dat";
            string outputFilePath = @"output.txt";
            string outputCropFilePath = @"output_cropped.txt";
            int numDataRow = 480;//8; //480
            int numDataColumn = 1440;//4; //1440
            double gridSize = 0.25;//1;//0.25;
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
            Console.WriteLine("Program akan mengkonversi file 'input.dat' menjadi dua file: 'output.txt' dan 'output_cropped.txt'");
            Console.WriteLine("Saat ini input data diasumsikan");
            Console.WriteLine("- Ukuran Grid: 1400 (horizontal) x 480 (vertikal)");
            Console.WriteLine("- Resolusi Grid: 0.25");
            //Console.WriteLine("- Koordinat Data: 60°N - 60°S, 0°E - 180°E - 180°W - 0°W");
            Console.WriteLine("- Data dipetakan dari kiri - kanan lalu atas - bawah.");
            Console.WriteLine("- Data pertama pada koordinat 0.05°E, 59.95°N.");
            Console.WriteLine("- Koordinat Crop Data: 2.5°S - 5°S, 102°E - 106°E");
            Console.WriteLine(" Tekan enter untuk melanjutkan.");
            Console.ReadKey();

            // Input file not exists
            if (!File.Exists(inputFilePath))
            {
                throw new ArgumentException("File not found!");
            }
            // Read data from file
            byte[] fileBytes = File.ReadAllBytes(inputFilePath);
            int count = fileBytes.Length / 4;

            // Number of bytes not match with number of data (row x column)
            if (count != (numDataRow * numDataColumn))
            {
                throw new ArgumentException("Number of data not match with row x column");
            }

            // Mapping
            var map = new Map(numDataRow, numDataColumn, gridSize, topLatDeg, bottomLatDeg, rightLongDeg, leftLongDeg);
            using (var reader = new BinaryReader(File.OpenRead(inputFilePath)))
            {
                for (int i = 0; i < numDataRow; i++)
                {
                    for (int j = 0; j < numDataColumn; j++)
                    {
                        map.data[i, j] = reader.ReadSingle();
                    }
                }
            }
            File.WriteAllText(outputFilePath, map.PrintData());

            // Interpolate Longitude. 
            // Data menggunakan 0 BT 180 BT 180BB 0 BB sedangkan program menggunakan 180BB 0 180BT
            // contoh: 102 (BT) direpresentasikan dlm -102 diinterpolasi menjadi -78
            // contoh: 3 BB direpresentasikan dlm 3 diinterpolasi menjadi 177
            var newCropLeftLongDeg = cropleftLongDeg < 0 ? -1 * (180 + cropleftLongDeg) : 180 - cropleftLongDeg;
            var newCropRightLongDeg = croprightLongDeg < 0 ? -1 * (180 + croprightLongDeg) : 180 - croprightLongDeg;

            var newMap = map.CropMap(croptopLatDeg, cropbottomLatDeg, newCropRightLongDeg, newCropLeftLongDeg);
            File.WriteAllText(outputCropFilePath, newMap.PrintData());

            Console.WriteLine("Berhasil mengkonversi data. Cek file output.txt dan output_cropped.txt");
            Console.ReadKey();
        }
    }
}
