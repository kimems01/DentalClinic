using HMS.ConHelper;
using HMS.Data;
using HMS.Helpers;
using HMS.Models;
using HMS.Models.MedicineHistoryViewModel;
using HMS.Models.MedicinesViewModel;
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
    public class MedicinesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IDBOperation _iDBOperation;

        public MedicinesController(ApplicationDbContext context, ICommon iCommon, IDBOperation iDBOperation)
        {
            _context = context;
            _iCommon = iCommon;
            _iDBOperation = iDBOperation;
        }

        [Authorize(Roles = Pages.RoleNameList.Medicines)]
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

                var _GetGridItem = _iCommon.GetAllMedicines();
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
                    || obj.MedicineCategoryName.ToLower().Contains(searchValue)
                    || obj.MedicineName.ToLower().Contains(searchValue)
                    || obj.UnitPrice.ToString().Contains(searchValue)
                    || obj.Description.ToLower().Contains(searchValue)

                   || obj.CreatedDate.ToString().ToLower().Contains(searchValue));
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
            MedicinesCRUDViewModel vm = await _iCommon.GetAllMedicines().Where(x => x.Id == id).SingleOrDefaultAsync();
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }

        public async Task<IActionResult> AddEdit(int id)
        {
            ViewBag._LoadddlMedicineCategories = new SelectList(_iCommon.LoadddlMedicineCategories(), "Id", "Name");
            ViewBag._LoadddlUnit = new SelectList(_iCommon.LoadddlUnit(), "Id", "Name");
            ViewBag._LoadddlMedicineManufacture = new SelectList(_iCommon.LoadddlMedicineManufacture(), "Id", "Name");

            MedicinesCRUDViewModel vm = new MedicinesCRUDViewModel();
            if (id > 0)
            {
                vm = await _context.Medicines.Where(x => x.Id == id).SingleOrDefaultAsync();
            }
            else
            {
                vm.Code = StaticData.GetUniqueID("M");
            }
            return PartialView("_AddEdit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(MedicinesCRUDViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        Medicines _Medicines = new Medicines();
                        MedicineHistoryCRUDViewModel _MedicineHistoryCRUDViewModel = new MedicineHistoryCRUDViewModel();
                        if (vm.Id > 0)
                        {
                            var _OldItems = await _context.Medicines.FindAsync(vm.Id);
                            double? _OldQuantity = _OldItems.Quantity;


                            vm.OldUnitPrice = _OldItems.UnitPrice;
                            vm.OldSellPrice = _OldItems.SellPrice;
                            vm.ModifiedDate = DateTime.Now;
                            vm.ModifiedBy = HttpContext.User.Identity.Name;
                            _context.Entry(_OldItems).CurrentValues.SetValues(vm);
                            await _context.SaveChangesAsync();

                            _MedicineHistoryCRUDViewModel = vm;
                            _MedicineHistoryCRUDViewModel.Id = 0;
                            if (_OldQuantity > vm.Quantity)
                            {
                                _MedicineHistoryCRUDViewModel.TranQuantity = (int)(_OldQuantity - vm.Quantity);
                                _MedicineHistoryCRUDViewModel.Action = "Update Item with minus-" + vm.MedicineName;
                            }
                            else if (_OldQuantity == vm.Quantity)
                            {
                                _MedicineHistoryCRUDViewModel.TranQuantity = (int)(vm.Quantity - _OldQuantity);
                                _MedicineHistoryCRUDViewModel.Action = "Update Item information only-" + vm.MedicineName;
                            }
                            else
                            {
                                _MedicineHistoryCRUDViewModel.TranQuantity = (int)(vm.Quantity - _OldQuantity);
                                _MedicineHistoryCRUDViewModel.Action = "Update Item with addition-" + vm.MedicineName;
                            }
                            _MedicineHistoryCRUDViewModel.OldQuantity = _OldQuantity;
                            _MedicineHistoryCRUDViewModel.NewQuantity = vm.Quantity;

                            _context.MedicineHistory.Add(_MedicineHistoryCRUDViewModel);
                            await _context.SaveChangesAsync();
                            TempData["successAlert"] = "Item Updated Successfully. Item Name: " + vm.MedicineName;
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            _Medicines = vm;
                            _Medicines.OldUnitPrice = vm.UnitPrice;
                            _Medicines.OldSellPrice = vm.SellPrice;

                            _Medicines.PaymentItemCode = StaticData.GetUniqueID("MED");
                            _Medicines.CreatedDate = DateTime.Now;
                            _Medicines.ModifiedDate = DateTime.Now;
                            _Medicines.CreatedBy = HttpContext.User.Identity.Name;
                            _Medicines.ModifiedBy = HttpContext.User.Identity.Name;
                            _context.Add(_Medicines);
                            await _context.SaveChangesAsync();

                            vm = _Medicines;
                            _MedicineHistoryCRUDViewModel = vm;
                            _MedicineHistoryCRUDViewModel.Id = 0;
                            _MedicineHistoryCRUDViewModel.Action = "Create New Item-" + vm.MedicineName;
                            _MedicineHistoryCRUDViewModel.TranQuantity = 0;
                            _MedicineHistoryCRUDViewModel.OldQuantity = vm.Quantity;
                            _MedicineHistoryCRUDViewModel.NewQuantity = vm.Quantity;

                            _context.MedicineHistory.Add(_MedicineHistoryCRUDViewModel);
                            await _context.SaveChangesAsync();

                            TempData["successAlert"] = "Medicines Created Successfully. ID: " + _Medicines.Id;
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

        public async Task<IActionResult> UpdateQuantity(Int64 id)
        {
            MedicinesCRUDViewModel vm = new MedicinesCRUDViewModel();
            if (id > 0) vm = await _iCommon.GetAllMedicines().Where(x => x.Id == id).SingleOrDefaultAsync();
            return PartialView("_UpdateQuantity", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(MedicinesCRUDViewModel vm)
        {
            try
            {
                var _Medicines = _context.Medicines.Where(x => x.Id == vm.Id).SingleOrDefault();
                double? _OldQuantity = _Medicines.Quantity;
                _Medicines.Quantity = _Medicines.Quantity + vm.AddNewQuantity;
                _Medicines.UpdateQntType = vm.UpdateQntType;
                _Medicines.UpdateQntNote = vm.UpdateQntNote;
                _Medicines.ModifiedDate = DateTime.Now;
                _Medicines.ModifiedBy = HttpContext.User.Identity.Name;
                _context.Medicines.Update(_Medicines);
                await _context.SaveChangesAsync();

                int _AddNewQuantity = vm.AddNewQuantity;
                vm = _Medicines;
                MedicineHistoryCRUDViewModel _MedicineHistoryCRUDViewModel = vm;
                _MedicineHistoryCRUDViewModel.Action = "Update Quantity with addition-" + _Medicines.MedicineName;
                _MedicineHistoryCRUDViewModel.TranQuantity = _AddNewQuantity;
                _MedicineHistoryCRUDViewModel.OldQuantity = _OldQuantity;
                _MedicineHistoryCRUDViewModel.NewQuantity = _OldQuantity + _AddNewQuantity;

                _context.MedicineHistory.Add(_MedicineHistoryCRUDViewModel);
                await _context.SaveChangesAsync();

                TempData["successAlert"] = "Item Quantity Updated Successfully. Item Name: " + _Medicines.MedicineName;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["errorAlert"] = "Operation failed.";
                return View("Index");
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Int64 id)
        {
            try
            {
                Medicines _Medicines = await _context.Medicines.FindAsync(id);
                _Medicines.ModifiedDate = DateTime.Now;
                _Medicines.ModifiedBy = HttpContext.User.Identity.Name;
                _Medicines.Cancelled = true;
                var result = _context.Update(_Medicines);
                await _context.SaveChangesAsync();


                MedicinesCRUDViewModel _MedicinesCRUDViewModel = _Medicines;
                MedicineHistoryCRUDViewModel _MedicineHistoryCRUDViewModel = _MedicinesCRUDViewModel;

                _MedicineHistoryCRUDViewModel.Action = "Delete medecine item from current stock-" + _MedicinesCRUDViewModel.MedicineName;
                _MedicineHistoryCRUDViewModel.OldQuantity = _Medicines.Quantity;
                _MedicineHistoryCRUDViewModel.TranQuantity = (int)_Medicines.Quantity;
                _MedicineHistoryCRUDViewModel.NewQuantity = 0;
                _context.MedicineHistory.Add(_MedicineHistoryCRUDViewModel);
                await _context.SaveChangesAsync();

                return new JsonResult(_MedicinesCRUDViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsExists(long id)
        {
            return _context.Medicines.Any(e => e.Id == id);
        }
    }
}
