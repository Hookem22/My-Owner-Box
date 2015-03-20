using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Users
/// </summary>
public class Users : BaseClass<Users>
{
	public Users()
	{
    }

    #region Properties

    public string Name { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    #endregion

}