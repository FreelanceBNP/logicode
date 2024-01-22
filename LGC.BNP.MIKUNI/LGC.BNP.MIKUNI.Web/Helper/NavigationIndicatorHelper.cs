using Microsoft.AspNetCore.Mvc.Rendering;

namespace LGC.BNP.MIKUNI.Web
{ 
    public static class HtmlHelpers
    {
        public static string IsSelected(this IHtmlHelper html, string controller = "", string action = "", string route = "")
        {
            string cssClass = "active";

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];
            string currentID = (string)html.ViewContext.RouteData.Values["id"];

            currentID = (String.IsNullOrWhiteSpace(currentID) ? html.ViewContext.HttpContext.Request.Query["tr"].ToString() : currentID);

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            if (String.IsNullOrEmpty(route))
            {
                route = currentID;
            }
                

            return controller == currentController && action == currentAction && (!string.IsNullOrEmpty(route) ? route == currentID : true) ?
                cssClass : String.Empty;
        }

		public static string IsSelectedTopic(this IHtmlHelper html, string controller = "")
		{
			string cssClass = "show";
			string currentController = (string)html.ViewContext.RouteData.Values["controller"];
		
			if (String.IsNullOrEmpty(controller))
				controller = currentController;

			return controller == currentController ?
				cssClass : String.Empty;
		}

		public static string PageClass(this IHtmlHelper htmlHelper)
        {
            string currentAction = (string)htmlHelper.ViewContext.RouteData.Values["action"];
            return currentAction;
        }
    }
}
