using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

/// <summary>
/// Extensions to functionality of an already existing class and/or interface.
/// </summary>
namespace DICOMcloud.Wado.WebApi.Extensions
{
    public static class ModelStateExtensions
    {
        /// <summary>
        /// Convert the validation errors into simple strings.
        /// </summary>
        /// <returns>List of error messages</returns>
        public static List<string> GetErrorMessages(this ModelStateDictionary dictionary)
        {
            return dictionary.SelectMany(m => m.Value.Errors)
                             .Select(m => m.ErrorMessage)
                             .ToList();
        }
    }
}