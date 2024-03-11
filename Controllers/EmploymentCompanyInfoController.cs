using HMS.Data;
using HMS.Models;
using HMS.Models.EmploymentCompanyInfoViewModel;
using HMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace HMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class EmploymentCompanyInfoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;

        public EmploymentCompanyInfoController(ApplicationDbContext context, ICommon iCommon)
        {
            _context = context;
            _iCommon = iCommon;
        }

        [Authorize(Roles = Pages.RoleNameList.EmploymentCompanyInfo)]
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

                var _GetGridItem = GetGridItem();
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
                    || obj.Name.ToLower().Contains(searchValue)
                    || obj.Address.ToLower().Contains(searchValue)
                    || obj.Phone.ToLower().Contains(searchValue)
                    || obj.Email.ToLower().Contains(searchValue)
                    || obj.CoverageDetails.ToLower().Contains(searchValue)
                    || obj.CreatedDate.ToString().Contains(searchValue));
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

        private IQueryable<EmploymentCompanyInfoGridViewModel> GetGridItem()
        {
            try
            {
                return (from _EmploymentCompanyInfo in _context.EmploymentCompanyInfo
                        where _EmploymentCompanyInfo.Cancelled == false
                        select new EmploymentCompanyInfoGridViewModel
                        {
                            Id = _EmploymentCompanyInfo.Id,
                            Name = _EmploymentCompanyInfo.Name,
                            Address = _EmploymentCompanyInfo.Address,
                            Phone = _EmploymentCompanyInfo.Phone,
                            Email = _EmploymentCompanyInfo.Email,
                            CoverageDetails = _EmploymentCompanyInfo.CoverageDetails,
                            CreatedDate = _EmploymentCompanyInfo.CreatedDate,

                        }).OrderByDescending(x => x.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();
            EmploymentCompanyInfoCRUDViewModel vm = await _context.EmploymentCompanyInfo.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }

        public async Task<IActionResult> AddEdit(int id)
        {
            EmploymentCompanyInfoCRUDViewModel vm = new EmploymentCompanyInfoCRUDViewModel();
            if (id > 0) vm = await _context.EmploymentCompanyInfo.Where(x => x.Id == id).SingleOrDefaultAsync();
            return PartialView("_AddEdit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(EmploymentCompanyInfoCRUDViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        EmploymentCompanyInfo _EmploymentCompanyInfo = new EmploymentCompanyInfo();
                        if (vm.Id > 0)
                        {
                            _EmploymentCompanyInfo = await _context.EmploymentCompanyInfo.FindAsync(vm.Id);

                            vm.CreatedDate = _EmploymentCompanyInfo.CreatedDate;
                            vm.CreatedBy = _EmploymentCompanyInfo.CreatedBy;
                            vm.ModifiedDate = DateTime.Now;
                            vm.ModifiedBy = HttpContext.User.Identity.Name;
                            _context.Entry(_EmploymentCompanyInfo).CurrentValues.SetValues(vm);
                            await _context.SaveChangesAsync();
                            TempData["successAlert"] = "Employment Company Info Updated Successfully. Name: " + _EmploymentCompanyInfo.Name;
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            _EmploymentCompanyInfo = vm;
                            _EmploymentCompanyInfo.CreatedDate = DateTime.Now;
                            _EmploymentCompanyInfo.ModifiedDate = DateTime.Now;
                            _EmploymentCompanyInfo.CreatedBy = HttpContext.User.Identity.Name;
                            _EmploymentCompanyInfo.ModifiedBy = HttpContext.User.Identity.Name;
                            _context.Add(_EmploymentCompanyInfo);
                            await _context.SaveChangesAsync();
                            TempData["successAlert"] = "Employment Company Info Created Successfully. Name: " + _EmploymentCompanyInfo.Name;
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    TempData["errorAlert"] = "Operation failed.";
                    return View("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!IsExists(vm.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Int64 id)
        {
            try
            {
                var _EmploymentCompanyInfo = await _context.EmploymentCompanyInfo.FindAsync(id);
                _EmploymentCompanyInfo.ModifiedDate = DateTime.Now;
                _EmploymentCompanyInfo.ModifiedBy = HttpContext.User.Identity.Name;
                _EmploymentCompanyInfo.Cancelled = true;

                _context.Update(_EmploymentCompanyInfo);
                await _context.SaveChangesAsync();
                return new JsonResult(_EmploymentCompanyInfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsExists(long id)
        {
            return _context.EmploymentCompanyInfo.Any(e => e.Id == id);
        }
    }
}
