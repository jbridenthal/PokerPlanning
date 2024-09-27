using FluentValidation;

namespace PokerPlanning.Models.Validators
{
    public class RoomModelValidator : AbstractValidator<RoomModel>
    {
        public RoomModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<RoomModel>.CreateWithOptions((RoomModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };

    }
}
