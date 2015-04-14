using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Restaurant
/// </summary>
public class Restaurant : BaseClass<Restaurant>
{
	public Restaurant()
	{
	}

    public string Name { get; set; }

    public int UserId { get; set; }

}