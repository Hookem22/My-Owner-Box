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