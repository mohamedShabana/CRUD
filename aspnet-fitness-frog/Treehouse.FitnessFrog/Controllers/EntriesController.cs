using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add()
        {
            var entry = new Entry()
            {
                Date = DateTime.Today.Date,
            };
            SetupActivitiesSelectListItems();
            return View(entry);
        }
        [HttpPost]
        public ActionResult Add(Entry entry)
        {
            //ModelState.AddModelError("","This is a global message");

            //string date = Request.Form["Date"];

            ValidateEntry(entry);

            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);

                // TODO Display the Entries list page
                TempData["Message"] = " Your entry was successfully added";
                return RedirectToAction("Index");
            }
            SetupActivitiesSelectListItems();

            return View(entry);
        }

        


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // TODO Get The Requested entry from the repository 
            Entry entry = _entriesRepository.GetEntry((int)id);
            // TODO Return  a status of " Not Found " if the Entry wase not found 
            if(entry == null)
            {
                return HttpNotFound();
            }
            // TODO Populate the active select list items ViewBag Property.
            SetupActivitiesSelectListItems();
            // TODO Pass that entry into the view 
            return View(entry);
        }

        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            // TODO Validate the entry.
            ValidateEntry(entry);
            // TODO If the Entry is Valid 
            // 1) Use The repository to update the entry 
            // 2) Redirect the user to the "Entries " List Page 
            if(ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);
                TempData["Message"] = " Your entry was successfully Updated";
                return RedirectToAction("Index");
            }
            // TODO Populate the activities select list items ViewBage property 

            SetupActivitiesSelectListItems();

            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // TODO retrieved entry for the provided ID parameter value .
            Entry entry = _entriesRepository.GetEntry((int)id);
            if(entry == null)
            {
                return HttpNotFound();
            }
            // TODO  Return "not found " if an entry wasn't found .

            // TODO pss entry to the view 

            return View(entry);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            // TODO Delete the Entry 
            _entriesRepository.DeleteEntry(id);
            // TODO redirect ot the Entry List page 
            TempData["Message"] = " Your entry was successfully Deleted";
            return RedirectToAction("Index");
        }

        private void ValidateEntry(Entry entry)
        {
            // if there aren't any Duration Field validation Errors
            // then make sure that duration is greater than "0".
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "The Duration Field Value must be greater than '0' ");
            }
        }
        private void SetupActivitiesSelectListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(Data.Data.Activities, "Id", "Name");
        }
    }
}