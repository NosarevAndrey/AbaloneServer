using AbaloneServer.Models;

namespace AbaloneServer.Services
{
    public class GalleryService
    {
        private readonly Dictionary<int, (byte[] ImageData, string ContentType)> _imageStore = new();
        private static readonly List<GalleryItemViewModel> _items = new();
        private int _imageNextId = 0;

        public int ID() => ++_imageNextId;
        public GalleryService()
        {
            //InitializeSampleData();
        }
        private void InitializeSampleData()
        {
            if (_items.Count != 0) return;

            for (int i = 1; i <= 50; i++)
            {
                int id = ID();
                _imageStore[id] = (Array.Empty<byte>(), "image/png");
                _items.Add(new GalleryItemViewModel(
                    id,
                    $"Submitter {id}",
                    $"Description for Shell {id}.",
                    new AbaloneSubmissionViewModel
                    {
                        Sex = (AbaloneSex)(id % 3),  // Randomly assign sex (M/F/I)
                        Length = 10 + id % 5,
                        Diameter = 5 + id % 3,
                        Height = 7 + id % 4,
                        WholeWeight = 100 + id * 2,
                        ShuckedWeight = 50 + id,
                        VisceraWeight = 30 + id,
                        ShellWeight = 40 + id,
                        EstimatedAge = 5 + (id % 10)
                    }
                ));
            }
        }
        public void AddToGallery(AbaloneSubmissionViewModel abaloneData, IFormFile image, string description, string submitter)
        {
            if (image == null || image.Length == 0) return;

            using (var memoryStream = new MemoryStream())
            {
                image.CopyTo(memoryStream);
                byte[] imageData = memoryStream.ToArray();

                int imageId = ID();
                Console.WriteLine($"adderID: {imageId}");
                _imageStore[imageId] = (imageData, image.ContentType);

                var galleryItem = new GalleryItemViewModel(
                    imageId,
                    submitter,
                    description,
                    abaloneData
                );

                _items.Add(galleryItem);
            }
        }

        public List<GalleryItemViewModel> GetGalleryItems()
        {
            return _items;
        }

        public (byte[] ImageData, string ContentType)? GetImage(int imageId)
        {
            if (!_imageStore.ContainsKey(imageId)) return null;
            if (_imageStore[imageId].ImageData.Length == 0) return null;
            return _imageStore[imageId];
        }
    }
}
