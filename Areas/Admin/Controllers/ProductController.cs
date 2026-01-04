using Fruitables_MVC.Data;
using Fruitables_MVC.Models;
using Fruitables_MVC.Areas.Admin.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fruitables_MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {

            IEnumerable<Product> products = await _context.Products
                .Include(m => m.Images) 
                .Include(m => m.Category)
                .Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.Id) 
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _context.Categories.Where(c => !c.IsDeleted).ToListAsync(), "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _context.Categories.Where(c => !c.IsDeleted).ToListAsync(), "Id", "Name");
                return View(vm);
            }
            Product newProduct = new Product
            {
                Name = vm.Name,
                Description = vm.Description,
                Price = vm.Price,
                CategoryId = vm.CategoryId,
                Quantity = 10, 
                Images = new List<ProductImages>()
            };

            if (vm.MainImage != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.MainImage.FileName);
                string path = Path.Combine(_env.WebRootPath, "uploads/products", fileName);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await vm.MainImage.CopyToAsync(stream);
                }

                newProduct.Images.Add(new ProductImages
                {
                    ImagePath = fileName,
                    isMain = true,
                    Product = newProduct
                });
            }

            if (vm.AdditionalImages != null)
            {
                foreach (var file in vm.AdditionalImages)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string path = Path.Combine(_env.WebRootPath, "uploads/products", fileName);

                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    newProduct.Images.Add(new ProductImages
                    {
                        ImagePath = fileName,
                        isMain = false,
                        Product = newProduct
                    });
                }
            }

            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (product == null) return NotFound();

            UpdateProductVM vm = new UpdateProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ExistingImages = product.Images 
            };

            ViewBag.Categories = new SelectList(await _context.Categories.Where(c => !c.IsDeleted).ToListAsync(), "Id", "Name");
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, UpdateProductVM vm)
        {
            if (id != vm.Id) return BadRequest();

            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (product == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _context.Categories.Where(c => !c.IsDeleted).ToListAsync(), "Id", "Name");
                vm.ExistingImages = product.Images; 
                return View(vm);
            }
            product.Name = vm.Name;
            product.Description = vm.Description;
            product.Price = vm.Price;
            product.CategoryId = vm.CategoryId;

            if (vm.MainImage != null)
            {
                var oldMain = product.Images.FirstOrDefault(p => p.isMain);
                if (oldMain != null)
                {
                    string oldPath = Path.Combine(_env.WebRootPath, "uploads/products", oldMain.ImagePath);
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);

                    _context.ProductImages.Remove(oldMain); 
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.MainImage.FileName);
                string path = Path.Combine(_env.WebRootPath, "uploads/products", fileName);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await vm.MainImage.CopyToAsync(stream);
                }

                product.Images.Add(new ProductImages
                {
                    ImagePath = fileName,
                    isMain = true,
                    ProductId = product.Id
                });
            }
            if (vm.AdditionalImages != null)
            {
                foreach (var file in vm.AdditionalImages)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string path = Path.Combine(_env.WebRootPath, "uploads/products", fileName);
                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    product.Images.Add(new ProductImages
                    {
                        ImagePath = fileName,
                        isMain = false,
                        ProductId = product.Id
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost] 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.IsDeleted = true;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}