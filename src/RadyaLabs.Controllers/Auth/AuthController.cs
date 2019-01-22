using RadyaLabs.Components.Mail;
using RadyaLabs.Objects;
using RadyaLabs.Resources.Views.Administration.Accounts.AccountView;
using RadyaLabs.Services;
using RadyaLabs.Validators;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RadyaLabs.Controllers
{
    [AllowAnonymous]
    public class AuthController : ValidatedController<IAccountValidator, IAccountService>
    {
        public IMailClient MailClient { get; }

        public AuthController(IAccountValidator validator, IAccountService service, IMailClient mailClient)
            : base(validator, service)
        {
            MailClient = mailClient;
        }

        [HttpGet]
        public ActionResult Recover()
        {
            if (Service.IsLoggedIn(User))
                return RedirectToDefault();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Recover(AccountRecoveryView account)
        {
            if (Service.IsLoggedIn(User))
                return RedirectToDefault();

            if (!Validator.CanRecover(account))
                return View(account);

            String token = Service.Recover(account);
            if (token != null)
            {
                String url = Url.Action("Reset", "Auth", new { token }, Request.Url.Scheme);

                await MailClient.SendAsync(
                    account.Email,
                    Messages.RecoveryEmailSubject,
                    String.Format(Messages.RecoveryEmailBody, url));
            }

            Alerts.AddInfo(Messages.RecoveryInformation);

            return RedirectIfAuthorized("Login");
        }

        [HttpGet]
        public ActionResult Reset(String token)
        {
            if (Service.IsLoggedIn(User))
                return RedirectToDefault();

            if (!Validator.CanReset(new AccountResetView { Token = token }))
                return RedirectIfAuthorized("Recover");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reset(AccountResetView account)
        {
            if (Service.IsLoggedIn(User))
                return RedirectToDefault();

            if (!Validator.CanReset(account))
                return RedirectIfAuthorized("Recover");

            Service.Reset(account);

            Alerts.AddSuccess(Messages.SuccessfulReset, 4000);

            return RedirectIfAuthorized("Login");
        }

        [HttpGet]
        public ActionResult Login(String returnUrl)
        {
            if (Service.IsLoggedIn(User))
                return RedirectToLocal(returnUrl);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AccountLoginView account, String returnUrl)
        {
            if (Service.IsLoggedIn(User))
                return RedirectToLocal(returnUrl);

            if (!Validator.CanLogin(account))
                return View(account);

            Service.Login(account.Username);

            return RedirectToLocal(returnUrl);
        }

        [HttpGet]
        public RedirectToRouteResult Logout()
        {
            Service.Logout();

            return RedirectIfAuthorized("Login");
        }
    }
}
