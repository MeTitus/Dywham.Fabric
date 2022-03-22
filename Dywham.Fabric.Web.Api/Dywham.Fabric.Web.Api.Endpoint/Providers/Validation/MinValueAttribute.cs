using System;
using System.ComponentModel.DataAnnotations;

namespace Dywham.Fabric.Web.Api.Endpoint.Providers.Validation
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MinValueAttribute : ValidationAttribute
    {
        private readonly int _minValue;


        public MinValueAttribute(int minValue)
        {
            _minValue = minValue;
        }

        public override bool IsValid(object value)
        {
            return (int)value >= _minValue;
        }
    }
}