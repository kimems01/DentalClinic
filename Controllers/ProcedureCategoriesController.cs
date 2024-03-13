using HMS.Data;
using HMS.Models;
using HMS.Models.ProcedureCategoriesViewModel;
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
    public class ProcedureCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;

        public ProcedureCategoriesController(ApplicationDbContext context, ICommon iCommon)
        {
            _context = context;
            _iCommon = iCommon;
        }

        [Authorize(Roles = Pages.RoleNameList.ProcedureCategories)]
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

        private IQueryable<ProcedureCategoriesGridViewModel> GetGridItem()
        {
            try
            {
                return (from _ProcedureCategories in _context.ProcedureCategories
                        where _ProcedureCategories.Cancelled == false
                        select new ProcedureCategoriesGridViewModel
                        {
                            Id = _ProcedureCategories.Id,
                            Name = _ProcedureCategories.Name,
                            Description = _ProcedureCategories.Description,
                            CreatedDate = _ProcedureCategories.CreatedDate,

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
            ProcedureCategoriesCRUDViewModel vm = await _context.ProcedureCategories.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }

        public async Task<IActionResult> AddEdit(int id)
        {
            ProcedureCategoriesCRUDViewModel vm = new ProcedureCategoriesCRUDViewModel();
            if (id > 0) vm = await _context.ProcedureCategories.Where(x => x.Id == id).SingleOrDefaultAsync();
            return PartialView("_AddEdit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(ProcedureCategoriesCRUDViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        ProcedureCategories _ProcedureCategories = new ProcedureCategories();
                        if (vm.Id > 0)
                        {
                            _ProcedureCategories = await _context.ProcedureCategories.FindAsync(vm.Id);

                            vm.CreatedDate = _ProcedureCategories.CreatedDate;
                            vm.CreatedBy = _ProcedureCategories.CreatedBy;
                            vm.ModifiedDate = DateTime.Now;
                            vm.ModifiedBy = HttpContext.User.Identity.Name;
                            _context.Entry(_ProcedureCategories).CurrentValues.SetValues(vm);
                            await _context.SaveChangesAsync();
                            TempData["successAlert"] = "Procedure Category Updated Successfully. Name: " + _ProcedureCategories.Name;
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            _ProcedureCategories = vm;
                            _ProcedureCategories.CreatedDate = DateTime.Now;
                            _ProcedureCategories.ModifiedDate = DateTime.Now;
                            _ProcedureCategories.CreatedBy = HttpContext.User.Identity.Name;
                            _ProcedureCategories.ModifiedBy = HttpContext.User.Identity.Name;
                            _context.Add(_ProcedureCategories);
                            await _context.SaveChangesAsync();
                            TempData["successAlert"] = "Procedure Category Created Successfully. Name: " + _ProcedureCategories.Name;
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
                var _ProcedureCategories = await _context.ProcedureCategories.FindAsync(id);
                _ProcedureCategories.ModifiedDate = DateTime.Now;
                _ProcedureCategories.ModifiedBy = HttpContext.User.Identity.Name;
                _ProcedureCategories.Cancelled = true;

                _context.Update(_ProcedureCategories);
                await _context.SaveChangesAsync();
                return new JsonResult(_ProcedureCategories);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsExists(long id)
        {
            return _context.ProcedureCategories.Any(e => e.Id == id);
        }
    }
}