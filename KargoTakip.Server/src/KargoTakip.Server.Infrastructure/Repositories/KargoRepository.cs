using GenericRepository;
using KargoTakip.Server.Domain.Kargolar;
using KargoTakip.Server.Infrastructure.Context;

namespace KargoTakip.Server.Infrastructure.Repositories;
internal sealed class KargoRepository : Repository<Kargo, ApplicationDbContext>, IKargoRepository
{
    public KargoRepository(ApplicationDbContext context) : base(context)
    {
    }
}
