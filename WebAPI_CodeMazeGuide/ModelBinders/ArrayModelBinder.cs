using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI_CodeMazeGuide.ModelBinders
{
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            var providedValue = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName)
                .ToString();

            if (string.IsNullOrEmpty(providedValue))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            var genericType = bindingContext.ModelType.GenericTypeArguments.First();
            var converter = TypeDescriptor.GetConverter(genericType) ?? 
                throw new ArgumentException($"It's not found converter for type {genericType.FullName}");
            
            var objectArray = providedValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => converter.ConvertFromString(x.Trim()))
                .ToArray();
            
            var guidArray = Array.CreateInstance(genericType, objectArray.Length);
            objectArray.CopyTo(guidArray, 0);

            bindingContext.Model = guidArray;
            bindingContext.Result = ModelBindingResult.Success(guidArray);
            return Task.CompletedTask;
        }
    }
}
