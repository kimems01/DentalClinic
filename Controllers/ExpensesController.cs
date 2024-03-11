using HMS.Data;
using HMS.Models;
using HMS.Models.ExpensesViewModel;
using HMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace HMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;

        public ExpensesController(ApplicationDbContext context, ICommon iCommon)
        {
            _context = context;
            _iCommon = iCommon;
        }

        [Authorize(Roles = Pages.RoleNameList.Expenses)]
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

                var _GetGridItem = _iCommon.GetExpensesGridItem();
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
                    || obj.ExpenseCategoriesName.ToLower().Contains(searchValue)
                    || obj.Amount.ToString().ToLower().Contains(searchValue)
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

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();
            ExpensesGridViewModel vm = await _iCommon.GetExpensesGridItem().Where(x => x.Id == id).SingleOrDefaultAsync();
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }

        public async Task<IActionResult> AddEdit(int id)
        {
            ViewBag._LoadddlExpenseCategories = new SelectList(_iCommon.LoadddlExpenseCategories(), "Id", "Name");
            ExpensesCRUDViewModel vm = new ExpensesCRUDViewModel();
            if (id > 0) vm = await _context.Expenses.Where(x => x.Id == id).SingleOrDefaultAsync();
            return PartialView("_AddEdit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(ExpensesCRUDViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        Expenses _Expenses = new Expenses();
                        if (vm.Id > 0)
                        {
                            _Expenses = await _context.Expenses.FindAsync(vm.Id);

                            vm.CreatedDate = _Expenses.CreatedDate;
                            vm.CreatedBy = _Expenses.CreatedBy;
                            vm.ModifiedDate = DateTime.Now;
                            vm.ModifiedBy = HttpContext.User.Identity.Name;
                            _context.Entry(_Expenses).CurrentValues.SetValues(vm);
                            await _context.SaveChangesAsync();
                            TempData["successAlert"] = "Expenses Updated Successfully. ID: " + _Expenses.Id;
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            _Expenses = vm;
                            _Expenses.CreatedDate = DateTime.Now;
                            _Expenses.ModifiedDate = DateTime.Now;
                            _Expenses.CreatedBy = HttpContext.User.Identity.Name;
                            _Expenses.ModifiedBy = HttpContext.User.Identity.Name;
                            _context.Add(_Expenses);
                            await _context.SaveChangesAsync();
                            TempData["successAlert"] = "Expenses Created Successfully. ID: " + _Expenses.Id;
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
                var _Expenses = await _context.Expenses.FindAsync(id);
                _Expenses.ModifiedDate = DateTime.Now;
                _Expenses.ModifiedBy = HttpContext.User.Identity.Name;
                _Expenses.Cancelled = true;

                _context.Update(_Expenses);
                await _context.SaveChangesAsync();
                return new JsonResult(_Expenses);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsExists(long id)
        {
            return _context.Expenses.Any(e => e.Id == id);
        }
    }
}
