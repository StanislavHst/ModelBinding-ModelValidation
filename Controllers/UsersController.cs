using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ProductsValidation.Models;
using ProductsValidation.Services;

namespace ProductsValidation.Controllers
{
    public class UsersController : Controller
    {
        private List<User> users;

        public UsersController(Data data)
        {
            users = data.Users;
        }
        
        public IActionResult Index(string id)
        {
            return View("Index", users);
        }

        [HttpGet]
        public IActionResult Create([FromQuery] User user)
        {
            return View(user);
        }

        [HttpPost("/Users/Create"), ValidateAntiForgeryToken]
        public IActionResult Save([FromQuery] User user)
        {
            if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email)
                || string.IsNullOrWhiteSpace(user.Role))
            {
                ModelState.AddModelError("", "Set name, email and role in the query string.");
                return View("Create", user);
            }

            user.Id = users.Max(item => item.Id) + 1;
            users.Add(user);
            return View("Details", user);
        }
    }
}
