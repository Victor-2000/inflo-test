using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models;

public class LogEntry
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string PropertyName { get; set; } = String.Empty;
    public string PropertyOldValue { get; set; } = String.Empty;
    public string PropertyNewValue { get; set; } = String.Empty;

    // Foreign key to the parent Log
    public long LogId { get; set; }
    public Log Log { get; set; } = default!;
}

