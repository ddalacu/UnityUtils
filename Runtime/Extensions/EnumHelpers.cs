using System;
using System.Collections.Generic;

namespace Framework.Utility
{
    /// <summary>
    /// This will be compiled so it s fast than Enum. methods
    /// Don t use this for enums with lots of values (like hundreds of values because this is o(N))
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    public static class EnumHelpers<TTarget>
    {
        static EnumHelpers()
        {
            Names = Enum.GetNames(typeof(TTarget));
            Values = (TTarget[])Enum.GetValues(typeof(TTarget));
            Comparer = EqualityComparer<TTarget>.Default;
        }

        private static readonly string[] Names;
        private static readonly TTarget[] Values;
        private static readonly EqualityComparer<TTarget> Comparer;

        public static string GetName(TTarget target)
        {
            var valuesLength = Values.Length;
            for (int i = 0; i < valuesLength; i++)
            {
                if (Comparer.Equals(target, Values[i]))
                    return Names[i];
            }

            return target.ToString();
        }

        public static bool TryParse(string value, out TTarget target)
        {
            var namesLength = Names.Length;
            for (int i = 0; i < namesLength; i++)
            {
                if (string.Equals(Names[i], value, StringComparison.Ordinal))
                {
                    target= Values[i];
                    return true;
                }
            }

            target = default;
            return false;
        }
    }
}