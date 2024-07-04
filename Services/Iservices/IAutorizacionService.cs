using ProyectoToken.Models.Custom;

namespace ProyectoToken.Services.Iservices;

public interface IAutorizacionService
{
    Task<AutorizacionResponse> DevolverToken(AutorizacionRequest request);
}
