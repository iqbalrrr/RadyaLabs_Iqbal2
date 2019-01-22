using RadyaLabs.Objects;
using System;
using System.Linq;
using System.Security.Principal;

namespace RadyaLabs.Services
{
    public interface IAccountService : IService
    {
        TView Get<TView>(Int32 id) where TView : BaseView;
        IQueryable<AccountView> GetViews();

        Boolean IsLoggedIn(IPrincipal user);
        Boolean IsActive(Int32 id);

        String Recover(AccountRecoveryView view);
        void Reset(AccountResetView view);

        void Create(AccountCreateView view);
        void Edit(AccountEditView view);

        void Edit(ProfileEditView view);
        void Delete(Int32 id);

        void Login(String username);
        void Logout();
    }
}
