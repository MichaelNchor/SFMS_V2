using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SFMS.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace SFMS.Controllers
{
    public class StagesController : Controller
    {
        private readonly SFMSContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        //private readonly UserManager<IdentityUser> _userManager;

        public StagesController(SFMSContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostEnvironment;
            //_userManager = userManager;

        }

        // GET: Stages
        public async Task<IActionResult> Index()
        {
            var stages = await _context.Stages.ToListAsync();
            foreach(var stage in stages)
            {
                stage.Url = Truncate(stage.Url, 20);
                stage.Description = Truncate(stage.Description, 25);
            }
              return _context.Stages != null ? 
                          View(stages) :
                          Problem("Entity set 'SFMSContext.Stages'  is null.");
        }

        // GET: Stages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Stages == null)
            {
                return NotFound();
            }

            var stage = await _context.Stages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stage == null)
            {
                return NotFound();
            }

            return View(stage);
        }

        // GET: Stages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Stages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Url,IconFile,IconPath,Description")] Stage stage)
        {
            if (ModelState.IsValid)
            {
                //var user = await GetUser();
                stage.IconPath = UploadFile(stage);
                stage.CreatedAt = DateTime.Now;
                //stage.CreatedBy = user.UserName;
                stage.Slug = stage.Name.ToLower().Replace(" ", "-");
                _context.Add(stage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(stage);
        }

        // GET: Stages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Stages == null)
            {
                return NotFound();
            }

            var stage = await _context.Stages.FindAsync(id);
            if (stage == null)
            {
                return NotFound();
            }
            return View(stage);
        }

        // POST: Stages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Url,IconFile,IconPath,Description")] Stage stage)
        {
            if (id != stage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    stage.IconPath = UploadFile(stage);
                    stage.UpdatedAt = DateTime.Now;
                    stage.Slug = stage.Name.ToLower().Replace(" ", "-"); ;
                    _context.Update(stage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StageExists(stage.Id))
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
            return View(stage);
        }

        // GET: Stages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Stages == null)
            {
                return NotFound();
            }

            var stage = await _context.Stages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stage == null)
            {
                return NotFound();
            }

            return View(stage);
        }

        // POST: Stages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Stages == null)
            {
                return Problem("Entity set 'SFMSContext.Stages'  is null.");
            }
            var stage = await _context.Stages.FindAsync(id);
            if (stage != null)
            {
                _context.Stages.Remove(stage);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StageExists(int id)
        {
          return (_context.Stages?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public string UploadFile(Stage stage)
        {
            string fileName = null;
            if(stage.IconFile != null)
            {
                string uploadDir = Path.Combine(_hostingEnvironment.WebRootPath, "img/stages");
                fileName = Guid.NewGuid().ToString() + "-" + stage.IconFile.FileName;
                string filePath = Path.Combine(uploadDir, fileName);

                using(var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    stage.IconFile.CopyTo(fileStream);
                }
            }

            return fileName;
        }

        public string Truncate(string value, int maxLength, string truncationSuffix = "…")
        {
            return value.Length > maxLength
                ? value.Substring(0, maxLength) + truncationSuffix
                : value;
        }
    }
}
