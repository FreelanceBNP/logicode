using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LGC.BNP.MIKUNI.Web.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string? Name { get; set; }

		public string? SurName { get; set; }

		public bool IsAdmin { get; set; }

		public bool IsEnable { get; set; } = true;
	}
}




public class Model_1 {
    public string Name { get; set; }
	public string SurName { get; set;}
}

public class Model_2
{
    public string Order { get; set; }
    public int Qty { get; set; }
    public int Price { get; set; }
}
public class Model_Show
{
    public Model_1 model1 { get; set; }
	public Model_2 model2 { get; set;}
}