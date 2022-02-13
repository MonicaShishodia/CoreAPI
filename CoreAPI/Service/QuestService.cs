using DataAccess;
using Entities.Model;
using Entities.ViewModel;
using Interface;
using Service.HelperMethods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class QuestService : IQuest
    {
        private readonly CoreApidbContext _dbContext;
        private readonly IImage _iImage;
        public QuestService(CoreApidbContext dbContext, IImage iImage)
        {
            _dbContext = dbContext;
            _iImage = iImage;
        }
        public int AddQuest(QuestViewModel questViewModel)
        {
            try
            {
                Entities.Model.Quest objquest = CreateQuestObject(questViewModel);
                if (objquest != null)
                {
                    foreach (ImageViewModel imageViewModel in questViewModel.Images)
                    {
                        Image img = _iImage.CreateImageObject(imageViewModel);
                        if (img != null)
                            objquest.Images.Add(img);
                    }
                    if (objquest.Images.Count > 0)
                    {
                        using var tx = _dbContext.Database.BeginTransaction();
                        _dbContext.Quest.Add(objquest);
                        _dbContext.Image.AddRange(objquest.Images);
                        _dbContext.SaveChanges();
                        tx.Commit();
                        return objquest.ID;
                    }
                    else return 0;
                }
                else return 0;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public QuestViewModel GetQuestByID(int QuestID)
        {
            Quest objQuest = _dbContext.Quest.FirstOrDefault(x => x.ID == QuestID);
            if (objQuest != null)
            {
                return ReadQuestObject(objQuest);
            }
            else return null;
        }
        public IEnumerable<QuestViewModel> GetQuestsByPoint(decimal Latitude, decimal Longtitude)
        {
            List<QuestViewModel> objQuests = new List<QuestViewModel>();
            GeoPoint gp = _dbContext.GeoPoint.FirstOrDefault(x => x.Latitude == Latitude && x.Longitude == Longtitude);
            if (gp != null)
            {
                var quests = _dbContext.Quest.AsEnumerable().Where(x => x.GeoPointID == gp.ID).ToList();
                foreach (Quest q in quests)
                    objQuests.Add(ReadQuestObject(q));
                return objQuests;
            }
            else return Enumerable.Empty<QuestViewModel>();
        }
        public IEnumerable<QuestViewModel> GetQuestsByName(string QuestName)
        {
            List<QuestViewModel> objQuests = new List<QuestViewModel>();
            var quests = _dbContext.Quest.AsEnumerable().Where(x => x.Name.ToLower() == QuestName.ToLower()).ToList();
            if (quests != null)
            {
                foreach (Quest q in quests)
                    objQuests.Add(ReadQuestObject(q));
                return objQuests;
            }
            else return Enumerable.Empty<QuestViewModel>();
        }
        public IEnumerable<QuestViewModel> GetQuestsByUser(string UserEmail)
        {
            List<QuestViewModel> objQuests = new List<QuestViewModel>();
            var quests = _dbContext.Quest.AsEnumerable().Where(x => x.UploadedBy == UserEmail).ToList();
            if (quests != null)
            {
                foreach (Quest q in quests)
                    objQuests.Add(ReadQuestObject(q));
                return objQuests;
            }
            else return Enumerable.Empty<QuestViewModel>();
        }
        private QuestViewModel ReadQuestObject(Quest quest) => new QuestViewModel
        {
            Name = quest.Name,
            UploadedBy = quest.UploadedBy,
            UploadedDateUTC = quest.UploadedDateUTC,
            LandCoverDistance = HelperMethods.EnumHelper<LandCoverDistanceEnum>.GetValueFromName(_dbContext.LandCoverDistance.First(x => x.ID == quest.LandCoverDistanceID).Value + " " + _dbContext.LandCoverDistance.First(x => x.ID == quest.LandCoverDistanceID).UnitOfMeasure),
            LandKind = (LandKindEnum)Enum.Parse(typeof(LandKindEnum), _dbContext.LandKind.First(x => x.ID == quest.LandKindID).Name),
            LandUse = (LandUseEnum)Enum.Parse(typeof(LandUseEnum), _dbContext.LandUse.First(x => x.ID == quest.LandUseID).Name),
            Latitude = _dbContext.GeoPoint.First(x => x.ID == quest.GeoPointID).Latitude,
            Longtitude = _dbContext.GeoPoint.First(x => x.ID == quest.GeoPointID).Longitude,
            Images = _iImage.GetImagesDetailsByQuestID(quest.ID)
        };
        private Quest CreateQuestObject(QuestViewModel quest)
        {
            GeoPoint gp = _dbContext.GeoPoint.FirstOrDefault(x => x.Latitude == quest.Latitude && x.Longitude == quest.Longtitude);
            if (gp == null)
            {
                _dbContext.GeoPoint.Add(new GeoPoint { Latitude = quest.Latitude, Longitude = quest.Longtitude });
                _dbContext.SaveChanges();
            }
            string[] LandCoverDistanceParameters = quest.LandCoverDistance.GetAttributeOfType<System.ComponentModel.DataAnnotations.DisplayAttribute>().Name.Split(' ');
            string LandCoverDistanceVal, LandCoverDistanceUOM;
            if (LandCoverDistanceParameters.Length == 2)
            {
                LandCoverDistanceVal = LandCoverDistanceParameters[0];
                LandCoverDistanceUOM = LandCoverDistanceParameters[1];
                Quest q = new Quest()
                {
                    GeoPointID = _dbContext.GeoPoint.First(x => x.Latitude == quest.Latitude && x.Longitude == quest.Longtitude).ID,
                    LandKindID = _dbContext.LandKind.FirstOrDefault(x => x.Name.ToLower() == Enum.GetName(typeof(LandKindEnum), quest.LandKind)) == null ? 0 : _dbContext.LandKind.FirstOrDefault(x => x.Name.ToLower() == Enum.GetName(typeof(LandKindEnum), quest.LandKind)).ID,
                    LandUseID = _dbContext.LandUse.FirstOrDefault(x => x.Name.ToLower() == Enum.GetName(typeof(LandUseEnum), quest.LandUse)) == null ? 0 : _dbContext.LandUse.FirstOrDefault(x => x.Name.ToLower() == Enum.GetName(typeof(LandUseEnum), quest.LandUse)).ID,
                    LandCoverDistanceID = _dbContext.LandCoverDistance.FirstOrDefault(x => x.Value == LandCoverDistanceVal && x.UnitOfMeasure.ToLower() == LandCoverDistanceUOM.ToLower()) == null ? 0 : _dbContext.LandCoverDistance.FirstOrDefault(x => x.Value == LandCoverDistanceVal && x.UnitOfMeasure.ToLower() == LandCoverDistanceUOM.ToLower()).ID,
                    Name = quest.Name,
                    UploadedDateUTC = DateTime.UtcNow,
                    UploadedBy = quest.UploadedBy,
                    Images = new List<Image>()
                };
                return (q.LandCoverDistanceID != 0 && q.LandKindID != 0 && q.LandUseID != 0) ? q : null;
            }
            else return null;
        }
    }
}
