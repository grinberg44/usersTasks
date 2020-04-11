using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersTasks.Models
{
    public class TasksShareData
    {
        public long UserIdToShare { get; set; }

        public List<long> TasksIdsToShare { get; set; }
    }
}
