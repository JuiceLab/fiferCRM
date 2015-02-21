using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityRepository;
using ConstructorRepositories;
using ConstructorLandingContext; 

namespace ConstructorWizards
{
    public class LandingWizard
    {
        private LandingRepository _repository;

        public LandingWizard(string connStr)
        {
            _repository = new LandingRepository(connStr);
 
        }

        public bool CheckAllias(string allias)
        {
            return _repository.IsExistAllias(allias);
        }

        public int InitDefaultLandingPage(int clientId, string allias)
        {
            var model = new LandingPage()
                {
                    C_ProjectClientId = clientId,
                    C_TemplateId = 1,
                    Allias = allias
                };

            using (LandingEntities context = new LandingEntities())
            {
                context.InsertUnit(model);
            }

            return model.LandingPageId;
        }
    }
}
