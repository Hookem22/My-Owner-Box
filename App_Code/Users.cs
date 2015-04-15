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

    public bool Annual { get; set; }

    public bool Cancelled { get; set; }

    public string StripeCustomerId { get; set; }

    public DateTime? Ended { get; set; }

    public DateTime Joined { get; set; }

    [NonDB]
    public CreditCard CreditCard { get; set; }

    #endregion

}

public class CreditCard
{
    public string CardNumber { get; set; }

    public string CardExpirationMonth { get; set; }

    public string CardExpirationYear { get; set; }

    public string Cvc { get; set; }
}