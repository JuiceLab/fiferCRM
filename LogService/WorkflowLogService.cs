using CRMRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketModel;
using EnumHelper;
using CompanyRepositories;

namespace LogService
{
    public class WorkflowLogService
    {
        private CRMLocalRepository repository;
        private Guid _userId;
        public WorkflowLogService(Guid userId)
        {
            _userId = userId;
            repository = new CRMLocalRepository(userId);
        }
        public void AddCallTicketChanges(CallTicket curState, CallTicket dbState)
        {
            if (curState.DateStarted != dbState.DateStarted)
                repository.WriteModify(string.Format("Изменена дата звонка.  {0} вместо {1}",curState.DateStarted.Value.Date == DateTime.Now.Date?
                    curState.DateStarted.Value.ToShortTimeString() : curState.DateStarted.Value.ToShortDateString(),
                    dbState.DateStarted.Value.Date == DateTime.Now.Date ?
                    dbState.DateStarted.Value.ToShortTimeString() : dbState.DateStarted.Value.ToShortDateString()), _userId, curState.TicketId);
   
            if (curState.TicketStatus != dbState.TicketStatus )
                repository.WriteModify(string.Format("Изменен cтатус звонка.  {0} вместо {1}", ((EnumHelper.TaskStatus)curState.TicketStatus).GetStringValue(),
                    ((EnumHelper.TaskStatus)dbState.TicketStatus.Value).GetStringValue()), _userId, curState.TicketId);
        StaffRepository staffRepository = new  StaffRepository();
            if (curState.Assigned != dbState.Assigned)
            {
                var curEmployee = staffRepository.GetEmployee( curState.Assigned);
                var dbEmployee = staffRepository.GetEmployee( dbState.Assigned);

                repository.WriteModify(string.Format("Назначен новый исполнитель.  {0} вместо {1}", curEmployee.FirstName + " " + curEmployee.LastName, dbEmployee.FirstName + " " + dbEmployee.LastName), _userId, curState.TicketId);
        }
        }

    }
}
