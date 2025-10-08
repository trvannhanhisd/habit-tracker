using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; } = UserRole.User; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        [MaxLength(200)]
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Navigation
        public List<Habit> Habits { get; set; } = new List<Habit>();
    }

    public enum UserRole
    {
        User,
        Admin
    }
}
