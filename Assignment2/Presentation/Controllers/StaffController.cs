﻿using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class StaffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
