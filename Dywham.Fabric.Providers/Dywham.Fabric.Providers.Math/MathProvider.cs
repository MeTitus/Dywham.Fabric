using System;

namespace Dywham.Fabric.Providers.Math
{
    public class MathProvider : IMathProvider
    {
        public decimal TruncateWithoutRounding(decimal value, byte decimals)
        {
            return value.TruncateWithoutRounding(decimals);
        }

        public decimal TruncateWithoutRounding(double value, byte decimals)
        {
            return Convert.ToDecimal(value).TruncateWithoutRounding(decimals);
        }
    }
}
