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

        CurrentUserId.Value = "1";// user.Id.ToString();
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
    public static string CancelAccount(int userId)
    {
        Users user = Users.LoadById(userId);
        try
        {
            var subscriptionService = new StripeSubscriptionService();
            string planId = user.Annual ? "MyOwnerBoxAnnual" : "MyOwnerBoxMonthly";
            subscriptionService.Cancel(user.StripeCustomerId, planId);

            user.Cancelled = true;
            user.Save();
        }
        catch(Exception ex)
        {
            return "Error: " + ex.Message;
        }
        return "";
    }


}