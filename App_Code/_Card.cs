using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Card
/// </summary>
public class _Card
{
	public _Card()
	{
	}
    public _Card(string question, _Card.ControlTypes control = 0, List<string> options = null, string skipCondition = "")
    {
        _Question q = new _Question(question);
        q.Type = _Question.QuestionType.Concept;
        q.Options = options ?? new List<string>();
        Questions = new List<_Question>() { q };
        Control = control;
        SkipCondition = skipCondition;
    }
    public _Card(string sectionHeader, string sheet, params string[] questions)
    {
        Control = ControlTypes.Multi;
        Sheet = sheet;
        Questions = _Question.NewFinancialList(sectionHeader, questions);
    }

    #region Properties

    public string CardHeader { get; set; }

    public string Sheet { get; set; }

    public List<_Question> Questions { get; set; }

    public ControlTypes Control { get; set; }
    
    public string SkipCondition { get; set; }

    public string Html
    {
        get
        {
            if (Questions.Count == 0)
                return "";
            
            string html = "";

            if (Control != ControlTypes.Multi)
            {
                if (!string.IsNullOrEmpty(Questions[0].Title))
                    html += string.Format("<div class='title'>{0}</div>", Questions[0].Title);
                //if(!string.IsNullOrEmpty(Subtitle))
                //    html += string.Format("<div class='subtitle'>{0}</div>", Subtitle);

                html += "<div class='content'>";
                switch (Control)
                {
                    case ControlTypes.Textbox:
                        if (Questions[0].Options.Count > 0)
                        {
                            string options = "";
                            foreach (string option in Questions[0].Options)
                            {
                                options += option + ", ";
                            }
                            if (!string.IsNullOrEmpty(options))
                                options = options.Substring(0, options.Length - 2);

                            html += string.Format("<input class='AnswerControl' type='text' placeholder='{0}' value='{1}' />", options, "{{Answer}}");
                        }
                        else
                        {
                            html += "<input class='AnswerControl' type='text' value='{{Answer}}' />";
                        }
                        break;
                    case ControlTypes.Textarea:
                        html += "<textarea class='AnswerControl'>{{Answer}}</textarea>";
                        html += "<div class='textarea'><ul><li style='text-align:center;margin-bottom: .5em;'>Examples: </li>";
                        foreach (string option in Questions[0].Options)
                            html += string.Format("<li>{0}</li>", option);

                        html += "</ul></div>";
                        break;
                    case ControlTypes.Select:
                        html += "<select class='AnswerControl'>";
                        foreach (string option in Questions[0].Options)
                            html += string.Format("<option>{0}</option>", option);

                        html += "</select>";
                        break;
                    case ControlTypes.Checkbox:
                        foreach (string option in Questions[0].Options)
                            html += string.Format("<input type='checkbox' class='AnswerControl' value='{0}'><div class='checkbox'>{0}</div>", option);
                        break;
                    case ControlTypes.Radio:
                        foreach (string option in Questions[0].Options)
                            html += string.Format("<input type='radio' name='AnswerControl' value='{0}'><div class='radio'>{0}</div>", option);

                        break;
                    case ControlTypes.List:
                        if (Questions[0].Options.Count > 0)
                        {
                            string options = "";
                            foreach (string option in Questions[0].Options)
                            {
                                options += option + ", ";
                            }
                            options = options.Substring(0, options.Length - 2);
                            html += string.Format("<input class='ListControl' type='text' placeholder='{0}' /><span class='glyphicon glyphicon-plus-sign' ></span>", options);
                        }
                        else
                        {
                            html += "<input class='ListControl' type='text' /><span class='glyphicon glyphicon-plus-sign'></span>";
                        }
                        break;
                }
                html += "</div>";
            }
            else
            {
                html = "";
                if(!string.IsNullOrEmpty(CardHeader))
                    html += string.Format("<h1>{0}</h1>", CardHeader);

                html += "<div class='multiTextGroup'>";
                html += string.Format("<h2>{0}</h2>", Questions[0].Header);
                for (int i = 0, ii = Questions.Count; i < ii; i++)
                {
                    if (i > 0 && Questions[i].Header != Questions[i - 1].Header)
                    {
                        html += string.Format("</div><div class='multiTextGroup divider'><h2>{0}</h2>", Questions[i].Header);
                    }
                    string options = "";
                    foreach (string option in Questions[i].Options)
                    {
                        options += option + ", ";
                    }
                    if (!string.IsNullOrEmpty(options))
                        options = options.Substring(0, options.Length - 2);

                    if (string.IsNullOrEmpty(Questions[i].Title))
                        html += "<div class='multiText' style='width:100%;'></div>";
                    else
                        html += string.Format("<div class='multiText'><span>{0}</span><input type='text' value='{1}' placeholder='{2}' /></div>", Questions[i].Title, "{{Answer}}", options);
                }
                html += "</div>";
            }
            return html;

        }
    }

    #endregion

    public enum ControlTypes
    {
        Textbox = 1,
        Textarea = 2,
        Select = 3,
        Checkbox = 4,
        Radio = 5,
        List = 6,
        Multi = 7
    }

    public static List<_Card> GetConcept()
    {
        List<_Card> cards = new List<_Card>();

        cards.Add(new _Card("How would you describe the pricing of your food?", ControlTypes.Select, new List<string> { "Inexpensive", "Moderate", "Expensive" }));
        cards.Add(new _Card("What kind of restaurant is this?", ControlTypes.Textbox, new List<string> { "Casual", "Fine Dining", "Buffet", "etc." }));
        cards.Add(new _Card("What type of food will you serve?", ControlTypes.Textbox, new List<string> { "Italian", "Tex Mex", "BBQ", "etc." }));
        cards.Add(new _Card("Will you serve alcohol?", ControlTypes.Radio, new List<string> { "Yes", "No" }));
        cards.Add(new _Card("What kind of alcohol will you serve?", ControlTypes.Checkbox, new List<string> { "Beer", "Wine", "Liquor" }, "Will you serve alcohol?|Yes"));
        cards.Add(new _Card("The service style of the restaurant will be?", ControlTypes.Textarea, new List<string> { "a table service featuring a professional wait staff", "a counter service style concept where the customer will order from the counter and the food will be delivered to the table", "a quick serve restaurant that will serve the guest immediately after ordering", "a buffet style concept with courteous wait staff to serve water and beverages", "a take out only style restaurant with no dining room" }));
        cards.Add(new _Card("How would you describe the decor?", ControlTypes.Textbox, new List<string> { "Hip", "Vintage", "Intimate", "etc." }));
        cards.Add(new _Card("What makes your restaurant different?", ControlTypes.List, new List<string> { "A couple unique selling points" }));
        cards.Add(new _Card("Who is this restaurant for?", ControlTypes.Textbox, new List<string> { "Adults", "Families", "Business clientele", "etc." }));
        cards.Add(new _Card("Will your restaurant be in a?", ControlTypes.Textbox, new List<string> { "Free standing, lease space, mall, trailer" }));
        cards.Add(new _Card("What days of the week will you be open?", ControlTypes.Textbox, new List<string> { "Monday through Sunday" }));
        cards.Add(new _Card("Which of these will you serve?", ControlTypes.Checkbox, new List<string> { "Breakfast", "Lunch", "Dinner" }));
        cards.Add(new _Card("What is the Breakfast Atmosphere?", ControlTypes.Textbox, new List<string> { "Grab and go", "Sit down", "Professional", "etc." }, "Which of these will you serve?|Breakfast"));
        cards.Add(new _Card("What is the average price for breakfast?", ControlTypes.Textbox, new List<string> { "Average Price" }, "Which of these will you serve?|Breakfast"));
        cards.Add(new _Card("What is the meal length for breakfast?", ControlTypes.Textbox, new List<string> { "Average Time" }, "Which of these will you serve?|Breakfast"));
        cards.Add(new _Card("What are a few breakfast entrees?", ControlTypes.List, new List<string> { "Bacon and Eggs", "Coffee cake", "Pancakes", "etc." }, "Which of these will you serve?|Breakfast"));
        cards.Add(new _Card("What about the lunch atmosphere?", ControlTypes.Textbox, new List<string> { "Casual", "Upscale", "Business lunch", "etc." }, "Which of these will you serve?|Lunch"));
        cards.Add(new _Card("The average lunch price?", ControlTypes.Textbox, new List<string> { "Average Price" }, "Which of these will you serve?|Lunch"));
        cards.Add(new _Card("And lunch length?", ControlTypes.Textbox, new List<string> { "Average Time" }, "Which of these will you serve?|Lunch"));
        cards.Add(new _Card("Name a couple items off your lunch menu:", ControlTypes.List, new List<string> { "Soups", "Salads", "Sandwiches" }, "Which of these will you serve?|Lunch"));
        cards.Add(new _Card("How would you describe your dinner atmosphere?", ControlTypes.Textbox, new List<string> { "Romantic", "Sports bar", "Family friendly", "etc." }, "Which of these will you serve?|Dinner"));
        cards.Add(new _Card("What about average dinner price?", ControlTypes.Textbox, new List<string> { "Average Price" }, "Which of these will you serve?|Dinner"));
        cards.Add(new _Card("What about dinner length?", ControlTypes.Textbox, new List<string> { "Average Time" }, "Which of these will you serve?|Dinner"));
        cards.Add(new _Card("What are a few of your dinner signature dishes?", ControlTypes.List, new List<string> { "Surf and Turf", "Chicken Fajitas", "Red Snapper" }, "Which of these will you serve?|Dinner"));
        cards.Add(new _Card("Name any additional services you will provide:", ControlTypes.Textbox, new List<string> { "Takeout", "drive-thru window", "catering", "delivery" }));
        
        return cards;
    }

    public static List<_Card> GetFinancial(string sheet)
    {
        return AllFinancial().FindAll(delegate(_Card c)
        {
            return c.Sheet == sheet;
        });
    }

    static List<_Card> AllFinancial()
    {
        List<_Card> cards = new List<_Card>();

        cards.Add(new _Card("Metrics", "Basic Info", "Number of Dining Seats", "Square Footage", "Projected Opening Date", "Equity Capital"));
        
        cards.Add(new _Card("Leasehold Improvements", "Capital Budget", "Construction Contract", "Landlord Contribution"));
        cards.Add(new _Card("Equipment", "Capital Budget", "Bar / Kitchen Equipment", "Bar / Dining Room Furniture"));
        cards.Add(new _Card("Professional Services", "Capital Budget", "Architect & Engineering", "Legal (lease & incorporation)", "Project Consultant", "Accounting & Tax", "Name, Logo & Graphic Design"));
        cards.Add(new _Card("Organizational and Development", "Capital Budget", "Deposits (utilities, sales tax, etc.)", "Insurance Binder (property, casualty, liability)", "Workers Comp. Binder", "Liquor License", "Building Permits", "Other Licenses & Permits", "Utility Deposits (gas, electric, water)", "Change, Operating Banks & Petty Cash", "Menus / Menu Boards", "Lease Deposit", "Travel, Research, Concept Development"));
        cards.Add(new _Card("Interior Finishes and Equipment", "Capital Budget", "Kitchen Smallwares", "Artwork & Specialty Décor", "Security System", "Music/Sound/Audio-Visual Systems", "Cash Register / Point of Sale", "Phone System", "Office Equipment / Computer", "Office Supplies", "Interior Signs"));
        cards.Add(new _Card("Exterior Finishes and Equipemnt", "Capital Budget", "Landscaping", "Exterior Signs & Decorations", "Resurfacing", "Parking Bumpers", "Parking Lot Striping"));
        cards.Add(new _Card("Pre-Opening Expenses", "Capital Budget", "Construction Period Utilities", "Construction Period Building Lease", "Construction Period Interest", "Uniforms"));
        cards[cards.Count - 1].Questions.AddRange(AddSection("Opening Inventories", "Food", "Beer, Liquor & Wine", "Paper & Other Supplies"));
        cards[cards.Count - 1].Questions.AddRange(AddSection("Marketing", "Advertising", "Public Relations", "Opening Parties"));
        cards[cards.Count - 1].Questions.AddRange(AddSection("Personnel", "Management & Chef", "Hourly Employees", "Payroll Taxes & Employee Beneifts"));
        cards.Add(new _Card("Working Capital and Contingency", "Capital Budget", "Working Capital", "Contingency"));
        
        cards.Add(new _Card("Breakfast Projections", "Sales Projection", "Entree Average Price", "Entree % Ordered", "Appetizer Average Price", "Appetizer % Ordered", "Dessert Average Price", "Dessert % Ordered"));
        cards[cards.Count - 1].Questions.AddRange(AddSection("Beverages", "Non-Alcoholic Average Price", "Non-Alcoholic Ordered", "Liquor Average Price", "Liquor % Ordered", "Beer Average Price", "Beer % Ordered", "Wine Average Price", "Wine % Ordered"));
        cards.Add(new _Card("Lunch Projections", "Sales Projection", "Entree Average Price", "Entree % Ordered", "Appetizer Average Price", "Appetizer % Ordered", "Dessert Average Price", "Dessert % Ordered"));
        cards[cards.Count - 1].Questions.AddRange(AddSection("Beverages", "Non-Alcoholic Average Price", "Non-Alcoholic Ordered", "Liquor Average Price", "Liquor % Ordered", "Beer Average Price", "Beer % Ordered", "Wine Average Price", "Wine % Ordered"));
        cards.Add(new _Card("Dinner Projections", "Sales Projection", "Entree Average Price", "Entree % Ordered", "Appetizer Average Price", "Appetizer % Ordered", "Dessert Average Price", "Dessert % Ordered"));
        cards[cards.Count - 1].Questions.AddRange(AddSection("Beverages", "Non-Alcoholic Average Price", "Non-Alcoholic Ordered", "Liquor Average Price", "Liquor % Ordered", "Beer Average Price", "Beer % Ordered", "Wine Average Price", "Wine % Ordered"));

        cards.Add(new _Card("Breakfast Table Turns", "Sales Projection", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"));
        cards.Add(new _Card("Lunch Table Turns", "Sales Projection", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"));
        cards.Add(new _Card("Dinner Table Turns", "Sales Projection", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"));
        
        cards.Add(new _Card("Dining Room", "Hourly Labor", "Server Rate", "Server Average Number of Hours", "Server Shifts per Day", "", "Host / Hostess Rate", "Host / Hostess Average Number of Hours", "Host / Hostess Shifts per Day", "", "Busser Rate", "Busser Average Number of Hours", "Busser Shifts per Day", "", "Bartender Rate", "Bartender Average Number of Hours", "Bartender Shifts per Day", ""));
        cards.Add(new _Card("Kitchen", "Hourly Labor", "Dishroom Rate", "Dishroom Average Number of Hours", "Dishroom Shifts per Day", "", "Prep Cook Rate", "Prep Cook Average Number of Hours", "Prep Cook Shifts per Day", "", "Line Cook", "Line Cook Average Number of Hours", "Line Cook Shifts per Day", "", "Expo Rate", "Expo Average Number of Hours", "Expo Shifts per Day", ""));
        
        cards.Add(new _Card("Cost of Sales", "Other Expenses", "Food Cost %", "Liquor Cost %", "Beer Cost %"));
        cards.Add(new _Card("Management Salaries (Annual)", "Other Expenses", "General Manager", "Chef / Kitchen Manager", "Assistant Manager", "Other"));
        cards.Add(new _Card("Employee Benefits", "Other Expenses", "FICA Taxes - as a % of Gross Payroll", "State Unemployment - as a % of Gross Payroll", "Federal Unemployment - as a % of Gross Payroll", "Other Payroll Taxes - as a % of Gross Payroll", "Worker's Comp. - as a % of Gross Payroll", "Group Medical Insurance - cost per employee", "Group Medical Ins.-# of employees covered", "Disability and Life Insurance", "401k Plan - per month", "Employee Meals - per month", "Employee Education - per month", "Awards and Prizes - per month", "Employee Christmas & Other Parties", "Transportation & Housing"));
        cards.Add(new _Card("Direct Operating Expenses", "Other Expenses", "Auto Expense", "Catering & Banquet Supplies", "Cleaning Supplies", "Contract Cleaning", "Extermination", "Flowers & Decorations", "Kitchen Utensils", "Laundry & Linen", "Licenses & Permits", "Menus & Wine Lists", "Miscellaneous", "Paper Supplies (enter monthly amount or cost as a % of food sales)", "Security System", "Tableware & Smallwares", "Uniforms"));
        cards.Add(new _Card("Music & Entertainment", "Other Expenses", "Musicians", "Sound System", "Other"));
        cards[cards.Count - 1].Questions.AddRange(AddSection("Marketing", "Selling & Promotions", "Advertising", "Printed Materials", "Research"));
        cards[cards.Count - 1].Questions.AddRange(AddSection("Utilities", "Electricity", "Gas", "Water", "Trash Removal"));
        cards.Add(new _Card("General & Administrative", "Other Expenses", "Accounting Services", "Bank Charges", "Bank Deposit Services", "Cash (Over) / Short", "Credit Card Charges", "Dues & Subscriptions", "Miscellaneous", "Office Supplies", "Payroll Processing", "Postage", "Professional Fees", "Protective Services", "Telephone", "Training Materials"));
        cards.Add(new _Card("Credit Card Charges", "Other Expenses", "Percentages of Credit Card Sales", "Average Discount Percentage"));
        cards.Add(new _Card("Repairs and Maintenance", "Other Expenses", "Building Repairs and Maintenance", "Equipment Repairs & Maintenance", "Grounds, Landscaping and Parking Lot"));
        cards.Add(new _Card("Occupancy Costs", "Other Expenses", "Base (minimum) Rent", "Percentage Rent - Percentage amount", "Percentage Rent - On annual sales above", "Common Area Maintenance (CAM)", "Equipment Rental", "Real Estate Taxes", "Personal Property Taxes", "Insurance on Building & Contents", "Liquor Liability"));
        cards.Add(new _Card("Loan Financing", "Other Expenses", "Rate %", "Term (years)"));

        int i = 0;
        foreach(_Card card in cards)
        {
            foreach(_Question question in card.Questions)
            {
                question.Id = i;
                i++;
            }
        }

        return cards;
    }

    static List<_Question> AddSection(string sectionHeader, params string[] questions)
    {
        return _Question.NewFinancialList(sectionHeader, questions);
    }
}