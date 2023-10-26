using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Memory;
using TestApp.Data;
using TestApp.Models;
using TestApp.Utility;

namespace TestApp.Controllers
{
    public class PostingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        [ActivatorUtilitiesConstructor]
        public PostingsController(ApplicationDbContext context, UserManager<IdentityUser> usermanager, SignInManager<IdentityUser> signinmanager)
        {
            _context = context;
            userManager = usermanager;
            signInManager = signinmanager;
        }

        // GET: Postings
        public async Task<IActionResult> Index()
        {
            if (_context.Posting != null)
            {
                //Get all posts
                List<Posting> recentposts = await _context.Posting.OrderByDescending(Posting => Posting.Postdt).Take(10).ToListAsync();
                //Return only recent posts
                return View(recentposts);

            }
            else
            {
                return Problem("Entity set 'ApplicationDbContext.Posting'  is null.");
            }
        }
        public async Task<IActionResult> MyPosts()
        {
            if (_context.Posting != null)
            {
                IdentityUser user = userManager.GetUserAsync(User).Result;
                List<Posting> myposts = await _context.Posting.Where(Posting => Posting.AuthorId.Contains(user.Id)).OrderByDescending(Posting => Posting.Postdt).ToListAsync();
                if(myposts.Any())
                {
                    return View(myposts);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return Problem("Entity set 'ApplicationDbContext.Posting'  is null.");
            }
        }
        // GET: Postings/Browse
        public async Task<IActionResult> Browse(string searchInput)
        {
            if (_context.Posting != null)
            {
                List<Posting> all;
                if (searchInput == null)
                {
                    all = await _context.Posting.OrderByDescending(Posting => Posting.Postdt).ToListAsync();
                }
                else
                {
                    //Where(Posting => Posting.Name.Contains(searchInput))
                    all = await _context.Posting.Where(Posting => Posting.Name.Contains(searchInput)).OrderByDescending(Posting => Posting.Postdt).ToListAsync();
                }
                

                return View(all);

            }
            else
            {
                return Problem("Entity set 'ApplicationDbContext.Posting'  is null.");
            }
        }
            // GET: Postings/Details/5
            public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posting == null)
            {
                return NotFound();
            }

            var posting = await _context.Posting
                .FirstOrDefaultAsync(m => m.Id == id);
            if (posting == null)
            {
                return NotFound();
            }

            return View(posting);
        }

        // GET: Postings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Postings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ImageData,ImageThumbnail,Author,AuthorId,Price,Postdt")] Posting posting, IFormFile imageFile)
        {
            //Deploy Image utility helper to generate image byte data
            ImageUtility imHelper = new ImageUtility();
            await imHelper.loadImage(imageFile);
            //Get Author ID from userManager
            IdentityUser user = userManager.GetUserAsync(User).Result;
            //Generate new posting with added non-form-data
            Posting newposting = new Posting();
            newposting.Id = posting.Id;
            newposting.Name = posting.Name;
            newposting.Description = posting.Description;
            newposting.ImageData = imHelper.getImage();
            newposting.ImageThumbnail = imHelper.getThumbnail();
            newposting.Author = posting.Author;
            newposting.AuthorId = user.Id;
            newposting.Price = posting.Price;
            newposting.Postdt = DateTime.Now;
                _context.Add(newposting);

            await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
        }

        // GET: Postings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posting == null)
            {
                return NotFound();
            }

            var posting = await _context.Posting.FindAsync(id);
            
            if (posting == null)
            {
                return NotFound();
            }
            
            EditModel editModel = new EditModel();
            editModel.Post = posting;

            editModel.ImageString = Convert.ToBase64String(posting.ImageData);
            editModel.ThumbnailString = Convert.ToBase64String(posting.ImageThumbnail);
            return View(editModel);
        }

        // POST: Postings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Post,ImageString,ThumbnailString")] EditModel model,IFormFile imageFile)
        {
            if (id != model.Post.Id)
            {
                return NotFound();
            }
            //Get Author ID from userManager
            IdentityUser user = userManager.GetUserAsync(User).Result;

            
            try
                {
                //Deploy Image utility helper to generate image byte data
                Posting newposting = new Posting();

                if (imageFile != null && imageFile.Length > 0)
                {
                    ImageUtility imHelper = new ImageUtility();
                    await imHelper.loadImage(imageFile);
                    newposting.ImageData = imHelper.getImage();
                    newposting.ImageThumbnail = imHelper.getThumbnail();
                }
                else
                {
                    newposting.ImageData = Convert.FromBase64String(model.ImageString);
                    newposting.ImageThumbnail = Convert.FromBase64String(model.ThumbnailString);
                }

                //var original_values = typeof(Posting).GetMethods(BindingFlags.Public)
                //                .Select(x => x.Invoke(original_posting, null));






                newposting.Id = model.Post.Id;
                newposting.Name = model.Post.Name;
                newposting.Description = model.Post.Description;
                newposting.Author = model.Post.Author;
                newposting.AuthorId = user.Id;
                newposting.Price = model.Post.Price;
                newposting.Postdt = model.Post.Postdt;

                

                _context.Update(newposting);
                await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostingExists(model.Post.Id))
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

        // GET: Postings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posting == null)
            {
                return NotFound();
            }

            var posting = await _context.Posting
                .FirstOrDefaultAsync(m => m.Id == id);
            if (posting == null)
            {
                return NotFound();
            }

            return View(posting);
        }

        // POST: Postings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posting == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Posting'  is null.");
            }
            var posting = await _context.Posting.FindAsync(id);
            if (posting != null)
            {
                _context.Posting.Remove(posting);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostingExists(int id)
        {
          return (_context.Posting?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
