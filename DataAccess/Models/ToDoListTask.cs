using System;

namespace POS.Models
{
    internal class ToDoListTask
    {
        public int TodoTaskId { get; set; }
        public string Content { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? CompletionDate { get; set; }
    }

}
