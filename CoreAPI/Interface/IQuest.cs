using Entities.ViewModel;
using System.Collections.Generic;

namespace Interface
{
    public interface IQuest
    {
        int AddQuest(QuestViewModel questViewModel);
        QuestViewModel GetQuestByID(int QuestID);
        IEnumerable<QuestViewModel> GetQuestsByName(string QuestName);
        IEnumerable<QuestViewModel> GetQuestsByPoint(decimal Latitude, decimal Longtitude);
        IEnumerable<QuestViewModel> GetQuestsByUser(string UserEmail);
    }
}
