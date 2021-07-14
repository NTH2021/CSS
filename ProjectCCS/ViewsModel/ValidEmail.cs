using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjectCCS.Models;

namespace ProjectCCS.ViewsModel
{
    public class ValidEmail : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ContextDB con = new ContextDB();
            var email = new EmailAddressAttribute();
            if (!email.IsValid(value))
            {
                return new ValidationResult(FormatErrorMessage(ErrorMessage = "Email is not valid !"));
            }
            return null;
         }
    }
}