using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAssignment.Models;
using X.PagedList;

namespace ProductAssignment.Controllers
{
    public class ProductController : Controller
    {

        
        private readonly ProductDbContext _context;

        public ProductController(ProductDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            try
            {
                var totalProducts = await _context.CentralArea.CountAsync();
                
                var productList = await _context.CentralArea
                    .OrderBy(p => p.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                
                var pagedList = new StaticPagedList<ProductModel>(productList, page, pageSize, totalProducts);

                return View(pagedList);
            }
            catch (Exception e)
            {
                TempData["errorMessage"] = "Error loading product list: " + e.Message;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel addProductModel, IFormFile? ImageFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
                    TempData["errorMessage"] = "Invalid input data: " + errors;
                    return View(addProductModel);
                }

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }
                    
                    addProductModel.ImageFilename = uniqueFileName;
                }

                await _context.CentralArea.AddAsync(addProductModel);
                await _context.SaveChangesAsync();

                TempData["successMessage"] = $"New Product '{addProductModel.Name}' added successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                TempData["errorMessage"] = "Error adding product: " + e.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var model = await _context.CentralArea.SingleOrDefaultAsync(p => p.Id == id);
                if (model == null)
                {
                    TempData["errorMessage"] = "Product not found!";
                    return RedirectToAction(nameof(Index));
                }
                return View(model);
            }
            catch (Exception e)
            {
                TempData["errorMessage"] = "Error loading product for editing: " + e.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductModel editProductModel, IFormFile? ImageFile)
        {
            try
            {
                var model = await _context.CentralArea.SingleOrDefaultAsync(p => p.Id == editProductModel.Id);
                if (model == null)
                {
                    TempData["errorMessage"] = "Product not found!";
                    return RedirectToAction(nameof(Index));
                }

                model.Name = editProductModel.Name;
                model.Description = editProductModel.Description;
                model.BuyingPrice = editProductModel.BuyingPrice;
                model.Supplier = editProductModel.Supplier;
                model.ManufacturingDate = editProductModel.ManufacturingDate;
                model.PurchasingDate = editProductModel.PurchasingDate;
                
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    if (!string.IsNullOrEmpty(model.ImageFilename))
                    {
                        string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", model.ImageFilename);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    model.ImageFilename = uniqueFileName;
                }

                await _context.SaveChangesAsync();
                TempData["successMessage"] = $"Product '{model.Name}' updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                TempData["errorMessage"] = "Error updating product: " + e.Message;
                return View(editProductModel);
            }
        }

        // DELETE GET method for displaying the confirmation page
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _context.CentralArea.SingleOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    TempData["errorMessage"] = "Product not found!";
                    return RedirectToAction(nameof(Index));
                }
                return View(product); // Show confirmation view
            }
            catch (Exception e)
            {
                TempData["errorMessage"] = "Error loading product for deletion: " + e.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // DELETE POST method for performing the actual deletion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var model = await _context.CentralArea.SingleOrDefaultAsync(p => p.Id == id);
                if (model == null)
                {
                    TempData["errorMessage"] = "Product not found!";
                    return RedirectToAction(nameof(Index));
                }

                // Delete Image File (Optional)
                if (!string.IsNullOrEmpty(model.ImageFilename))
                {
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", model.ImageFilename);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.CentralArea.Remove(model);
                await _context.SaveChangesAsync();
                TempData["successMessage"] = $"Product '{model.Name}' deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                TempData["errorMessage"] = "Error deleting product: " + e.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}


