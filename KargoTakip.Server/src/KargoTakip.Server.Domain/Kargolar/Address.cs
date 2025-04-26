namespace KargoTakip.Server.Domain.Kargolar;

public sealed record Address(
    string City,
    string Town,
    string Mahalle,
    string Street,
    string FullAddress
    );

