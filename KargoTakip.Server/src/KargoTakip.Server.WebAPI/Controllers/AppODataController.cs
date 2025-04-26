using KargoTakip.Server.Application.Kargolar;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace KargoTakip.Server.WebAPI.Controllers;

[Route("odata")]
[ApiController]
[EnableQuery]
public class AppODataController(
    ISender sender
    ) : ODataController
{
    public static IEdmModel GetEdmModel()
    {
        ODataConventionModelBuilder builder = new();
        builder.EnableLowerCamelCase();
        builder.EntitySet<KargoGetAllQueryResponse>("kargolar");
        return builder.GetEdmModel();
    }


    [HttpGet("kargolar")]
    public async Task<IQueryable<KargoGetAllQueryResponse>> KargoGetAll(CancellationToken cancellationToken)
    {
        var response = await sender.Send(new KargoGetAllQuery(), cancellationToken);
        return response;
    }
}

/* NEDEN MODULE VE CONTROLLER KULLANDIK?
 Minimal API (Module) → Command-Handler yapısıyla MediatR kullanarak işlemleri yönetiyor.
Controller (OData API) → OData ile sorgulama işlemleri (Query) için kullanılıyor.
*/

// KargoGetAllQueryResponse modelindeki KargoDurumValue'yu int olarak tanımla
//var entity = builder.EntityType<KargoGetAllQueryResponse>();
//entity.Property(p => p.KargoDurumValue);  // Burada 'int' olarak kabul edilir, explicit olarak belirtmemiz gerekmez.
