using System;
using HPages.Models.Enums;

namespace HPages.Database.Tables
{
    public class WorkerTask
    {
        public int WorkerTaskId { get; set; }
        public int? ObjectId { get; set; }
        public int? ObjectId2 { get; set; }
        public TaskResponse? ResultId { get; set; }
        public TaskType Type { get; set; }
        public string ErrorMessage { get; set; }
        public int? ResultObjectId { get; set; }
        public string ResultMessage { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public bool RestartOnFailure { get; set; }
    }
}