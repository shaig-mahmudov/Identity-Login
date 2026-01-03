using Fruitables_MVC.Areas.Admin.ViewModels.Category;
using Fruitables_MVC.Data;
using Fruitables_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Fruitables_MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<CategoryVM> categories = await _context.Categories
                .OrderByDescending(m => m.Id)
                .Where(m => !m.isDeleted)
                .Select(m =>new CategoryVM
                {
                    Id = m.Id,
                    Name = m.Name
                })
                .ToListAsync();

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryVM);
            }

            bool isExist = await _context.Categories.AnyAsync(m => m.Name == categoryVM.Name && !m.isDeleted);

            if (isExist)
            {
                ModelState.AddModelError("Name", "Bu adda kateqoriya artıq mövcuddur");
                return View(categoryVM);
            }

            Category newCategory = new()
            {
                Name = categoryVM.Name,
            };
            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            var category = await _context.Categories
                .Where(m => !m.isDeleted)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null) return NotFound();

            CategoryVM categoryVM = new()
            {
                Id = category.Id,
                Name = category.Name
            };

            return View(categoryVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            category.isDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();

            var category = await _context.Categories
                .Where(m => !m.isDeleted)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null) return NotFound();


            CategoryVM categoryVM = new()
            {
                Id = category.Id,
                Name = category.Name
            };

            return View(categoryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryVM categoryVM)
        {
            if (id != categoryVM.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(categoryVM);
            }

            bool isExist = await _context.Categories
                .AnyAsync(m => m.Name == categoryVM.Name && m.Id != id && !m.isDeleted);

            if (isExist)
            {
                ModelState.AddModelError("Name", "Bu adda kateqoriya artıq mövcuddur");
                return View(categoryVM);
            }

            var dbCategory = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);

            if (dbCategory == null) return NotFound();

            dbCategory.Name = categoryVM.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
