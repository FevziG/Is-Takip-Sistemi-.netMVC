using System;
using System.Collections.Generic;

namespace IsTakipSistemiMVC.Models;

public partial class Birimler
{
    public int BirimId { get; set; }

    public string? BirimAd { get; set; }

    public virtual ICollection<Personeller> Personellers { get; set; } = new List<Personeller>();
}
