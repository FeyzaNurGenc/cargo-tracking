using KargoTakip.Server.Application.Kargolar;
using KargoTakip.Server.Domain.Kargolar;
using MediatR;
using TS.Result;

namespace KargoTakip.Server.WebAPI.Modules;

public static class KargoModule
{
    public static void RegisterKargoRoutes(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/kargolar").WithTags("kargolar").RequireAuthorization();

        //update işlemi için Get metodu yazıldı
        group.MapGet("{id}",
            async (Guid id, ISender sender, CancellationToken cancellationtoken) =>
            {
                var response = await sender.Send(new KargoGetQuery(id), cancellationtoken);
                return response.IsSuccessful ? Results.Ok(response) : Results.InternalServerError(response);
            })
            .Produces<Result<Kargo>>()
            .WithName("KargoGet");


        group.MapPost(string.Empty,
            async (ISender sender, KargoCreateCommand request, CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(request, cancellationToken);
                return response.IsSuccessful ? Results.Ok(response) : Results.InternalServerError(response);
            })
            .Produces<Result<string>>()
            .WithName("KargoCreate");

        group.MapPut(string.Empty,
            async (ISender sender, KargoUpdateCommand request, CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(request, cancellationToken);
                return response.IsSuccessful ? Results.Ok(response) : Results.InternalServerError(response);
            })
            .Produces<Result<string>>()
            .WithName("KargoUpdate");

        //ikinci bir putu kargoDurumUpdate için yaptık
        group.MapPut("update-status",
            async (ISender sender, KargoDurumUpdateCommand request, CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(request, cancellationToken);
                return response.IsSuccessful ? Results.Ok(response) : Results.InternalServerError(response);
            })
            .Produces<Result<string>>()
            .WithName("KargoDurumUpdate");

        group.MapDelete("{id}",
            async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(new KargoDeleteCommand(id), cancellationToken);
                return response.IsSuccessful ? Results.Ok(response) : Results.InternalServerError(response);
            })
            .Produces<Result<string>>()
            .WithName("KargoDelete");
    }
}
/*
 MapPost (Create)	KargoCreateCommand request	request, HTTP body’den otomatik model binding ile geliyor. Yeni nesne oluşturmaya gerek yok.
MapDelete (Delete)	new KargoDeleteCommand(id)	id, sadece Guid olarak URL’den geliyor. Bunu KargoDeleteCommand nesnesine çevirmek için new ile oluşturulmalı.
*/