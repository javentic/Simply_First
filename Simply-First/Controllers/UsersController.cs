﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Simply_First.ViewModels;

namespace Simply_First.Controllers
{
    public class UsersController : Controller
    {
        private SimplyFirstVMContext db = new SimplyFirstVMContext();

        public string FindUserId()
        {
            string name = User.Identity.Name;

            IdentityUser user = db.Users.Where(u => u.UserName == name).FirstOrDefault();
            string userId = user.Id;

            return userId;
        }

        [AcceptVerbs("Get", "Post")]
        public ActionResult EditInformation()
        {
            db.UserInformation = null;
            return View();
        }
    }
}
