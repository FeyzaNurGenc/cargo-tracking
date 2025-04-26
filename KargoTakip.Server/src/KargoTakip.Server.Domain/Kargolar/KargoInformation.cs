namespace KargoTakip.Server.Domain.Kargolar;

public sealed record KargoInformation
{
    public KargoTipiEnum KargoTipi { get; set; } = default!;
    public int KargoTipiValue => KargoTipi.Value; //Compututed property
    public int Agirlik { get; set; }
}

