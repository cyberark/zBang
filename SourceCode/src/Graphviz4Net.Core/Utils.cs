
namespace Graphviz4Net
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;

    public static class Utils
    {
        public static bool TryParseInvariantDouble(string str, out double number)
        {
            if (str == null)
            {
                number = 0;
                return false;
            }

            return double.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out number);
        }

        public static double ParseInvariantDouble(string str)
        {
            double result;
            if (TryParseInvariantDouble(str, out result) == false)
            {
                throw new FormatException(string.Format("String {0} is not valid double representation", str));
            }

            return result;
        }

        public static double? ParseInvariantNullableDouble(string str)
        {
            if (str == null)
            {
                return null;
            }

            double result;
            if (TryParseInvariantDouble(str, out result) == false)
            {
                return null;
            }

            return result;            
        }

        public static string ToInvariantString(this double num)
        {
            return num.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToInvariantString(this double? num)
        {
            if (num.HasValue)
            {
                return num.Value.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                return null;
            }
        }

        public static bool AreEqual(double a, double b, double epsilon = 0.001)
        {
            return a >= b - epsilon && a <= b + epsilon;
        }

        public static bool AreEqual(Point a, Point b, double epsilon = 0.001)
        {
            return AreEqual(a.Y, b.Y) && AreEqual(a.X, a.Y);
        }

        public static void InsertAt<T>(this IList<T> list, int index, T value)
        {
            while (index >= list.Count)
            {
                list.Add(default(T));
            }

            list[index] = value;
        }

        public static T GetValue<TKey, T>(this IDictionary<TKey, T> dict, TKey key, T defaultValue)
        {
            T value;
            if (dict.TryGetValue(key, out value) == false)
            {
                return defaultValue;
            }

            return value;
        }

        public static T GetValue<TKey, T>(this IDictionary<TKey, T> dict, TKey key)
        {
            T value;
            if (dict.TryGetValue(key, out value) == false)
            {
                return default(T);
            }

            return value;
        }
    }
}
