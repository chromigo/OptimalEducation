﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OptimalEducation.Models;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.Areas.Admin.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class CityController : Controller
    {
        private readonly IOptimalEducationDbContext _dbContext;

        public CityController(IOptimalEducationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // GET: /Admin/City/
        public async Task<ActionResult> Index()
        {
            return View(await _dbContext.Cities.ToListAsync());
        }

        // GET: /Admin/City/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = await _dbContext.Cities.FindAsync(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }

        // GET: /Admin/City/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Admin/City/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="Id,Name,Prestige")] City city)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Cities.Add(city);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(city);
        }

        // GET: /Admin/City/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = await _dbContext.Cities.FindAsync(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }

        // POST: /Admin/City/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Id,Name,Prestige")] City city)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Entry(city).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(city);
        }

        // GET: /Admin/City/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = await _dbContext.Cities.FindAsync(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }

        // POST: /Admin/City/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            City city = await _dbContext.Cities.FindAsync(id);
            _dbContext.Cities.Remove(city);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
