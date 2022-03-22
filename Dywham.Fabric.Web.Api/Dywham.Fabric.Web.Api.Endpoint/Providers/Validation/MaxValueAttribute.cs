using System;
using System.ComponentModel.DataAnnotations;

namespace Dywham.Fabric.Web.Api.Endpoint.Providers.Validation
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MaxValueAttribute : ValidationAttribute
    {
        private readonly int _maxValue;


        public MaxValueAttribute(int maxValue)
        {
            _maxValue = maxValue;
        }

        public override bool IsValid(object value)
        {
            return (int)value <= _maxValue;
        }
    }
}