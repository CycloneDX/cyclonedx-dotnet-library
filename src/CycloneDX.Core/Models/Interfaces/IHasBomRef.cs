using System;
using System.Collections.Generic;
using System.Text;

namespace CycloneDX.Models
{
    public interface IHasBomRef
    {
        string BomRef { get; set; }
    }
}
