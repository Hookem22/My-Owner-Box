using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Answer
/// </summary>
public class Answer : BaseClass<Answer>
{
	public Answer()
	{
    }

    #region Properties

    public int QuestionId { get; set; }

    public int UserId { get; set; }

    public string Text { get; set; }

    #endregion

}