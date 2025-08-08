using System.ComponentModel.DataAnnotations;

namespace SAR.TrackingSystem.Web.Models;

public class RequiredIfAttribute : ValidationAttribute
{
    private readonly string _propertyName;
    private readonly object _expectedValue;

    public RequiredIfAttribute(string propertyName, object expectedValue)
    {
        _propertyName = propertyName;
        _expectedValue = expectedValue;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetProperty(_propertyName) ?? throw new ArgumentException($"Property '{_propertyName}' not found");

        var propertyValue = property.GetValue(validationContext.ObjectInstance);
        
        if (Equals(propertyValue, _expectedValue) && (value == null || (value is Guid guid && guid == Guid.Empty)))
        {
            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required");
        }

        return ValidationResult.Success;
    }
}
