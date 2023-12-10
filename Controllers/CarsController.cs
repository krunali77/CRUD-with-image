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

        public CarsController(CarDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: Cars
        public async Task<IActionResult> Index(int? page)
        {
                const int pageSize = 6; // Adjust the page size as needed
                int pageNumber = page ?? 1;

                var cars = _context.CarModels.ToList(); // Replace this with your actual data retrieval logic

                var totalItems = cars.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var paginatedCars = cars
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = totalPages;

                return View(paginatedCars);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Class,ModelName,ModelCode,Description,Features,Price,ManufacturingDate,Active,SortOrder,CarImage")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
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
