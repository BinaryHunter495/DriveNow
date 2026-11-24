using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DriveNow.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _users;
    private readonly SignInManager<IdentityUser> _signIn;

    public AccountController(UserManager<IdentityUser> users, SignInManager<IdentityUser> signIn)
    {
        _users = users;
        _signIn = signIn;
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string? email, string? password)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@') ||
            string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            ViewBag.Error = "Enter a valid email and a password of at least 6 characters.";
            return View();
        }

        email = email.Trim();
        var user = new IdentityUser { UserName = email, Email = email };
        var result = await _users.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            ViewBag.Error = string.Join(" ", result.Errors.Select(e => e.Description));
            return View();
        }

        // Public registration can ONLY create Customer accounts.
        await _users.AddToRoleAsync(user, "Customer");
        await _signIn.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Customer");
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string? email, string? password, string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Enter both your email and password.";
            return View();
        }

        var result = await _signIn.PasswordSignInAsync(email.Trim(), password, false, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            ViewBag.Error = result.IsLockedOut
                ? "Your account is temporarily locked. Please try again later."
                : "Email or password is incorrect.";
            return View();
        }

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        var user = await _users.FindByEmailAsync(email.Trim());
        return user != null && await _users.IsInRoleAsync(user, "Admin")
            ? RedirectToAction("Index", "Admin")
            : RedirectToAction("Index", "Customer");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();
}
