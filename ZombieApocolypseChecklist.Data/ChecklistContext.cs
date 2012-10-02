using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace ZombieApocolypseChecklist.Data
{
    public class ChecklistContext : DbContext
    {
        public DbSet<ChecklistItem> Items { get; set; }
    }

    public class ChecklistItem
    {
        [Key]
        public int id { get; set; }
        public string Content { get; set; }
        public bool IsCompleted { get; set; }
    }
}
