﻿using KraujoBankasASP.Models;
using KraujoBankasASP.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KraujoBankasASP.Controllers
{
    public class ModeratorController : Controller
    {
        private UserManager<User> UserMgr { get; set; }
        private AppDbContext _context;

        public ModeratorController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            UserMgr = userManager;
        }

        public async Task<IActionResult> Index()
        {
            User CurrentUser = await UserMgr.GetUserAsync(HttpContext.User);

            var employees = _context.Employees.Join(_context.Users,
                e => e.UserFk,
                u => u.Id,
                (emp, user) => new EmployeesSummaryViewModel
                {
                   FName = user.FName,
                   LName = user.LName,
                  // Position = emp.PositionFK
                }
           );

            return View("Index", employees);
        }

        [HttpGet]
        public IActionResult CreateEmployee()
        {

            return View("AddEmployee");
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(AddEmployeeViewModel model)
        {
            var user = new User
            {
                UserName = model.Email,
                FName = model.FName,
                LName = model.LName
            };

            var result = await UserMgr.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                User Currentuser = await UserMgr.GetUserAsync(HttpContext.User);

                var epmloyee = new Employee
                {
                    //PositionFk = model.PositionFk,
                    InstitutionFK = Currentuser.Employee.InstitutionFK,
                    UserFk = user.Id
                };

                _context.Employees.Add(epmloyee);
                _context.SaveChanges();
            }

            return View("AddEmployee", model);
        }
    }
}