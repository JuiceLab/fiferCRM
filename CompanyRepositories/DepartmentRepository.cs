using CompanyContext;
using CompanyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CompanyRepositories
{
    public class DepartmentRepository : CompanyRepository, IDisposable
    {
        public DepartmentRepository()
            : base()
        { }

        public List<PositionViewModel> GetPositions(Guid userId)
        {
            return GetPositions(ViewContext.UserInDepartmentView.FirstOrDefault(m => m.UserId == userId).C_CompanyId);
        }

        public List<PositionViewModel> GetPositions(int companyId)
        {
            return ViewContext.PositionInDepartmentView
                    .Where(m => m.C_CompanyId == companyId)
                    .Select(m => new PositionViewModel()
                    {
                        DepartmentId = m.C_DepartmentId,
                        IsActive = m.IsActive,
                        Name = m.Name,
                        NameGenetive = m.NameGenetive,
                        PosId = m.PositionId,
                        DepartmentName = m.Title,
                        HeaderPosId = m.C_ParentPosId
                    }).ToList();
        }

        public List<DepartmentViewModel> GetDepartments(Guid userId)
        {
            return GetDepartments(ViewContext.UserInDepartmentView
                .FirstOrDefault(m => m.UserId == userId)
                .C_CompanyId);
        }

        public List<DepartmentViewModel> GetDepartments(int companyId)
        {
            return Context.Departments
                .Where(m => m.C_CompanyId == companyId)
                .Select(m => new DepartmentViewModel()
                {
                    DepartmentId = m.DepartmentId,
                    IsActive = m.IsActive,
                    Title = m.Title,
                    Description = m.Description
                }).ToList();
        }

        public DepartmentViewModel GetDepartment(int? departmentId)
        {
            DepartmentViewModel model = new DepartmentViewModel();
            if (departmentId.HasValue)
            {
                var departemt = Context.Departments.FirstOrDefault(m => m.DepartmentId == departmentId.Value);
                model.DepartmentId = departemt.DepartmentId;
                model.Description = departemt.Description;
                model.Title = departemt.Title;
                model.IsActive = departemt.IsActive;
                model.Groups = string.IsNullOrEmpty(departemt.WorkGroups) ?
                    new List<int>()
                    : departemt.WorkGroups.Split(',').Select(m => Convert.ToInt32(m)).ToList();
            }
            return model;
        }

        public PositionViewModel GetPosition(int? positionId)
        {
            PositionViewModel model = new PositionViewModel();
            if (positionId.HasValue)
            {
                var position = Context.Positions.FirstOrDefault(m => m.PositionId == positionId.Value);
                model.DepartmentId = position.C_DepartmentId;
                model.PosId = position.PositionId;
                model.Name = position.Name;
                model.NameGenetive = position.NameGenetive;
                model.IsActive = position.IsActive;
                model.HeaderPosId = position.C_ParentPosId;
            }
            return model;
        }

        public WorkGroupViewModel GetGroup(int? groupId)
        {
            WorkGroupViewModel model = new WorkGroupViewModel();
            if (groupId.HasValue)
            {
                var group = Context.WorkGroups.FirstOrDefault(m => m.WorkGroupId == groupId.Value);
                model.Name = group.Name;
                model.Descritpion = group.Descritption;
                model.GroupId = group.WorkGroupId;
            }
            return model;
        }
        public void AddOrUpdateDepartment(DepartmentViewModel model, Guid userId)
        {
            
            if (model.DepartmentId != 0)
            {
                var department = Context.Departments.FirstOrDefault(m => m.DepartmentId == model.DepartmentId);
                department.Title = model.Title;
                department.Description = model.Description;
                department.IsActive = model.IsActive;
                department.WorkGroups = model.Groups!=null && model.Groups.Count > 0 ? string.Join(",", model.Groups) : string.Empty;
            }
            else
            {
                Department department = new Department()
                {
                    Title = model.Title,
                    Description = model.Description,
                    IsActive = model.IsActive,
                    CreatedBy = userId,
                    C_CompanyId = ViewContext.UserInDepartmentView
                                  .FirstOrDefault(m => m.UserId == userId)
                                  .C_CompanyId,
                    WorkGroups = model.Groups != null && model.Groups.Count > 0 ? string.Join(",", model.Groups) : string.Empty
                };
                Context.Departments.Add(department);
            }
            Context.SaveChanges();
        }

        public void AddOrUpdatePosition(PositionViewModel model, Guid guid)
        {
            if (model.PosId != 0)
            {
                var position = Context.Positions.FirstOrDefault(m => m.PositionId == model.PosId);
                position.Name = model.Name;
                position.NameGenetive = model.NameGenetive;
                position.IsActive = model.IsActive;
                position.C_DepartmentId = model.DepartmentId;
            }
            else
            {
                Position position = new Position()
                {
                    Name = model.Name,
                    NameGenetive = model.NameGenetive,
                    IsActive = model.IsActive,
                    C_DepartmentId = model.DepartmentId
                };
                Context.Positions.Add(position);
            }
            Context.SaveChanges();
        }

        public void AddOrUpdateGroup(WorkGroupViewModel model, Guid userId)
        {
            var companyId = ViewContext.UserInDepartmentView.FirstOrDefault(m => m.UserId == userId).C_CompanyId;
            if (model.GroupId == 0)
            {
                Context.WorkGroups.Add(new WorkGroup()
                {
                    Descritption = model.Descritpion,
                    Name = model.Name,
                    C_CompanyId = companyId
                });
            }
            else
            {
                var group = Context.WorkGroups.FirstOrDefault(m => m.WorkGroupId == model.GroupId);
                group.Name = model.Name;
                group.Descritption = model.Descritpion;
            }
            Context.SaveChanges();
        }

        public IEnumerable<SelectListItem> GetItemPositions(Guid userId, int? posId)
        {
            if (posId.HasValue)
            {
                var curPos = ViewContext.PositionInDepartmentView.FirstOrDefault(m => m.PositionId == posId);

                return ViewContext.PositionInDepartmentView
                         .Where(m => m.C_CompanyId == curPos.C_CompanyId && m.C_ParentPosId != posId && m.PositionId != posId)
                         .ToList()
                         .Select(m => new SelectListItem()
                         {
                             Text = string.Format("{0} в отделе {1}", m.Name, m.Title),
                             Value = m.PositionId.ToString(),
                             Selected = m.C_ParentPosId.HasValue && curPos.C_ParentPosId.HasValue && curPos.C_ParentPosId.Value == m.C_ParentPosId
                         }).ToList();
            }
            else
            {
                var companyId = ViewContext.UserInDepartmentView.FirstOrDefault(m => m.UserId == userId).C_CompanyId;
                return ViewContext.PositionInDepartmentView
                        .Where(m => m.C_CompanyId == companyId)
                        .ToList()
                        .Select(m => new SelectListItem()
                        {
                            Text = string.Format("{0} в отделе {1}", m.Name, m.Title),
                            Value = m.PositionId.ToString(),
                        }).ToList();
            }
        }

        public IEnumerable<SelectListItem> GetGroups(Guid userId)
        {
            var user = ViewContext.UserInDepartmentView
                .FirstOrDefault(m => m.UserId == userId);

            var model = new List<SelectListItem>();
            var departments = Context.Departments
                        .Where(m => m.C_CompanyId == m.C_CompanyId)
                        .Select(m => new { name = m.Title, ids = m.WorkGroups })
                        .ToList()
                        .Where(m => !string.IsNullOrEmpty(m.ids))
                        .Select(m => new { name = m.name, ids = m.ids.Split(',') });

            if (user.IsKeyEmployee)
            {
                var workGroups = Context.WorkGroups
                 .Where(m => m.C_CompanyId == user.C_CompanyId)
                 .ToList();
                foreach (var item in departments)
                {
                    model.AddRange(item.ids
                                     .Where(m=>workGroups.Any(n => m == n.WorkGroupId.ToString()))
                                     .Select(m => new SelectListItem()
                                     {
                                         Value = item.name + "_" + m.ToString(),
                                         Text = item.name + "||" + workGroups.FirstOrDefault(n => m == n.WorkGroupId.ToString()).Name
                                     }));
                }
            }
            else if (Context.Positions.FirstOrDefault(m => m.C_DepartmentId == user.DepartmentId && !m.C_ParentPosId.HasValue).Employees.Any(m => m.UserId == userId))
            {
                departments = Context.Departments
                       .Where(m => m.C_CompanyId == user.DepartmentId)
                       .Select(m => new { name = m.Title, ids = m.WorkGroups })
                       .ToList()
                       .Where(m => !string.IsNullOrEmpty(m.ids))
                       .Select(m => new { name = m.name, ids = m.ids.Split(',') });

                model = Context.WorkGroups
                    .Where(m => m.C_CompanyId == user.C_CompanyId)
                    .ToList()
                    .Select(m => new SelectListItem()
                    {
                        Value = m.WorkGroupId.ToString(),
                        Text = departments.FirstOrDefault(n => n.ids.Contains(m.WorkGroupId.ToString())) + "||" + m.Name
                    }).ToList();
            }
            else if (user.C_GroupHeaderId.HasValue)
            {
                Context.WorkGroups.Where(m => m.WorkGroupId == user.C_GroupHeaderId.Value)
                                 .ToList()
                                 .Select(m => new SelectListItem()
                                 {
                                     Value = m.WorkGroupId.ToString(),
                                     Text = departments.FirstOrDefault(n => n.ids.Contains(m.WorkGroupId.ToString())) + "||" + m.Name
                                 }).ToList();
            }
            return model;
        }

        public IEnumerable<WorkGroupViewModel> GetGroups(int companyId)
        {
            var departments = Context.Departments
                .Where(m => m.C_CompanyId == m.C_CompanyId)
                .Select(m => new { name = m.Title, ids = m.WorkGroups })
                .ToList()
                .Where(m => !string.IsNullOrEmpty(m.ids))
                .Select(m => new { name = m.name, ids = m.ids.Split(',') });

            var employees = ViewContext.UserInDepartmentView
                .Where(m => m.C_CompanyId == m.C_CompanyId)
                .Select(m => new { name = m.FirstName + " " + m.LastName, ids = m.WorkGroups })
                .ToList()
                .Where(m => !string.IsNullOrEmpty(m.ids))
                .Select(m => new { name = m.name, ids = m.ids.Split(',') });

            var model = Context.WorkGroups
                .Where(m => m.C_CompanyId == companyId)
                .Select(m => new WorkGroupViewModel()
                {
                    GroupId = m.WorkGroupId,
                    Name = m.Name,
                    Descritpion = m.Descritption
                })
                .ToList();

            foreach (var item in model)
            {
                item.Departments = string.Join(", ", departments.Where(m => m.ids.Any(n => n == item.GroupId.ToString())).Select(m => m.name));
                item.Employees = string.Join(", ", employees.Where(m => m.ids.Any(n => n == item.GroupId.ToString())).Select(m => m.name));
            }
            return model;
        }

        public IEnumerable<SelectListItem> GetGroups4Company(Guid userId, List<int> list)
        {
            var companyId = ViewContext.UserInDepartmentView.FirstOrDefault(m => m.UserId == userId).C_CompanyId;
            var model= Context.WorkGroups.Where(m => m.C_CompanyId == companyId)
                .ToList()
                .Select(m => new SelectListItem()
                {
                    Text = m.Name,
                    Value = m.WorkGroupId.ToString(),
                    Selected = list.Contains(m.WorkGroupId)
                })
                .ToList();
            model.Insert(0, new SelectListItem() { Text = "Не руководит группой", Value ="" });
            return model;
        }

    }
}
