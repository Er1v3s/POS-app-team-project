using System;

namespace POS.Models
{
    public class ToDoListTask
    {
        public int TodoTask_Id { get; set; }
        public string Content { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? CompletionDate { get; set; }
    }

}
