using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Todo.Models;
using Todo.Repository;

namespace WebApplicationminimalproject.API
{
    public class Authenticate
    {

        public static IResult GenerateToken(HttpContext context, AuthenticateAPIModel model)
        {
            //Authen
            if (model.UserName == model.Password)
            {
                var handler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(CustomUtility.PRIVATEKEY);
                var credentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature);

                var tokenExpiresIn = DateTime.Now.AddDays(15);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = GenerateClaims(model),
                    Expires = tokenExpiresIn,
                    SigningCredentials = credentials,
                };

                var Sectoken = handler.CreateToken(tokenDescriptor);
                var token = handler.WriteToken(Sectoken);
                return TypedResults.Ok(new AuthenticationAPIResponseModel(token, tokenExpiresIn));
            }
            return TypedResults.BadRequest("The UserName Or Password InCorrect");


        }


        private static ClaimsIdentity GenerateClaims(AuthenticateAPIModel model)
        {
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Name, model.UserName));
            claims.AddClaim(new Claim(ClaimTypes.Role, model.Role));
            //foreach (var role in user.Roles)
            //    claims.AddClaim(new Claim(ClaimTypes.Role, role));

            return claims;
        }


    }


    public record AuthenticateAPIModel(string UserName, string Password,string Role);
    public record AuthenticationAPIResponseModel(string Token, DateTime TokenExpiresIn);
}
