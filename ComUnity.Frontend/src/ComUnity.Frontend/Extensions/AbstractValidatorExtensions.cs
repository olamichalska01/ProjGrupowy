using FluentValidation;

namespace ComUnity.Frontend.Extensions
{
    public static class AbstractValidatorExtensions
    {
       public static Func<TModel, string, Task<IEnumerable<string>>> CreateValueValidator<TModel>(this AbstractValidator<TModel> validator)
       {
            return async (model, propertyName) =>
            {
                var result = await validator.ValidateAsync(ValidationContext<TModel>.CreateWithOptions(model, x => x.IncludeProperties(propertyName)));
                if (result.IsValid)
                    return Array.Empty<string>();
                return result.Errors.Select(e => e.ErrorMessage);
            };
       }
    }
}