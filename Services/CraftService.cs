using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Ostawy.Models;
using Ostawy.Repositories;
using Ostawy.ViewModels;

namespace Ostawy.Services
{
    public class CraftService : ICraftService
    {
        private readonly ICraftRepository _repo;
        private readonly IWebHostEnvironment _env;

        public CraftService(ICraftRepository repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }

        public async Task<(bool Success, string ErrorMessage)> CreateCraftAsync(CraftSubmissionViewModel model)
        {
            if (model == null)
            {
                return (false, "Submission model is null.");
            }

            if (model.CoverImage == null || model.CoverImage.Length == 0)
            {
                return (false, "Cover image is required.");
            }

            try
            {
                var wwwRoot = _env.WebRootPath;
                if (string.IsNullOrEmpty(wwwRoot))
                {
                    return (false, "Web root path is not configured.");
                }

                // Prepare directories
                var coversFolder = Path.Combine(wwwRoot, "uploads", "covers");
                var galleryFolder = Path.Combine(wwwRoot, "uploads", "gallery");
                Directory.CreateDirectory(coversFolder);
                Directory.CreateDirectory(galleryFolder);

                // Save cover image
                var coverExt = Path.GetExtension(model.CoverImage.FileName);
                var coverFileName = Guid.NewGuid().ToString("N") + coverExt;
                var coverPhysicalPath = Path.Combine(coversFolder, coverFileName);
                using (var stream = new FileStream(coverPhysicalPath, FileMode.Create))
                {
                    await model.CoverImage.CopyToAsync(stream);
                }
                var coverRelativePath = "/uploads/covers/" + coverFileName; // safe URL path

                var craft = new Craft
                {
                    Title = model.Title,
                    Category = model.CategoryId,
                    Description = model.Description,
                    CoverImagePath = coverRelativePath
                };

                // Save gallery images if present
                if (model.GalleryImages != null && model.GalleryImages.Any())
                {
                    foreach (var file in model.GalleryImages.Where(f => f != null && f.Length > 0))
                    {
                        var ext = Path.GetExtension(file.FileName);
                        var fileName = Guid.NewGuid().ToString("N") + ext;
                        var physicalPath = Path.Combine(galleryFolder, fileName);
                        using (var stream = new FileStream(physicalPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        var rel = "/uploads/gallery/" + fileName;
                        craft.GalleryImages.Add(new CraftImage { ImagePath = rel });
                    }
                }

                await _repo.AddCraftAsync(craft);
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                // In production consider logging the exception details and returning a generic message
                return (false, ex.Message);
            }
        }
    }
}
