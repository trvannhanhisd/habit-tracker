using HabitTracker.Application.Common.Mappings;
using HabitTracker.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Features.HabitLogs.Queries.GetHabitLogs
{
    public class HabitLogViewModel : IMapFrom<HabitLog>
    {
        public int Id { get; set; }          
        public int HabitId { get; set; }         
        public DateTime Date { get; set; }      
        public bool IsCompleted { get; set; }   
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
