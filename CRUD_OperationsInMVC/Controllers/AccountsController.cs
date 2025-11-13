using CRUD_OperationsInMVC.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;

namespace CRUD_OperationsInMVC.Controllers
{
    public class AccountsController : Controller
    {
        #region Public Actions

        /// <summary>
        /// Displays the login page.
        /// </summary>
        /// <returns>The login view.</returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Handles the user login attempt.
        /// </summary>
        /// <param name="model">The user model containing login credentials.</param>
        /// <returns>Redirects to the Employee index on success, otherwise redisplays the login page with an error.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User model)
        {
            using(UserDBContext context =  new UserDBContext())
            {
                bool IsValidUser = context.Users.Any(user => user.Username.ToLower() == model.Username.ToLower() &&
                    user.PasswordHash == model.PasswordHash);

                if (IsValidUser)
                {
                    FormsAuthentication.SetAuthCookie(model.Username, false);
                    return RedirectToAction("Index", "Employee");
                }

                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Displays the registration page.
        /// </summary>
        /// <returns>The registration view.</returns>
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Handles the user registration attempt.
        /// </summary>
        /// <param name="model">The view model containing user registration details.</param>
        /// <returns>Redirects to the Employee index on success, otherwise redisplays the registration page with errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var context = new UserDBContext())
            {
                if (IsUsernameTaken(context, model.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists. Please choose another one.");
                    return View(model);
                }

                if (IsEmailTaken(context, model.Email))
                {
                    ModelState.AddModelError("Email", "This email address is already registered.");
                    return View(model);
                }

                var newUser = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PasswordHash = HashPassword(model.Password),
                    DateRegistered = DateTime.UtcNow
                };

                context.Users.Add(newUser);
                context.SaveChanges();

                FormsAuthentication.SetAuthCookie(newUser.Username, false);
                return RedirectToAction("Index", "Employee");
            }
        }
        #endregion

        #region Private Helpers

        /// <summary>
        /// Hashes a password using SHA256.
        /// </summary>
        /// <param name="password">The plain-text password to hash.</param>
        /// <returns>The SHA256 hashed password as a hex string.</returns>
        private string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return string.Empty;
            }

            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Checks if a username is already taken.
        /// </summary>
        private bool IsUsernameTaken(UserDBContext context, string username)
        {
            return context.Users.Any(u => u.Username.ToLower() == username.ToLower());
        }

        /// <summary>
        /// Checks if an email is already registered.
        /// </summary>
        private bool IsEmailTaken(UserDBContext context, string email)
        {
            return context.Users.Any(u => u.Email.ToLower() == email.ToLower());
        }

        #endregion
    }
}
