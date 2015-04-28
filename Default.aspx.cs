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
        try
        {
            user.Joined = DateTime.Now;
            user.Save();
            Restaurant restaurant = new Restaurant() { Name = restaurantName, UserId = user.Id };
            restaurant.Save();

            HttpContext.Current.Session["CurrentUser"] = user;

            if (ConfigurationManager.AppSettings["IsProduction"] == "true")
            {
                string body = "New Sign Up";
                body += string.Format("<br/>Name: {0}<br/>Email: {1}", user.Name, user.Email);
                Email email1 = new Email("MyOwnerBox@MyOwnerBox.com", "myownerbox@gmail.com", "New Sign Up", body);
                email1.Send();
                Email email2 = new Email("MyOwnerBox@MyOwnerBox.com", "williamallenparks@gmail.com", "New Sign Up", body);
                email2.Send();
            }
        }
        catch (Exception ex)
        {
            Email email1 = new Email("MyOwnerBox@MyOwnerBox.com", "myownerbox@gmail.com", "Sign Up Error", ex.Message);
            //email1.Send();
            Email email2 = new Email("MyOwnerBox@MyOwnerBox.com", "williamallenparks@gmail.com", "Sign Up Error", ex.Message);
            email2.Send();
        }

        return "";
        
        
        
        //string error = BuyPlan(user);
        //if(string.IsNullOrEmpty(error))
        //{
        //    user.Joined = DateTime.Now;
        //    user.Save();
        //    Restaurant restaurant = new Restaurant() { Name = restaurantName, UserId = user.Id };
        //    restaurant.Save();

        //    HttpContext.Current.Session["CurrentUser"] = user;

        //    if (ConfigurationManager.AppSettings["IsProduction"] == "true")
        //    {
        //        string body = user.Annual ? "Purchase: Annual" : "Purchase: Monthly";
        //        body += string.Format("<br/>Name: {0}<br/>Email: {1}", user.Name, user.Email);
        //        Email email1 = new Email("MyOwnerBox@MyOwnerBox.com", "myownerbox@gmail.com", "New Purchase", body);
        //        email1.Send();
        //        Email email2 = new Email("MyOwnerBox@MyOwnerBox.com", "williamallenparks@gmail.com", "New Purchase", body);
        //        email2.Send();
        //    }
        //    return "";
        //}
        //return error;
    }

    static string BuyPlan(Users user)
    {
        try
        {
            var myCharge = new StripeChargeCreateOptions();

            // always set these properties
            int price = 0;
            int.TryParse(user.CreditCard.Price, out price);
            myCharge.Amount = price * 100;
            myCharge.Currency = "usd";

            // set this if you want to
            myCharge.Description = user.Name + ", " + user.Email;

            // set this property if using a token
            //myCharge.TokenId = *tokenId*;

            // set these properties if passing full card details
            // (do not set these properties if you have set a TokenId)
            myCharge.Card = new StripeCreditCardOptions();
            myCharge.Card.CardNumber = user.CreditCard.CardNumber;
            myCharge.Card.CardExpirationYear = user.CreditCard.CardExpirationYear;
            myCharge.Card.CardExpirationMonth = user.CreditCard.CardExpirationMonth;
            //myCharge.CardAddressCountry = "US";               // optional
            //myCharge.CardAddressLine1 = "24 Beef Flank St";   // optional
            //myCharge.CardAddressLine2 = "Apt 24";             // optional
            //myCharge.CardAddressState = "NC";                 // optional
            //myCharge.CardAddressZip = "27617";                // optional
            myCharge.Card.CardName = user.Name;                    // optional
            myCharge.Card.CardCvc = user.CreditCard.Cvc;       // optional

            // set this property if using a customer
            //myCharge.CustomerId = *customerId*;

            // if using a customer, you may also set this property to charge
            // a card other than the customer's default card
            //myCharge.Card = *cardId*;

            // set this if you have your own application fees (you must have your application configured first within Stripe)
            //myCharge.ApplicationFee = 25;

            // (not required) set this to false if you don't want to capture the charge yet - requires you call capture later
            myCharge.Capture = true;

            var chargeService = new StripeChargeService();

            StripeCharge stripeCharge = chargeService.Create(myCharge);
        }
        catch (Exception ex)
        {
            Email email1 = new Email("MyOwnerBox@MyOwnerBox.com", "myownerbox@gmail.com", "Stripe Error", ex.Message);
            email1.Send();
            Email email2 = new Email("MyOwnerBox@MyOwnerBox.com", "williamallenparks@gmail.com", "Stripe Error", ex.Message);
            email2.Send();

            return "Error: " + ex.Message;
        }

        return "";



        //var myCustomer = new StripeCustomerCreateOptions();

        //// set these properties if it makes you happy
        //myCustomer.Email = user.Email;
        //myCustomer.Description = user.Name;

        //// setting up the card
        //myCustomer.Card = new StripeCreditCardOptions()
        //{
        //    CardNumber = user.CreditCard.CardNumber,
        //    CardExpirationYear = "20" + user.CreditCard.CardExpirationYear,
        //    CardExpirationMonth = user.CreditCard.CardExpirationMonth,
        //    //CardAddressCountry = "US",                // optional
        //    //CardAddressLine1 = "24 Beef Flank St",    // optional
        //    //CardAddressLine2 = "Apt 24",              // optional
        //    //CardAddressCity = "Biggie Smalls",        // optional
        //    //CardAddressState = "NC",                  // optional
        //    //CardAddressZip = "27617",                 // optional
        //    CardName = user.Name,                       // optional
        //    CardCvc = user.CreditCard.Cvc               // optional
        //};

        ////myCustomer.PlanId = planId;                          // only if you have a plan
        ////myCustomer.TaxPercent = 20;                            // only if you are passing a plan, this tax percent will be added to the price.
        ////myCustomer.Coupon = *couponId*;                        // only if you have a coupon
        ////myCustomer.TrialEnd = DateTime.UtcNow.AddMonths(1);    // when the customers trial ends (overrides the plan if applicable)
        ////myCustomer.Quantity = 1;                               // optional, defaults to 1

        //var customerService = new StripeCustomerService();
        //try
        //{
        //    StripeCustomer stripeCustomer = customerService.Create(myCustomer);
        //    var subscriptionService = new StripeSubscriptionService();

        //    string planId = "MyOwnerBox";
        //    if(user.Annual)
        //    {
        //        int price = 0;
        //        int.TryParse(user.CreditCard.Price, out price);
        //        planId += ((price / 2) * 12).ToString() + "Year";
        //    }
        //    else
        //    {
        //        planId += user.CreditCard.Price + "Month";
        //    }
        //    StripeSubscription stripeSubscription = subscriptionService.Create(stripeCustomer.Id, planId);
        //    user.StripeCustomerId = stripeCustomer.Id;
        //}
        //catch (Exception ex)
        //{
        //    Email email1 = new Email("MyOwnerBox@MyOwnerBox.com", "myownerbox@gmail.com", "Stripe Error", ex.Message);
        //    email1.Send();
        //    Email email2 = new Email("MyOwnerBox@MyOwnerBox.com", "williamallenparks@gmail.com", "Stripe Error", ex.Message);
        //    email2.Send();

        //    return "Error: " + ex.Message;
        //}

        //return "";
    }

    [WebMethod]
    public static string Login(string email, string password)
    {
        List<Users> users = Users.LoadByPropName("Email", email);
        if (users.Count == 0 || users[0].Id == 0 || users[0].Password != password)
            return "Incorrect email or password";

        if (users[0].Cancelled && users[0].Ended < DateTime.Now)
            return "This subscription has been cancelled";

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
        body = body.Replace("undefined<br/><br/><br/><br/>", "Request for Sign Up<br/><br/>");
        Email email1 = new Email("MyOwnerBox@MyOwnerBox.com", "myownerbox@gmail.com", subject, body);
        email1.Send();
        Email email2 = new Email("MyOwnerBox@MyOwnerBox.com", "williamallenparks@gmail.com", subject, body);
        email2.Send();

        PotentialSummary(body);

    }

    static void PotentialSummary(string body)
    {
        try
        {
            Users user;
            List<Users> users;
            if (body.Contains("Clicked Yes, I do"))
            {
                Random rand = new Random();
                user = new Users() { Id = rand.Next() };
                HttpContext.Current.Session["PotentialUserId"] = user.Id;
                users = HttpContext.Current.Cache.Get("PotentialUsers") as List<Users>;
                if (users == null)
                    users = new List<Users>();

                users.Add(user);
            }
            else
            {
                int userId = (int)HttpContext.Current.Session["PotentialUserId"];
                users = HttpContext.Current.Cache.Get("PotentialUsers") as List<Users>;
                user = users.Find(delegate(Users u)
                {
                    return u.Id == userId;
                });
                if(user == null)
                {
                    user = new Users() { Id = userId };
                }
            }

            user.Name = string.Format("{0}<br/><br/>{1}", user.Name, body);

            HttpContext.Current.Cache.Remove("PotentialUsers");
            HttpContext.Current.Cache.Insert("PotentialUsers", users);

            if (HttpContext.Current.Cache.Get("LastEmailSent") == null)
            {
                HttpContext.Current.Cache.Insert("LastEmailSent", DateTime.Now);
            }
            else
            {
                DateTime lastEmail = (DateTime)HttpContext.Current.Cache.Get("LastEmailSent");
                if (lastEmail.AddMinutes(30) < DateTime.Now)
                {
                    string summary = "All visitors in the last 30 minutes<br/><br/>";
                    foreach (Users u in users)
                    {
                        if(u.Id != user.Id)
                            summary += u.Name + "<br/><br/><br/><br/>";
                    }
                    //summary = summary.Replace("Price $45<br/><br/>", "").Replace("undefined<br/><br/>", "");

                    Email email1 = new Email("MyOwnerBox@MyOwnerBox.com", "myownerbox@gmail.com", "Visitor Summary", summary);
                    email1.Send();
                    Email email2 = new Email("MyOwnerBox@MyOwnerBox.com", "williamallenparks@gmail.com", "Visitor Summary", summary);
                    email2.Send();

                    users = users.FindAll(delegate(Users u)
                    {
                        return u.Id == user.Id;
                    });
                    HttpContext.Current.Cache.Remove("PotentialUsers");
                    HttpContext.Current.Cache.Insert("PotentialUsers", users);

                    HttpContext.Current.Cache.Remove("LastEmailSent");
                    HttpContext.Current.Cache.Insert("LastEmailSent", DateTime.Now);

                }
            }
        }
        catch(Exception ex)
        {
            Email error = new Email("MyOwnerBox@MyOwnerBox.com", "williamallenparks@gmail.com", "Summary error", ex.Message);
            error.Send();

            HttpContext.Current.Cache.Remove("PotentialUsers");
            HttpContext.Current.Cache.Remove("LastEmailSent");

            HttpContext.Current.Cache.Insert("PotentialUsers", new List<Users>());
            HttpContext.Current.Cache.Insert("LastEmailSent", DateTime.Now);
        }

    }


}