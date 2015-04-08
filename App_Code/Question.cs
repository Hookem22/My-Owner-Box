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

    public string Help { get; set; }

    public string Type { get; set; }

    public string Sheet { get; set; }

    public string Page { get; set; }

    public string Section { get; set; }

    public string SkipCondition { get; set; }

    [NonDB]
    public Answer Answer { get; set; }

    #endregion

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
            int HELP = rdr.GetOrdinal("Help");
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
                question.Help = rdr.IsDBNull(HELP) ? "" : rdr.GetString(HELP);
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
