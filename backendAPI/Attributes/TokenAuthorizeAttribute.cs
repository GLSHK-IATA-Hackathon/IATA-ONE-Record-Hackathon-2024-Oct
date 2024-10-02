using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace WebAPITemplate.Attributes
{
    public class TokenAuthorizeAttribute : TypeFilterAttribute
    {
        public TokenAuthorizeAttribute() : base(typeof(TokenAuthorizeAttributeFilter))
        {
        }

    }

    public class TokenAuthorizeAttributeFilter : ActionFilterAttribute
    {

        private readonly bool isDebugmode;

        public TokenAuthorizeAttributeFilter(IConfiguration _config)
        {
            isDebugmode = _config.GetSection("AppSettings:IsDebugMode").Get<bool>();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!isDebugmode)
            {
                AuthenticationHeaderValue authorization = AuthenticationHeaderValue.TryParse(context.HttpContext.Request.Headers["Authorization"], out authorization) ? authorization : null;

                if (authorization == null)
                    throw new Exception("Invalid Token");


                //Decrypt token

                //Check Access Right of the access token for the current action
            }

        }
    }
}
