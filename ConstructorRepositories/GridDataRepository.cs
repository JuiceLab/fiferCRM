using LandingModel.GridModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorRepositories
{
    public class GridDataRepository : BaseConstructorRepository
    {
        public GridDataRepository(string connStr)
            :base(connStr)
        { 
        }

        public IList<GridPageModel> GetPagesTable(string baseDomain)
        {
            IList<GridPageModel> result = Context.LandingPageViews.Select(m => new GridPageModel()
            {
                Client = m.FirstName + " " + m.LastName,
                Phone = m.Phone,
                Created = m.Created,
                LandingName = m.TemplateName,
                PageId = m.LandingPageId,
                LandingUrl = baseDomain + m.Allias
            }).ToList();
            return result;
        }
    }
}
