using System;
using System.ComponentModel.DataAnnotations;

namespace Dywham.Fabric.Web.Api.Endpoint.Providers.Validation
{
    [AttributeUsage(AttributeTargets.Method)]
    public class BetweenRangeAttribute : ValidationAttribute
    {
        private readonly int _minValue;
        private readonly int _maxValue;


        public BetweenRangeAttribute(int minValue, int maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public override bool IsValid(object value)
        {
            return (int)value >= _minValue && (int)value <= _maxValue;
        }
    }
}