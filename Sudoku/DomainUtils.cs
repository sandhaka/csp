using System;
using System.Linq;

namespace Sudoku
{
    public static class DomainUtils
    {
        public static float N = 3f;

        public static string X(string id)
        {
            return id.Split('.').First();
        }

        public static string Y(string id)
        {
            return id.Split('.').Last();
        }

        public static int Square(string id)
        {
            var x = X(id);
            var y = Y(id);

            var xIndex = float.Parse(x) / N;
            var yIndex = float.Parse(y) / N;

            if (xIndex <= 1 && yIndex <= 1)
            {
                return 1;
            }
            if (xIndex <= 2 && yIndex <= 1)
            {
                return 2;
            }
            if (xIndex <= 3 && yIndex <= 1)
            {
                return 3;
            }

            if (xIndex <= 1 && yIndex <= 2)
            {
                return 4;
            }
            if (xIndex <= 2 && yIndex <= 2)
            {
                return 5;
            }
            if (xIndex <= 3 && yIndex <= 2)
            {
                return 6;
            }

            if (xIndex <= 1 && yIndex <= 3)
            {
                return 7;
            }
            if (xIndex <= 2 && yIndex <= 3)
            {
                return 8;
            }
            if (xIndex <= 3 && yIndex <= 3)
            {
                return 9;
            }

            throw new InvalidOperationException($"Unexpected: {id}");
        }
    }
}