using System.ComponentModel.DataAnnotations;

namespace AspNetCore.JWTDemo.Validates
{
    public class AtLeastOneRequiredAttribute : ValidationAttribute
    {
        public string[] OtherProperties { get; set; }

        public AtLeastOneRequiredAttribute(params string[] otherProperties)
        {
            OtherProperties = otherProperties;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (!IsNullOrEmpty(value))
            {
                return ValidationResult.Success;
            }
            var willValidateProps = validationContext.ObjectType.GetProperties().Where(x => OtherProperties.Contains(x.Name));
            foreach (var prop in willValidateProps)
            {
                if (!IsNullOrEmpty(prop.GetValue(validationContext.ObjectInstance)))
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult($"At least one property is required in [{validationContext.MemberName},{string.Join(",", OtherProperties)}]");
        }
        private static bool IsNullOrEmpty(object? value)
        {
            if (value is string sv)
            {
                return string.IsNullOrWhiteSpace(sv);
            }
            return value == null;
        }
    }
}
