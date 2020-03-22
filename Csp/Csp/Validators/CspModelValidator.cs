using Csp.Csp.Model;
using FluentValidation;

namespace Csp.Csp.Validators
{
    internal class CspModelValidator<T> : AbstractValidator<CspModel<T>>
        where T : class
    {
        public CspModelValidator()
        {

        }
    }
}