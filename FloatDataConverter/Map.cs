using System;
using System.Collections.Generic;
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

        public Map(int numRow, int numColumn, double gridSize, double topLatDeg, double bottomLatDeg, double rightLongDeg, double leftLongDeg)
        {
            this.data = new float[numRow, numColumn];
            this.numRow = numRow;
            this.numColumn = numColumn;
            this.gridSize = gridSize;
            this.topLatDeg = topLatDeg;
            this.bottomLatDeg = bottomLatDeg;
            this.rightLongDeg = rightLongDeg;
            this.leftLongDeg = leftLongDeg;
        }

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

        public string PrintDataCsvFormat()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Data Value, Latitude, Longitude");
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
    }
}
