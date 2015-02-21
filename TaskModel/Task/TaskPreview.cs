using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumHelper;

namespace TaskModel.Task
{
    public class TaskPreview
    {
        public string TaskNumber { get; set; }
        public Guid TaskId { get; set; }
        public string Title { get; set; }
        public EnumHelper.TaskStatus Status { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByPhotoPath { get; set; }
        public Guid AssignedById { get; set; }
        public string AssignedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime? DateStarted { get; set; }
        public string Msg { get; set; }
        public bool IsHighProirity{ get; set;}

        public bool HasGroup { get; set; }

        public bool HasPeriod { get; set; }
    }
}
