using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WithoutIdentity.Models;
using WithoutIdentity.Models.ManagerViewModels;

namespace WithoutIdentity.Controllers
{
    public class ManagerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ManagerController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // User - objeto disponibilizado pelo 'Controller' Base
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Não foi possivel carregar o usuario com ID {_userManager.GetUserId(User)}");
            }

            var model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // User - objeto disponibilizado pelo 'Controller' Base
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Não foi possivel carregar o usuario com ID {_userManager.GetUserId(User)}");
            }

            var email = user.Email;
            if (email != model.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);

                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Erro inesperado ao atribuir o email ao usuario com ID {_userManager.GetUserId(User)}");
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (phoneNumber != model.PhoneNumber)
            {
                var setPhoneNumberResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);

                if (!setPhoneNumberResult.Succeeded)
                {
                    throw new ApplicationException($"Erro inesperado ao atribuir o telefone ao usuario com ID {_userManager.GetUserId(User)}");
                }
            }

            StatusMessage = "Seu perfil foi atualizado";

            return RedirectToAction(nameof(Index));
        }
    }
}