﻿using HMS.Services;

namespace HMS.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, IFunctional functional)
        {
            context.Database.EnsureCreated();
            if (context.ApplicationUser.Any())
            {
                return;
            }
            else
            {
                await functional.CreateDefaultSuperAdmin();
                await functional.CreateDefaultEmailSettings();
                await functional.CreateDefaultIdentitySettings();
                
                functional.InitAppData();
                await functional.GenerateUserUserRole();
                await functional.CreateDefaultDoctorUser();
                await functional.CreateDefaultOtherUser();
                
            }
        }
    }
}
