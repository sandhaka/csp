using Csp.Csp.Model;
using FluentValidation;

namespace Csp.Csp.Validators
{
    internal class DomainValidator<T> : AbstractValidator<Domain<T>>
        where T : class
    {
        public DomainValidator()
        {
            RuleFor(d => d.Key).NotEmpty();
            RuleFor(d => d.Values).NotNull();
            RuleFor(d => d.Pruned).NotNull();
        }
    }
}