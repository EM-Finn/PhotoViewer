using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoViewer.Core.Models;

public class SearchResult
{
    public Photo Photo { get; set; } = null!;

    public string MatchReason { get; set; } = string.Empty;
}