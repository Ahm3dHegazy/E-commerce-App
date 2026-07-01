using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CartFlow.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace CartFlow.Data.Validation
{
    /// <summary>
    /// DataAnnotation attribute that validates an email is unique in the database.
    /// Uses ValidationContext.GetService to resolve AppDbContext at validation time.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Try to resolve AppDbContext from the validation context's service provider
            var db = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;
            if (db == null)
            {
                // If we cannot resolve the context, fall back to success so validation doesn't block operation.
                return ValidationResult.Success;
            }

            var email = value as string;
            if (string.IsNullOrWhiteSpace(email))
            {
                return ValidationResult.Success; // let [Required] or [EmailAddress] handle empties
            }

            // Check case-insensitively for existing email
            var exists = db.Users.AsNoTracking().Any(u => u.Email.ToLower() == email.ToLower());
            if (exists)
            {
                var msg = string.IsNullOrWhiteSpace(ErrorMessage) ? "This email is already registered." : ErrorMessage;
                return new ValidationResult(msg);
            }

            return ValidationResult.Success;
        }
    }
}
