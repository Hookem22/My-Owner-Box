using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Question
/// </summary>
public class Question : BaseClass<Question>
{
	public Question()
	{
    }

    #region Properties

    public string Title { get; set; }

    public string Options { get; set; }

    public string Type { get; set; }

    public string Sheet { get; set; }

    public string Page { get; set; }

    public string Section { get; set; }

    public string SkipCondition { get; set; }

    [NonDB]
    public Answer Answer { get; set; }

    //[NonDB]
    //public string Html
    //{
    //    get
    //    {
    //        string html = "";

    //        if (ControlType != ControlTypes.MultiTextbox)
    //        {
    //            if (!string.IsNullOrEmpty(Title))
    //                html += string.Format("<div class='title'>{0}</div>", Title);
    //            //if(!string.IsNullOrEmpty(Subtitle))
    //            //    html += string.Format("<div class='subtitle'>{0}</div>", Subtitle);

    //            html += "<div class='content'>";
    //            switch (ControlType)
    //            {
    //                case ControlTypes.Textbox:
    //                    if (!string.IsNullOrEmpty(Options))
    //                    {
    //                        string options = "";
    //                        foreach (string option in Options.Split('|'))
    //                        {
    //                            options += option + ", ";
    //                        }
    //                        if (!string.IsNullOrEmpty(options))
    //                            options = options.Substring(0, options.Length - 2);

    //                        html += string.Format("<input class='AnswerControl' type='text' placeholder='{0}' value='{1}' />", options, "{{Answer}}");
    //                    }
    //                    else
    //                    {
    //                        html += "<input class='AnswerControl' type='text' value='{{Answer}}' />";
    //                    }
    //                    break;
    //                case ControlTypes.Textarea:
    //                    html += "<textarea class='AnswerControl'>{{Answer}}</textarea>";
    //                    html += "<div class='textarea'><ul><li style='text-align:center;margin-bottom: .5em;'>Examples: </li>";
    //                    foreach (string option in Options.Split('|'))
    //                        html += string.Format("<li>{0}</li>", option);

    //                    html += "</ul></div>";
    //                    break;
    //                case ControlTypes.Select:
    //                    html += "<select class='AnswerControl'>";
    //                    foreach (string option in Options.Split('|'))
    //                        html += string.Format("<option>{0}</option>", option);

    //                    html += "</select>";
    //                    break;
    //                case ControlTypes.Checkbox:
    //                    foreach (string option in Options.Split('|'))
    //                        html += string.Format("<input type='checkbox' class='AnswerControl' value='{0}'><div class='checkbox'>{0}</div>", option);
    //                    break;
    //                case ControlTypes.Radio:
    //                    foreach (string option in Options.Split('|'))
    //                        html += string.Format("<input type='radio' name='AnswerControl' value='{0}'><div class='radio'>{0}</div>", option);

    //                    break;
    //                case ControlTypes.List:
    //                    if (!string.IsNullOrEmpty(Options))
    //                    {
    //                        string options = "";
    //                        foreach (string option in Options.Split('|'))
    //                        {
    //                            options += option + ", ";
    //                        }
    //                        options = options.Substring(0, options.Length - 2);
    //                        html += string.Format("<input class='ListControl' type='text' placeholder='{0}' /><span class='glyphicon glyphicon-plus-sign' ></span>", options);
    //                    }
    //                    else
    //                    {
    //                        html += "<input class='ListControl' type='text' /><span class='glyphicon glyphicon-plus-sign'></span>";
    //                    }
    //                    break;
    //            }
    //            html += "</div>";
    //        }
    //        else
    //        {
    //            html = "";


    //            //if (!string.IsNullOrEmpty(CardHeader))
    //            //    html += string.Format("<h1>{0}</h1>", CardHeader);

    //            //html += "<div class='multiTextGroup'>";
    //            //html += string.Format("<h2>{0}</h2>", Header);
    //            //for (int i = 0, ii = Questions.Count; i < ii; i++)
    //            //{
    //            //    if (i > 0 && Questions[i].Header != Questions[i - 1].Header)
    //            //    {
    //            //        html += string.Format("</div><div class='multiTextGroup divider'><h2>{0}</h2>", Questions[i].Header);
    //            //    }
    //            //    string options = "";
    //            //    foreach (string option in Questions[i].Options)
    //            //    {
    //            //        options += option + ", ";
    //            //    }
    //            //    if (!string.IsNullOrEmpty(options))
    //            //        options = options.Substring(0, options.Length - 2);

    //            //    if (string.IsNullOrEmpty(Questions[i].Title))
    //            //        html += "<div class='multiText' style='width:100%;'></div>";
    //            //    else
    //            //        html += string.Format("<div class='multiText'><span>{0}</span><input type='text' value='{1}' placeholder='{2}' /></div>", Questions[i].Title, "{{Answer}}", options);
    //            //}
    //            //html += "</div>";
    //        }
    //        return html;

    //    }
    //}


    #endregion

    //public enum ControlTypes
    //{
    //    None = 0,
    //    Textbox = 1,
    //    Textarea = 2,
    //    Select = 3,
    //    Checkbox = 4,
    //    Radio = 5,
    //    List = 6,
    //    MultiTextbox = 7
    //}

    //public static List<Question> Get(string header, string category)
    //{
    //    if (header == "Concept")
    //        return Question.GetConcept();
    //    else if (header == "Financials")
    //        return Question.GetFinancials(category);

    //    return new List<Question>();
    //}

    //public static List<Question> GetConcept()
    //{
    //    List<Question> questions = LoadAll().Where(q => q.Type == "Concept").ToList();
    //    return questions;
    //}

    //public static List<Question> GetFinancials(string category)
    //{
    //    List<Question> questions = LoadAll().Where(q => q.Type == "Financials" && q.Sheet == category).ToList();
    //        //.OrderBy(q => q.Sheet).ThenBy(q => q.Page).ThenBy(q => q.Section).ThenBy(q => q.Id).ToList();
    //    return questions;
    //}

    public static List<Question> Get(string header, string category, int userId = 0)
    {
        List<Question> all = new List<Question>();
        SqlConnection conn = null;
        SqlDataReader rdr = null;

        try
        {
            string connection = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            conn = new SqlConnection(connection);
            conn.Open();

            SqlCommand cmdGet = new SqlCommand("Question_Get", conn);
            cmdGet.CommandType = CommandType.StoredProcedure;
            cmdGet.Parameters.Add(new SqlParameter("@UserId", userId));
            cmdGet.Parameters.Add(new SqlParameter("@Header", header));
            cmdGet.Parameters.Add(new SqlParameter("@Sheet", category));

            rdr = cmdGet.ExecuteReader();

            int ID = rdr.GetOrdinal("Id");
            int TITLE = rdr.GetOrdinal("Title");
            int OPTIONS = rdr.GetOrdinal("Options");
            int TYPE = rdr.GetOrdinal("Type");
            int SHEET = rdr.GetOrdinal("Sheet");
            int PAGE = rdr.GetOrdinal("Page");
            int SECTION = rdr.GetOrdinal("Section");
            int SKIPCONDITION = rdr.GetOrdinal("SkipCondition");
            int ANSWERID = rdr.GetOrdinal("AnswerId");
            int TEXT = rdr.GetOrdinal("Text");

            while (rdr.Read())
            {
                Question question = new Question();
                all.Add(question);
                question.Id = rdr.IsDBNull(ID) ? 0 : rdr.GetInt32(ID);
                question.Title = rdr.IsDBNull(TITLE) ? "" : rdr.GetString(TITLE);
                question.Options = rdr.IsDBNull(OPTIONS) ? "" : rdr.GetString(OPTIONS);
                question.Type = rdr.IsDBNull(TYPE) ? "" : rdr.GetString(TYPE);
                question.Sheet = rdr.IsDBNull(SHEET) ? "" : rdr.GetString(SHEET);
                question.Page = rdr.IsDBNull(PAGE) ? "" : rdr.GetString(PAGE);
                question.Section = rdr.IsDBNull(SECTION) ? "" : rdr.GetString(SECTION);
                question.SkipCondition = rdr.IsDBNull(SKIPCONDITION) ? "" : rdr.GetString(SKIPCONDITION);
                question.Answer = new Answer();
                question.Answer.Id = rdr.IsDBNull(ANSWERID) ? 0 : rdr.GetInt32(ANSWERID);
                question.Answer.UserId = userId;
                question.Answer.QuestionId = question.Id;
                question.Answer.Text = rdr.IsDBNull(TEXT) ? "" : rdr.GetString(TEXT);
            }
        }
        finally
        {
            if (conn != null)
            {
                conn.Close();
            }
            if (rdr != null)
            {
                rdr.Close();
            }
        }

        return all;

    }

}

public static class QuestionList
{
    public static string ByTitle(this List<Question> questions, string title, string emptyValue = "")
    {
        List<Question> qs = questions.Where(q => q.Title == title).ToList();
        return qs.Count > 0 && qs[0].Answer != null && !string.IsNullOrEmpty(qs[0].Answer.Text) ? qs[0].Answer.Text : emptyValue;
    }
    public static double ByTitleSum(this List<Question> questions, string[] titles)
    {
        double sum = 0;
        List<Question> qs = questions.FindAll(delegate (Question q) { return titles.Contains(q.Title); });
        foreach(Question q in qs)
        {
            double val = 0;
            if(q != null && double.TryParse(q.Answer.Text, out val))
                sum += val;
        }
        return sum;
    }
    public static double BySectionSum(this List<Question> questions, string sectionName = "", int startIndex = 0, int count = 0)
    {
        if(!string.IsNullOrEmpty(sectionName))
            questions = questions.FindAll(delegate(Question q) { return q.Section == sectionName; });

        count = count == 0 ? questions.Count : count + startIndex;

        double sum = 0;
        for (int i = startIndex; i < count; i++)
        {
            double val;
            if (double.TryParse(questions[i].Answer.Text, out val))
                sum += val;
        }
        return sum;
    }
}
