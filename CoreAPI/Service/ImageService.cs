using DataAccess;
using Entities.Model;
using Entities.ViewModel;
using Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Drawing = System.Drawing;

namespace Service
{
    public class ImageService : IImage
    {
        private readonly CoreApidbContext _dbContext;
        public ImageService(CoreApidbContext dbContext) => _dbContext = dbContext;
        public bool AddImage(Image img)
        {
            _dbContext.Image.Add(img);
            return _dbContext.SaveChanges() > 0;
        }
        public Image CreateImageObject(ImageViewModel imageViewModel)
        {
            Image img = new Image
            {
                Name = imageViewModel.Name,
                Path = imageViewModel.Path,
                SizeInBytes = imageViewModel.SizeInBytes,
                DirectionID = _dbContext.Direction
                            .FirstOrDefault(x => x.Name.ToLower() == Enum.GetName(typeof(DirectionEnum), imageViewModel.Direction)) == null ? 0 :
                            _dbContext.Direction.FirstOrDefault(x => x.Name.ToLower() == Enum.GetName(typeof(DirectionEnum), imageViewModel.Direction)).ID
            };
            return img.DirectionID == 0 ? null : img;
        }
        public IEnumerable<ImageViewModel> GetImagesDetailsByQuestID(int QuestID)
        {
            List<ImageViewModel> imageViewModel = new List<ImageViewModel>();
            var images = _dbContext.Image.AsEnumerable().Where(x => x.QuestID == QuestID);
            if (images != null)
            {
                foreach (Image img in images)
                {
                    imageViewModel.Add(new ImageViewModel
                    {
                        Name = img.Name,
                        QuestID = img.QuestID,
                        SizeInBytes = img.SizeInBytes,
                        Path = img.Path,
                        ImageFile = File.Exists(img.Path) ? File.ReadAllBytes(img.Path) : null
                    });
                }
                return imageViewModel;
            }
            else return Enumerable.Empty<ImageViewModel>();
        }
        public byte[] GetImagesByQuestIDAndDirection(int QuestID, DirectionEnum Direction)
        {
            Direction d = _dbContext.Direction.FirstOrDefault(x => x.Name.ToLower() == Enum.GetName(typeof(DirectionEnum), Direction));
            if (d != null)
            {
                var img = _dbContext.Image.AsEnumerable().FirstOrDefault(x => x.QuestID == QuestID && x.DirectionID == d.ID);
                return File.Exists(img?.Path) ? File.ReadAllBytes(img?.Path) : null;
            }
            else return new byte[0];
        }
        public byte[] GetImagesByQuestIDDirectionSize(int QuestID, DirectionEnum Direction, (int, int) size)
        {
            Direction d = _dbContext.Direction.FirstOrDefault(x => x.Name.ToLower() == Enum.GetName(typeof(DirectionEnum), Direction));
            if (d != null)
            {
                var img = _dbContext.Image.AsEnumerable().FirstOrDefault(x => x.QuestID == QuestID && x.DirectionID == d.ID);
                return File.Exists(img?.Path) ? GetResizedImage(img?.Path, new Drawing.Size(size.Item1, size.Item2)).ToArray() : null;
            }
            else return new byte[0];
        }
        public Image GetImageByName(string ImageName)
        {
            return _dbContext.Image.Find(ImageName);
        }
        private MemoryStream GetResizedImage(string imagePath, Drawing.Size imageSize)
        {
            FileStream fs = new System.IO.FileStream(imagePath, FileMode.Open);
            using (var resized = new Drawing.Bitmap(System.Drawing.Image.FromStream(fs), imageSize))
            {
                using (var memoryStream = new MemoryStream())
                {
                    resized.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    fs.Close();
                    return memoryStream;
                }
            }
        }
    }
}
