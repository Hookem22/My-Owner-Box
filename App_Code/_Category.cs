using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Financial
/// </summary>
public class _Category : BaseClass<_Category>
{
	public _Category()
	{
    }
    public _Category(string name, params string[] questions)
    {
        Name = name;
        Cards = new List<_Card>() { new _Card() };
        foreach (string question in questions)
        {
            Cards[0].Questions.Add(new _Question(question));
        }
    }
    //public Category(string question, Card.ControlTypes control = 0, List<string> options = null, string skipCondition = "")
    //{
    //    Name = "";
    //    Question q = new Question(question);
    //    Card card = new Card("", new List<Question>() { q }, control, skipCondition);
    //    Cards = new List<Card>() { card };
    //}
    #region Properties

    public string Name { get; set; }

    public List<_Card> Cards { get; set; }

    #endregion

    //public static string GetConceptTemplate(List<_Question> questions)
    //{
    //    string html = ConceptTemplate;

    //    string replace = questions.ByTitle("Will you serve alcohol?") == "Yes" ? ConceptAlcohol : "";
    //    html = html.Replace("[Alcohol]", replace);

    //    replace = questions.ByTitle("Which of these will you serve?").Contains("Breakfast") ? ConceptBreakfast : "";
    //    html += replace;

    //    replace = questions.ByTitle("Which of these will you serve?").Contains("Lunch") ? ConceptLunch : "";
    //    html += replace;

    //    replace = questions.ByTitle("Which of these will you serve?").Contains("Dinner") ? ConceptDinner : "";
    //    html += replace;

    //    html = html.Replace("{Name of Restaurant}", "Bob's Restaurant");

    //    //foreach (Question question in questions)
    //    //{
    //    //    if (!string.IsNullOrEmpty(question.Answer))
    //    //    {
    //    //        if ((question.Control == Card.ControlTypes.Checkbox || question.Control == Card.ControlTypes.List) && question.Answer.Contains(", "))
    //    //        {
    //    //            string[] items = question.Answer.Split(',');
    //    //            if (items.Length == 2)
    //    //            {
    //    //                question.Answer = items[0].Trim() + " and " + items[1].Trim();
    //    //            }
    //    //            else
    //    //            {
    //    //                question.Answer = "";
    //    //                for (int i = 0; i < items.Length; i++)
    //    //                {
    //    //                    if (i > 0 && i == items.Length - 1)
    //    //                        question.Answer += "and " + items[i] + ", ";
    //    //                    else
    //    //                        question.Answer += items[i] + ", ";
    //    //                }
    //    //                question.Answer = question.Answer.Substring(0, question.Answer.Length - 2);
    //    //            }
    //    //        }
    //    //        html = html.Replace("{" + question.Title + "}", question.Answer.ToLower());
    //    //    }
    //    //}

    //    return html;
    //}

    #region Concept Templates

    static string ConceptTemplate = "{Name of Restaurant} is going to be a {How would you describe the pricing of your food?}, {What kind of restaurant is this?} style restaurant offering  {What type of food will you serve?}. [Alcohol] The food will be served with {The service style of the restaurant will be?}. The decor for {Name of Restaurant} can be described as {How would you describe the decor?}. The furnishings will reflect the projected image of the décor and restaurant concept.<br/><br/>A few of our unique selling points for {Name of Restaurant} include {What makes your restaurant different?}.<br/><br/>{Name of Restaurant} will reside in a {Will your restaurant be in a?} and be open {What days of the week will you be open?} during the week. The restaurant anticipates serving during the {Which of these will you serve?} meal periods. The expected hours of operation are as follows:<br/><br/>This type of restaurant will be seen as a {What kind of restaurant is this?} institution that appeals to {Who is this restaurant for?}.";
    static string ConceptAlcohol = "We will also serve {What kind of alcohol will you serve?}.";
    static string ConceptBreakfast = "Breakfast will have a {What is the Breakfast Atmosphere?} and will serve key items like {What are a few breakfast entrees?}. The average meal price for breakfast will be {What is the average price for breakfast?}. The average length of the dining experience at breakfast should be {What is the meal length for breakfast?}.";
    static string ConceptLunch = "<br/><br/>Lunch will offer menu choices like {Name a couple items off your lunch menu:} in a {What about the lunch atmosphere?} atmosphere. Meal prices for lunch are expected to average a {The average lunch price?} price point and will last approximately {And lunch length?}.";
    static string ConceptDinner = "<br/><br/>Finally, Dinner will serve guests signature dishes such as {What are a few of your dinner signature dishes?} at an average price point of {What about average dinner price?} with a {How would you describe your dinner atmosphere?} feel. Our belief is that each meal will last on average {What about dinner length?}.";

    #endregion

}
