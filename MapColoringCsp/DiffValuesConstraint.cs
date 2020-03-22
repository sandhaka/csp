namespace MapColoringCsp
{
    public class DiffValuesConstraint
    {
        public static bool Eval(string a, ColorWrapper aVal, string b, ColorWrapper bVal)
        {
            return aVal != bVal;
        }
    }
}