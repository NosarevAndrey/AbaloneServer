using AbaloneServer.Models;
using AbaloneServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbaloneServer.Controllers
{
    public class UploadController : Controller
    {
        private readonly GalleryService _galleryService;
        private readonly AgeEstimatorService _ageEstimatorService;

        public UploadController(GalleryService galleryService, AgeEstimatorService ageEstimatorService)
        {
            _galleryService = galleryService;
            _ageEstimatorService = ageEstimatorService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Submit(AbaloneSubmissionViewModel model)
        {
            if (ModelState.IsValid)
            {

                model.EstimatedAge = (int)Math.Round(_ageEstimatorService.EstimateAge(model) + 1.5f); // age = rings + 1.5

                return View("Results", model);
            }

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult SubmitToGallery(AbaloneSubmissionViewModel abaloneData, string submitter, IFormFile image, string description)
        {
            if (image != null && image.Length > 0)
            {
                _galleryService.AddToGallery(abaloneData, image, description, submitter);
                return RedirectToAction("Index", "Gallery");
            }
            return BadRequest("No image uploaded or invalid image.");
        }
    }
}
