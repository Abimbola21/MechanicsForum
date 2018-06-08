using System;
using System.ComponentModel.DataAnnotations;

namespace MechanicsForum.Models
{
    public class Answers
    {
        [Key]
        public int Id { get; set; }
        public int ProblemId { get; set; }       
        public string AnsweredBy { get; set; }
        public string Answer { get; set; }
        public string MediaPath { get; set; }  
        public DateTime AnswerDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}