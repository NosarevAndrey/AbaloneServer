using AbaloneServer.Models;
using AbaloneServer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;

namespace AbaloneServer.Controllers
{
    public class GalleryController : Controller
    {
        private readonly GalleryService _galleryService;
        public GalleryController(GalleryService galleryService)
        {
            _galleryService = galleryService;
        }

        public IActionResult Index()
        {
            var items = _galleryService.GetGalleryItems();
            return View(items);
        }
        public IActionResult GetImage(int id)
        {
            var image = _galleryService.GetImage(id); // Retrieving image by ID from the service
            if (image != null)
            {
                return File(image.Value.ImageData, image.Value.ContentType);
            }
            else
            {
                string defaultImageUrl = "/images/abalone-default.png";
                return Redirect(defaultImageUrl); 
            }
        }
    }
}
