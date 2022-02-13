using DataAccess;
using Entities.Model;
using Interface;

namespace Service
{
    public class GeoPointService : IGeoPoint
    {
        private readonly CoreApidbContext _dbContext;
        public GeoPointService(CoreApidbContext dbContext) => _dbContext = dbContext;
        public bool AddGeoPoint(GeoPoint geoPoint)
        {
            _dbContext.GeoPoint.Add(geoPoint);
            _dbContext.SaveChanges();
            return true;
        }
    }
}
