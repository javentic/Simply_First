﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Simply_First.Models;

namespace Simply_First.Controllers
{
    //[Authorize(Roles = "3d50c8fc-ae81-4f7f-b328-1ce5ca630662")]
    public class ProductsController : Controller
    {
        private SimplyFirstVMContext db = new SimplyFirstVMContext();

        // GET: Products
        // [Authorize(Roles = "3d50c8fc-ae81-4f7f-b328-1ce5ca630662")]
        public async Task<ActionResult> Index()
        {
            return View(await db.Products.ToListAsync());
        }

        // GET: Products/Details/5
        // [Authorize(Roles = "3d50c8fc-ae81-4f7f-b328-1ce5ca630662")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Products products = await db.Products.FindAsync(id);

            if (products == null)
            {
                return HttpNotFound();
            }

            return View(products);
        }

        // GET: Products/Create
        // [Authorize(Roles = "3d50c8fc-ae81-4f7f-b328-1ce5ca630662")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [Authorize(Roles = "3d50c8fc-ae81-4f7f-b328-1ce5ca630662")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ProductId,ProductName,ProductImage,ProductDescription,Manufacturer,Quantity,Price")] Products products)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(products);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(products);
        }

        // GET: Products/Edit/5
        // [Authorize(Roles = "3d50c8fc-ae81-4f7f-b328-1ce5ca630662")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Products products = await db.Products.FindAsync(id);
            if (products == null)
            {
                return HttpNotFound();
            }

            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Roles = "3d50c8fc-ae81-4f7f-b328-1ce5ca630662")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ProductId,ProductName,ProductImage,ProductDescription,Manufacturer,Quantity,Price")] Products products)
        {
            if (ModelState.IsValid)
            {
                db.Entry(products).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(products);
        }

        // GET: Products/Delete/5
        //  [Authorize(Roles = "3d50c8fc-ae81-4f7f-b328-1ce5ca630662")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Products products = await db.Products.FindAsync(id);
            if (products == null)
            {
                return HttpNotFound();
            }

            return View(products);
        }

        // POST: Products/Delete/5
        //  [Authorize(Roles = "3d50c8fc-ae81-4f7f-b328-1ce5ca630662")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Products products = await db.Products.FindAsync(id);
            db.Products.Remove(products);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        //public void AddToCart(object sender, EventArgs e)
        //{
        //    // Add product 1 to the shopping cart
        //    ShoppingCart.Instance.AddItem(1);

        //    // Redirect the user to view their shopping cart
        //   // Response.Redirect("ViewCart.aspx");
        //}

        public ActionResult AddtoCart(int id)
        {
            ShoppingCart.Instance.AddItem(id);
            //Console.WriteLine("SOMETHING HAPPENED");
            return RedirectToAction("Purchase", "Home");
        }

        public ActionResult ViewCart()
        {

            //Console.WriteLine("SOMETHING HAPPENED");
            return View();
        }

    }

}
