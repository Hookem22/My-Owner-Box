using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Excel_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //ExcelPackage pck = Excel.Sample();

        //pck.SaveAs(Response.OutputStream);
        //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //Response.AddHeader("content-disposition", "attachment;  filename=Sample1.xlsx");
    }
}