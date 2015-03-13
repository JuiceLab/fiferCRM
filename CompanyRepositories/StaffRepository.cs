using AccessModel;
using AccessRepositories;
using CompanyContext;
using CompanyModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CompanyRepositories
{
    public class StaffRepository : DepartmentRepository, IDisposable
    {
        public StaffRepository()
            :base()
        {}

        public IEnumerable<EmployeeViewModel> GetEmployees(Guid userId)
        {
            var companyId = GetCompanyId(userId);
            return GetEmployees(companyId);
        }

        public IEnumerable<EmployeeViewModel> GetEmployees(int companyId)
        {
            List<EmployeeViewModel> model = ViewContext.UserInDepartmentView
                .Where(m => m.C_CompanyId == companyId)
                .Select(m => new EmployeeViewModel()
                {
                    EmployeeId = m.EmployeeId,
                    UserId = m.UserId,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Patronymic = m.Patronymic,
                    FullNameGenetive = m.FullNameGenitive,
                    PhotoPath = m.PhotoPath,
                    NegativeOpinion = m.NegativeMention,
                    PositionId = m.C_PosId,
                    PositiveOpinion = m.PositiveMention,
                    SocialLinks = m.SocialLinks,
                    LastLogin = m.LastLogin,
                    IsDismissed = m.IsDismissed,
                    PassportId = m.C_PassportId,
                    BirthDate = m.BirthDate,
                    Position = m.Name,
                    Department = m.Title,
                    IsOnline = false
                }).ToList();
            return model;
        }

        public EmployeeViewModel GetEmployee(int? employeeId)
        {
            EmployeeViewModel model = new EmployeeViewModel();
            if (employeeId.HasValue)
            {
                var employee = ViewContext.UserInDepartmentView.FirstOrDefault(m => m.EmployeeId == employeeId.Value);
                model.EmployeeId = employee.EmployeeId;
                model.FirstName = employee.FirstName;
                model.LastName = employee.LastName;
                model.IsKeyEmployee = employee.IsKeyEmployee;
                model.Patronymic = employee.Patronymic;
                model.FullNameGenetive = employee.FullNameGenitive;
                model.PhotoPath = employee.PhotoPath;
                model.NegativeOpinion = employee.NegativeMention;
                model.PositionId = employee.C_PosId;
                model.Skype = employee.Skype;
                model.ICQ = employee.ICQ;
                model.PositiveOpinion = employee.PositiveMention;
                model.SocialLinks = employee.SocialLinks;
                model.UserId = employee.UserId;
                model.LastLogin = employee.LastLogin;
                model.IsDismissed = employee.IsDismissed;
                model.PassportId = employee.C_PassportId;
                model.BirthDate = employee.BirthDate;
                model.Position = employee.Name;
                model.Department = employee.Title;
                model.IsOnline = false;
                model.Salary = employee.Salary;
                model.TransactionPercent = employee.TransactionPercent;
                model.GroupHeaderId = employee.C_GroupHeaderId;
                model.WorkGroups = string.IsNullOrEmpty(employee.WorkGroups) ?
                    new List<int>()
                    : employee.WorkGroups.Split(',').Select(m => Convert.ToInt32(m)).ToList();
            }
            return model;
        }
        public EmployeeViewModel GetEmployee(Guid? employeeId)
        {
            EmployeeViewModel model = new EmployeeViewModel();
            if (employeeId.HasValue)
            {
                var employee = ViewContext.UserInDepartmentView.FirstOrDefault(m => m.UserId ==  employeeId.Value);
                model.EmployeeId = employee.EmployeeId;
                model.FirstName = employee.FirstName;
                model.LastName = employee.LastName;
                model.IsKeyEmployee = employee.IsKeyEmployee;
                model.Patronymic = employee.Patronymic;
                model.FullNameGenetive = employee.FullNameGenitive;
                model.PhotoPath = employee.PhotoPath;
                model.NegativeOpinion = employee.NegativeMention;
                model.PositionId = employee.C_PosId;
                model.Skype = employee.Skype;
                model.ICQ = employee.ICQ;
                model.PositiveOpinion = employee.PositiveMention;
                model.SocialLinks = employee.SocialLinks;
                model.UserId = employee.UserId;
                model.LastLogin = employee.LastLogin;
                model.IsDismissed = employee.IsDismissed;
                model.PassportId = employee.C_PassportId;
                model.BirthDate = employee.BirthDate;
                model.Position = employee.Name;
                model.Department = employee.Title;
                model.IsOnline = false;
                model.Salary = employee.Salary;
                model.TransactionPercent = employee.TransactionPercent;
                model.GroupHeaderId = employee.C_GroupHeaderId;
                model.WorkGroups = string.IsNullOrEmpty(employee.WorkGroups) ?
                    new List<int>()
                    : employee.WorkGroups.Split(',').Select(m => Convert.ToInt32(m)).ToList();
            }
            return model;
        }
        
        public PassportViewModel GetEmployeePassport(int? employeeId)
        {
                var model = new PassportViewModel()
            {
                DateIssue = DateTime.Now.Date.ToString("dd.MM.yyyy")
            };
            if (employeeId.HasValue)
            {
                var passportId = Context.Employees.FirstOrDefault(m => m.EmployeeId == employeeId.Value).C_PassportId;
                if (passportId.HasValue)
                {
                    var passport = Context.Passports.FirstOrDefault(m => m.PassportId == passportId.Value);
                    model.PassportId = passport.PassportId;
                    model.Serial = passport.Serial;
                    model.CodeIssue = passport.CodeIssue;
                    model.DateIssue = passport.DateIssue.ToString("dd.MM.yyyy");
                    model.Number = passport.Number;
                    model.WhoIssue = passport.WhoIssue;
                    model.ScanPath = passport.ScanPath
                        .Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    model.BirthLocation = passport.BirthLocation;
                    model.RegLocation = passport.RegLocation;
                    model.EmployeeId = employeeId.Value;
                }
            }
            return model;
        }

        public void AddEmployee(EmployeeRegisterModel model, Guid userId)
        {
            Employee employee = new Employee()
            {
                C_PosId = model.PositionId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserId = userId,
                LastLogin = DateTime.Now,
                IsDismissed = false,
                IsKeyEmployee = false,
                PhotoPath = string.Empty,
                Salary = model.Salary,
                TransactionPercent = model.TransactionPercent
            };
            Context.Employees.Add(employee);
            Context.SaveChanges();
            employee.Departments.Add(Context.Positions.FirstOrDefault(m => m.PositionId == model.PositionId).Department);
            Context.SaveChanges();
        }

        public void UpdateEmployee(EmployeeViewModel model)
        {
            if (model.EmployeeId != 0)
            {
                var employee = Context.Employees.FirstOrDefault(m => m.EmployeeId == model.EmployeeId);
                employee.EmployeeId = model.EmployeeId;
                if (employee.C_PosId != model.PositionId)
                {
                    employee.Departments.Clear();
                    employee.Departments.Add(Context.Positions.FirstOrDefault(m => m.PositionId == model.PositionId).Department);
                }
                if (model.GroupHeaderId.HasValue)
                {
                    if (model.WorkGroups == null)
                        model.WorkGroups = new List<int>() { model.GroupHeaderId.Value };
                    else if (!model.WorkGroups.Contains(model.GroupHeaderId.Value))
                        model.WorkGroups.Add(model.GroupHeaderId.Value);
                }
                employee.C_PosId = model.PositionId;
                employee.C_GroupHeaderId = model.GroupHeaderId;
                employee.FirstName = model.FirstName;
                employee.LastName = model.LastName;
                employee.Patronymic = model.Patronymic;
                employee.Salary = model.Salary;
                employee.Skype = model.Skype;
                employee.ICQ = model.ICQ;
                employee.TransactionPercent = model.TransactionPercent;

                if (!string.IsNullOrEmpty(model.PhotoPath))
                    employee.PhotoPath = model.PhotoPath;
                employee.SocialLinks = model.SocialLinks;
                employee.IsDismissed = model.IsDismissed;
                employee.BirthDate = model.BirthDate;
                employee.FullNameGenitive = model.FullNameGenetive;
                employee.PositiveMention = model.PositiveOpinion;
                employee.NegativeMention = model.NegativeOpinion;
                employee.IsKeyEmployee = model.IsKeyEmployee;
                employee.WorkGroups = model.WorkGroups!=null && model.WorkGroups.Count > 0 ?
                    string.Join(",", model.WorkGroups) 
                    : string.Empty;
            }
            Context.SaveChanges();
        }
        
        public void UpdateEmployee(CompanyEmployeeViewModel model)
        {
            var employee = Context.Employees.FirstOrDefault(m => m.EmployeeId == model.EmployeeId);
            employee.FirstName = model.FirstName;
            employee.LastName = model.LastName;
            employee.Patronymic = model.Patronymic;
            employee.FullNameGenitive = model.FullNameGenetive;
            employee.Position.Name = model.PositionName;
            employee.Position.NameGenetive = model.PositionNameGenetive;
            Context.SaveChanges();
        }

        public void AddOrUpdateEmployeePassport(PassportViewModel model)
        {
            IFormatProvider ruDateFormat = new CultureInfo("ru-RU").DateTimeFormat;

            if (model.PassportId != 0)
            {
                var passport = Context.Passports.FirstOrDefault(m => m.PassportId == model.PassportId);
                passport.Serial = model.Serial;
                passport.Number = model.Number;
                passport.RegLocation = model.RegLocation;
                passport.ScanPath = string.Join(",", model.ScanPath);
                passport.WhoIssue = model.WhoIssue;
                passport.DateIssue =  Convert.ToDateTime(model.DateIssue, ruDateFormat);
                passport.CodeIssue = model.CodeIssue;
                passport.BirthLocation = model.BirthLocation;
                Context.SaveChanges();
            }
            else
            {
                Passport passport = new Passport()
                {
                    Serial = model.Serial,
                    Number = model.Number,
                    RegLocation = model.RegLocation,
                    ScanPath = string.Join(",", model.ScanPath),
                    WhoIssue = model.WhoIssue,
                    DateIssue = Convert.ToDateTime(model.DateIssue, ruDateFormat),
                    CodeIssue = model.CodeIssue,
                    BirthLocation = model.BirthLocation
                };
                Context.Passports.Add(passport);
                Context.SaveChanges();
                Context.Employees.FirstOrDefault(m => m.EmployeeId == model.EmployeeId).C_PassportId = passport.PassportId;
                Context.SaveChanges();

            }
        }

        //todo create group of users
        public IEnumerable<SelectListItem> GetSubordinatedUsers(Guid userId)
        {
            List<UserInDepartmentView> user4Assign = new List<UserInDepartmentView>();
            CRMAccessRepository accessRepository = new CRMAccessRepository();

            var funcRoles = accessRepository.GetFuncRoles4Employee(userId);
            var companyId = GetCompanyByUserId(userId).CompanyId;

            var existUsers = ViewContext.UserInDepartmentView
                .Where(m => m.C_CompanyId == companyId && m.IsActive && !m.IsDismissed)
                .ToList();
            var user = ViewContext.UserInDepartmentView
               .FirstOrDefault(m => m.UserId == userId);

            var departmentHeaderId = GetDepartmentHeader(user.DepartmentId);

            //Ключевой сотрудник может создавать задачи кому угодно
            if (user.IsKeyEmployee)
            {
                user4Assign.AddRange(existUsers);
            }
            else if (user.UserId == departmentHeaderId)
            {
                user4Assign.AddRange(GetUsersInDepartment(existUsers, user.DepartmentId));
            }
            // руководитель группы назначает только своей группе 
            // и руководителям др. групп в пределах своего отдела
            else if (user.C_GroupHeaderId.HasValue)
            {
                user4Assign.AddRange(GetUsersInDepartment(existUsers, user.DepartmentId, true));
                user4Assign.AddRange(GetUsersInDepartmentGroups(existUsers, user));
            }
            else if (!string.IsNullOrEmpty(user.WorkGroups))
            {
                user4Assign.AddRange(GetUsersInEmployeeGroups(existUsers, user));
            }
            else
            {
                user4Assign.AddRange(GetUsersInDepartment(existUsers, user.DepartmentId));
            }

            user4Assign = user4Assign.Distinct().ToList();
            if (user.UserId != departmentHeaderId)
            {
                user4Assign = user4Assign
                    .Where(m => m.UserId != departmentHeaderId)
                    .ToList();
            }

            return user4Assign.Select(m => new SelectListItem()
            {
                Text = m.FirstName + " " + m.LastName,
                Value = m.UserId.ToString(),
                Selected = m.UserId == userId
            }).OrderBy(m => m.Text);
        }

        private Guid GetDepartmentHeader(int departmentId)
        {
            var position = ViewContext.PositionInDepartmentView
                .FirstOrDefault(m => m.C_DepartmentId == departmentId && !m.C_ParentPosId.HasValue);

            if (position == null)
            {
                var existPositions = ViewContext.PositionInDepartmentView
                                                .Where(m => m.C_DepartmentId == departmentId)
                                                .Select(m => m.PositionId).ToList();

                position = ViewContext.PositionInDepartmentView
                                      .FirstOrDefault(m => m.C_DepartmentId == departmentId
                                        && !existPositions.Contains(m.C_ParentPosId.Value));
            }

            return ViewContext.UserInDepartmentView
                .FirstOrDefault(m => m.C_PosId == position.PositionId)
                .UserId;
        }
        
        private IEnumerable<UserInDepartmentView> GetUsersInEmployeeGroups(List<UserInDepartmentView> existUsers, UserInDepartmentView user)
        {
            var groups = user.WorkGroups.Split(',');
            return existUsers
                 .Where(m => m.DepartmentId == user.DepartmentId
                     && m.WorkGroups.Split(',').Intersect(groups).Count() > 0);
        }

        private IEnumerable<UserInDepartmentView> GetUsersInDepartment(List<UserInDepartmentView> existUsers, int departmentId, bool onlyGroupHeader = false)
        {
            var result = existUsers
                .Where(m => m.DepartmentId == departmentId);
            if (onlyGroupHeader)
                result = result.Where(m => m.C_GroupHeaderId.HasValue);
            return result;
        }

        private IEnumerable<UserInDepartmentView> GetUsersInDepartmentGroups(List<UserInDepartmentView> existUsers, UserInDepartmentView groupHeader)
        {
            var groupIdstr = groupHeader.C_GroupHeaderId.ToString();
            return existUsers
                .Where(m => m.DepartmentId == groupHeader.DepartmentId
                            && !string.IsNullOrEmpty(m.WorkGroups)
                            && m.WorkGroups.Split(',').Any(n => n == groupIdstr));
        }

        public void SetLoginEmployee(Guid userId,string ip, bool isLogin)
        {
            if (isLogin)
            {
                var employee = Context.Employees.FirstOrDefault(m => m.UserId == userId);
                employee.LastLogin = DateTime.Now;
                Context.EmployeeTimesheets.Add(new EmployeeTimesheet()
                {
                    C_EmployeeId = employee.EmployeeId,
                    Created = DateTime.Now,
                    BeginWork = DateTime.Now.TimeOfDay,
                    IpLocationBegin = ip
                });
                Context.SaveChanges();
            }
            else
            {
                var employee = Context.Employees.FirstOrDefault(m => m.UserId == userId);
                var timesheets = Context.EmployeeTimesheets
                    .Where(m=> m.C_EmployeeId == employee.EmployeeId && !m.EndTime.HasValue)
                    .OrderByDescending(m=>m.EmployeeTimesheetId)
                    .ToList();
                var timesheet = timesheets.FirstOrDefault();
                if (timesheet != null)
                {
                    timesheet.IpLocationEnd = ip;
                    timesheet.EndTime = DateTime.Now.TimeOfDay;
                    Context.SaveChanges();
                }
            }
        }

        public IEnumerable<TimeSheetViewModel> GetTimesheetEmployees(IEnumerable<EmployeeViewModel> employees)
        {

            var userIds = employees.Select(m => m.EmployeeId).ToList();
            List<TimeSheetViewModel> timesheets = Context.EmployeeTimesheets
                .Where(m => userIds.Contains(m.C_EmployeeId))
                .Select(m => new TimeSheetViewModel()
                {
                    EmployeeId = m.C_EmployeeId,
                    Created = m.Created,
                    IpIn = m.IpLocationBegin,
                    IpOut = m.IpLocationEnd,
                    TimeIn = m.BeginWork,
                    TimeOut = m.EndTime
                })
                .ToList();
            foreach (var item in timesheets)
            {
                var employee = employees.FirstOrDefault(m => m.EmployeeId == item.EmployeeId);
                item.UserName = employee.FirstName + " " + employee.LastName;
            }
            return timesheets;
        }
        public Dictionary<Guid, string> GetEmployeesPhoto(Guid userId)
        {
          var companyId =  GetShortCompanyInfoByUserId(userId).CompanyId;
          return GetEmployeesPhoto(companyId);
        }

        public Dictionary<Guid, string> GetEmployeesPhoto(int companyId)
        {
            return ViewContext.UserInDepartmentView
                .Where(m => m.C_CompanyId == companyId)
                .ToDictionary(m => m.UserId, m => m.PhotoPath);
        }

        public IEnumerable<SelectListItem> GetAvailableDepartmetns(Guid userId)
        {
            var user = ViewContext.UserInDepartmentView
                .FirstOrDefault(m => m.UserId == userId);
            var model = new List<SelectListItem>();


            if (user.IsKeyEmployee)
            {
                model = Context.Departments
                        .Where(m => m.C_CompanyId == user.C_CompanyId)
                        .Select(m => new { name = m.Title, id = m.DepartmentId })
                        .ToList()
                        .Select(m => new SelectListItem() { Text = m.name, Value = m.id.ToString() })
                        .ToList();
            }
            else if(Context.Positions
                .FirstOrDefault(m=>m.C_DepartmentId == user.DepartmentId && !m.C_ParentPosId.HasValue)
                .Employees.Any(m=>m.UserId == userId))
            {
                model = Context.Departments
                        .Where(m => m.DepartmentId == user.DepartmentId)
                        .Select(m => new { name = m.Title, id = m.DepartmentId })
                        .ToList()
                        .Select(m => new SelectListItem() { Text = m.name, Value = m.id.ToString() })
                        .ToList();
            }
            return model;
        }
    }
}
