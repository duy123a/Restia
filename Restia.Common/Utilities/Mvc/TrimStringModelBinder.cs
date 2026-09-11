using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Restia.Common.Utilities.Mvc
{
    public class TrimStringModelBinder : IModelBinder
    {
        private readonly IModelBinder _fallbackBinder;

        public TrimStringModelBinder(IModelBinder fallbackBinder)
        {
            _fallbackBinder = fallbackBinder ?? throw new ArgumentNullException(nameof(fallbackBinder));
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext);
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult.FirstValue is string str &&
              !string.IsNullOrEmpty(str))
            {
                bindingContext.Result = ModelBindingResult.Success(str.Trim());
                return Task.CompletedTask;
            }
            return _fallbackBinder.BindModelAsync(bindingContext);
        }
    }
}
