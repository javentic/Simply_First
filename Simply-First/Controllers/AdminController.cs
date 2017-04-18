﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Simply_First.ViewModels;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net;
using Simply_First.Repositories;

namespace Simply_First.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private SimplyFirstVMContext db = new SimplyFirstVMContext();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(new SimplyFirstVMContext()));

            var user = userManager.Users.ToList();
            
            // This code could go in a repo.
            List<SiteUserVM> siteUsers = new List<SiteUserVM>();

            foreach (var users in user)
            {
                SiteUserVM siteUser = new SiteUserVM();
                siteUser.Id = users.Id;
                siteUser.Email = users.Email;
                siteUser.UserName = users.UserName;
                siteUser.EmailConfirmed = users.EmailConfirmed;
                siteUser.PhoneNumber = users.PhoneNumber;
                siteUsers.Add(siteUser);
            }
            
            return View(siteUsers);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UserDetails(string id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);    
            }

            DatabaseUsersRepo dbUsers = new DatabaseUsersRepo();
            var users = dbUsers.GetAll(id);
            
            if(users == null)
            {
                return HttpNotFound();
            }

            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UserRoles()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new SimplyFirstVMContext()));

            var role = roleManager.Roles.ToList();

            List<SiteUsersRoleVM> siteUsersRoles = new List<SiteUsersRoleVM>();

            foreach (var roles in role)
            {
                SiteUsersRoleVM userRoles = new SiteUsersRoleVM();

                userRoles.UserId = roles.Id;
                userRoles.RoleId = roles.Name;
                userRoles.Users = roles.Users.ToList();
                siteUsersRoles.Add(userRoles);
            }

            return View(siteUsersRoles);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddRole()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddRole(RoleVM roleVM)
        {
            if (ModelState.IsValid)
            {
                // *** New: Connect to AspNetRole using code first.
                using (var db = new SimplyFirstVMContext())
                {
                    IdentityRole role = new IdentityRole();

                    Guid g = Guid.NewGuid();

                    // See if role exists.
                    var existingRoles = db.Roles.Where(r => r.Id == g.ToString() || r.Name == roleVM.RoleName);

                    if (existingRoles.Count() > 0)
                    {
                        return View();
                    }
                    role.Id = g.ToString();
                    role.Name = roleVM.RoleName;
                    db.Roles.Add(role);
                    db.SaveChanges();

                    TempData["AddRoleSuccess"] = "Added '" + role.Id + "' to the Simply First Role!";
                    return RedirectToAction("AddRole");
                }
            }

            return View(roleVM);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddUserToRole()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddUserToRole(UserRoleVM userRoleVM)
        {
            if (ModelState.IsValid)
            {
                using (var db = new SimplyFirstVMContext())
                {
                    var user = db.Users.Where(e => e.Email == userRoleVM.Email).FirstOrDefault();
                    //var role = db.Roles.Where(r => r.Name == userRoleVM.RoleName).FirstOrDefault();

                    //var userRole = new IdentityRole();
                    //userRole.Id = role.Id;
                    //userRole.Name = user.Id;
                    //db.Roles.Add(userRole);
                    //db.SaveChanges();

                    var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(new SimplyFirstVMContext()));

                    if (user == null) return RedirectToAction("AddUserToRole");

                    userManager.AddToRoles(user.Id, new string[] { userRoleVM.RoleName });

                    TempData["AddUserToRoleSuccess"] = "Added '" + user.Email + "' to the " + userRoleVM.RoleName + " Simply First Role!";

                    return RedirectToAction("AddUserToRole");
                }
            }

            return View(userRoleVM);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EditUser(string id)
        {
            string name = id;
            var userManagerMain = new UserManager<IdentityUser>(new UserStore<IdentityUser>(new SimplyFirstVMContext()));
            using (var db = new SimplyFirstVMContext())
            {
                var user = db.Users.Where(e => e.Id == id).FirstOrDefault();
                //System.Diagnostics.Debug.WriteLine(user.Email);

                //userManager.SetPhoneNumber(id, "6045553217");

                if (user != null)
                {
                    TempData["Id"] = name;
                    TempData["Email"] = user.Email;
                    TempData["PhoneNumber"] = user.PhoneNumber;
                    TempData["UserName"] = user.UserName;
                    TempData["EmailConfirmed"] = user.EmailConfirmed;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("User has a null value!");
                }


            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditUserPost(SiteUserVM userTable)
        {
            System.Diagnostics.Debug.WriteLine(userTable.Id);
            string userId = userTable.Id;
            var userManagerMain = new UserManager<IdentityUser>(new UserStore<IdentityUser>(new SimplyFirstVMContext()));
            using (var db = new SimplyFirstVMContext())
            {
                var user = db.Users.Where(e => e.Id == userId).FirstOrDefault();
                System.Diagnostics.Debug.WriteLine(user.Email);
                if (userTable.Email != userManagerMain.GetEmail(userId))
                {
                    userManagerMain.SetEmail(userTable.Id, userTable.Email);
                }

                if (userTable.UserName != user.UserName)
                {
                    user.UserName = userTable.UserName;
                    db.SaveChanges();
                }

                if (userTable.EmailConfirmed != userManagerMain.IsEmailConfirmed(userId))
                {
                    user.EmailConfirmed = userTable.EmailConfirmed;
                    db.SaveChanges();
                }

                if (userTable.PhoneNumber != userManagerMain.GetPhoneNumber(userId))
                {
                    userManagerMain.SetPhoneNumber(userTable.Id, userTable.PhoneNumber);
                }


            }

            return RedirectToAction("Index", "Admin");
        }
    }
}
