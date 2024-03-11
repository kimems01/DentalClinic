using HMS.Data;
using HMS.Models;
using HMS.Models.PatientInfoViewModel;
using HMS.Services;
using HMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class PatientInfoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;

        public PatientInfoController(ApplicationDbContext context, ICommon iCommon)
        {
            _context = context;
            _iCommon = iCommon;
        }

        [Authorize(Roles = Pages.RoleNameList.PatientInfo)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetDataTabelData()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();

                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnAscDesc = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int resultTotal = 0;

                var _GetGridItem = _iCommon.GetPatientInfoGridItem();
                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnAscDesc)))
                {
                    _GetGridItem = _GetGridItem.OrderBy(sortColumn + " " + sortColumnAscDesc);
                }

                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    searchValue = searchValue.ToLower();
                    _GetGridItem = _GetGridItem.Where(obj => obj.Id.ToString().Contains(searchValue)
                    || obj.OtherNames.ToLower().Contains(searchValue)
                    || obj.Surname.ToLower().Contains(searchValue)
                    || obj.Gender.ToLower().Contains(searchValue)
                    || obj.Phone.ToLower().Contains(searchValue)
                    || obj.NationalID.ToString().ToLower().Contains(searchValue)
                    || obj.Residence.ToLower().Contains(searchValue));
                }

                resultTotal = _GetGridItem.Count();

                var result = _GetGridItem.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = resultTotal, recordsTotal = resultTotal, data = result });

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();
            PatientInfoCRUDViewModel vm = await _context.PatientInfo.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            await InitializeDropdownData();
            return PartialView("_Details", vm);
        }

        public async Task<IActionResult> AddEdit(int id)
        {
            PatientInfoCRUDViewModel vm = new PatientInfoCRUDViewModel();
            if (id > 0) vm = await _context.PatientInfo.Where(x => x.Id == id).SingleOrDefaultAsync();
            await InitializeDropdownData();
            return PartialView("_AddEdit", vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(PatientInfoCRUDViewModel vm)
        {
            try
            {
                JsonResultViewModel _JsonResultViewModel = new();
                PatientInfo _PatientInfo = new PatientInfo();
                    if (vm.Id > 0)
                    {
                        _PatientInfo = await _context.PatientInfo.FindAsync(vm.Id);

                        vm.CreatedDate = _PatientInfo.CreatedDate;
                        vm.CreatedBy = _PatientInfo.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = HttpContext.User.Identity.Name;
                        _context.Entry(_PatientInfo).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();

                        _JsonResultViewModel.AlertMessage = "Patient Info Updated Successfully. ID: " + _PatientInfo.Id;
                        _JsonResultViewModel.IsSuccess = true;
                        return new JsonResult(_JsonResultViewModel);
                    }
                    else
                    {
                        var _Code = "P" + DateTime.Now.ToString("yyyyMMddHHmmss");
                        _PatientInfo = vm;
                        _PatientInfo.PatientCode = _Code;
                        _PatientInfo.CreatedDate = DateTime.Now;
                        _PatientInfo.ModifiedDate = DateTime.Now;
                        _PatientInfo.CreatedBy = HttpContext.User.Identity.Name;
                        _PatientInfo.ModifiedBy = HttpContext.User.Identity.Name;
                        _context.Add(_PatientInfo);
                        await _context.SaveChangesAsync();

                        _JsonResultViewModel.AlertMessage = "Patient Info Created Successfully. ID: " + _PatientInfo.Id;
                        _JsonResultViewModel.IsSuccess = true;
                        return new JsonResult(_JsonResultViewModel);
                    }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new JsonResult(ex.Message);
                throw ex;
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Int64 id)
        {
            try
            {
                var _PatientInfo = await _context.PatientInfo.FindAsync(id);
                _PatientInfo.ModifiedDate = DateTime.Now;
                _PatientInfo.ModifiedBy = HttpContext.User.Identity.Name;
                _PatientInfo.Cancelled = true;

                _context.Update(_PatientInfo);
                await _context.SaveChangesAsync();
                return new JsonResult(_PatientInfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task InitializeDropdownData()
        {
            var employmentCompanies = await _context.EmploymentCompanyInfo
               .Select(c => new SelectListItem
               {
                   Value = c.Id.ToString(),
                   Text = c.Name
               })
               .ToListAsync();

            var insuranceCompanies = await _context.InsuranceCompanyInfo
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            var families = await _context.FamilyInfo
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            ViewBag._EmploymentCompanies = employmentCompanies;
            ViewBag._InsuranceCompanies = insuranceCompanies;
            ViewBag._Families = families;
        }
    }
}
