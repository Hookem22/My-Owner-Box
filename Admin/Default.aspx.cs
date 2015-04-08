
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    [WebMethod]
    public static List<Question> Get(string header, string category, int userId)
    {        
        return Question.Get(header, category, userId);
    }

    [WebMethod]
    public static int SaveQuestion(Question question)
    {
        question.Title = question.Title.Replace("\n", "<br/>");
        question.Save();
        return question.Id;
    }
}