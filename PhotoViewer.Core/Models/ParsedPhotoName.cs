using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoViewer.Core.Models;

public class ParsedPhotoName
{
    public string OriginalName { get; set; } = string.Empty;

    public List<string> Entities { get; set; } = new();

    public List<string> Properties { get; set; } = new();

    public int? Index { get; set; }

    public bool IsComposite { get; set; }
}
