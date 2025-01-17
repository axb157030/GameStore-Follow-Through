using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace GameStore.Api.Validators;
public static class ValidationHelper
{

    public static bool ValidateObject(object obj)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(obj); // Reuse this context for every object

        Validator.TryValidateObject(obj, context, results, validateAllProperties: true);
        if (results.Count > 0)
        {
            return false;
        }
        return true;
    }


}
