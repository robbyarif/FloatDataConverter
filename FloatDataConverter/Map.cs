using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloatDataConverter
{
    public class Map
    {
        public float[,] data;
        public int numRow;
        public int numColumn;
        public double gridSize;

        public double firstDataLat;
        public double firstDataLon;

        public double topLatDeg;
        public double bottomLatDeg;
        public double rightLongDeg;
        public double leftLongDeg;

        public Map(int numRow, int numColumn, double gridSize, double firstDataLat, double firstDataLon,
            double topLatDeg, double bottomLatDeg, double rightLongDeg, double leftLongDeg)
        {
            this.data = new float[numRow, numColumn];
            this.numRow = numRow;
            this.numColumn = numColumn;
            this.gridSize = gridSize;
            this.firstDataLat = firstDataLat;
            this.firstDataLon = firstDataLon;
            this.topLatDeg = topLatDeg;
            this.bottomLatDeg = bottomLatDeg;
            this.rightLongDeg = rightLongDeg;
            this.leftLongDeg = leftLongDeg;
        }

        /// <summary>
        /// Fill in map data from path
        /// </summary>
        /// <param name="inputFilePath"></param>
        public void ImportData(string inputFilePath)
        {
            // Input file not exists validation
            if (!File.Exists(inputFilePath))
            {
                throw new ArgumentException("File not found!");
            }

            // Input file number of data not match validation
            byte[] fileBytes = File.ReadAllBytes(inputFilePath);
            int count = fileBytes.Length / 4;
            if (count != (this.numRow * this.numColumn))
            {
                throw new ArgumentException("Number of data not match with row x column");
            }

            // Map Data
            using (var reader = new BinaryReader(File.OpenRead(inputFilePath)))
            {
                for (int i = 0; i < this.numRow; i++)
                {
                    for (int j = 0; j < this.numColumn; j++)
                    {
                        this.data[i, j] = reader.ReadSingle();
                    }
                }
            }
        }

        /// <summary>
        /// Returns new map that represent corresponding cropped map
        /// </summary>
        /// <param name="croptopLatDeg"></param>
        /// <param name="cropbottomLatDeg"></param>
        /// <param name="croprightLongDeg"></param>
        /// <param name="cropleftLongDeg"></param>
        /// <returns></returns>
        public Map CropMap(double croptopLatDeg, double cropbottomLatDeg, double croprightLongDeg, double cropleftLongDeg)
        {
            var rowOffset = (int)((topLatDeg - croptopLatDeg) / gridSize);
            var rowCount = (int)((croptopLatDeg - cropbottomLatDeg) / gridSize);
            var columnOffset = (int)(-1 * (leftLongDeg - cropleftLongDeg) / gridSize);
            var columnCount = (int)(-1 * (cropleftLongDeg - croprightLongDeg) / gridSize);



            var newRow = rowCount;
            var newColumn = columnCount;
            var newGridSize = gridSize;
            var newTopLatDeg = topLatDeg + (rowOffset * gridSize);
            var newBottomLatDeg = newTopLatDeg + (rowCount * gridSize);
            var newLeftLongDeg = leftLongDeg + (columnOffset * gridSize);
            var newRightLongDeg = newLeftLongDeg + (columnCount * gridSize);
            var newMap = new Map(newRow, newColumn, newGridSize, newTopLatDeg, newBottomLatDeg, newRightLongDeg, newLeftLongDeg);

            for (int i = rowOffset; i < rowOffset + rowCount; i++)
            {
                for (int j = columnOffset; j < columnOffset + columnCount; j++)
                {
                    newMap.data[i - rowOffset, j - columnOffset] = data[i, j];
                }
            }
            return newMap;
        }

        /// <summary>
        /// Return string for plain text format
        /// </summary>
        /// <returns></returns>
        public string PrintData()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < numRow; i++)
            {
                for (int j = 0; j < numColumn; j++)
                {
                    sb.AppendFormat("{0,15} ", data[i, j].ToString());
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Return string for CSV file format
        /// </summary>
        /// <returns></returns>
        public string PrintDataCsvFormat()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("{0}, {1}, {2}", "Data Value", "Latitude", "Longitude"));
            var currentLat = this.firstDataLat;
            var currentLon = this.firstDataLon;
            for (int i = 0; i < numRow; i++)
            {
                for (int j = 0; j < numColumn; j++)
                {
                    sb.AppendFormat("{0,15}, {1}, {2}", data[i, j].ToString(), currentLat, currentLon);
                    currentLon += this.gridSize;
                }
                currentLat += this.gridSize;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Save content to file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="contents"></param>
        public void SaveToFile(string filePath, string contents)
        {
            File.WriteAllText(filePath, contents);
        }
    }
}
