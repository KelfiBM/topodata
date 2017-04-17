using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Topodata2.Models.Entities;

namespace Topodata2.Models.Service
{
    public class HomeSlideDataService
    {
        private readonly TopodataContext _db = new TopodataContext();
        public HomeSlideData GetLast()
        {
            var result = _db.HomeSlideDatas.OrderByDescending(p => p.regDate).FirstOrDefault();
            return result;
        }
    }
}