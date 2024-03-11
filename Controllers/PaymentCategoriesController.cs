using HMS.ConHelper;
using HMS.Data;
using HMS.Helpers;
using HMS.Models;
using HMS.Models.PaymentCategoriesViewModel;
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
    public class PaymentCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IDBOperation _iDBOperation;

        public PaymentCategoriesController(ApplicationDbContext context, ICommon iCommon, IDBOperation iDBOperation)
        {
            _context = context;
            _iCommon = iCommon;
            _iDBOperation = iDBOperation;
        }

        [Authorize(Roles = Pages.RoleNameList.PaymentCategories)]
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
                    || obj.UnitPrice.ToString().ToLower().Contains(searchValue)
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

        private IQueryable<PaymentCategoriesGridViewModel> GetGridItem()
        {
            try
            {
                return (from _PaymentCategories in _context.PaymentCategories
                        where _PaymentCategories.Cancelled == false
                        select new PaymentCategoriesGridViewModel
                        {
                            Id = _PaymentCategories.Id,
                            Name = _PaymentCategories.Name,
                            UnitPrice = _PaymentCategories.UnitPrice,
                            Description = _PaymentCategories.Description,
                            CreatedDate = _PaymentCategories.CreatedDate,
                            ModifiedDate = _PaymentCategories.ModifiedDate,
                            CreatedBy = _PaymentCategories.CreatedBy,
                            ModifiedBy = _PaymentCategories.ModifiedBy,

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
            PaymentCategoriesCRUDViewModel vm = await _context.PaymentCategories.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }

        public async Task<IActionResult> AddEdit(int id)
        {
            PaymentCategoriesCRUDViewModel vm = new PaymentCategoriesCRUDViewModel();
            if (id > 0) vm = await _context.PaymentCategories.Where(x => x.Id == id).SingleOrDefaultAsync();
            return PartialView("_AddEdit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(PaymentCategoriesCRUDViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        PaymentCategories _PaymentCategories = new PaymentCategories();
                        if (vm.Id > 0)
                        {
                            _PaymentCategories = await _context.PaymentCategories.FindAsync(vm.Id);
                            vm.CreatedDate = _PaymentCategories.CreatedDate;
                            vm.CreatedBy = _PaymentCategories.CreatedBy;
                            vm.ModifiedDate = DateTime.Now;
                            vm.ModifiedBy = HttpContext.User.Identity.Name;
                            _context.Entry(_PaymentCategories).CurrentValues.SetValues(vm);
                            await _context.SaveChangesAsync();
                            TempData["successAlert"] = "Payment Categories Updated Successfully. ID: " + _PaymentCategories.Id;
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            _PaymentCategories = vm;
                            _PaymentCategories.PaymentItemCode = StaticData.GetUniqueID("CMN");
                            _PaymentCategories.CreatedDate = DateTime.Now;
                            _PaymentCategories.ModifiedDate = DateTime.Now;
                            _PaymentCategories.CreatedBy = HttpContext.User.Identity.Name;
                            _PaymentCategories.ModifiedBy = HttpContext.User.Identity.Name;
                            _context.Add(_PaymentCategories);
                            await _context.SaveChangesAsync();

                            TempData["successAlert"] = "Payment Categories Created Successfully. ID: " + _PaymentCategories.Id;
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
                var _PaymentCategories = await _context.PaymentCategories.FindAsync(id);
                _PaymentCategories.ModifiedDate = DateTime.Now;
                _PaymentCategories.ModifiedBy = HttpContext.User.Identity.Name;
                _PaymentCategories.Cancelled = true;

                _context.Update(_PaymentCategories);
                await _context.SaveChangesAsync();
                return new JsonResult(_PaymentCategories);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsExists(long id)
        {
            return _context.PaymentCategories.Any(e => e.Id == id);
        }
    }
}
