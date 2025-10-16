using System;

namespace PetShop.Models
{
    public class Objective
    {
        public int Id { get; set; }
        public string Account { get; set; }
        public int TargetDays { get; set; }
        public float TargetWeight { get; set; }
        public decimal DailyCalories { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}