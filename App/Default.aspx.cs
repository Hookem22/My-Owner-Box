using OfficeOpenXml;
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

        CurrentUserId.Value = "1";
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

    //[WebMethod]
    //public static string GetTemplate(List<Question> questions)
    //{
    //    return Question.GetTemplate(questions, Question.CategoryTypes.Concept);
    //}

}