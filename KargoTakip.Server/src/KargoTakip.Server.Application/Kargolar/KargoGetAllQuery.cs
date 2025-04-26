using KargoTakip.Server.Domain.Abstractions;
using KargoTakip.Server.Domain.Kargolar;
using KargoTakip.Server.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace KargoTakip.Server.Application.Kargolar;

//Bu sorgu, KargoGetAllQueryResponse türünde bir cevap bekler ve bunun bir IQueryable (veritabanından sorgulama yapabilen) türünde dönmesini sağlar.
public sealed record KargoGetAllQuery() : IRequest<IQueryable<KargoGetAllQueryResponse>>;


////Bu sınıf DTO (Data Transfer Object) örneğidir ve genellikle veritabanından alınan veri ile işlenen verinin taşınmasını sağlar.
//public sealed class KargoInformationGetAllDto
//{
//    public string KargoTipiName { get; set; } = default!;
//    public string Agirlik { get; set; } = default!;

//}

//KargoGetAllQueryResponse, kargo sorgu cevabını temsil eder ve EntityDto sınıfından türetilmiştir.
public sealed class KargoGetAllQueryResponse : EntityDto//EntityDto sınıfının amacı, veri transferi esnasında kullanılacak ortak bir model sağlamaktır. EntityDto, genellikle ID veya başka ortak özellikler içerebilir.
{
    public string GonderenFullName { get; set; } = default!;
    public string AliciFullName { get; set; } = default!;
    public string TeslimAdresiCity { get; set; } = default!;
    public string TeslimAdresiTown { get; set; } = default!;
    public string KargoTipiName { get; set; } = default!;
    public int Agirlik { get; set; } = default!;
    public int KargoDurumValue { get; set; }
    public string KargoDurumName { get; set; } = default!;
}

internal sealed class KargoGetAllQueryHandler(
    IKargoRepository kargoRepository,
    UserManager<AppUser> userManager) : IRequestHandler<KargoGetAllQuery, IQueryable<KargoGetAllQueryResponse>>
{
    public Task<IQueryable<KargoGetAllQueryResponse>> Handle(KargoGetAllQuery request, CancellationToken cancellationToken)
    {
        var response = (from entity in kargoRepository.GetAll()
                            //join satırında, Kargo tablosundaki her bir entity (kargo) kaydının CreateUserId özelliği, UserManager ile ilişkili kullanıcıların Id değeriyle eşleştirilir. Bu işlem, kargonun oluşturulmasında görevli kullanıcı bilgilerini almanızı sağlar.
                        join create_user in userManager.Users.AsQueryable() on entity.CreateUserId equals create_user.Id
                        join update_user in userManager.Users.AsQueryable() on entity.CreateUserId equals update_user.Id into update_user
                        from update_users in update_user.DefaultIfEmpty()
                        select new KargoGetAllQueryResponse
                        {
                            AliciFullName = entity.Alici.FirstName + " " + entity.Alici.LastName,
                            GonderenFullName = entity.Gonderen.FirstName + "  " + entity.Gonderen.LastName,
                            Agirlik = entity.KargoInformation.Agirlik,
                            KargoTipiName = entity.KargoInformation.KargoTipi.Name,
                            TeslimAdresiCity = entity.TeslimAdresi.City,
                            TeslimAdresiTown = entity.TeslimAdresi.Town,
                            KargoDurumValue = (int)entity.KargoDurum,
                            KargoDurumName = entity.KargoDurum.GetDisplayName(),
                            CreateAt = entity.CreateAt,
                            DeleteAt = entity.DeleteAt,
                            Id = entity.Id,
                            IsDeleted = entity.IsDeleted,
                            UpdateAt = entity.UpdateAt,
                            CreateUserId = entity.CreateUserId,
                            CreateUserName = create_user.FirstName + " " + create_user.LastName + " (" + create_user.Email + ")",
                            UpdateUserId = entity.UpdateUserId,
                            UpdateUserName = entity.UpdateUserId == null
                            ? null
                            : update_users.FirstName + " " + update_users.LastName + " (" + update_users.Email + ")",
                        });
        return Task.FromResult(response);
    }
}
/*Bu işlemin genel amacı, kargo verilerini ve ilişkili kullanıcı bilgilerini birleştirip, son kullanıcıya anlamlı bir formatta (kargo bilgileri + kullanıcı bilgileri) sunmaktır. Bu şekilde, API çağrısı yapıldığında her bir kargo kaydının yanında hangi kullanıcıların kargo üzerinde işlem yaptığı bilgisi de yer alır.*/