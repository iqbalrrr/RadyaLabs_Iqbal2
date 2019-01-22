using RadyaLabs.Data.Core;
using RadyaLabs.Objects;
using RadyaLabs.Validators;
using System;
using System.Linq.Expressions;

namespace RadyaLabs.Tests.Unit.Validators
{
    public class BaseValidatorProxy : BaseValidator
    {
        public BaseValidatorProxy(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public Boolean BaseIsSpecified<TView>(TView view, Expression<Func<TView, Object>> property) where TView : BaseView
        {
            return IsSpecified(view, property);
        }
    }
}
