using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models;

public class Log
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public long UserId { get; set; } = default!;
    public List<string> PropertyNames { get; set; } = new List<string>();
    public List<string> PropertyOldValues { get; set; } = new List<string>();
    public List<string> PropertyNewValues { get; set; } = new List<string>();
    public LogType Type { get; set; } = default!;
    public DateTime DateTimeOfIssue { get; set; } = default!;
}
