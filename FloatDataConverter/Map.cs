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

        public double firstTopDataLat;
        public double firstLeftDataLon;

        public double topLatDeg;
        public double bottomLatDeg;
        public double rightLongDeg;
        public double leftLongDeg;

        public Map(int numRow, int numColumn, double gridSize, double firstTopDataLat, double firstLeftDataLon)
        {
            this.data = new float[numRow, numColumn];
            this.numRow = numRow;
            this.numColumn = numColumn;
            this.gridSize = gridSize;
            this.firstTopDataLat = firstTopDataLat;
            this.firstLeftDataLon = firstLeftDataLon;
            this.topLatDeg = firstTopDataLat;
            this.bottomLatDeg = firstTopDataLat - (numRow * gridSize);
            this.leftLongDeg = firstLeftDataLon;
            this.rightLongDeg = firstLeftDataLon + (numColumn * gridSize);
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
        /// <param name="cropleftLongDeg"></param>
        /// <param name="croprightLongDeg"></param>
        /// <returns></returns>
        public Map CropMap(double croptopLatDeg, double cropbottomLatDeg, double cropleftLongDeg, double croprightLongDeg)
        {
            // Validate crop area
            if (croptopLatDeg > topLatDeg || cropbottomLatDeg < bottomLatDeg
                || cropleftLongDeg < leftLongDeg || croprightLongDeg > rightLongDeg)
            {
                throw new ArgumentException("Invalid crop parameters. Crop area exceeding available map area.");
            }
            else if (croptopLatDeg < cropbottomLatDeg || cropleftLongDeg > croprightLongDeg)
            {
                throw new ArgumentException("Invalid crop parameters. Top crop area can't be less than bottom crop area and left crop area can't be more than right crop area");
            }

            var rowOffset = Convert.ToInt32(Math.Round((topLatDeg - croptopLatDeg) / gridSize, 2));
            var rowCount = Convert.ToInt32(Math.Round((croptopLatDeg - cropbottomLatDeg) / gridSize, 2));
            var columnOffset = Convert.ToInt32(Math.Round(-1 * (leftLongDeg - cropleftLongDeg) / gridSize, 2));
            var columnCount = Convert.ToInt32(Math.Round(-1 * (cropleftLongDeg - croprightLongDeg) / gridSize, 2));

            var newRow = rowCount;
            var newColumn = columnCount;
            var newGridSize = gridSize;
            var newTopDataLat = Math.Round(topLatDeg - (rowOffset * gridSize), 2);
            var newLeftDataLon = Math.Round(leftLongDeg + (columnOffset * gridSize), 2);
            var newMap = new Map(newRow, newColumn, newGridSize, newTopDataLat, newLeftDataLon);

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
            var currentLat = this.firstTopDataLat;
            var currentLon = this.firstLeftDataLon.DeInterpolateLon();
            for (int i = 0; i < numRow; i++)
            {
                currentLon = this.firstLeftDataLon.DeInterpolateLon();
                for (int j = 0; j < numColumn; j++)
                {
                    sb.AppendFormat("{0,15}, {1}, {2} \n", data[i, j].ToString(), currentLat, currentLon);
                    currentLon -= this.gridSize;
                }
                currentLat -= this.gridSize;
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
