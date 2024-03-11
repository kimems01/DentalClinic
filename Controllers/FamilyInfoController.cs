using HMS.Data;
using HMS.Models;
using HMS.Models.FamilyInfoViewModel;
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
    public class FamilyInfoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;

        public FamilyInfoController(ApplicationDbContext context, ICommon iCommon)
        {
            _context = context;
            _iCommon = iCommon;
        }

        [Authorize(Roles = Pages.RoleNameList.FamilyInfo)]
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

        private IQueryable<FamilyInfoGridViewModel> GetGridItem()
        {
            try
            {
                return (from _FamilyInfo in _context.FamilyInfo
                        where _FamilyInfo.Cancelled == false
                        select new FamilyInfoGridViewModel
                        {
                            Id = _FamilyInfo.Id,
                            Name = _FamilyInfo.Name,
                            CreatedDate = _FamilyInfo.CreatedDate,

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
            FamilyInfoCRUDViewModel vm = await _context.FamilyInfo.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }

        public async Task<IActionResult> AddEdit(int id)
        {
            FamilyInfoCRUDViewModel vm = new FamilyInfoCRUDViewModel();
            if (id > 0) vm = await _context.FamilyInfo.Where(x => x.Id == id).SingleOrDefaultAsync();
            return PartialView("_AddEdit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(FamilyInfoCRUDViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        FamilyInfo _FamilyInfo = new FamilyInfo();
                        if (vm.Id > 0)
                        {
                            _FamilyInfo = await _context.FamilyInfo.FindAsync(vm.Id);

                            vm.CreatedDate = _FamilyInfo.CreatedDate;
                            vm.CreatedBy = _FamilyInfo.CreatedBy;
                            vm.ModifiedDate = DateTime.Now;
                            vm.ModifiedBy = HttpContext.User.Identity.Name;
                            _context.Entry(_FamilyInfo).CurrentValues.SetValues(vm);
                            await _context.SaveChangesAsync();
                            TempData["successAlert"] = "Family Info Updated Successfully. Name: " + _FamilyInfo.Name;
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            _FamilyInfo = vm;
                            _FamilyInfo.CreatedDate = DateTime.Now;
                            _FamilyInfo.ModifiedDate = DateTime.Now;
                            _FamilyInfo.CreatedBy = HttpContext.User.Identity.Name;
                            _FamilyInfo.ModifiedBy = HttpContext.User.Identity.Name;
                            _context.Add(_FamilyInfo);
                            await _context.SaveChangesAsync();
                            TempData["successAlert"] = "Family Info Created Successfully. Name: " + _FamilyInfo.Name;
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
                var _FamilyInfo = await _context.FamilyInfo.FindAsync(id);
                _FamilyInfo.ModifiedDate = DateTime.Now;
                _FamilyInfo.ModifiedBy = HttpContext.User.Identity.Name;
                _FamilyInfo.Cancelled = true;

                _context.Update(_FamilyInfo);
                await _context.SaveChangesAsync();
                return new JsonResult(_FamilyInfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsExists(long id)
        {
            return _context.FamilyInfo.Any(e => e.Id == id);
        }
    }
}
