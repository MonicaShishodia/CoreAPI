using Entities.Model;
using Entities.ViewModel;
using System.Collections.Generic;
namespace Interface
{
    public interface IImage
    {
        Image GetImageByName(string ImageName);
        bool AddImage(Image img);
        Image CreateImageObject(ImageViewModel imageViewModel);
        IEnumerable<ImageViewModel> GetImagesDetailsByQuestID(int QuestID);
        byte[] GetImagesByQuestIDAndDirection(int QuestID, DirectionEnum Direction);
        byte[] GetImagesByQuestIDDirectionSize(int QuestID, DirectionEnum Direction, (int, int) size);
        Image ChangeSharpness(Image img) => new Image { ID = 0 };
    }
}
