using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cars.Data;
using Cars.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;

namespace Cars.Controllers
{
    public class CarsController : Controller
    {
        private readonly CarDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILogger<CarsController> _logger;

        public CarsController(CarDbContext context, IWebHostEnvironment hostEnvironment, ILogger<CarsController> logger)
        {
            _context = context;
            _logger = logger;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: Cars
        public async Task<IActionResult> Index(int? page, string sortOrder, string searchString)
        {
            var items = _context.CarModels.AsQueryable(); // Start with IQueryable

            // Sorting logic
            ViewBag.NameSortParam = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParam = sortOrder == "ManufacturingDate" ? "ManufacturingDate_desc" : "ManufacturingDate";

            switch (sortOrder)
            {
                case "name_desc":
                    // Sort by name descending
                    items = items.OrderByDescending(s => s.ModelName);
                    break;
                case "ManufacturingDate":
                case "ManufacturingDate_desc":
                    // Sort by date descending
                    items = items.OrderByDescending(s => s.ManufacturingDate);
                    break;
                default:
                    // Default sorting, e.g., by name ascending
                    items = items.OrderBy(s => s.ModelName);
                    break;
            }

            // Searching logic
            _logger.LogInformation($"Search String: {searchString}");
            if (!string.IsNullOrEmpty(searchString))
            {
                items = items.Where(s => s.ModelName.Contains(searchString) || s.ModelCode.Contains(searchString));
            }

            // Materialize the query
            var paginatedItems = await items.ToListAsync();

            // Pagination logic
            int pageSize = 7; // Set your desired page size
            int pageNumber = page ?? 1; // If page is null, default to page 1

            // Set ViewBag properties for use in the view
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(paginatedItems.Count() / (double)pageSize);

            // Apply pagination to the items
            paginatedItems = paginatedItems.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Pass the sorted and paginated items to the view
            var sql = items.ToQueryString();
            _logger.LogInformation($"Generated SQL Query: {sql}");

            return View(paginatedItems);
        }






        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CarModels == null)
            {
                return NotFound();
            }

            var car = await _context.CarModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cars/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Brand,Class,ModelName,ModelCode,Description,Features,Price,ManufacturingDate,Active,SortOrder,CarImages")] Car car)
        {
            if (ModelState.IsValid)
            {
                if (car.Id == 0 && car.CarImages != null && car.CarImages.Count > 0)
                {
                    // Save images to wwwroot/image
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    List<string> imageNames = new List<string>();
                    //List<string> ExistingImageNames = new List<string>();

                    foreach (var image in car.CarImages)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(image.FileName);
                        string extension = Path.GetExtension(image.FileName);
                        string uniqueFileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath, "Image", uniqueFileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }

                        // Store the unique file name
                        imageNames.Add(uniqueFileName);
                        //ExistingImageNames.Add(uniqueFileName);
                    }

                    // Concatenate the image names with a comma as a delimiter
                    car.ImageName = string.Join(",", imageNames);

                    _context.Add(car);
                }
                else
                {
                    // Handle update logic if needed
                    _context.Update(car);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(car);
        }


        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CarModels == null)
            {
                return NotFound();
            }

            var car = await _context.CarModels.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Class,ModelName,ModelCode,Description,Features,Price,ManufacturingDate,Active,SortOrder,CarImages")] Car car)
        {
            _logger.LogInformation("Edit action started.");

            if (id != car.Id)
            {
                _logger.LogError("Invalid ID in Edit action.");
                return NotFound();
            }

            // Add the logging statement here
            _logger.LogInformation($"Form data: {string.Join(", ", Request.Form.Keys)}");

            if (ModelState.IsValid)
            {
                try
                {

                    if (car.Id != 0 && car.CarImages != null && car.CarImages.Count > 0)
                    {
                        // Save images to wwwroot/image
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        List<string> imageNames = new List<string>();
                        //List<string> ExistingImageNames = new List<string>();

                        foreach (var image in car.CarImages)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(image.FileName);
                            string extension = Path.GetExtension(image.FileName);
                            string uniqueFileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                            string path = Path.Combine(wwwRootPath, "Image", uniqueFileName);

                            using (var fileStream = new FileStream(path, FileMode.Create))
                            {
                                await image.CopyToAsync(fileStream);
                            }

                            // Store the unique file name
                            imageNames.Add(uniqueFileName);
                            //ExistingImageNames.Add(uniqueFileName);
                        }

                        // Concatenate the image names with a comma as a delimiter
                        car.ImageName = string.Join(",", imageNames);

                        _context.Update(car);
                        await _context.SaveChangesAsync();

                        _logger.LogInformation("Edit action successful.");

                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
                    {
                        _logger.LogError("Car not found in Edit action.");
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError("Concurrency exception in Edit action.");
                        throw;
                    }
                }
            }

            return View(car);
        }



        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CarModels == null)
            {
                return NotFound();
            }

            var car = await _context.CarModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CarModels == null)
            {
                return Problem("Entity set 'CarDbContext.CarModels'  is null.");
            }
            var car = await _context.CarModels.FindAsync(id);
            if (car != null)
            {
                _context.CarModels.Remove(car);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
          return (_context.CarModels?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
