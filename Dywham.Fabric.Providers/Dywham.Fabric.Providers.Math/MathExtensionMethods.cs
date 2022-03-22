using System;

namespace Dywham.Fabric.Providers.Math
{
    public static class MathExtensionMethods
    {
        public static decimal TruncateWithoutRounding(this decimal value, byte decimals)
        {
            var rounded = System.Math.Round(value, decimals);

            if (value > 0 && rounded > value)
            {
                return rounded - new decimal(1, 0, 0, false, decimals);
            }

            if (value < 0 && rounded < value)
            {
                return rounded + new decimal(1, 0, 0, false, decimals);
            }

            return rounded;
        }

        public static decimal TruncateWithoutRounding(this double value, byte decimals)
        {
            return Convert.ToDecimal(value).TruncateWithoutRounding(decimals);
        }
    }
}