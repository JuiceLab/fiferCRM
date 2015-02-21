using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskModel.Task
{
    [JsonObject]
    public class JsonTaskPreview
    {
        [JsonProperty("taskid")]
        public Guid TaskId { get; set; }
        [JsonProperty("number")]
        public string TaskNumber { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("start")]
        public string DateStarted { get; set; }
        [JsonProperty("period")]
        public bool HasPeriod{ get; set; }
        [JsonProperty("group")]
        public bool HasGroup { get; set; }
    }
}
