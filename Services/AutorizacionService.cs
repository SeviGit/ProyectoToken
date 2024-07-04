using Microsoft.IdentityModel.Tokens;
using ProyectoToken.Models;
using ProyectoToken.Models.Custom;
using ProyectoToken.Services.Iservices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProyectoToken.Services;

public class AutorizacionService : IAutorizacionService {

    private readonly DbPruebaContext _context;
    private readonly IConfiguration _configuracion;

    public AutorizacionService(DbPruebaContext context, IConfiguration configuracion) {
        _context = context;
        _configuracion = configuracion;
    }

    private string GenerarToken(string idUsuario) {
        var key = _configuracion.GetValue<string>("JwtStting:key");
        var keyBytes = Encoding.ASCII.GetBytes(key);

        var claims = new ClaimsIdentity();
        claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, idUsuario));

        var credencialesToken = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256Signature
            );

        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = claims,
            Expires = DateTime.UtcNow.AddMinutes(1),
            SigningCredentials = credencialesToken
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

        string tokenCreado = tokenHandler.WriteToken(tokenConfig);

        return tokenCreado;
    }


    public async Task<AutorizacionResponse> DevolverToken(AutorizacionRequest request) {
        var usuario_encontrado = _context.Usuarios.FirstOrDefault(x =>
        x.NombreUsuario == request.NombreUsuario &&
        x.Clave == request.Clave
        );

        if (usuario_encontrado == null) {
            return await Task.FromResult<AutorizacionResponse>(null);
        }

        string tokenCreado = GenerarToken(usuario_encontrado.IdUsuario.ToString());

        var respuesta = new AutorizacionResponse { Token = tokenCreado, Resultado = true, Msg = "Ok" };

        return respuesta;
    }
}
