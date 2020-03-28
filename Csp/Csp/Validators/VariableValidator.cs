using Csp.Csp.Model;
using FluentValidation;

namespace Csp.Csp.Validators
{
    internal class VariableValidator<T> : AbstractValidator<Variable<T>>
        where T : CspValue
    {
        public VariableValidator()
        {
            RuleFor(v => v.Key).NotEmpty();
        }
    }
}