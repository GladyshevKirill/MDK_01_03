using System;
using System.Collections.Generic;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public List<string> Tags { get; set; } = new();
}
