using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace UserManagement.Models;

public class Log
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public long UserId { get; set; } = default!;
    public LogType Type { get; set; } = default!;
    public DateTime DateTimeOfIssue { get; set; } = default!;

    [JsonIgnore]
    public User User { get; set; } = default!;

    // Collection of LogEntries for property changes
    public ICollection<LogEntry> LogEntries { get; set; } = new List<LogEntry>();
}

