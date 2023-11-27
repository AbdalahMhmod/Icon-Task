using Icon.Core.DTOs.UserDTOs;
using Icon.Core.Models;
using Icon.EF.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Net.Mail;
using System.Security.Claims;

namespace Icon.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IconDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IconDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        // GET: AccountController
        [HttpGet]
        public ActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(UserInputDTO input)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = input.Email,
                    UserName = input.UserName,
                };

                var result = await _userManager.CreateAsync(user, input.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Hi", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(input);
        }

        // GET: AccountController
        [HttpGet]
        public ActionResult Login() => View();

        [HttpPost]
        public async Task<ActionResult> Login(UserLoginDTO input)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(input.Email);
                if (user == null)
                {
                    ViewData["EmailError"] = "this email not found in our system PLZ Sign Up";
                    return View(input);
                }
                var result = await _signInManager.PasswordSignInAsync(user, input.Password, false, false);
                if (result.Succeeded)
                {
                    user.AccessFailedCount = 0;
                    _context.SaveChanges();
                    return RedirectToAction("Hi", "Home");
                }
                else if (result.IsLockedOut)
                {
                    return RedirectToAction("BLocked", "Home");
                }
                else
                {
                    var counter = 0;
                    user.AccessFailedCount += 1;
                    _context.SaveChanges();
                    counter = counter + user.AccessFailedCount;
                    if (counter == 3)
                    {
                        user.LockoutEnd = DateTime.Now.AddSeconds(30);
                        _context.SaveChanges();
                        ViewBag.DisableLoginButton = true; // send it to javaScript to disable button
                    }
                    else if (counter >= 3)
                    {
                        user.LockoutEnd = DateTime.Now.AddSeconds(1000);
                        _context.SaveChanges();
                        return RedirectToAction("BLocked", "Home");
                    }
                    else
                    {
                        ViewData["PasswordError"] = 
                            "your password is wrong please check it and try again you have : " 
                            + (3 - counter)+ 
                            " chance";
                    }

                    return View(input);
                }
            }
            return View(input);
        }

        public ActionResult ForgetPassword() => View();
        [HttpPost]
        public async Task<ActionResult> ForgetPassword(string email)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    ViewData["EmailError"] = "this email not found in our system check it out";
                    ViewData["Email"] = email;
                    return View();
                }
                try
                {
                    Random random = new Random();
                    var code = random.Next(1, 1000000);
                    user.UNumber = code;
                    _context.SaveChanges();

                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress("support@moussaacademy.com", "Icon Solution");
                        message.To.Add(user.Email);
                        message.Subject = "Verify Your Email";
                        message.Body =
                            "<h2>Hello,</h2> <br> your verification code is: <br>"
                            + code
                            + "<br> Thank You";
                        message.IsBodyHtml = true;

                        using (SmtpClient smtp = new SmtpClient("moussaacademy.com", 25))
                        {
                            //smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new System.Net.NetworkCredential("support@moussaacademy.com", "Mam@New@Email@34");
                            smtp.EnableSsl = default;
                            smtp.Send(message);
                        }
                    }
                    return RedirectToAction("Verify", "Account");
                }
                catch (Exception ex)
                {
                    ViewData["Error"] = ex.InnerException.Message;
                }
            }
            return View();
        }

        public ActionResult Verify() => View();
        [HttpPost]
        public async Task<IActionResult> Verify(int code) 
        {
            var user = await _context.Users.Where(u => u.UNumber == code).FirstOrDefaultAsync();
            if (user != null)
            {
                return RedirectToAction("SetPassword", "Account", new UserInputDTO { Email = user.Email});
            }
            return View();
        }
        public ActionResult SetPassword(UserInputDTO input) 
        {
            return View(input);
        }
        [HttpPost]
        public async Task<IActionResult> SetPassword(int id, UserInputDTO input) 
        {
            var user = _context.Users.Where(u => u.Email == input.Email).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, input.Password);
            var result = await _userManager.UpdateAsync(user);
            _context.SaveChanges();
            if (!result.Succeeded)
            {
                return NotFound();
            }
            return RedirectToAction("Login", "Account");

        }


    }
}
