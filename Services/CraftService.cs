using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Ostawy.Models;
using Ostawy.Repositories;
using Ostawy.ViewModels;

namespace Ostawy.Services
{
    public class CraftService : ICraftService
    {
        private readonly ICraftRepository _repo;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<CraftService>? _logger;

        public CraftService(ICraftRepository repo, IWebHostEnvironment env, ILogger<CraftService>? logger = null)
        {
            _repo = repo;
            _env = env;
            _logger = logger;
        }

        public async Task<bool> CreateCraftAsync(CraftSubmissionViewModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrWhiteSpace(model.Title) ||
                string.IsNullOrWhiteSpace(model.CategoryId) ||
                string.IsNullOrWhiteSpace(model.Description) ||
                model.CoverImage == null)
            {
                return false;
            }

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var coversPath = Path.Combine(webRoot, "uploads", "covers");
            var galleryPath = Path.Combine(webRoot, "uploads", "gallery");

            Directory.CreateDirectory(coversPath);
            Directory.CreateDirectory(galleryPath);

            var coverExt = Path.GetExtension(model.CoverImage.FileName);
            var coverFileName = $"{Guid.NewGuid()}{coverExt}";
            var coverPhysicalPath = Path.Combine(coversPath, coverFileName);

            await using (var stream = new FileStream(coverPhysicalPath, FileMode.Create))
            {
                await model.CoverImage.CopyToAsync(stream);
            }

            var coverRelativeUrl = $"/uploads/covers/{coverFileName}";

            var galleryEntities = new List<CraftImage>();
            if (model.GalleryImages != null && model.GalleryImages.Any())
            {
                foreach (var file in model.GalleryImages)
                {
                    if (file == null || file.Length == 0) continue;

                    var ext = Path.GetExtension(file.FileName);
                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var physicalPath = Path.Combine(galleryPath, fileName);

                    try
                    {
                        await using var gstream = new FileStream(physicalPath, FileMode.Create);
                        await file.CopyToAsync(gstream);
                        var rel = $"/uploads/gallery/{fileName}";
                        galleryEntities.Add(new CraftImage { ImagePath = rel });
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Failed to write gallery image {FileName}", file.FileName);
                    }
                }
            }

            var craft = new Craft
            {
                Title = model.Title,
                Category = model.CategoryId,
                Description = model.Description,
                CoverImagePath = coverRelativeUrl,
                GalleryImages = galleryEntities
            };

            await _repo.AddAsync(craft);
            return true;
        }

        public async Task<List<CraftIndexViewModel>> GetAllCraftsAsync()
        {
            var entities = await _repo.GetAllAsync();
            var list = entities.Select(e => new CraftIndexViewModel
            {
                Id = e.Id,
                Title = e.Title,
                Category = e.Category,
                Description = e.Description,
                CoverImagePath = e.CoverImagePath,
                GalleryImagePaths = e.GalleryImages?.Select(g => g.ImagePath).ToList() ?? new List<string>()
            }).ToList();

            return list;
        }
    }
}
