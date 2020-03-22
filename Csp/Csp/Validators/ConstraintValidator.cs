using Csp.Csp.Model;
using FluentValidation;

namespace Csp.Csp.Validators
{
    internal class ConstraintValidator<T> : AbstractValidator<Constraint<T>>
        where T : class
    {
        public ConstraintValidator()
        {
            RuleFor(c => c.Rule).NotNull();
        }
    }
}