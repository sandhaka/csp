namespace Sudoku
{
    public class DiffValuesConstraint
    {
        public static bool Eval(string a, Number aVal, string b, Number bVal)
        {
            return aVal != bVal;
        }
    }
}