using System;
using System.Collections.Generic;

namespace IsTakipSistemiMVC.Models;

public partial class Durumlar
{
    public int DurumId { get; set; }

    public string? DurumAd { get; set; }

    public virtual ICollection<Isler> Islers { get; set; } = new List<Isler>();
}
