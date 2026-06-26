using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoViewer.Core.Models;

public class Tag
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Color { get; set; }

    public string? Emoji { get; set; }

    public bool IsAutomatic { get; set; }
}
