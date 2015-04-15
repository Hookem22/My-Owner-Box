using OfficeOpenXml;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class App_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (ConfigurationManager.AppSettings["IsProduction"] == "true")
            Response.Redirect("http://myownerbox.com");

        //Users user = HttpContext.Current.Session["CurrentUser"] as Users;
        //if (user == null || user.Id == 0)
        //    Response.Redirect("http://myownerbox.com");

        Users user = Users.LoadById(1);
        CurrentUserId.Value = user.Id.ToString();
        UserName.Value = user.Name;

        List<Restaurant> restaurants = Restaurant.LoadByPropName("UserId", user.Id.ToString());
        if (restaurants.Count > 0)
            RestaurantName.Value = restaurants[0].Name;

        List<Question> questions = Question.Get("Concept", "Create Your Concept", 1);
        if(questions.Count > 0)
        {
            ConceptOverview.Value = questions[0].Title;
        }
    }

    [WebMethod]
    public static List<Question> Get(string header, string category, int userId)
    {
        return Question.Get(header, category, userId);
    }

    [WebMethod]
    public static Users GetUser(string id)
    {
        int userId;
        if(int.TryParse(id, out userId))
        {
            return Users.LoadById(userId);
        }
        return new Users();
    }

    [WebMethod]
    public static void SaveUser(string id, string name, string email, string restaurant)
    {
        int userId;
        if (int.TryParse(id, out userId))
        {
            Users user = Users.LoadById(userId);
            user.Name = name;
            user.Email = email;
            user.Save();

            List<Restaurant> restaurants = Restaurant.LoadByPropName("UserId", id);
            if(restaurants.Count == 1)
            {
                restaurants[0].Name = restaurant;
                restaurants[0].Save();
            }
        }
    }

    [WebMethod]
    public static string SavePassword(string id, string oldPassword, string newPassword)
    {
        int userId;
        if (int.TryParse(id, out userId))
        {
            Users user = Users.LoadById(userId);
            if (user.Password != oldPassword)
                return "Your old password is not correct";

            user.Password = newPassword;
            user.Save();
        }
        return "";
    }

    [WebMethod]
    public static int SaveAnswer(Answer answer)
    {
        answer.Save();
        return answer.Id;
    }

    [WebMethod]
    public static List<Answer> SaveAnswers(List<Answer> answers)
    {
        foreach(Answer answer in answers)
            answer.Save();

        return answers;
    }

    [WebMethod]
    public static string CancelSubscription(int userId)
    {
        Users user = Users.LoadById(userId);
        try
        {
            var subscriptionService = new StripeSubscriptionService();
            foreach (StripeSubscription subscription in subscriptionService.List(user.StripeCustomerId))
            {
                user.Ended = subscription.PeriodEnd;
                user.Cancelled = true;
                user.Save();

                subscriptionService.Cancel(user.StripeCustomerId, subscription.Id);
            }
        }
        catch(Exception ex)
        {
            return "Error: " + ex.Message;
        }
        return "";
    }

    [WebMethod]
    public static void SendEmail(int userId, string body)
    {
        Users user = Users.LoadById(userId);
        body = string.Format("UserId: {0}<br>Name: {1}<br>Email: {2}<br><br>{3}", user.Id, user.Name, user.Email, body);
        //body = body.Replace("%20", " ").Replace("%3Cbr%3E", "<br/>").Replace("%3Cbr/%3E", "<br/>");
        Email email1 = new Email("MyOwnerBox@MyOwnerBox.com", "myownerbox@gmail.com", "App Contact", body);
        email1.Send();
        Email email2 = new Email("MyOwnerBox@MyOwnerBox.com", "williamallenparks@gmail.com", "App Contact", body);
        email2.Send();
    }

    [WebMethod]
    public static void SignOut()
    {
        HttpContext.Current.Session["CurrentUser"] = null;
    }


}