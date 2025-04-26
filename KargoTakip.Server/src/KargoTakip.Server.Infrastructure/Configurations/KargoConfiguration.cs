using KargoTakip.Server.Domain.Kargolar;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KargoTakip.Server.Infrastructure.Configurations;
public sealed class KargoConfiguration : IEntityTypeConfiguration<Kargo>
{
    //Bu konfigürasyon sınıfının amacı, Entity Framework Core (EF Core) ile çalışan Kargo varlığının (entity) veritabanındaki davranışını belirlemek ve uygun veri dönüşümlerini tanımlamaktır. 

    //Configure metodu, EF Core'un Fluent API kullanarak Kargo nesnesinin nasıl haritalanacağını (mapping) belirleyen metottur.
    public void Configure(EntityTypeBuilder<Kargo> builder)
    {
        //Kargo nesnesinin iç içe geçmiş (owned) nesneleri olduğunu gösterir.
        //Anlamı: Gonderen, Alici ve TeslimAdresi, bağımsız entity olarak değil, Kargo nesnesine gömülü(owned entity) olarak kabul edilir.
        //Bu tür nesneler ayrı bir tablo olarak değil, Kargo tablosunun içinde saklanır.
        builder.OwnsOne(p => p.Gonderen);
        builder.OwnsOne(p => p.Alici);
        builder.OwnsOne(p => p.TeslimAdresi);
        builder.OwnsOne(p => p.KargoInformation, builder =>
        {
            builder
            .Property(p => p.KargoTipi)
            .HasConversion(tip => tip.Value, value => KargoTipiEnum.FromValue(value)); //bu satırdaki amaç KargoInformation içinde KargoTipi enum sınıfından alınıyor bilgisini vermek
        });

        //bu şart bize, listeyi getirirken silinmişleri getirme diyor
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
