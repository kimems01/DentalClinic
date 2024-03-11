﻿using HMS.ConHelper;
using HMS.Data;
using HMS.Helpers;
using HMS.Models;
using HMS.Models.LabTestConfigurationViewModel;
using HMS.Models.LabTestsViewModel;
using HMS.Models.ProcedureViewModel;
using HMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace HMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ProceduresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IDBOperation _iDBOperation;

        public ProceduresController(ApplicationDbContext context, ICommon iCommon, IDBOperation iDBOperation)
        {
            _context = context;
            _iCommon = iCommon;
            _iDBOperation = iDBOperation;
        }

        [Authorize(Roles = Pages.RoleNameList.Procedures)]
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

                var _GetGridItem = _iCommon.GetAllProcedures();
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
                    || obj.ProcedureCategoryName.ToLower().Contains(searchValue)
                    || obj.ProcedureName.ToLower().Contains(searchValue)
                    || obj.Unit.ToLower().Contains(searchValue)
                    || obj.ReferenceRange.ToLower().Contains(searchValue)
                    || obj.Status.ToString().ToLower().Contains(searchValue)
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
            ManageLabTestConfigurationViewModel vm = new ManageLabTestConfigurationViewModel();
            vm.LabTestsCRUDViewModel = await _iCommon.GetAllLabTests().Where(x => x.Id == id).SingleOrDefaultAsync();
            vm.listLabTestConfiguration = _context.LabTestConfiguration.Where(x => x.LabTestsId == id && x.Cancelled == false).OrderBy(x => x.Sorting).ToList();
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }

        public async Task<IActionResult> AddEdit(int id)
        {
            ViewBag._LoadddlProcedureCategories = new SelectList(_iCommon.LoadddlProcedureCategories(), "Id", "Name");

            ProceduresCRUDViewModel vm = new ProceduresCRUDViewModel();
            if (id > 0) vm = await _context.Procedures.Where(x => x.Id == id).SingleOrDefaultAsync();
            return PartialView("_AddEdit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(ProceduresCRUDViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        Procedures _Procedures = new Procedures();
                        if (vm.Id > 0)
                        {
                            _Procedures = await _context.Procedures.FindAsync(vm.Id);

                            vm.CreatedDate = _Procedures.CreatedDate;
                            vm.CreatedBy = _Procedures.CreatedBy;
                            vm.ModifiedDate = DateTime.Now;
                            vm.ModifiedBy = HttpContext.User.Identity.Name;
                            _context.Entry(_Procedures).CurrentValues.SetValues(vm);
                            await _context.SaveChangesAsync();
                            TempData["successAlert"] = "Procedures Updated Successfully. ID: " + _Procedures.Id;
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            _Procedures = vm;
                            _Procedures.PaymentItemCode = StaticData.GetUniqueID("PROC");
                            _Procedures.CreatedDate = DateTime.Now;
                            _Procedures.ModifiedDate = DateTime.Now;
                            _Procedures.CreatedBy = HttpContext.User.Identity.Name;
                            _Procedures.ModifiedBy = HttpContext.User.Identity.Name;
                            _context.Add(_Procedures);
                            await _context.SaveChangesAsync();

                            TempData["successAlert"] = "Procedure Updated Successfully. ID: " + _Procedures.Id;
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
                var _Procedure = await _context.Procedures.FindAsync(id);
                _Procedure.ModifiedDate = DateTime.Now;
                _Procedure.ModifiedBy = HttpContext.User.Identity.Name;
                _Procedure.Cancelled = true;

                _context.Update(_Procedure);
                await _context.SaveChangesAsync();
                return new JsonResult(_Procedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsExists(long id)
        {
            return _context.Procedures.Any(e => e.Id == id);
        }
    }
}
