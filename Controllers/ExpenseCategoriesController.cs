using HMS.Data;
using HMS.Models;
using HMS.Models.ExpenseCategoriesViewModel;
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
    public class ExpenseCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;

        public ExpenseCategoriesController(ApplicationDbContext context, ICommon iCommon)
        {
            _context = context;
            _iCommon = iCommon;
        }

        [Authorize(Roles = Pages.RoleNameList.ExpenseCategories)]
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
                    || obj.Description.ToLower().Contains(searchValue)
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

        private IQueryable<ExpenseCategoriesGridViewModel> GetGridItem()
        {
            try
            {
                return (from _ExpenseCategories in _context.ExpenseCategories
                        where _ExpenseCategories.Cancelled == false
                        select new ExpenseCategoriesGridViewModel
                        {
                            Id = _ExpenseCategories.Id,
                            Name = _ExpenseCategories.Name,
                            Description = _ExpenseCategories.Description,
                            CreatedDate = _ExpenseCategories.CreatedDate,
                            ModifiedDate = _ExpenseCategories.ModifiedDate,
                            CreatedBy = _ExpenseCategories.CreatedBy,
                            ModifiedBy = _ExpenseCategories.ModifiedBy,
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
            ExpenseCategoriesCRUDViewModel vm = await _context.ExpenseCategories.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }

        public async Task<IActionResult> AddEdit(int id)
        {
            ExpenseCategoriesCRUDViewModel vm = new ExpenseCategoriesCRUDViewModel();
            if (id > 0) vm = await _context.ExpenseCategories.Where(x => x.Id == id).SingleOrDefaultAsync();
            return PartialView("_AddEdit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(ExpenseCategoriesCRUDViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        ExpenseCategories _ExpenseCategories = new ExpenseCategories();
                        if (vm.Id > 0)
                        {
                            _ExpenseCategories = await _context.ExpenseCategories.FindAsync(vm.Id);

                            vm.CreatedDate = _ExpenseCategories.CreatedDate;
                            vm.CreatedBy = _ExpenseCategories.CreatedBy;
                            vm.ModifiedDate = DateTime.Now;
                            vm.ModifiedBy = HttpContext.User.Identity.Name;
                            _context.Entry(_ExpenseCategories).CurrentValues.SetValues(vm);
                            await _context.SaveChangesAsync();
                            TempData["successAlert"] = "Expense Categories Updated Successfully. ID: " + _ExpenseCategories.Id;
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            _ExpenseCategories = vm;
                            _ExpenseCategories.CreatedDate = DateTime.Now;
                            _ExpenseCategories.ModifiedDate = DateTime.Now;
                            _ExpenseCategories.CreatedBy = HttpContext.User.Identity.Name;
                            _ExpenseCategories.ModifiedBy = HttpContext.User.Identity.Name;
                            _context.Add(_ExpenseCategories);
                            await _context.SaveChangesAsync();
                            TempData["successAlert"] = "Expense Categories Created Successfully. ID: " + _ExpenseCategories.Id;
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
                var _ExpenseCategories = await _context.ExpenseCategories.FindAsync(id);
                _ExpenseCategories.ModifiedDate = DateTime.Now;
                _ExpenseCategories.ModifiedBy = HttpContext.User.Identity.Name;
                _ExpenseCategories.Cancelled = true;

                _context.Update(_ExpenseCategories);
                await _context.SaveChangesAsync();
                return new JsonResult(_ExpenseCategories);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsExists(long id)
        {
            return _context.ExpenseCategories.Any(e => e.Id == id);
        }
    }
}
