using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductMVC.Contexts;
using ProductMVC.Models;
using ProductMVC.ViewModels.AuthViewModel;
using System.Threading.Tasks;

namespace ProductMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly ProniaDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AuthController(ProniaDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var existUser=await _userManager.FindByNameAsync(vm.UserName);
            if (existUser is { })
            {
                ModelState.AddModelError("Username", "This username is already taken");
            }

            existUser=await _userManager.FindByEmailAsync(vm.Email);
            if (existUser is { })
            {
                ModelState.AddModelError("Email", "This email is already exist");
            }

            AppUser user = new AppUser()
            {
                FirstName= vm.FirstName,
                LastName= vm.LastName,
                Email= vm.Email,
                UserName=vm.UserName,
            };

           var result= await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return Ok("Ok");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email or password is wrong");
                return View(vm);
            }

            var result=await _userManager.CheckPasswordAsync(user,vm.Password);
            if (!result)
            {
                ModelState.AddModelError("", "Email or password is wrong");
                return View(vm);
            }

            await _signInManager.SignInAsync(user, vm.IsRemember);

            return Ok($"{user.FirstName} {user.LastName}");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
