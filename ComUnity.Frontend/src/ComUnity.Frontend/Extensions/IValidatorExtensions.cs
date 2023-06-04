using FluentValidation;

namespace ComUnity.Frontend.Extensions
{
    public static class IValidatorExtensions
    {
       public static Func<object, string, Task<IEnumerable<string>>> CreateValueValidator<TModel>(this IValidator<TModel> validator)
       {
            return async (model, propertyName) =>
            {
                var type = typeof(TModel);
                var result = await validator.ValidateAsync(ValidationContext<TModel>.CreateWithOptions((TModel)model, x => x.IncludeProperties(propertyName)));
                if (result.IsValid)
                    return Array.Empty<string>();
                return result.Errors.Select(e => e.ErrorMessage);
            };
       }
    }
}