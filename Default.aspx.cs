using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod]
    public static string CreateUser(Users user, string restaurantName)
    {
        string error = BuyPlan(user);
        if(string.IsNullOrEmpty(error))
        {
            user.Joined = DateTime.Now;
            user.Save();
            Restaurant restaurant = new Restaurant() { Name = restaurantName, UserId = user.Id };
            restaurant.Save();

            HttpContext.Current.Session["CurrentUser"] = user;
            return "";
        }
        return error;
    }

    static string BuyPlan(Users user)
    {
        var myCustomer = new StripeCustomerCreateOptions();

        // set these properties if it makes you happy
        myCustomer.Email = user.Email;
        myCustomer.Description = user.Name;

        // setting up the card
        myCustomer.Card = new StripeCreditCardOptions()
        {
            CardNumber = user.CreditCard.CardNumber,
            CardExpirationYear = "20" + user.CreditCard.CardExpirationYear,
            CardExpirationMonth = user.CreditCard.CardExpirationMonth,
            //CardAddressCountry = "US",                // optional
            //CardAddressLine1 = "24 Beef Flank St",    // optional
            //CardAddressLine2 = "Apt 24",              // optional
            //CardAddressCity = "Biggie Smalls",        // optional
            //CardAddressState = "NC",                  // optional
            //CardAddressZip = "27617",                 // optional
            CardName = user.Name,                       // optional
            CardCvc = user.CreditCard.Cvc               // optional
        };

        //myCustomer.PlanId = planId;                          // only if you have a plan
        //myCustomer.TaxPercent = 20;                            // only if you are passing a plan, this tax percent will be added to the price.
        //myCustomer.Coupon = *couponId*;                        // only if you have a coupon
        //myCustomer.TrialEnd = DateTime.UtcNow.AddMonths(1);    // when the customers trial ends (overrides the plan if applicable)
        //myCustomer.Quantity = 1;                               // optional, defaults to 1

        var customerService = new StripeCustomerService();
        try
        {
            StripeCustomer stripeCustomer = customerService.Create(myCustomer);
            var subscriptionService = new StripeSubscriptionService();
            
            string planId = user.Annual ? "MyOwnerBoxAnnual" : "MyOwnerBoxMonthly";
            StripeSubscription stripeSubscription = subscriptionService.Create(stripeCustomer.Id, planId);
            user.StripeCustomerId = stripeCustomer.Id;
        }
        catch (Exception ex)
        {
            return "Error: " + ex.Message;
        }

        return "";
    }

    [WebMethod]
    public static string Login(string email, string password)
    {
        List<Users> users = Users.LoadByPropName("Email", email);
        if (users.Count == 0 || users[0].Id == 0 || users[0].Password != password)
            return "Incorrect email or password";

        HttpContext.Current.Session["CurrentUser"] = users[0];
        return "";
    }

    [WebMethod]
    public static string SendPassword(string email)
    {
        List<Users> users = Users.LoadByPropName("Email", email);
        if (users.Count == 0 || users[0].Id == 0)
            return "We do not have that email on file.";

        string body = string.Format("Your password for MyOwnerBox.com is: {0}<br/><br/>Thanks, <br/>My Owner Box Team", users[0].Password);
        Email message = new Email("MyOwnerBox@MyOwnerBox.com", email, "My Owner Box Password", body);
        message.Send();

        return "";
    }

    [WebMethod]
    public static void SendEmail(string subject, string body)
    {
        if (ConfigurationManager.AppSettings["IsProduction"] != "true")
            return;

        body = body.Replace("%20", " ").Replace("%3Cbr%3E", "<br/>").Replace("%3Cbr/%3E", "<br/>");
        Email email1 = new Email("MyOwnerBox@MyOwnerBox.com", "myownerbox@gmail.com", subject, body);
        email1.Send();
        Email email2 = new Email("MyOwnerBox@MyOwnerBox.com", "williamallenparks@gmail.com", subject, body);
        email2.Send();
    }


}