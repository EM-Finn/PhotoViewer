using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoViewer.Core.Models;

public class PhotoTag
{
    public Guid PhotoId { get; set; }

    public Photo? Photo { get; set; }

    public Guid TagId { get; set; }

    public Tag? Tag { get; set; }
}
