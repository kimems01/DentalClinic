﻿using HMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HMS.Models;
using HMS.Models.UserProfileViewModel;
using HMS.Services;
using Microsoft.EntityFrameworkCore;
using HMS.Models.ManageUserRolesVM;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class UserProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileController(UserManager<ApplicationUser> userManager, ICommon iCommon,
            ApplicationDbContext context)
        {
            _context = context;
            _iCommon = iCommon;
            _userManager = userManager;
        }

        [Authorize(Roles = HMS.Pages.RoleNameList.UserProfile)]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var _UserName = User.Identity.Name;
            var _UserProfile = _context.UserProfile.FirstOrDefault(x => x.Email == _UserName);
            var result = await _iCommon.GetUserProfileDetails().Where(x => x.UserProfileId == _UserProfile.UserProfileId).SingleOrDefaultAsync();
            ManageUserRolesCRUDViewModel _ManageUserRolesCRUDViewModel = new();
            _ManageUserRolesCRUDViewModel.listManageUserRolesDetails = await _iCommon.GetManageRoleDetailsList(result.RoleId);
            result.ManageUserRolesCRUDViewModel = _ManageUserRolesCRUDViewModel;       
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string ApplicationUserId)
        {
            var _ApplicationUser = await _userManager.FindByIdAsync(ApplicationUserId);
            ResetPasswordViewModel _ResetPasswordViewModel = new();
            _ResetPasswordViewModel.ApplicationUserId = _ApplicationUser.Id;
            return PartialView("_ResetPassword", _ResetPasswordViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveResetPassword(ResetPasswordViewModel vm)
        {
            try
            {
                string AlertMessage = string.Empty;
                var _ApplicationUser = await _userManager.FindByIdAsync(vm.ApplicationUserId);
                if (vm.NewPassword.Equals(vm.ConfirmPassword))
                {
                    var result = await _userManager.ChangePasswordAsync(_ApplicationUser, vm.OldPassword, vm.NewPassword);
                    if (result.Succeeded)
                        AlertMessage = "Change Password Succeeded. User name: " + _ApplicationUser.Email;
                    else
                    {
                        string errorMessage = string.Empty;
                        foreach (var item in result.Errors)
                        {
                            errorMessage = errorMessage + " " + item.Description;
                        }
                        AlertMessage = "error" + errorMessage;
                    }
                }
                return new JsonResult(AlertMessage);
            }
            catch (Exception ex)
            {
                return new JsonResult("error" + ex.Message);
                throw;
            }
        }
    }
}