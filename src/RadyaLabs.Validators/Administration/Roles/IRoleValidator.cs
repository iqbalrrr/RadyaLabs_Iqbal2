using RadyaLabs.Objects;
using System;

namespace RadyaLabs.Validators
{
    public interface IRoleValidator : IValidator
    {
        Boolean CanCreate(RoleView view);
        Boolean CanEdit(RoleView view);
    }
}
