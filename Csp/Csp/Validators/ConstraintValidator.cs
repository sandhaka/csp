using Csp.Csp.Model;
using FluentValidation;

namespace Csp.Csp.Validators
{
    internal class ConstraintValidator<T> : AbstractValidator<Constraint<T>>
        where T : CspValue
    {
        public ConstraintValidator()
        {
            RuleFor(c => c.Rule).NotNull();
        }
    }
}