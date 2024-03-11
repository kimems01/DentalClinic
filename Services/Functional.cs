using HMS.ConHelper;
using HMS.Data;
using HMS.Helpers;
using HMS.Models;
using HMS.Models.CommonViewModel;
using HMS.Models.DashboardViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace HMS.Services
{
    public class Functional : IFunctional
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRoles _roles;
        private readonly SuperAdminDefaultOptions _superAdminDefaultOptions;
        private readonly ApplicationInfo _applicationInfo;
        private readonly ICommon _iCommon;
        private readonly IDBOperation _iDBOperation;
        private readonly IAccount _iAccount;

        public Functional(UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager,
           ApplicationDbContext context,
           SignInManager<ApplicationUser> signInManager,
           IRoles roles,
           IOptions<SuperAdminDefaultOptions> superAdminDefaultOptions,
           IOptions<ApplicationInfo> applicationInfo,
           ICommon iCommon,
           IDBOperation iDBOperation,
           IAccount iAccount)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _signInManager = signInManager;
            _roles = roles;
            _superAdminDefaultOptions = superAdminDefaultOptions.Value;
            _applicationInfo = applicationInfo.Value;
            _iCommon = iCommon;
            _iDBOperation = iDBOperation;
            _iAccount = iAccount;
        }

        public async Task SendEmailBySendGridAsync(string apiKey,
            string fromEmail,
            string fromFullName,
            string subject,
            string message,
            string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(fromEmail, fromFullName),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email, email));
            await client.SendEmailAsync(msg);

        }

        public async Task SendEmailByGmailAsync(string fromEmail,
            string fromFullName,
            string subject,
            string messageBody,
            string toEmail,
            string toFullName,
            string smtpUser,
            string smtpPassword,
            string smtpHost,
            int smtpPort,
            bool smtpSSL)
        {
            var body = messageBody;
            var message = new MailMessage();
            message.To.Add(new MailAddress(toEmail, toFullName));
            message.From = new MailAddress(fromEmail, fromFullName);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                smtp.UseDefaultCredentials = false;
                var credential = new NetworkCredential
                {
                    UserName = smtpUser,
                    Password = smtpPassword
                };
                smtp.Credentials = credential;
                smtp.Host = smtpHost;
                smtp.Port = smtpPort;
                smtp.EnableSsl = smtpSSL;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                await smtp.SendMailAsync(message);

            }
        }

        public async Task CreateDefaultSuperAdmin()
        {
            try
            {
                await _roles.GenerateRolesFromPageList();

                ApplicationUser superAdmin = new ApplicationUser();
                superAdmin.Email = _superAdminDefaultOptions.Email;
                superAdmin.UserName = superAdmin.Email;
                superAdmin.EmailConfirmed = true;

                var result = await _userManager.CreateAsync(superAdmin, _superAdminDefaultOptions.Password);

                if (result.Succeeded)
                {
                    UserProfile _UserProfile = new UserProfile();
                    _UserProfile.UserType = UserType.General;
                    _UserProfile.EmployeeId = StaticData.RandomDigits(6);
                    _UserProfile.RoleId = 1;
                    _UserProfile.ApplicationUserId = superAdmin.Id;
                    _UserProfile.FirstName = "Super";
                    _UserProfile.LastName = "Admin";
                    _UserProfile.PhoneNumber = "+8801674411603";
                    _UserProfile.Email = superAdmin.Email;
                    _UserProfile.Address = "R/A, Dhaka";
                    _UserProfile.Country = "Bangladesh";
                    _UserProfile.ProfilePicture = "/images/UserIcon/Admin.png";

                    _UserProfile.Designation = 1;
                    _UserProfile.Department = 1;
                    _UserProfile.SubDepartment = 1;
                    _UserProfile.JoiningDate = DateTime.Now.AddYears(-1);
                    _UserProfile.LeavingDate = DateTime.Now;

                    _UserProfile.CreatedDate = DateTime.Now;
                    _UserProfile.ModifiedDate = DateTime.Now;
                    _UserProfile.CreatedBy = "Admin";
                    _UserProfile.ModifiedBy = "Admin";

                    await _context.UserProfile.AddAsync(_UserProfile);
                    await _context.SaveChangesAsync();

                    await _roles.AddToRoles(superAdmin);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<string> UploadFile(List<IFormFile> files, IWebHostEnvironment env, string uploadFolder)
        {
            var result = "";

            var webRoot = env.WebRootPath;
            var uploads = Path.Combine(webRoot, uploadFolder);
            var extension = "";
            var filePath = "";
            var fileName = "";


            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    extension = Path.GetExtension(formFile.FileName);
                    fileName = Guid.NewGuid().ToString() + extension;
                    filePath = Path.Combine(uploads, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    result = fileName;

                }
            }

            return result;
        }

        public void InitAppData()
        {
            SeedData _SeedData = new SeedData();

            var _GetMedicineCategoriesList = _SeedData.GetMedicineCategoriesList();
            foreach (var item in _GetMedicineCategoriesList)
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = "Admin";
                item.ModifiedBy = "Admin";
                _context.MedicineCategories.Add(item);
                _context.SaveChanges();
            }
            var _GetUnitList = _SeedData.GetUnitList();
            foreach (var item in _GetUnitList)
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = "Admin";
                item.ModifiedBy = "Admin";
                _context.Unit.Add(item);
                _context.SaveChanges();
            }
            var _GetMedicineManufactureList = _SeedData.GetMedicineManufactureList();
            foreach (var item in _GetMedicineManufactureList)
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = "Admin";
                item.ModifiedBy = "Admin";
                _context.MedicineManufacture.Add(item);
                _context.SaveChanges();
            }
            var _GetMedicinesList = _SeedData.GetMedicinesList();
            foreach (var item in _GetMedicinesList)
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = "Admin";
                item.ModifiedBy = "Admin";
                _context.Medicines.Add(item);
                _context.SaveChanges();
            }

            var _GetBedCategoriesList = _SeedData.GetBedCategoriesList();
            foreach (var item in _GetBedCategoriesList)
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = "Admin";
                item.ModifiedBy = "Admin";
                _context.BedCategories.Add(item);
                _context.SaveChanges();
            }

            var _GetBedList = _SeedData.GetBedList();
            foreach (var item in _GetBedList)
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = "Admin";
                item.ModifiedBy = "Admin";
                _context.Bed.Add(item);
                _context.SaveChanges();
            }

            var _GetPatientInfoList = _SeedData.GetPatientInfoList();
            foreach (var item in _GetPatientInfoList)
            {
                Task.Delay(1000).Wait();
                item.PatientCode = "P" + DateTime.Now.ToString("yyyyMMddHHmmss");
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = "Admin";
                item.ModifiedBy = "Admin";
                _context.PatientInfo.Add(item);
                _context.SaveChanges();
            }

            var _GetExpenseCategoriesList = _SeedData.GetExpenseCategoriesList();
            foreach (var item in _GetExpenseCategoriesList)
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = "Admin";
                item.ModifiedBy = "Admin";
                _context.ExpenseCategories.Add(item);
                _context.SaveChanges();
            }

            var _GetPaymentCategoriesList = _SeedData.GetPaymentCategoriesList();
            foreach (var item in _GetPaymentCategoriesList)
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = "Admin";
                item.ModifiedBy = "Admin";
                _context.PaymentCategories.Add(item);
                _context.SaveChanges();
            }

            var _GetLabTestCategoriesList = _SeedData.GetLabTestCategoriesList();
            foreach (var item in _GetLabTestCategoriesList)
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = "Admin";
                item.ModifiedBy = "Admin";
                _context.LabTestCategories.Add(item);
                _context.SaveChanges();
            }
            var _GetLabTestsList = _SeedData.GetLabTestsList();
            foreach (var item in _GetLabTestsList)
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = "Admin";
                item.ModifiedBy = "Admin";
                _context.LabTests.Add(item);
                _context.SaveChanges();
            }

            var _GetCurrencyList = _SeedData.GetCurrencyList();
            foreach (var item in _GetCurrencyList)
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = "Admin";
                item.ModifiedBy = "Admin";
                _context.Currency.Add(item);
                _context.SaveChanges();
            }
            var _GetInsuranceCompanyInfoList = _SeedData.GetInsuranceCompanyInfoList();
            foreach (var item in _GetInsuranceCompanyInfoList)
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = "Admin";
                item.ModifiedBy = "Admin";
                _context.InsuranceCompanyInfo.Add(item);
                _context.SaveChanges();
            }
            var _GetManageUserRolesList = _SeedData.GetManageUserRolesList();
            foreach (var item in _GetManageUserRolesList)
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = "Admin";
                item.ModifiedBy = "Admin";
                _context.ManageUserRoles.Add(item);
                _context.SaveChanges();
            }
            var _GetDesignationList = _SeedData.GetDesignationList();
            foreach (var item in _GetDesignationList)
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = "Admin";
                item.ModifiedBy = "Admin";
                _context.Designation.Add(item);
                _context.SaveChanges();
            }
            var _GetDepartmentList = _SeedData.GetDepartmentList();
            foreach (var item in _GetDepartmentList)
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = "Admin";
                item.ModifiedBy = "Admin";
                _context.Department.Add(item);
                _context.SaveChanges();
            }
            var _GetSubDepartmentList = _SeedData.GetSubDepartmentList();
            foreach (var item in _GetSubDepartmentList)
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = "Admin";
                item.ModifiedBy = "Admin";
                _context.SubDepartment.Add(item);
                _context.SaveChanges();
            }

            var _GetCompanyInfo = _SeedData.GetCompanyInfo();
            _GetCompanyInfo.CreatedDate = DateTime.Now;
            _GetCompanyInfo.ModifiedDate = DateTime.Now;
            _GetCompanyInfo.CreatedBy = "Admin";
            _GetCompanyInfo.ModifiedBy = "Admin";
            _context.CompanyInfo.Add(_GetCompanyInfo);
            _context.SaveChanges();
        }

        public async Task GenerateUserUserRole()
        {
            var _ManageRole = await _context.ManageUserRoles.ToListAsync();
            var _GetRoleList = await _roles.GetRoleList();

            foreach (var role in _ManageRole)
            {
                foreach (var item in _GetRoleList)
                {
                    ManageUserRolesDetails _ManageRoleDetails = new();
                    _ManageRoleDetails.ManageRoleId = role.Id;
                    _ManageRoleDetails.RoleId = item.RoleId;
                    _ManageRoleDetails.RoleName = item.RoleName;

                    if (role.Id == 1)
                    {
                        _ManageRoleDetails.IsAllowed = true;
                    }
                    else if (role.Id == 2)
                    {
                        if (item.RoleName == "User Profile" || item.RoleName == "Dashboard")
                            _ManageRoleDetails.IsAllowed = true;
                    }
                    else if (role.Id == 3)
                    {
                        if (item.RoleName == "User Profile" || item.RoleName == "Dashboard" || item.RoleName == "Checkup" || item.RoleName == "Manage Clinic")
                            _ManageRoleDetails.IsAllowed = true;
                    }
                    else
                    {
                        _ManageRoleDetails.IsAllowed = false;
                    }

                    _ManageRoleDetails.CreatedDate = DateTime.Now;
                    _ManageRoleDetails.ModifiedDate = DateTime.Now;
                    _ManageRoleDetails.CreatedBy = "Admin";
                    _ManageRoleDetails.ModifiedBy = "Admin";
                    _context.Add(_ManageRoleDetails);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task CreateDefaultEmailSettings()
        {
            //SMTP
            var CountSMTPEmailSetting = _context.SMTPEmailSetting.Count();
            if (CountSMTPEmailSetting < 1)
            {
                SMTPEmailSetting _SMTPEmailSetting = new SMTPEmailSetting
                {
                    UserName = "devmlbd@gmail.com",
                    Password = "",
                    Host = "smtp.gmail.com",
                    Port = 587,
                    IsSSL = true,
                    FromEmail = "devmlbd@gmail.com",
                    FromFullName = "Web Admin Notification",
                    IsDefault = true,

                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    CreatedBy = "Admin",
                    ModifiedBy = "Admin",
                };
                _context.Add(_SMTPEmailSetting);
                await _context.SaveChangesAsync();
            }
            //SendGridOptions
            var CountSendGridSetting = _context.SendGridSetting.Count();
            if (CountSendGridSetting < 1)
            {
                SendGridSetting _SendGridOptions = new SendGridSetting
                {
                    SendGridUser = "",
                    SendGridKey = "",
                    FromEmail = "devmlbd@gmail.com",
                    FromFullName = "Web Admin Notification",
                    IsDefault = false,

                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    CreatedBy = "Admin",
                    ModifiedBy = "Admin",
                };
                _context.Add(_SendGridOptions);
                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateDefaultDoctorUser()
        {
            SeedData _SeedData = new SeedData();
            var _GetDoctorsInfoList = _SeedData.GetDoctorsInfoList();
            foreach (var item in _GetDoctorsInfoList)
            {
                item.RoleId = 3;
                var _ApplicationUser = await _iAccount.CreateUserProfile(item, "Admin");
                if (_ApplicationUser.Item2 == "Success")
                {
                    DoctorsInfo _DoctorsInfo = new DoctorsInfo
                    {
                        ApplicationUserId = _ApplicationUser.Item1.Id,
                        DesignationId = 1,
                        DoctorsID = "MB" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                        DoctorFee = 500,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        CreatedBy = "Admin",
                        ModifiedBy = "Admin"
                    };
                    _context.DoctorsInfo.Add(_DoctorsInfo);
                    var result = _context.SaveChanges();
                }
            }
        }
        public async Task CreateDefaultOtherUser()
        {
            SeedData _SeedData = new SeedData();
            var _GetUserProfileList = _SeedData.GetUserProfileList();

            foreach (var item in _GetUserProfileList)
            {
                item.RoleId = 2;
                item.EmployeeId = StaticData.RandomDigits(6);
                item.DateOfBirth = DateTime.Now.AddYears(-25);
                item.Designation = 1;
                item.Department = 1;
                item.SubDepartment = 1;
                item.JoiningDate = DateTime.Now.AddYears(-1);
                item.LeavingDate = DateTime.Now;
                var _ApplicationUser = await _iAccount.CreateUserProfile(item, "Admin");
            }
        }
        public async Task<SharedUIDataViewModel> GetSharedUIData(ClaimsPrincipal _ClaimsPrincipal)
        {
            SharedUIDataViewModel _SharedUIDataViewModel = new SharedUIDataViewModel();
            ApplicationUser _ApplicationUser = await _userManager.GetUserAsync(_ClaimsPrincipal);
            _SharedUIDataViewModel.UserProfile = _context.UserProfile.SingleOrDefault(x => x.ApplicationUserId.Equals(_ApplicationUser.Id));
            _SharedUIDataViewModel.MainMenuViewModel = await _roles.RolebaseMenuLoad(_ApplicationUser);
            _SharedUIDataViewModel.ApplicationInfo = _applicationInfo;
            return _SharedUIDataViewModel;
        }
        public async Task<DefaultIdentityOptions> GetDefaultIdentitySettings()
        {
            return await _context.DefaultIdentityOptions.Where(x => x.Id == 1).SingleOrDefaultAsync();
        }
        public async Task CreateDefaultIdentitySettings()
        {
            if (_context.DefaultIdentityOptions.Count() < 1)
            {
                DefaultIdentityOptions _DefaultIdentityOptions = new DefaultIdentityOptions
                {
                    PasswordRequireDigit = false,
                    PasswordRequiredLength = 3,
                    PasswordRequireNonAlphanumeric = false,
                    PasswordRequireUppercase = false,
                    PasswordRequireLowercase = false,
                    PasswordRequiredUniqueChars = 0,
                    LockoutDefaultLockoutTimeSpanInMinutes = 30,
                    LockoutMaxFailedAccessAttempts = 5,
                    LockoutAllowedForNewUsers = false,
                    UserRequireUniqueEmail = true,
                    SignInRequireConfirmedEmail = false,

                    CookieHttpOnly = true,
                    CookieExpiration = 150,
                    CookieExpireTimeSpan = 120,
                    LoginPath = "/Account/Login",
                    LogoutPath = "/Account/Logout",
                    AccessDeniedPath = "/Account/AccessDenied",
                    SlidingExpiration = true,

                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    CreatedBy = "Admin",
                    ModifiedBy = "Admin",
                };
                _context.Add(_DefaultIdentityOptions);
                await _context.SaveChangesAsync();
            }
        }
    }
}
