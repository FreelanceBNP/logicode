
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace LGC.BNP.MIKUNI.Web.Helper
{
    //public class ClaimRequirementAttribute : TypeFilterAttribute
    //{
      
    //    public ClaimRequirementAttribute(string claimType, string claimValue) : base(typeof(ClaimRequirementFilter))
    //    {
    //        Arguments = new object[] { new Claim(claimType, claimValue) };
    //    }
    //}

    //public class ClaimRequirementFilter : IAuthorizationFilter
    //{
    //    readonly Claim _claim;
    //    private readonly UserService _serviceUser;
    //    public ClaimRequirementFilter(Claim claim, UserService serviceUser)
    //    {
    //        _claim = claim;
    //        _serviceUser = serviceUser;
    //    }

    //    public void OnAuthorization(AuthorizationFilterContext context)
    //    {
    //        //var hasClaim = context.HttpContext.User.Claims.Where(c => c.Type == _claim.Type).Select(c => c.Value).ToArray()[0];
    //        var hasClaim = _serviceUser.GetAuthorization(context.HttpContext.User.Identity.Name).data;
    //        bool isPass = false;
    //        if (_claim.Value.ToString().Contains(hasClaim))
    //        {
    //            isPass = true;
    //        }
    //        if (!isPass)
    //            context.Result = new ForbidResult();
    //    }
    //}
}