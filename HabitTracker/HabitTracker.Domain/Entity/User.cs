using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Domain.Entity
{
    public class User
    {
        public int Id { get; set; }             
        public string UserName { get; set; } = ""; 
        public string Email { get; set; } = "";

        // Navigation
        public ICollection<Habit> Habits { get; set; } = new List<Habit>();
    }
}
