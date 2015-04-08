using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for WordDoc
/// </summary>
public class WordDoc
{
    public WordDoc()
    {
    }

    static int currentUserId = 1;
    static double[] TotalSalesPerDay = new double[7];
    static int[] CoversPerDay = new int[7];
    static double WeeklyFood = 0;
    static double WeeklyLiquor = 0;
    static double WeeklyBeer = 0;
    static double WeeklyWine = 0;
    static double YearlyTotalSales = 0;
    static double YearlyHourlyCosts = 0;
    static double[] FiveYearSales = new double[5];
    static double[] FiveYearCashFlow = new double[5];
    static double Principal = 0;

    public static void PrintConcept(MemoryStream mem)
    {
        // Create Document
        using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(mem,
        WordprocessingDocumentType.Document, true))
        {
            // Add a main document part. 
            MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

            // Create the document structure and add some text.
            mainPart.Document = new Document();
            Body body = mainPart.Document.AppendChild(new Body());

            body.AddHeader("Restaurant Concept");

            //Concept deleted, get from questions
            //foreach(string text in Concept.GetDoc())
            //{
            //    body.AddParagraph(text);
            //}
            

        }
    }

    public static void Financials(MemoryStream mem)
    {
        using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(mem, WordprocessingDocumentType.Document, true))
        {
            MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

            // Create the document structure and add some text.
            mainPart.Document = new Document();
            Body body = mainPart.Document.AppendChild(new Body());

            FinancialsOverview(wordDocument);
            body.AddPageBreak();
            FinancialsCapitalBudget(wordDocument);
            body.AddPageBreak();
            FinancialsSalesProjection(wordDocument);
            body.AddPageBreak();
            FinancialsHourlyLabor(wordDocument);
            body.AddPageBreak();
            FinancialsIncomeDetailed(wordDocument);
            body.AddPageBreak();
            FinancialsIncomeSummary(wordDocument);
            body.AddPageBreak();
            FinancialsIncome5Year(wordDocument);
            body.AddPageBreak();
            FinancialsInvestmentReturn(wordDocument);
            body.AddPageBreak();
            FinancialsBreakEven(wordDocument);

        }
    }

    public static void FinancialsOverview(WordprocessingDocument wordDocument)
    {
        Body body = wordDocument.MainDocumentPart.Document.Body;

        body.AddHeader("Financials Overview");
        
        Table tbl = NewTable(body, 3);

        string[] detailStyle = new string[] { "LeftIndent:400", "JustifyRight|RightIndent:1000" };
        string[] totalStyle = new string[] { "Bold|Background:ABCDEF", "Background:ABCDEF", "Background:ABCDEF|JustifyRight|RightIndent:1000|Bold" };

        List<Question> questions = Question.Get("Financials", "Capital Budget", currentUserId);
        double totalSum = 0;
        double sectionSum = 0;
        List<string[]> usesRows = new List<string[]>();
        for (int i = 0, ii = questions.Count; i < ii; i++)
        {
            int val = 0;
            if (int.TryParse(questions[i].Answer.Text, out val))
            {
                totalSum += val;
                sectionSum += val;
            }
            string[] preOpening = new string[] { "Pre-Opening Expenses", "Opening Inventories", "Marketing" };
            if (i >= questions.Count - 1 || (questions[i].Section != questions[i + 1].Section && !preOpening.Contains(questions[i].Section)))
            {
                if (questions[i].Section == "Personnel")
                    questions[i].Section = "Pre-Opening Expenses";
                usesRows.Add(new string[] { questions[i].Section, sectionSum.ToString("#,##0;(#,##0)"), "" });
                sectionSum = 0;
            }
        }

        List<Question> basicInfo = Question.Get("Financials", "Basic Info", currentUserId);
        double equityCapital = basicInfo.ByTitleSum(new string[] { "Equity Capital" });
        
        tbl.AddRow(new string[] { "SOURCES OF CASH", "", "" }, new string[] { "Bold" });
        tbl.AddRow(new string[] { "Equity Contributions", equityCapital.ToString("#,##0;(#,##0)"), "" }, detailStyle);
        Principal = totalSum - equityCapital;
        tbl.AddRow(new string[] { "Loan Financing", Principal.ToString("#,##0;(#,##0)"), "" }, detailStyle);
        tbl.AddRow(new string[] { "TOTAL SOURCES OF CASH", "", totalSum.ToString("#,##0;(#,##0)") }, totalStyle);
        tbl.AddRow(new string[] { "", "", "" });

        tbl.AddRow(new string[] { "USES OF CASH", "", "" }, new string[] { "Bold" });
        foreach (string[] row in usesRows)
        {
            tbl.AddRow(row, detailStyle);
        }

        tbl.AddRow(new string[] { "TOTAL USES OF CASH", "", totalSum.ToString("#,##0;(#,##0)") }, totalStyle);

    }

    public static void FinancialsCapitalBudget(WordprocessingDocument wordDocument)
    {       
        Body body = wordDocument.MainDocumentPart.Document.Body;

        body.AddHeader("Capital Budget");

        Table tbl = NewTable(body, 3);

        string[] header = new string[] { "Bold", "JustifyRight|RightIndent:600", "JustifyRight|RightIndent:600" };
        string[] detailOdd = new string[] { "LeftIndent:400|Background:ABCDEF", "Background:ABCDEF|JustifyRight|RightIndent:600", "Background:ABCDEF|JustifyRight|RightIndent:600" };
        string[] detailEven = new string[] { "LeftIndent:400", "JustifyRight|RightIndent:600", "JustifyRight|RightIndent:600" };
        string[] sumStyle = new string[] { "", "", "Bold" };
        bool odd = true;
        int sum = 0;

        List<Question> questions = Question.Get("Financials", "Capital Budget", currentUserId);
        for (int i = 0, ii = questions.Count; i < ii; i++)
        {
            if(i == 0)
            {
                tbl.AddRow(new string[] { questions[i].Section, "", "" }, header);
            }
            else if (questions[i].Section != questions[i - 1].Section)
            {
                tbl.AddRow(new string[] { "", "", sum.ToString("#,##0;(#,##0)") }, sumStyle, false);
                sum = 0;
                tbl.AddRow(new string[] { "", "", "" });
                tbl.AddRow(new string[] { questions[i].Section, "", "" }, header);
                odd = true;
            }
            int val = 0;
            if (int.TryParse(questions[i].Answer.Text, out val))
            {
                if (questions[i].Title == "Landlord Contribution")
                    val *= -1;
                questions[i].Answer.Text = val.ToString("#,###;(#,###)");
                sum += val;
            }
            else
            {
                questions[i].Answer.Text = "0";
            }
            tbl.AddRow(new string[] { questions[i].Title, questions[i].Answer.Text, "" }, odd ? detailOdd : detailEven);

            odd = !odd;
        }
        tbl.AddRow(new string[] { "", "", sum.ToString("#,##0;(#,##0)") }, sumStyle, false);
    }

    public static void FinancialsSalesProjection(WordprocessingDocument wordDocument)
    {
        Body body = wordDocument.MainDocumentPart.Document.Body;
        
        body.AddHeader("Sales Projections - Average Check Price");

        //if breakfast
        double [] breakfastSums = AverageCheck(body, "Breakfast");
        body.AddLineBreak();
        //if lunch
        double[] lunchSums = AverageCheck(body, "Lunch");
        body.AddLineBreak();
        //if dinner
        double[] dinnerSums = AverageCheck(body, "Dinner");
        
        body.AddPageBreak();

        TableTurns(body, breakfastSums, lunchSums, dinnerSums);

    }

    static double[] AverageCheck(Body body, string meal)
    {
        int offset = meal == "Lunch" ? 12 : meal == "Dinner" ? 26 : 0;
        Table tbl = NewTable(body, 6);

        string[] header = new string[] { "FontSize:36|Bold|JustifyCenter|Background:2B579A|FontColor:FFFFFF", "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF" };
        string[] sumStyle = new string[] { "Bold", "JustifyCenter|Bold|Borders:Top", "JustifyCenter|Bold|Borders:Top", "JustifyCenter|Bold|Borders:Top", "JustifyCenter|Bold|Borders:Top", "JustifyCenter|Bold|Borders:Top" };
        double foodSum = 0;
        double liquorSum = 0;
        double beerSum = 0;
        double wineSum = 0;

        List<Question> questions = Question.Get("Financials", "Sales Projection", currentUserId);
        //if breakfast
        tbl.AddRow(new string[] { meal, "Average|Price Point", "% Ordered", "Average|Food", "Average|Beverage", "Average|Check" }, header);
        tbl.AddRow(new string[] { "Food" }, new string[] { "Bold" });
        foodSum += AddCheckPrice(tbl, "Entree", questions[offset].Answer.Text, questions[offset + 1].Answer.Text, true);
        foodSum += AddCheckPrice(tbl, "Appetizer", questions[offset + 2].Answer.Text, questions[offset + 3].Answer.Text, true);
        if (meal != "Breakfast")
            foodSum += AddCheckPrice(tbl, "Dessert", questions[offset + 4].Answer.Text, questions[offset + 5].Answer.Text, true);
        else
            offset -= 2;
        tbl.AddRow(new string[] { "Beverage" }, new string[] { "Bold" });
        foodSum += AddCheckPrice(tbl, "Non-Alcoholic", questions[offset + 6].Answer.Text, questions[offset + 7].Answer.Text, true);
        liquorSum += AddCheckPrice(tbl, "Liquor", questions[offset + 8].Answer.Text, questions[offset + 9].Answer.Text, false);
        beerSum += AddCheckPrice(tbl, "Beer", questions[offset + 10].Answer.Text, questions[offset + 11].Answer.Text, false);
        wineSum += AddCheckPrice(tbl, "Wine", questions[offset + 12].Answer.Text, questions[offset + 13].Answer.Text, false);
        double bevSum = liquorSum + beerSum + wineSum;
        tbl.AddRow(new string[] { "TOTALS", "", "", foodSum.ToString("0.00"), bevSum.ToString("0.00"), (foodSum + bevSum).ToString("0.00") }, sumStyle);

        return new double[] { foodSum, liquorSum, beerSum, wineSum };
    }

    static double AddCheckPrice(Table tbl, string rowName, string s_price, string s_ordered, bool isFood)
    {
        string[] detail = new string[] { "LeftIndent:400", "JustifyCenter", "JustifyCenter", "JustifyCenter", "JustifyCenter", "JustifyCenter", "JustifyCenter" };
        double price = 0;
        double ordered = 0;
        double.TryParse(s_price, out price);
        double.TryParse(s_ordered, out ordered);
        if(isFood)
            tbl.AddRow(new string[] { rowName, price.ToString("0.00"), ordered.ToString() + "%", (price * ordered / 100).ToString("0.00") }, detail);
        else
            tbl.AddRow(new string[] { rowName, price.ToString("0.00"), ordered.ToString() + "%", "",  (price * ordered / 100).ToString("0.00") }, detail);

        return (price * ordered / 100);
    }

    static void TableTurns(Body body, double[] breakfastSums, double[] lunchSums, double[] dinnerSums)
    {
        body.AddHeader("Sales Projections - Typical Week");

        Table tbl = NewTable(body, 9);

        string[] header = new string[] { "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF" };
        string[] style = new string[] { "JustifyCenter|Bold", "Bold", "JustifyRight|RightIndent:300", "JustifyRight|RightIndent:300", "JustifyRight|RightIndent:300", "JustifyRight|RightIndent:300", "JustifyRight|RightIndent:300", "JustifyRight|RightIndent:300", "JustifyRight|RightIndent:300" };
        string[] sumStyle = new string[] { "JustifyCenter|Bold", "Bold", "JustifyRight|RightIndent:300|Borders:Top", "JustifyRight|RightIndent:300|Borders:Top", "JustifyRight|RightIndent:300|Borders:Top", "JustifyRight|RightIndent:300|Borders:Top", "JustifyRight|RightIndent:300|Borders:Top", "JustifyRight|RightIndent:300|Borders:Top", "JustifyRight|RightIndent:300|Borders:Top|Bold" };

        double foodSum = 0;
        double bevSum = 0;

        List<Question> questions = Question.Get("Financials", "Sales Projection", currentUserId);
        tbl.AddRow(new string[] { "", "", "Table|Turns", "Covers", "Food", "Liquor", "Beer", "Wine", "Total" }, header);
        
        double numberSeats = 150;
        int totalCovers = 0;

        WeeklyFood = 0;
        WeeklyLiquor = 0;
        WeeklyBeer = 0;
        WeeklyWine = 0;

        for (int i = 0; i < 7; i++)
        {
            string day = GetDay(i);

            double turns = 0;
            double.TryParse(questions[i + 40].Answer.Text, out turns);
            double covers = Math.Floor(turns * numberSeats);
            totalCovers += (int)covers;
            double breakfastFood = covers * breakfastSums[0];
            double breakfastLiquor = covers * breakfastSums[1];
            double breakfastBeer = covers * breakfastSums[2];
            double breakfastWine = covers * breakfastSums[3];
            tbl.AddRow(new string[] { "", "Breakfast", turns.ToString(), covers.ToString(), breakfastFood.ToString("#,##0;(#,##0)"), breakfastLiquor.ToString("#,##0;(#,##0)"), breakfastBeer.ToString("#,##0;(#,##0)"), breakfastWine.ToString("#,##0;(#,##0)"), (breakfastFood + breakfastLiquor + breakfastBeer + breakfastWine).ToString("#,##0;(#,##0)") }, style);

            double.TryParse(questions[i + 47].Answer.Text, out turns);
            covers = Math.Floor(turns * numberSeats);
            totalCovers += (int)covers;
            double lunchFood = covers * lunchSums[0];
            double lunchLiquor = covers * lunchSums[1];
            double lunchBeer = covers * lunchSums[2];
            double lunchWine = covers * lunchSums[3];
            tbl.AddRow(new string[] { day, "Lunch", turns.ToString(), covers.ToString(), lunchFood.ToString("#,##0;(#,##0)"), lunchLiquor.ToString("#,##0;(#,##0)"), lunchBeer.ToString("#,##0;(#,##0)"), lunchWine.ToString("#,##0;(#,##0)"), (lunchFood + lunchLiquor + lunchBeer + lunchWine).ToString("#,##0;(#,##0)") }, style);

            double.TryParse(questions[i + 54].Answer.Text, out turns);
            covers = Math.Floor(turns * numberSeats);
            totalCovers += (int)covers;
            double dinnerFood = covers * dinnerSums[0];
            double dinnerLiquor = covers * dinnerSums[1];
            double dinnerBeer = covers * dinnerSums[2];
            double dinnerWine = covers * dinnerSums[3];
            tbl.AddRow(new string[] { "", "Dinner", turns.ToString(), covers.ToString(), dinnerFood.ToString("#,##0;(#,##0)"), dinnerLiquor.ToString("#,##0;(#,##0)"), dinnerBeer.ToString("#,##0;(#,##0)"), dinnerWine.ToString("#,##0;(#,##0)"), (dinnerFood + dinnerLiquor + dinnerBeer + dinnerWine).ToString("#,##0;(#,##0)") }, style);

            WeeklyFood += breakfastFood + lunchFood + dinnerFood;
            WeeklyLiquor += breakfastLiquor + lunchLiquor + dinnerLiquor;
            WeeklyBeer += breakfastBeer + lunchBeer + dinnerBeer;
            WeeklyWine += breakfastWine + lunchWine + dinnerWine;

            double totalDay = breakfastFood + lunchFood + dinnerFood + breakfastLiquor + lunchLiquor + dinnerLiquor + breakfastBeer + lunchBeer + dinnerBeer + breakfastWine + lunchWine + dinnerWine;
            tbl.AddRow(new string[] { "", "TOTALS", "", "", (breakfastFood + lunchFood + dinnerFood).ToString("#,##0;(#,##0)"), (breakfastLiquor + lunchLiquor + dinnerLiquor).ToString("#,##0;(#,##0)"), (breakfastBeer + lunchBeer + dinnerBeer).ToString("#,##0;(#,##0)"), (breakfastWine + lunchWine + dinnerWine).ToString("#,##0;(#,##0)"), totalDay.ToString("#,##0;(#,##0)") }, sumStyle);
            tbl.AddRow(new string[] { ""});

            CoversPerDay[i] = totalCovers;
            totalCovers = 0;
            TotalSalesPerDay[i] = totalDay;
        }

        string[] totalStyle = new string[] { "JustifyCenter|Bold|FontSize:26|Background:ABCDEF", "Background:ABCDEF", "Background:ABCDEF", "Background:ABCDEF", "JustifyRight|RightIndent:100|FontSize:26|Background:ABCDEF", "JustifyRight|RightIndent:100|FontSize:26|Background:ABCDEF", "JustifyRight|RightIndent:100|FontSize:26|Background:ABCDEF", "JustifyRight|RightIndent:100|FontSize:26|Background:ABCDEF", "JustifyRight|RightIndent:100|Bold|FontSize:26|Background:ABCDEF" };
        tbl.AddRow(new string[] { "TOTALS", "", "", "", WeeklyFood.ToString("#,##0;(#,##0)"), WeeklyLiquor.ToString("#,##0;(#,##0)"), WeeklyBeer.ToString("#,##0;(#,##0)"), WeeklyWine.ToString("#,##0;(#,##0)"), (WeeklyFood + WeeklyLiquor + WeeklyBeer + WeeklyWine).ToString("#,##0;(#,##0)") }, totalStyle);
    
        //TODO: Add key Sales stats
    }

    static void FinancialsHourlyLabor(WordprocessingDocument wordDocument)
    {
        Body body = wordDocument.MainDocumentPart.Document.Body;

        Table tbl = NewTable(body, 17);

        string[] firstStyle = new string[] { "VerticalText:1500|Bold", "VerticalText:1500|Bold", "VerticalText:1500|TopIndent:200", "VerticalText:1500|Background:ABCDEF|TopIndent:200", "VerticalText:1500|TopIndent:200", "VerticalText:1500|Background:ABCDEF|TopIndent:200", "VerticalText:1500|Bold|FontSize:26", "VerticalText:1500|Background:ABCDEF", "VerticalText", "VerticalText:1500|Background:ABCDEF", "VerticalText", "VerticalText:1500|Background:ABCDEF", "VerticalText:1500|Bold|FontSize:26", "VerticalText:1500|Bold", "VerticalText:1500|Bold", "VerticalText:1500|Bold", "VerticalText:1500|Bold" };
        string[] secondStyle = new string[] { "VerticalText:650|Bold|JustifyCenter", "VerticalText:650|Bold|JustifyCenter", "VerticalText:650|Borders:Top:Left:Bottom|JustifyCenter", "VerticalText:650|Background:ABCDEF|Borders:Top:Bottom|JustifyCenter", "VerticalText:650|Borders:Top:Bottom|JustifyCenter", "VerticalText:650|Background:ABCDEF|Borders:Top:Bottom|JustifyCenter", "VerticalText:650|Borders:Top:Bottom|JustifyCenter", "VerticalText:650|Background:ABCDEF|Borders:Top:Bottom|JustifyCenter", "VerticalText:650|Borders:Top:Bottom|JustifyCenter", "VerticalText:650|Background:ABCDEF|Borders:Top:Bottom|JustifyCenter", "VerticalText:650|Borders:Top:Bottom|JustifyCenter", "VerticalText:650|Background:ABCDEF|Borders:Top:Bottom:Right|JustifyCenter", "VerticalText:650|Bold|JustifyCenter", "VerticalText:650", "VerticalText:650", "VerticalText:650", "VerticalText:650|VerticalMerge:Restart|JustifyCenter|FontSize:44" };
        string[] style = new string[] { "VerticalText:650|Bold|JustifyCenter|VerticalMerge:Restart", "VerticalText:650|Bold|JustifyCenter|VerticalMerge:Restart", "VerticalText:650|JustifyCenter|Borders:Left", "VerticalText:650|Background:ABCDEF|JustifyCenter", "VerticalText:650|JustifyCenter", "VerticalText:650|Background:ABCDEF|JustifyCenter", "VerticalText:650|Bold|JustifyCenter", "VerticalText:650|Background:ABCDEF|JustifyCenter", "VerticalText:650|JustifyCenter", "VerticalText:650|Background:ABCDEF|JustifyCenter", "VerticalText:650|JustifyCenter", "VerticalText:650|Background:ABCDEF|JustifyCenter", "VerticalText:650|JustifyCenter|Bold|Borders:Left", "VerticalText:650|JustifyCenter|VerticalMerge:Restart", "VerticalText:650|JustifyCenter|VerticalMerge:Restart", "VerticalText:650|Bold|JustifyCenter|VerticalMerge:Restart", "VerticalText:650|JustifyCenter|VerticalMerge:Continue" };
        string[] borderStyle = new string[] { "VerticalText:650|Bold|JustifyCenter|VerticalMerge:Continue", "VerticalText:650|Bold|JustifyCenter|VerticalMerge:Continue", "VerticalText:650|JustifyCenter|Borders:Left:Bottom", "VerticalText:650|Background:ABCDEF|JustifyCenter|Borders:Bottom", "VerticalText:650|JustifyCenter|Borders:Bottom", "VerticalText:650|Background:ABCDEF|JustifyCenter|Borders:Bottom", "VerticalText:650|Bold|JustifyCenter|Borders:Bottom", "VerticalText:650|Background:ABCDEF|JustifyCenter|Borders:Bottom", "VerticalText:650|JustifyCenter|Borders:Bottom", "VerticalText:650|Background:ABCDEF|JustifyCenter|Borders:Bottom", "VerticalText:650|JustifyCenter|Borders:Bottom", "VerticalText:650|Background:ABCDEF|JustifyCenter|Borders:Bottom:Right", "VerticalText:650|JustifyCenter|Bold", "VerticalMerge:Continue", "VerticalMerge:Continue", "VerticalMerge:Continue", "VerticalMerge:Continue" };
        string[] lastStyle = new string[] { "VerticalText:900|Bold|JustifyCenter", "VerticalText:900|Bold|JustifyCenter", "VerticalText:900|JustifyCenter|Borders:Left:Bottom", "VerticalText:900|Background:ABCDEF|JustifyCenter|Borders:Bottom", "VerticalText:900|JustifyCenter|Borders:Bottom", "VerticalText:900|Background:ABCDEF|JustifyCenter|Borders:Bottom", "VerticalText:900|Bold|JustifyCenter|Borders:Bottom", "VerticalText:900|Background:ABCDEF|JustifyCenter|Borders:Bottom", "VerticalText:900|JustifyCenter|Borders:Bottom", "VerticalText:900|Background:ABCDEF|JustifyCenter|Borders:Bottom", "VerticalText:900|JustifyCenter|Borders:Bottom", "VerticalText:900|Background:ABCDEF|JustifyCenter|Borders:Bottom:Right", "VerticalText:900|JustifyCenter|Bold", "VerticalText:900|JustifyCenter|Bold", "VerticalText:900|JustifyCenter|Bold", "VerticalText:900|JustifyCenter|Bold", "VerticalMerge:Restart" };

        List<Question> questions = Question.Get("Financials", "Hourly Labor", currentUserId);
        string serverRate = questions.ByTitle("Server Rate", "0.00");
        string serverHours = questions.ByTitle("Server Average Number of Hours", "0");
        string serverShifts = questions.ByTitle("Server Shifts per Day", "0");
        double serverCost = (double.Parse(serverRate) * double.Parse(serverHours) * double.Parse(serverShifts));

        string hostRate = questions.ByTitle("Host / Hostess Rate", "0.00");
        string hostHours = questions.ByTitle("Host / Hostess Average Number of Hours", "0");
        string hostShifts = questions.ByTitle("Host / Hostess Shifts per Day", "0");
        double hostCost = (double.Parse(hostRate) * double.Parse(hostHours) * double.Parse(hostShifts));

        string busserRate = questions.ByTitle("Busser Rate", "0.00");
        string busserHours = questions.ByTitle("Busser Average Number of Hours", "0");
        string busserShifts = questions.ByTitle("Busser Shifts per Day", "0");
        double busserCost = (double.Parse(busserRate) * double.Parse(busserHours) * double.Parse(busserShifts));

        string bartenderRate = questions.ByTitle("Bartender Rate", "0.00");
        string bartenderHours = questions.ByTitle("Bartender Average Number of Hours", "0");
        string bartenderShifts = questions.ByTitle("Bartender Shifts per Day", "0");
        double bartenderCost = (double.Parse(bartenderRate) * double.Parse(bartenderHours) * double.Parse(bartenderShifts));

        string dishroomRate = questions.ByTitle("Dishroom Rate", "0.00");
        string dishroomHours = questions.ByTitle("Dishroom Average Number of Hours", "0");
        string dishroomShifts = questions.ByTitle("Dishroom Shifts per Day", "0");
        double dishroomCost = (double.Parse(dishroomRate) * double.Parse(dishroomHours) * double.Parse(dishroomShifts));

        string prepCookRate = questions.ByTitle("Prep Cook Rate", "0.00");
        string prepCookHours = questions.ByTitle("Prep Cook Average Number of Hours", "0");
        string prepCookShifts = questions.ByTitle("Prep Cook Shifts per Day", "0");
        double prepCookCost = (double.Parse(prepCookRate) * double.Parse(prepCookHours) * double.Parse(prepCookShifts));

        string lineCookRate = questions.ByTitle("Line Cook Rate", "0.00");
        string lineCookHours = questions.ByTitle("Line Cook Average Number of Hours", "0");
        string lineCookShifts = questions.ByTitle("Line Cook Shifts per Day", "0");
        double lineCookCost = (double.Parse(lineCookRate) * double.Parse(lineCookHours) * double.Parse(lineCookShifts));

        string expoRate = questions.ByTitle("Expo Rate", "0.00");
        string expoHours = questions.ByTitle("Expo Average Number of Hours", "0");
        string expoShifts = questions.ByTitle("Expo Shifts per Day", "0");
        double expoCost = (double.Parse(expoRate) * double.Parse(expoHours) * double.Parse(expoShifts));
        
        //TODO: Missing Cashiers
        double laborCost = serverCost + hostCost + busserCost + bartenderCost + dishroomCost + prepCookCost + lineCookCost + expoCost;


        tbl.AddRow(new string[] { "Labor Cost %", "Labor Cost", "Expo", "Line Cooks", "Prep Cooks", "Dishroom", "Kitchen", "Cashiers", "Bartenders", "Bussers", "Host / Hostess", "Servers", "Dining Room", "Total Sales", "Covers", "", "" }, firstStyle, false);
        tbl.AddRow(new string[] { "", "", expoRate, lineCookRate, prepCookRate, dishroomRate, "", "0.00", bartenderRate, busserRate, hostRate, serverRate, "Rate", "", "", "", "Hourly Labor Projections" }, secondStyle, false);

        for (int i = 0; i < 7; i++)
        {
            tbl.AddRow(new string[] { ToPercent(laborCost * 100 / TotalSalesPerDay[i]), laborCost.ToString("#,##0"), expoHours, lineCookHours, prepCookHours, dishroomHours, "", "0", bartenderHours, busserHours, hostHours, serverHours, "Hours", TotalSalesPerDay[i].ToString("#,##0"), CoversPerDay[i].ToString("#,##0"), GetDay(i), "" }, style, false);
            tbl.AddRow(new string[] { "", "", expoShifts, lineCookShifts, prepCookShifts, dishroomShifts, "", "0", bartenderShifts, busserShifts, hostShifts, serverShifts, "Shifts", "", "", "", "" }, borderStyle, false);
        }

        double totalSales = 0;
        foreach (double sale in TotalSalesPerDay) totalSales += sale;
        int totalCovers = 0;
        foreach (int cover in CoversPerDay) totalCovers += cover;
        YearlyHourlyCosts = laborCost * 7 * 52;
        tbl.AddRow(new string[] { ToPercent(laborCost * 700 / totalSales), (laborCost * 7).ToString("#,##0"), (expoCost * 7).ToString("#,##0"), (lineCookCost * 7).ToString("#,##0"), (prepCookCost * 7).ToString("#,##0"), (dishroomCost * 7).ToString("#,##0"), "", "0", (bartenderCost * 7).ToString("#,##0"), (busserCost * 7).ToString("#,##0"), (hostCost * 7).ToString("#,##0"), (serverCost * 7).ToString("#,##0"), "Labor", totalSales.ToString("#,##0"), totalCovers.ToString("#,###"), "WEEK", "" }, lastStyle, false);


    }

    static void FinancialsIncomeDetailed(WordprocessingDocument wordDocument)
    {
        Body body = wordDocument.MainDocumentPart.Document.Body;

        body.AddHeader("Detailed Income and Cash Flow");

        Table tbl = NewTable(body, 5);

        List<Question> questions = Question.Get("Financials", "Expenses", currentUserId);
        
        string[] headerStyle = new string[] { "LeftIndent:200|Bold" };
        string[] detailStyle = new string[] { "LeftIndent:600", "JustifyRight|RightIndent:600", "JustifyRight|RightIndent:600", "JustifyRight|RightIndent:600", "JustifyRight|RightIndent:600" };
        string[] detailStyle2 = new string[] { "LeftIndent:1000", "JustifyRight|RightIndent:600", "JustifyRight|RightIndent:600", "JustifyRight|RightIndent:600", "JustifyRight|RightIndent:600" };
        string[] totalStyle = new string[] { "LeftIndent:200", "Borders:Top|JustifyRight|RightIndent:600", "Borders:Top|JustifyRight|RightIndent:600", "Borders:Top|JustifyRight|RightIndent:600", "Borders:Top|JustifyRight|RightIndent:600" };
        string[] totalStyle2 = new string[] { "LeftIndent:600", "Borders:Top|JustifyRight|RightIndent:600", "Borders:Top|JustifyRight|RightIndent:600", "Borders:Top|JustifyRight|RightIndent:600", "Borders:Top|JustifyRight|RightIndent:600" };
        string[] sectionTotal = new string[] { "Bold|LeftIndent:200|Background:ABCDEF|FontSize:26", "JustifyRight|RightIndent:600|Background:ABCDEF", "JustifyRight|RightIndent:600|Background:ABCDEF", "JustifyRight|RightIndent:600|Background:ABCDEF", "JustifyRight|RightIndent:600|Background:ABCDEF" };

        tbl.AddRow(new string[] { "", "MONTHLY", "", "ANNUAL", "" }, new string[] { "Background:ABCDEF", "JustifyCenter|Background:ABCDEF|HorizontalMerge:Restart|FontSize:32", "Background:ABCDEF|HorizontalMerge:Continue", "JustifyCenter|Background:ABCDEF|HorizontalMerge:Restart|FontSize:32", "Background:ABCDEF|HorizontalMerge:Continue" });
        
        double weeklyTotal = WeeklyFood + WeeklyLiquor + WeeklyBeer + WeeklyWine;
        YearlyTotalSales = weeklyTotal * 52;
        tbl.AddRow(new string[] { "Sales", "", "", "", "" }, headerStyle);
        tbl.AddRow(AddIncomeRow("Food", WeeklyFood * 52), detailStyle);
        tbl.AddRow(AddIncomeRow("Liquor", WeeklyLiquor * 52), detailStyle);
        tbl.AddRow(AddIncomeRow("Beer", WeeklyBeer * 52), detailStyle);
        tbl.AddRow(AddIncomeRow("Wine", WeeklyWine * 52), detailStyle);
        tbl.AddRow(AddIncomeRow("TOTAL SALES", YearlyTotalSales), totalStyle);
        tbl.AddRow(new string[] { "", "", "", "", "" });

        double foodCostPct = double.Parse(questions.ByTitle("Food Cost %", "0"));
        double liquorCostPct = double.Parse(questions.ByTitle("Liquor Cost %", "0"));
        double beerCostPct = double.Parse(questions.ByTitle("Beer Cost %", "0"));
        double wineCostPct = double.Parse(questions.ByTitle("Wine Cost %", "0"));

        tbl.AddRow(new string[] { "Cost of Sales", "", "", "", "" }, headerStyle);
        tbl.AddRow(new string[] { "Food", (foodCostPct / 100 * WeeklyFood * 52 / 12).ToString("#,##0"), ToPercent(foodCostPct), (foodCostPct / 100 * WeeklyFood * 52).ToString("#,##0"), ToPercent(foodCostPct) }, detailStyle);
        tbl.AddRow(new string[] { "Liquor", (liquorCostPct / 100 * WeeklyLiquor * 52 / 12).ToString("#,##0"), ToPercent(liquorCostPct), (liquorCostPct / 100 * WeeklyLiquor * 52).ToString("#,##0"), ToPercent(liquorCostPct) }, detailStyle);
        tbl.AddRow(new string[] { "Beer", (beerCostPct / 100 * WeeklyBeer * 52 / 12).ToString("#,##0"), ToPercent(beerCostPct), (beerCostPct / 100 * WeeklyBeer * 52).ToString("#,##0"), ToPercent(beerCostPct) }, detailStyle);
        tbl.AddRow(new string[] { "Wine", (wineCostPct / 100 * WeeklyWine * 52 / 12).ToString("#,##0"), ToPercent(wineCostPct), (wineCostPct / 100 * WeeklyWine * 52).ToString("#,##0"), ToPercent(wineCostPct) }, detailStyle);

        double yearlyCost = (foodCostPct * WeeklyFood + liquorCostPct * WeeklyLiquor + beerCostPct * WeeklyBeer + wineCostPct * WeeklyWine) / 100 * 52;
        double yearlyCostPct = 100 * yearlyCost / YearlyTotalSales;
        tbl.AddRow(AddIncomeRow("TOTAL COST OF SALES", yearlyCost), totalStyle);
        tbl.AddRow(AddIncomeRow("GROSS PROFIT", YearlyTotalSales - yearlyCost), sectionTotal);
        tbl.AddRow(new string[] { "", "", "", "", "" });

        tbl.AddRow(new string[] { "Payroll", "", "", "", "" }, headerStyle);
        tbl.AddRow(new string[] { "Salaries and Wages", "", "", "", "" }, detailStyle);
        double management = questions.BySectionSum("Management Salaries (Annual)");
        tbl.AddRow(AddIncomeRow("Management", management), detailStyle2);
        tbl.AddRow(AddIncomeRow("Hourly Employees", YearlyHourlyCosts), detailStyle2);
        double totalGrossPay = management + YearlyHourlyCosts;
        tbl.AddRow(AddIncomeRow("Total Salaries and Wages", totalGrossPay), totalStyle2);
        tbl.AddRow(new string[] { "", "", "", "", "" });

        double totalBenefits = 0;
        tbl.AddRow(new string[] { "Employee Benefits", "", "", "", "" }, detailStyle);
        double benefits = questions.BySectionSum("Employee Benefits", 0, 4);
        double yearlyBenefits = benefits / 100 * totalGrossPay;
        tbl.AddRow(AddIncomeRow("Payroll Taxes", yearlyBenefits), detailStyle2);
        totalBenefits += yearlyBenefits;

        benefits = questions.BySectionSum("Employee Benefits", 4, 1);
        yearlyBenefits = benefits / 100 * totalGrossPay;
        tbl.AddRow(AddIncomeRow("Worker's Comp", yearlyBenefits), detailStyle2);
        totalBenefits += yearlyBenefits;

        benefits = questions.BySectionSum("Employee Benefits", 5, 1);
        double numberEmployees = questions.BySectionSum("Employee Benefits", 6, 1);
        yearlyBenefits = benefits * numberEmployees * 12;
        tbl.AddRow(AddIncomeRow("Group Medical Insurance", yearlyBenefits), detailStyle2);
        totalBenefits += yearlyBenefits;

        benefits = questions.BySectionSum("Employee Benefits", 7);
        yearlyBenefits = benefits * 12;
        tbl.AddRow(AddIncomeRow("Other", yearlyBenefits), detailStyle2);
        totalBenefits += yearlyBenefits;

        tbl.AddRow(AddIncomeRow("Total Benefits", totalBenefits), totalStyle2);
        tbl.AddRow(AddIncomeRow("TOTAL PAYROLL", totalGrossPay + totalBenefits), totalStyle);
        tbl.AddRow(AddIncomeRow("PRIME COST", totalGrossPay + totalBenefits + yearlyCost), sectionTotal);
        tbl.AddRow(new string[] { "", "", "", "", "" });


        tbl.AddRow(new string[] { "Other Controllable Expenses", "", "", "", "" }, headerStyle);
        string[] controllableSections = new string[] { "Direct Operating Expenses", "Music & Entertainment", "Marketing", "Utilities", "General & Administrative", "Repairs and Maintenance" };
        List<Question> expenses = questions.FindAll(delegate(Question q)
        {
            return controllableSections.Contains(q.Section);
        });

        double expenseSum = 0;
        double sectionSum = 0;
        for (int i = 0, ii = expenses.Count; i < ii; i++)
        {
            if (i == 0 || expenses[i].Section != expenses[i - 1].Section)
            {
                tbl.AddRow(new string[] { expenses[i].Section, "", "", "", "" }, detailStyle);
            }
            if(expenses[i].Title == "Credit Card Charges")
            {
                double ccPct = 0, ccRate = 0;
                double.TryParse(questions.ByTitle("Percentages of Credit Card Sales"), out ccPct);
                double.TryParse(questions.ByTitle("Average Discount Percentage"), out ccRate);
                expenses[i].Answer.Text = (Math.Round((ccPct * .01 * ccRate * .01) * YearlyTotalSales / 12)).ToString();
            }
            int val = 0;
            if (int.TryParse(expenses[i].Answer.Text, out val))
            {
                sectionSum += val;
                expenseSum += val;
            }
            AddExpenseRow(tbl, expenses[i], detailStyle2);

            if (i >= ii - 1 || expenses[i].Section != expenses[i + 1].Section)
            {
                tbl.AddRow(AddIncomeRow("Total " + expenses[i - 1].Section, sectionSum * 12), totalStyle2);
                sectionSum = 0;
                if(i < ii - 1)
                    tbl.AddRow(new string[] { "", "", "", "", "" });
            }
        }

        double controllableProfit = YearlyTotalSales - yearlyCost - totalGrossPay - totalBenefits - expenseSum * 12;
        tbl.AddRow(AddIncomeRow("CONTROLLABLE PROFIT", controllableProfit), sectionTotal);
        tbl.AddRow(new string[] { "", "", "", "", "" });


        tbl.AddRow(new string[] { "Occupancy Costs and Depreciation", "", "", "", "" }, headerStyle);
        expenses = questions.FindAll(delegate(Question q) { return q.Section == "Occupancy Costs"; });

        expenseSum = 0;
        sectionSum = 0;
        for (int i = 0, ii = expenses.Count; i < ii; i++)
        {
            if (i == 0 || expenses[i].Section != expenses[i - 1].Section)
            {
                tbl.AddRow(new string[] { expenses[i].Section, "", "", "", "" }, detailStyle);
            }

            if(expenses[i].Title == "Percentage Rent - Percentage amount")
            {
                double rentPct = 0, aboveAmount = 0;
                double.TryParse(expenses[i].Answer.Text, out rentPct);
                double.TryParse(expenses[i + 1].Answer.Text, out aboveAmount);
                double monthRent = 0;
                if(YearlyTotalSales > aboveAmount)
                    monthRent = (YearlyTotalSales - aboveAmount) * rentPct * .01 / 12;

                sectionSum += monthRent;
                expenseSum += monthRent;
                expenses[i].Title = "Percentage Rent";
                expenses[i].Answer.Text = monthRent.ToString();
                AddExpenseRow(tbl, expenses[i], detailStyle2);
                i++;
                continue;
            }

            int val = 0;
            if (int.TryParse(expenses[i].Answer.Text, out val))
            {
                sectionSum += val;
                expenseSum += val;
            }
            AddExpenseRow(tbl, expenses[i], detailStyle2);

            if (i >= ii - 1 || expenses[i].Section != expenses[i + 1].Section)
            {
                tbl.AddRow(AddIncomeRow("Total " + expenses[i - 1].Section, sectionSum * 12), totalStyle2);
                sectionSum = 0;

                tbl.AddRow(new string[] { "", "", "", "", "" });
            }
        }

        List<Question> capital = Question.Get("Financials", "Capital Budget", currentUserId);
        double building = capital.BySectionSum("Land and Building");
        building /= 30; //30 Year Depreciation
        double leasehold = capital.ByTitleSum(new string[] { "Construction Contract", "Architect & Engineering", "Legal (lease & incorporation)", "Project Consultant", "Accounting & Tax", "Name, Logo & Graphic Design", "Building Permits", "Other Licenses & Permits", "Landscaping", "Exterior Signs & Decorations", "Resurfacing", "Parking Bumpers", "Parking Lot Striping", "Contingency" });
        leasehold -= capital.ByTitleSum(new string[] { "Landlord Contribution" });
        leasehold /= 39.5; //39.5 Year Amortization
        double equipment = capital.ByTitleSum(new string[] { "Bar / Kitchen Equipment", "Bar / Dining Room Furniture", "Artwork & Specialty Decor", "Security System", "Music/Sound/Audio-Visual Systems", "Cash Register / Point of Sale", "Phone System", "Office Equipment / Computer", "Interior Signs" });
        equipment /= 7; //7 Year Amortization
        double preOpening = capital.ByTitleSum(new string[] { "Liquor License", "Menus / Menu Boards", "Travel, Research, Concept Development", "Kitchen Smallwares", "Office Supplies" });
        preOpening += capital.BySectionSum("Pre-Opening Expenses") + capital.BySectionSum("Opening Inventories") + capital.BySectionSum("Marketing") + capital.BySectionSum("Personnel");
        preOpening /= 5; //5 Year Amortization

        tbl.AddRow(new string[] { "Depreciation and Amortization", "", "", "", "" }, detailStyle);
        tbl.AddRow(AddIncomeRow("Building", building), detailStyle2);
        tbl.AddRow(AddIncomeRow("Leasehold Improvements", leasehold), detailStyle2);
        tbl.AddRow(AddIncomeRow("Furniture and Equipment", equipment), detailStyle2);
        tbl.AddRow(AddIncomeRow("Pre-Opening Costs", preOpening), detailStyle2);
        double totalDepreciation = building + leasehold + equipment + preOpening;
        tbl.AddRow(AddIncomeRow("Total Depreciation", totalDepreciation), totalStyle2);
        tbl.AddRow(AddIncomeRow("Total Occupancy and Depreciation", totalDepreciation + expenseSum * 12), totalStyle);
        tbl.AddRow(new string[] { "", "", "", "", "" });
        
        double principal = Principal;
        double loanRate = .01 * questions.ByTitleSum(new string [] { "Rate %" });
        double term = questions.ByTitleSum(new string[] { "Term (years)" });
        double monthlyPayment = (loanRate / 12 * principal) / (1 - Math.Pow(1 + loanRate / 12, -1 * term * 12));
        if (double.IsNaN(monthlyPayment))
            monthlyPayment = 0;
        double interest = 0;
        double loanPrincipalPayments = 0;
        for (int i = 0; i < 12; i++)
        {
            double monthlyInterest = principal * loanRate / 12;
            interest += monthlyInterest;
            loanPrincipalPayments -= (monthlyPayment - monthlyInterest);
            principal -= (monthlyPayment - monthlyInterest);
        }
        tbl.AddRow(AddIncomeRow("Loan Interest", interest), detailStyle);
        tbl.AddRow(new string[] { "", "", "", "", "" });

        double yearlyNetIncome = controllableProfit - expenseSum * 12 - totalDepreciation - interest;
        tbl.AddRow(AddIncomeRow("NET INCOME BEFORE INCOME TAXES", yearlyNetIncome), sectionTotal);
        tbl.AddRow(new string[] { "", "", "", "", "" });
        tbl.AddRow(new string[] { "ADD BACK", "", "", "", "" }, detailStyle);
        tbl.AddRow(AddIncomeRow("Depreciation and Amortization", totalDepreciation), detailStyle2);
        tbl.AddRow(new string[] { "DEDUCT", "", "", "", "" }, detailStyle);
        tbl.AddRow(AddIncomeRow("Loan Principal Payments", loanPrincipalPayments), detailStyle2);
        tbl.AddRow(new string[] { "", "", "", "", "" });

        tbl.AddRow(AddIncomeRow("CASH FLOW BEFORE INCOME TAXES", yearlyNetIncome + totalDepreciation + loanPrincipalPayments), sectionTotal);

    }

    static void FinancialsIncomeSummary(WordprocessingDocument wordDocument)
    {
        Body body = wordDocument.MainDocumentPart.Document.Body;

        body.AddHeader("Summary Income and Cash Flow");

        Table tbl = NewTable(body, 5);

        List<Question> questions = Question.Get("Financials", "Expenses", currentUserId);

        string[] headerStyle = new string[] { "LeftIndent:200|Bold" };
        string[] detailStyle = new string[] { "LeftIndent:600", "JustifyRight|RightIndent:600", "JustifyRight|RightIndent:600", "JustifyRight|RightIndent:600", "JustifyRight|RightIndent:600" };
        string[] detailStyle2 = new string[] { "LeftIndent:1000", "JustifyRight|RightIndent:600", "JustifyRight|RightIndent:600", "JustifyRight|RightIndent:600", "JustifyRight|RightIndent:600" };
        string[] totalStyle = new string[] { "LeftIndent:200", "Borders:Top|JustifyRight|RightIndent:600", "Borders:Top|JustifyRight|RightIndent:600", "Borders:Top|JustifyRight|RightIndent:600", "Borders:Top|JustifyRight|RightIndent:600" };
        string[] totalStyle2 = new string[] { "LeftIndent:600", "Borders:Top|JustifyRight|RightIndent:600", "Borders:Top|JustifyRight|RightIndent:600", "Borders:Top|JustifyRight|RightIndent:600", "Borders:Top|JustifyRight|RightIndent:600" };
        string[] sectionTotal = new string[] { "Bold|LeftIndent:200|Background:ABCDEF|FontSize:26", "JustifyRight|RightIndent:600|Background:ABCDEF", "JustifyRight|RightIndent:600|Background:ABCDEF", "JustifyRight|RightIndent:600|Background:ABCDEF", "JustifyRight|RightIndent:600|Background:ABCDEF" };

        tbl.AddRow(new string[] { "", "MONTHLY", "", "ANNUAL", "" }, new string[] { "Background:ABCDEF", "JustifyCenter|Background:ABCDEF|HorizontalMerge:Restart|FontSize:32", "Background:ABCDEF|HorizontalMerge:Continue", "JustifyCenter|Background:ABCDEF|HorizontalMerge:Restart|FontSize:32", "Background:ABCDEF|HorizontalMerge:Continue" });

        double weeklyTotal = WeeklyFood + WeeklyLiquor + WeeklyBeer + WeeklyWine;
        double weeklyBeverage = WeeklyLiquor + WeeklyBeer + WeeklyWine;

        tbl.AddRow(new string[] { "Sales", "", "", "", "" }, headerStyle);
        tbl.AddRow(AddIncomeRow("Food", WeeklyFood * 52), detailStyle);
        tbl.AddRow(AddIncomeRow("Beverage", weeklyBeverage * 52), detailStyle);
        tbl.AddRow(AddIncomeRow("TOTAL SALES", YearlyTotalSales), totalStyle);
        tbl.AddRow(new string[] { "", "", "", "", "" });

        double foodCostPct = double.Parse(questions.ByTitle("Food Cost %", "0"));
        double liquorCostPct = double.Parse(questions.ByTitle("Liquor Cost %", "0"));
        double beerCostPct = double.Parse(questions.ByTitle("Beer Cost %", "0"));
        double wineCostPct = double.Parse(questions.ByTitle("Wine Cost %", "0")); 
        double beverageCost = liquorCostPct * .01 * WeeklyLiquor + beerCostPct * .01 * WeeklyBeer + wineCostPct * .01 * WeeklyWine;
        double beverageCostPct = beverageCost / weeklyBeverage;

        tbl.AddRow(new string[] { "Cost of Sales", "", "", "", "" }, headerStyle);
        tbl.AddRow(new string[] { "Food", (foodCostPct / 100 * WeeklyFood * 52 / 12).ToString("#,##0"), ToPercent(foodCostPct), (foodCostPct / 100 * WeeklyFood * 52).ToString("#,##0"), ToPercent(foodCostPct) }, detailStyle);
        tbl.AddRow(new string[] { "Beverage", (beverageCost * 52 / 12).ToString("#,##0"), ToPercent(100 * beverageCostPct), (beverageCost * 52).ToString("#,##0"), ToPercent(100 * beverageCostPct) }, detailStyle);
        double yearlyCost = (foodCostPct * WeeklyFood + liquorCostPct * WeeklyLiquor + beerCostPct * WeeklyBeer + wineCostPct * WeeklyWine) / 100 * 52;
        double yearlyCostPct = 100 * yearlyCost / YearlyTotalSales;
        tbl.AddRow(AddIncomeRow("TOTAL COST OF SALES", yearlyCost), totalStyle);
        tbl.AddRow(AddIncomeRow("GROSS PROFIT", YearlyTotalSales - yearlyCost), sectionTotal);
        tbl.AddRow(new string[] { "", "", "", "", "" });


        tbl.AddRow(new string[] { "Payroll", "", "", "", "" }, headerStyle);
        double management = questions.BySectionSum("Management Salaries (Annual)");
        double totalGrossPay = management + YearlyHourlyCosts;
        tbl.AddRow(AddIncomeRow("Total Salaries and Wages", totalGrossPay), detailStyle2);

        double totalBenefits = 0;
        double benefits = questions.BySectionSum("Employee Benefits", 0, 4);
        double yearlyBenefits = benefits / 100 * totalGrossPay;
        totalBenefits += yearlyBenefits;

        benefits = questions.BySectionSum("Employee Benefits", 4, 1);
        yearlyBenefits = benefits / 100 * totalGrossPay;
        totalBenefits += yearlyBenefits;

        benefits = questions.BySectionSum("Employee Benefits", 5, 1);
        double numberEmployees = questions.BySectionSum("Employee Benefits", 6, 1);
        yearlyBenefits = benefits * numberEmployees * 12;
        totalBenefits += yearlyBenefits;

        benefits = questions.BySectionSum("Employee Benefits", 7);
        yearlyBenefits = benefits * 12;
        totalBenefits += yearlyBenefits;

        tbl.AddRow(AddIncomeRow("Total Benefits", totalBenefits), detailStyle2);
        tbl.AddRow(AddIncomeRow("TOTAL PAYROLL", totalGrossPay + totalBenefits), totalStyle);
        tbl.AddRow(AddIncomeRow("PRIME COST", totalGrossPay + totalBenefits + yearlyCost), sectionTotal);
        tbl.AddRow(new string[] { "", "", "", "", "" });

        tbl.AddRow(new string[] { "Other Controllable Expenses", "", "", "", "" }, headerStyle);
        string[] controllableSections = new string[] { "Direct Operating Expenses", "Music & Entertainment", "Marketing", "Utilities", "General & Administrative", "Repairs and Maintenance" };
        List<Question> expenses = questions.FindAll(delegate(Question q)
        {
            return controllableSections.Contains(q.Section);
        });

        double expenseSum = 0;
        double sectionSum = 0;
        for (int i = 0, ii = expenses.Count; i < ii; i++)
        {
            if (expenses[i].Title == "Credit Card Charges")
            {
                double ccPct = 0, ccRate = 0;
                double.TryParse(questions.ByTitle("Percentages of Credit Card Sales"), out ccPct);
                double.TryParse(questions.ByTitle("Average Discount Percentage"), out ccRate);
                expenses[i].Answer.Text = (Math.Round((ccPct * .01 * ccRate * .01) * YearlyTotalSales / 12)).ToString();
            }
            int val = 0;
            if (int.TryParse(expenses[i].Answer.Text, out val))
            {
                sectionSum += val;
                expenseSum += val;
            }
            if (i >= ii - 1 || expenses[i].Section != expenses[i + 1].Section)
            {
                tbl.AddRow(AddIncomeRow(expenses[i].Section, sectionSum * 12), detailStyle);
                sectionSum = 0;
            }
        }

        tbl.AddRow(AddIncomeRow("Total Other Controllable Expenses", expenseSum * 12), totalStyle);
        double controllableProfit = YearlyTotalSales - yearlyCost - totalGrossPay - totalBenefits - expenseSum * 12;
        tbl.AddRow(AddIncomeRow("CONTROLLABLE PROFIT", controllableProfit), sectionTotal);
        tbl.AddRow(new string[] { "", "", "", "", "" });


        tbl.AddRow(new string[] { "Occupancy Costs and Depreciation", "", "", "", "" }, headerStyle);
        expenses = questions.FindAll(delegate(Question q) { return q.Section == "Occupancy Costs"; });

        expenseSum = 0;
        sectionSum = 0;
        for (int i = 0, ii = expenses.Count; i < ii; i++)
        {
            if (expenses[i].Title == "Percentage Rent - Percentage amount")
            {
                double rentPct = 0, aboveAmount = 0;
                double.TryParse(expenses[i].Answer.Text, out rentPct);
                double.TryParse(expenses[i + 1].Answer.Text, out aboveAmount);
                double monthRent = 0;
                if (YearlyTotalSales > aboveAmount)
                    monthRent = (YearlyTotalSales - aboveAmount) * rentPct * .01 / 12;

                sectionSum += monthRent;
                expenseSum += monthRent;
                expenses[i].Answer.Text = monthRent.ToString();
                i++;
                continue;
            }

            int val = 0;
            if (int.TryParse(expenses[i].Answer.Text, out val))
            {
                sectionSum += val;
                expenseSum += val;
            }
        }
        tbl.AddRow(AddIncomeRow(expenses[expenses.Count - 1].Section, sectionSum * 12), detailStyle);


        List<Question> capital = Question.Get("Financials", "Capital Budget", currentUserId);
        double building = capital.BySectionSum("Land & Building");
        building /= 30; //30 Year Depreciation
        double leasehold = capital.ByTitleSum(new string[] { "Construction Contract", "Architect & Engineering", "Legal (lease & incorporation)", "Project Consultant", "Accounting & Tax", "Name, Logo & Graphic Design", "Building Permits", "Other Licenses & Permits", "Landscaping", "Exterior Signs & Decorations", "Resurfacing", "Parking Bumpers", "Parking Lot Striping", "Contingency" });
        leasehold -= capital.ByTitleSum(new string[] { "Landlord Contribution" });
        leasehold /= 39.5; //39.5 Year Amortization
        double equipment = capital.ByTitleSum(new string[] { "Bar / Kitchen Equipment", "Bar / Dining Room Furniture", "Artwork & Specialty Decor", "Security System", "Music/Sound/Audio-Visual Systems", "Cash Register / Point of Sale", "Phone System", "Office Equipment / Computer", "Interior Signs" });
        equipment /= 7; //7 Year Amortization
        double preOpening = capital.ByTitleSum(new string[] { "Liquor License", "Menus / Menu Boards", "Travel, Research, Concept Development", "Kitchen Smallwares", "Office Supplies" });
        preOpening += capital.BySectionSum("Pre-Opening Expenses") + capital.BySectionSum("Opening Inventories") + capital.BySectionSum("Marketing") + capital.BySectionSum("Personnel");
        preOpening /= 5; //5 Year Amortization

        double totalDepreciation = building + leasehold + equipment + preOpening;
        tbl.AddRow(AddIncomeRow("Depreciation and Amortization", totalDepreciation), detailStyle);
        tbl.AddRow(new string[] { "", "", "", "", "" });

        double principal = Principal;
        double loanRate = .01 * questions.ByTitleSum(new string[] { "Rate %" });
        double term = questions.ByTitleSum(new string[] { "Term (years)" });
        double monthlyPayment = (loanRate / 12 * principal) / (1 - Math.Pow(1 + loanRate / 12, -1 * term * 12));
        if (double.IsNaN(monthlyPayment))
            monthlyPayment = 0;
        double interest = 0;
        double loanPrincipalPayments = 0;
        for (int i = 0; i < 12; i++)
        {
            double monthlyInterest = principal * loanRate / 12;
            interest += monthlyInterest;
            loanPrincipalPayments -= (monthlyPayment - monthlyInterest);
            principal -= (monthlyPayment - monthlyInterest);
        }
        tbl.AddRow(AddIncomeRow("Loan Interest", interest), detailStyle);
        tbl.AddRow(new string[] { "", "", "", "", "" });

        double yearlyNetIncome = controllableProfit - expenseSum * 12 - totalDepreciation - interest;
        tbl.AddRow(AddIncomeRow("NET INCOME BEFORE INCOME TAXES", yearlyNetIncome), sectionTotal);
        tbl.AddRow(new string[] { "", "", "", "", "" });
        tbl.AddRow(new string[] { "ADD BACK", "", "", "", "" }, detailStyle);
        tbl.AddRow(AddIncomeRow("Depreciation and Amortization", totalDepreciation), detailStyle2);
        tbl.AddRow(new string[] { "DEDUCT", "", "", "", "" }, detailStyle);
        tbl.AddRow(AddIncomeRow("Loan Principal Payments", loanPrincipalPayments), detailStyle2);
        tbl.AddRow(new string[] { "", "", "", "", "" });
        tbl.AddRow(AddIncomeRow("CASH FLOW BEFORE INCOME TAXES", yearlyNetIncome + totalDepreciation + loanPrincipalPayments), sectionTotal);

        //TODO Add Key Ratios

    }

    static void FinancialsIncome5Year(WordprocessingDocument wordDocument)
    {
        Body body = wordDocument.MainDocumentPart.Document.Body;

        body.AddHeader("5 Year Operating Projections");

        Table tbl = NewTable(body, 11);

        List<Question> questions = Question.Get("Financials", "Expenses", currentUserId);

        string[] headerStyle = new string[] { "Bold|FontSize:20|HorizontalMerge:Restart", "HorizontalMerge:Continue", "HorizontalMerge:Continue", "HorizontalMerge:Continue", "HorizontalMerge:Continue", "HorizontalMerge:Restart" };
        string[] detailStyle = new string[] { "LeftIndent:200|FontSize:20", "JustifyRight|RightIndent:10|FontSize:20", "JustifyRight|RightIndent:200|FontSize:20", "JustifyRight|RightIndent:10|FontSize:20", "JustifyRight|RightIndent:200|FontSize:20", "JustifyRight|RightIndent:10|FontSize:20", "JustifyRight|RightIndent:200|FontSize:20", "JustifyRight|RightIndent:10|FontSize:20", "JustifyRight|RightIndent:200|FontSize:20", "JustifyRight|RightIndent:10|FontSize:20", "JustifyRight|RightIndent:200|FontSize:20" };
        string[] detailStyle2 = new string[] { "LeftIndent:400|FontSize:20", "JustifyRight|RightIndent:10|FontSize:20", "JustifyRight|RightIndent:200|FontSize:20", "JustifyRight|RightIndent:10|FontSize:20", "JustifyRight|RightIndent:200|FontSize:20", "JustifyRight|RightIndent:10|FontSize:20", "JustifyRight|RightIndent:200|FontSize:20", "JustifyRight|RightIndent:10|FontSize:20", "JustifyRight|RightIndent:200|FontSize:20", "JustifyRight|RightIndent:10|FontSize:20", "JustifyRight|RightIndent:200|FontSize:20" };
        string[] totalStyle = new string[] { "|FontSize:20", "Borders:Top|JustifyRight|RightIndent:10|FontSize:20", "Borders:Top|JustifyRight|RightIndent:200|FontSize:20", "Borders:Top|JustifyRight|RightIndent:10|FontSize:20", "Borders:Top|JustifyRight|RightIndent:200|FontSize:20", "Borders:Top|JustifyRight|RightIndent:10|FontSize:20", "Borders:Top|JustifyRight|RightIndent:200|FontSize:20", "Borders:Top|JustifyRight|RightIndent:10|FontSize:20", "Borders:Top|JustifyRight|RightIndent:200|FontSize:20", "Borders:Top|JustifyRight|RightIndent:10|FontSize:20", "Borders:Top|JustifyRight|RightIndent:200|FontSize:20" };
        string[] totalStyle2 = new string[] { "LeftIndent:200|FontSize:20", "Borders:Top|JustifyRight|RightIndent:10|FontSize:20", "Borders:Top|JustifyRight|RightIndent:200|FontSize:20", "Borders:Top|JustifyRight|RightIndent:10|FontSize:20", "Borders:Top|JustifyRight|RightIndent:200|FontSize:20", "Borders:Top|JustifyRight|RightIndent:10|FontSize:20", "Borders:Top|JustifyRight|RightIndent:200|FontSize:20", "Borders:Top|JustifyRight|RightIndent:10|FontSize:20", "Borders:Top|JustifyRight|RightIndent:200|FontSize:20", "Borders:Top|JustifyRight|RightIndent:10|FontSize:20", "Borders:Top|JustifyRight|RightIndent:200|FontSize:20" };
        string[] sectionTotal = new string[] { "Bold|Background:ABCDEF|FontSize:24", "JustifyRight|RightIndent:10|Background:ABCDEF|FontSize:20", "JustifyRight|RightIndent:200|Background:ABCDEF|FontSize:20", "JustifyRight|RightIndent:10|Background:ABCDEF|FontSize:20", "JustifyRight|RightIndent:200|Background:ABCDEF|FontSize:20", "JustifyRight|RightIndent:10|Background:ABCDEF|FontSize:20", "JustifyRight|RightIndent:200|Background:ABCDEF|FontSize:20", "JustifyRight|RightIndent:10|Background:ABCDEF|FontSize:20", "JustifyRight|RightIndent:200|Background:ABCDEF|FontSize:20", "JustifyRight|RightIndent:10|Background:ABCDEF|FontSize:20", "JustifyRight|RightIndent:200|Background:ABCDEF|FontSize:20" };

        tbl.AddRow(new string[] { "", "Year 1", "", "Year 2", "", "Year 3", "", "Year 4", "", "Year 5", "" }, new string[] { "Background:2B579A|FontColor:FFFFFF", "JustifyCenter|Background:2B579A|FontColor:FFFFFF|HorizontalMerge:Restart|FontSize:32", "Background:2B579A|FontColor:FFFFFF|HorizontalMerge:Continue", "JustifyCenter|Background:2B579A|FontColor:FFFFFF|HorizontalMerge:Restart|FontSize:32", "Background:2B579A|FontColor:FFFFFF|HorizontalMerge:Continue", "JustifyCenter|Background:2B579A|FontColor:FFFFFF|HorizontalMerge:Restart|FontSize:32", "Background:2B579A|FontColor:FFFFFF|HorizontalMerge:Continue", "JustifyCenter|Background:2B579A|FontColor:FFFFFF|HorizontalMerge:Restart|FontSize:32", "Background:2B579A|FontColor:FFFFFF|HorizontalMerge:Continue", "JustifyCenter|Background:2B579A|FontColor:FFFFFF|HorizontalMerge:Restart|FontSize:32", "Background:2B579A|FontColor:FFFFFF|HorizontalMerge:Continue" });

        double weeklyTotal = WeeklyFood + WeeklyLiquor + WeeklyBeer + WeeklyWine;
        double weeklyBeverage = WeeklyLiquor + WeeklyBeer + WeeklyWine;
        YearlyTotalSales = weeklyTotal * 52;

        List<Question> investmentQuestions = Question.Get("Financials", "Investment", currentUserId);
        double salesPct = investmentQuestions.ByTitleSum(new string[] { "Sales % Increase" });
        double costPct = salesPct;
        double salaryPct = investmentQuestions.ByTitleSum(new string[] { "Salary % Increase" });
        double benefitsPct = salaryPct;
        double otherExpensePct = investmentQuestions.ByTitleSum(new string[] { "Operating Expense % Increase" }); ;
        double occupancyPct = investmentQuestions.ByTitleSum(new string[] { "Occupancy % Increase" }); ;

        for (int i = 0; i < 5; i++)
        {
            FiveYearSales[i] = YearlyTotalSales * Math.Pow(1 + salesPct * .01, i);
        }

        tbl.AddRow(new string[] { "Sales", "", "", "", "" }, headerStyle);
        tbl.AddRow(Add5YearRow("Food", WeeklyFood * 52, salesPct), detailStyle);
        tbl.AddRow(Add5YearRow("Beverage", weeklyBeverage * 52, salesPct), detailStyle);
        tbl.AddRow(Add5YearRow("TOTAL SALES", YearlyTotalSales, salesPct), totalStyle);
        tbl.AddRow(new string[] { "", "", "", "", "", "", "", "", "", "", "" });

        double foodCostPct = double.Parse(questions.ByTitle("Food Cost %", "0"));
        double liquorCostPct = double.Parse(questions.ByTitle("Liquor Cost %", "0"));
        double beerCostPct = double.Parse(questions.ByTitle("Beer Cost %", "0"));
        double wineCostPct = double.Parse(questions.ByTitle("Wine Cost %", "0"));
        double beverageCost = liquorCostPct * .01 * WeeklyLiquor + beerCostPct * .01 * WeeklyBeer + wineCostPct * .01 * WeeklyWine;

        tbl.AddRow(new string[] { "Cost of Sales", "", "", "", "" }, headerStyle);
        tbl.AddRow(Add5YearRow("Food", foodCostPct / 100 * WeeklyFood * 52, costPct), detailStyle);
        tbl.AddRow(Add5YearRow("Beverage", beverageCost * 52, costPct), detailStyle);
        double yearlyCost = (foodCostPct * WeeklyFood + liquorCostPct * WeeklyLiquor + beerCostPct * WeeklyBeer + wineCostPct * WeeklyWine) / 100 * 52;
        string[] costRow = Add5YearRow("TOTAL COST", yearlyCost, costPct);
        tbl.AddRow(costRow, totalStyle);
        tbl.AddRow(Add5YearRow("GROSS PROFIT", YearlyTotalSales - yearlyCost, costPct), sectionTotal);
        tbl.AddRow(new string[] { "", "", "", "", "", "", "", "", "", "", "" });

        tbl.AddRow(new string[] { "Payroll", "", "", "", "" }, headerStyle);
        double management = questions.BySectionSum("Management Salaries (Annual)");
        double totalGrossPay = management + YearlyHourlyCosts;
        string[] salaryRow = Add5YearRow("Salaries", totalGrossPay, salaryPct);
        tbl.AddRow(salaryRow, detailStyle);

        double totalBenefits = 0;
        totalBenefits += totalGrossPay * .01 * questions.BySectionSum("Employee Benefits", 0, 5);

        double numberEmployees = questions.BySectionSum("Employee Benefits", 6, 1);
        totalBenefits += questions.BySectionSum("Employee Benefits", 5, 1) * numberEmployees * 12;
        totalBenefits += questions.BySectionSum("Employee Benefits", 7) * 12;
        
        string[] benefitsRow = Add5YearRow("Benefits", totalBenefits, benefitsPct);
        tbl.AddRow(benefitsRow, detailStyle);
        tbl.AddRow(SumRows("TOTAL PAYROLL", salaryRow, benefitsRow), totalStyle);
        tbl.AddRow(SumRows("PRIME COST", salaryRow, benefitsRow, costRow), sectionTotal);
        tbl.AddRow(new string[] { "", "", "", "", "", "", "", "", "", "", "" });


        tbl.AddRow(new string[] { "Other Controllable Expenses", "", "", "", "" }, headerStyle);
        string[] controllableSections = new string[] { "Direct Operating Expenses", "Music & Entertainment", "Marketing", "Utilities", "General & Administrative", "Repairs and Maintenance" };
        List<Question> expenses = questions.FindAll(delegate(Question q)
        {
            return controllableSections.Contains(q.Section);
        });

        double expenseSum = 0;
        double sectionSum = 0;
        for (int i = 0, ii = expenses.Count; i < ii; i++)
        {
            if (expenses[i].Title == "Credit Card Charges")
            {
                double ccPct = 0, ccRate = 0;
                double.TryParse(questions.ByTitle("Percentages of Credit Card Sales"), out ccPct);
                double.TryParse(questions.ByTitle("Average Discount Percentage"), out ccRate);
                expenses[i].Answer.Text = (Math.Round((ccPct * .01 * ccRate * .01) * YearlyTotalSales / 12)).ToString();
            }
            int val = 0;
            if (int.TryParse(expenses[i].Answer.Text, out val))
            {
                sectionSum += val;
                expenseSum += val;
            }
            if (i >= ii - 1 || expenses[i].Section != expenses[i + 1].Section)
            {
                string section = expenses[i].Section;
                if (section == "Direct Operating Expenses")
                    section = "Operating Expenses";
                if (section == "Music & Entertainment")
                    section = "Music";
                if (section == "Repairs and Maintenance")
                    section = "Maintenance";

                tbl.AddRow(Add5YearRow(section, sectionSum * 12, otherExpensePct), detailStyle);
                sectionSum = 0;
            }
        }

        tbl.AddRow(Add5YearRow("TOTAL", expenseSum * 12, otherExpensePct), totalStyle);
        double controllableProfit = YearlyTotalSales - yearlyCost - totalGrossPay - totalBenefits - expenseSum * 12;
        string[] controllableRow = Add5YearRow("CONTROLLABLE PROFIT", controllableProfit, otherExpensePct);
        tbl.AddRow(controllableRow, sectionTotal);
        tbl.AddRow(new string[] { "", "", "", "", "", "", "", "", "", "", "" });


        tbl.AddRow(new string[] { "Occupancy Costs and Depreciation", "", "", "", "" }, headerStyle);
        expenses = questions.FindAll(delegate(Question q) { return q.Section == "Occupancy Costs"; });

        expenseSum = 0;
        sectionSum = 0;
        for (int i = 0, ii = expenses.Count; i < ii; i++)
        {
            if (expenses[i].Title == "Percentage Rent - Percentage amount")
            {
                double rentPct = 0, aboveAmount = 0;
                double.TryParse(expenses[i].Answer.Text, out rentPct);
                double.TryParse(expenses[i + 1].Answer.Text, out aboveAmount);
                double monthRent = 0;
                if (YearlyTotalSales > aboveAmount)
                    monthRent = (YearlyTotalSales - aboveAmount) * rentPct * .01 / 12;

                sectionSum += monthRent;
                expenseSum += monthRent;
                expenses[i].Answer.Text = monthRent.ToString();
                i++;
                continue;
            }

            int val = 0;
            if (int.TryParse(expenses[i].Answer.Text, out val))
            {
                sectionSum += val;
                expenseSum += val;
            }
        }
        string[] occupancyRow = Add5YearRow("Occupancy", sectionSum * 12, occupancyPct);
        tbl.AddRow(occupancyRow, detailStyle);

        List<Question> capital = Question.Get("Financials", "Capital Budget", currentUserId);
        double building = capital.BySectionSum("Land & Building");
        building /= 30; //30 Year Depreciation
        double leasehold = capital.ByTitleSum(new string[] { "Construction Contract", "Architect & Engineering", "Legal (lease & incorporation)", "Project Consultant", "Accounting & Tax", "Name, Logo & Graphic Design", "Building Permits", "Other Licenses & Permits", "Landscaping", "Exterior Signs & Decorations", "Resurfacing", "Parking Bumpers", "Parking Lot Striping", "Contingency" });
        leasehold -= capital.ByTitleSum(new string[] { "Landlord Contribution" });
        leasehold /= 39.5; //39.5 Year Amortization
        double equipment = capital.ByTitleSum(new string[] { "Bar / Kitchen Equipment", "Bar / Dining Room Furniture", "Artwork & Specialty Decor", "Security System", "Music/Sound/Audio-Visual Systems", "Cash Register / Point of Sale", "Phone System", "Office Equipment / Computer", "Interior Signs" });
        equipment /= 7; //7 Year Amortization
        double preOpening = capital.ByTitleSum(new string[] { "Liquor License", "Menus / Menu Boards", "Travel, Research, Concept Development", "Kitchen Smallwares", "Office Supplies" });
        preOpening += capital.BySectionSum("Pre-Opening Expenses") + capital.BySectionSum("Opening Inventories") + capital.BySectionSum("Marketing") + capital.BySectionSum("Personnel");
        preOpening /= 5; //5 Year Amortization

        double totalDepreciation = building + leasehold + equipment + preOpening;
        string[] depreciationRow = Add5YearRow("Depreciation", totalDepreciation, 0);
        tbl.AddRow(depreciationRow, detailStyle);
        tbl.AddRow(new string[] { "", "", "", "", "", "", "", "", "", "", "" });

        double principal = Principal;
        double loanRate = .01 * questions.ByTitleSum(new string[] { "Rate %" });
        double term = questions.ByTitleSum(new string[] { "Term (years)" });
        double monthlyPayment = (loanRate / 12 * principal) / (1 - Math.Pow(1 + loanRate / 12, -1 * term * 12));
        if (double.IsNaN(monthlyPayment))
            monthlyPayment = 0;
        double interest1 = 0;
        double interest2 = 0;
        double interest3 = 0;
        double interest4 = 0;
        double interest5 = 0;
        double loanPrincipalPayments1 = 0;
        double loanPrincipalPayments2 = 0;
        double loanPrincipalPayments3 = 0;
        double loanPrincipalPayments4 = 0;
        double loanPrincipalPayments5 = 0;
        for (int i = 0; i < 12; i++)
        {
            double monthlyInterest = principal * loanRate / 12;
            interest1 += monthlyInterest;
            loanPrincipalPayments1 -= (monthlyPayment - monthlyInterest);
            principal -= (monthlyPayment - monthlyInterest);
        }
        string loanPct1 = ToPercent(100 * interest1 / (YearlyTotalSales * Math.Pow(1 + salesPct / 100, 0)));
        string principalPct1 = ToPercent(100 * loanPrincipalPayments1 / (YearlyTotalSales * Math.Pow(1 + salesPct / 100, 0)));
        for (int i = 12; i < 24; i++)
        {
            double monthlyInterest = principal * loanRate / 12;
            interest2 += monthlyInterest;
            loanPrincipalPayments2 -= (monthlyPayment - monthlyInterest);
            principal -= (monthlyPayment - monthlyInterest);
        }
        string loanPct2 = ToPercent(100 * interest2 / (YearlyTotalSales * Math.Pow(1 + salesPct / 100, 1)));
        string principalPct2 = ToPercent(100 * loanPrincipalPayments2 / (YearlyTotalSales * Math.Pow(1 + salesPct / 100, 1)));
        for (int i = 24; i < 36; i++)
        {
            double monthlyInterest = principal * loanRate / 12;
            interest3 += monthlyInterest;
            loanPrincipalPayments3 -= (monthlyPayment - monthlyInterest);
            principal -= (monthlyPayment - monthlyInterest);
        }
        string loanPct3 = ToPercent(100 * interest3 / (YearlyTotalSales * Math.Pow(1 + salesPct / 100, 2)));
        string principalPct3 = ToPercent(100 * loanPrincipalPayments3 / (YearlyTotalSales * Math.Pow(1 + salesPct / 100, 2)));
        for (int i = 36; i < 48; i++)
        {
            double monthlyInterest = principal * loanRate / 12;
            interest4 += monthlyInterest;
            loanPrincipalPayments4 -= (monthlyPayment - monthlyInterest);
            principal -= (monthlyPayment - monthlyInterest);
        }
        string loanPct4 = ToPercent(100 * interest4 / (YearlyTotalSales * Math.Pow(1 + salesPct / 100, 3)));
        string principalPct4 = ToPercent(100 * loanPrincipalPayments4 / (YearlyTotalSales * Math.Pow(1 + salesPct / 100, 3)));
        for (int i = 48; i < 60; i++)
        {
            double monthlyInterest = principal * loanRate / 12;
            interest5 += monthlyInterest;
            loanPrincipalPayments5 -= (monthlyPayment - monthlyInterest);
            principal -= (monthlyPayment - monthlyInterest);
        }
        string loanPct5 = ToPercent(100 * interest5 / (YearlyTotalSales * Math.Pow(1 + salesPct / 100, 4)));
        string principalPct5 = ToPercent(100 * loanPrincipalPayments5 / (YearlyTotalSales * Math.Pow(1 + salesPct / 100, 4)));

        string[] loanRow = new string[] { "Loan Interest", interest1.ToString("#,##0"), loanPct1, interest2.ToString("#,##0"), loanPct2, interest3.ToString("#,##0"), loanPct3, interest4.ToString("#,##0"), loanPct4, interest5.ToString("#,##0"), loanPct5 };
        tbl.AddRow(loanRow, detailStyle);
        tbl.AddRow(new string[] { "", "", "", "", "", "", "", "", "", "", "" });

        string[] netIncomeRow = DifferenceRows("NET INCOME", controllableRow, occupancyRow, depreciationRow, loanRow);
        tbl.AddRow(netIncomeRow, sectionTotal);
        tbl.AddRow(new string[] { "ADD BACK", "", "", "", "", "", "", "", "", "", "" }, new string[] { "FontSize:20" });
        tbl.AddRow(depreciationRow, detailStyle);
        tbl.AddRow(new string[] { "DEDUCT", "", "", "", "", "", "", "", "", "", "" }, new string[] { "FontSize:20" });
        string[] principalRow = new string[] { "Loan Principal", loanPrincipalPayments1.ToString("#,##0;(#,##0)"), principalPct1, loanPrincipalPayments2.ToString("#,##0;(#,##0)"), principalPct2, loanPrincipalPayments3.ToString("#,##0;(#,##0)"), principalPct3, loanPrincipalPayments4.ToString("#,##0;(#,##0)"), principalPct4, loanPrincipalPayments5.ToString("#,##0;(#,##0)"), principalPct5 };
        tbl.AddRow(principalRow, detailStyle);

        string[] cashFlow = SumRows("CASH FLOW", netIncomeRow, depreciationRow, principalRow);
        tbl.AddRow(cashFlow, sectionTotal);

        for (int i = 1; i < cashFlow.Length; i++)
        {
            if (i % 2 == 1)
            {
                double val = 0;
                double.TryParse(cashFlow[i], out val);
                FiveYearCashFlow[i / 2] = val;
            }
        }

    }

    static void FinancialsInvestmentReturn(WordprocessingDocument wordDocument)
    {
        Body body = wordDocument.MainDocumentPart.Document.Body;

        body.AddHeader("Projected Investment Returns");

        Table tbl = NewTable(body, 6);

        List<Question> investmentQuestions = Question.Get("Financials", "Investment", currentUserId);
        double distributeYear1 = investmentQuestions.ByTitleSum(new string[] { "Year 1 Percentage" });
        double distributeYear2 = investmentQuestions.ByTitleSum(new string[] { "Year 2 Percentage" });
        double distributeYear3 = investmentQuestions.ByTitleSum(new string[] { "Year 3 Percentage" });
        double distributeYear4 = investmentQuestions.ByTitleSum(new string[] { "Year 4 Percentage" });
        double distributeYear5 = investmentQuestions.ByTitleSum(new string[] { "Year 5 Percentage" });

        double equityContribution = investmentQuestions.ByTitleSum(new string[] { "Operating Partner Equity Contribution" });
        double cashBeforePayback = investmentQuestions.ByTitleSum(new string[] { "Cash Distribtion % Before Investor Payback" });
        double cashAfterPayback = investmentQuestions.ByTitleSum(new string[] { "Cash Distribtion % After Investor Payback" });
        
        List<Question> basicInfo = Question.Get("Financials", "Basic Info", currentUserId);
        double initialInvestment = basicInfo.ByTitleSum(new string[] { "Equity Capital" });
        double totalInvestment = initialInvestment;

        string[] headerStyle = new string[] { "Bold" };
        string[] detailStyle = new string[] { "", "JustifyRight|RightIndent:400", "JustifyRight|RightIndent:400", "JustifyRight|RightIndent:400", "JustifyRight|RightIndent:400", "JustifyRight|RightIndent:400" };
        string[] detailStyle2 = new string[] { "LeftIndent:400", "JustifyRight|RightIndent:400", "JustifyRight|RightIndent:400", "JustifyRight|RightIndent:400", "JustifyRight|RightIndent:400", "JustifyRight|RightIndent:400" };
        string[] totalStyle = new string[] { "LeftIndent:400", "JustifyRight|RightIndent:400|Borders:Top", "JustifyRight|RightIndent:400|Borders:Top", "JustifyRight|RightIndent:400|Borders:Top", "JustifyRight|RightIndent:400|Borders:Top", "JustifyRight|RightIndent:400|Borders:Top" };

        tbl.AddRow(new string[] { "", "Year 1", "Year 2", "Year 3", "Year 4", "Year 5" }, new string[] { "", "JustifyRight|RightIndent:400|LeftIndent:400|Background:2B579A|FontColor:FFFFFF|FontSize:24", "JustifyRight|RightIndent:400|Background:2B579A|FontColor:FFFFFF|FontSize:24", "JustifyRight|RightIndent:400|Background:2B579A|FontColor:FFFFFF|FontSize:24", "JustifyRight|RightIndent:400|Background:2B579A|FontColor:FFFFFF|FontSize:24", "JustifyRight|RightIndent:400|Background:2B579A|FontColor:FFFFFF|FontSize:24" });
        tbl.AddRow(new string[] { "Distributable Cash Flow Percent", ToPercent(distributeYear1), ToPercent(distributeYear2), ToPercent(distributeYear3), ToPercent(distributeYear4), ToPercent(distributeYear5) }, detailStyle);
        double[] distTotal = new double[] { (.01 * distributeYear1 * FiveYearCashFlow[0]), (.01 * distributeYear2 * FiveYearCashFlow[1]), (.01 * distributeYear3 * FiveYearCashFlow[2]), (.01 * distributeYear4 * FiveYearCashFlow[3]), (.01 * distributeYear5 * FiveYearCashFlow[4]) };
        tbl.AddRow(new string[] { "Distributable Cash Flow", distTotal[0].ToString("#,##0;(#,##0)"), distTotal[1].ToString("#,##0;(#,##0)"), distTotal[2].ToString("#,##0;(#,##0)"), distTotal[3].ToString("#,##0;(#,##0)"), distTotal[4].ToString("#,##0;(#,##0)") }, detailStyle);
        tbl.AddRow(new string[] { "", "", "", "", "", "" });

        double[] investmentDist = new double[5];
        double[] operatorDist = new double[5];
        double[] investmentLeft = new double[5];
        double[] annualReturn = new double[5];
        double paybackPeriod = 0;
        for (int i = 0; i < distTotal.Length; i++)
        {
            if(totalInvestment >= 0)
            {
                investmentDist[i] = distTotal[i] * .01 * (100 - cashBeforePayback);
                operatorDist[i] = distTotal[i] * .01 * (cashBeforePayback);
                paybackPeriod++;
                totalInvestment -= investmentDist[i];
                if(totalInvestment <= 0)
                {
                    double prevInvestment = totalInvestment + investmentDist[i];
                    double toZeroInvest = prevInvestment;
                    double toZeroOperate = prevInvestment * (cashBeforePayback / (100 - cashBeforePayback));
                    double afterZeroInvest = (distTotal[i] - toZeroInvest - toZeroOperate) * .01 * (100 - cashAfterPayback);
                    double afterZeroOperate = (distTotal[i] - toZeroInvest - toZeroOperate) * .01 * (cashAfterPayback);

                    investmentDist[i] = toZeroInvest + afterZeroInvest;
                    operatorDist[i] = toZeroOperate + afterZeroOperate;

                    paybackPeriod += prevInvestment / investmentDist[i] - 1;
                    if (double.IsNaN(paybackPeriod))
                        paybackPeriod = 0;
                }
            }
            else
            {
                investmentDist[i] = distTotal[i] * .01 * (100 - cashAfterPayback);
                operatorDist[i] = distTotal[i] * .01 * (cashAfterPayback);
            }
            investmentLeft[i] = totalInvestment >= 0 ? totalInvestment : 0;
            annualReturn[i] = 100 * investmentDist[i] / initialInvestment;
        }



        tbl.AddRow(new string[] { "Cash Distribution", "", "", "", "", "" }, headerStyle);
        tbl.AddRow(new string[] { "Investment Partner", (investmentDist[0]).ToString("#,##0;(#,##0)"), (investmentDist[1]).ToString("#,##0;(#,##0)"), (investmentDist[2]).ToString("#,##0;(#,##0)"), (investmentDist[3]).ToString("#,##0;(#,##0)"), (investmentDist[4]).ToString("#,##0;(#,##0)") }, detailStyle2);
        tbl.AddRow(new string[] { "Operating Partner", (operatorDist[0]).ToString("#,##0;(#,##0)"), (operatorDist[1]).ToString("#,##0;(#,##0)"), (operatorDist[2]).ToString("#,##0;(#,##0)"), (operatorDist[3]).ToString("#,##0;(#,##0)"), (operatorDist[4]).ToString("#,##0;(#,##0)") }, detailStyle2);
        tbl.AddRow(new string[] { "", "", "", "", "", "" });
        tbl.AddRow(new string[] { "Investment Returns", "", "", "", "", "" }, headerStyle);
        tbl.AddRow(new string[] { "Net Investment After Distribution", (investmentLeft[0]).ToString("#,##0;(#,##0)"), (investmentLeft[1]).ToString("#,##0;(#,##0)"), (investmentLeft[2]).ToString("#,##0;(#,##0)"), (investmentLeft[3]).ToString("#,##0;(#,##0)"), (investmentLeft[4]).ToString("#,##0;(#,##0)") }, detailStyle2);
        tbl.AddRow(new string[] { "Annual Return on Investment", ToPercent(annualReturn[0]), ToPercent(annualReturn[1]), ToPercent(annualReturn[2]), ToPercent(annualReturn[3]), ToPercent(annualReturn[4]) }, detailStyle2);
        tbl.AddRow(new string[] { "Average Annual Return", ToPercent((annualReturn[0] + annualReturn[1] + annualReturn[2] + annualReturn[3] + annualReturn[4]) / 5), "", "", "", "" }, totalStyle);
        tbl.AddRow(new string[] { "", "", "", "", "", "" });
        tbl.AddRow(new string[] { "Payback Period", paybackPeriod.ToString("0.#") + " years", "", "", "", "" }, new string[] { "", "Bold" });


    }

    static void FinancialsBreakEven(WordprocessingDocument wordDocument)
    {
        Body body = wordDocument.MainDocumentPart.Document.Body;

        body.AddHeader("Cash Flow Break Even");

        Table tbl = NewTable(body, 4);

        List<Question> investmentQuestions = Question.Get("Financials", "Investment", currentUserId);
        double minLabor = investmentQuestions.ByTitleSum(new string[] { "Minimum Hourly Labor Percentage" });

        List<Question> questions = Question.Get("Financials", "Expenses", currentUserId);

        string[] headerStyle = new string[] { "LeftIndent:200|Bold|Background:ABCDEF", "Bold|Background:ABCDEF|JustifyRight|RightIndent:800", "Bold|Background:ABCDEF|JustifyRight|RightIndent:600", "Bold|Background:ABCDEF|JustifyRight|RightIndent:600" };
        string[] detailStyle = new string[] { "LeftIndent:200", "JustifyRight|RightIndent:600", "JustifyRight|RightIndent:600", "JustifyRight|RightIndent:600" };
        string[] breakEvenStyle = new string[] { "LeftIndent:200|Background:E7E6E6|FontSize:26", "JustifyRight|RightIndent:600|Background:E7E6E6|FontSize:26", "JustifyRight|RightIndent:600|Background:E7E6E6|FontSize:26", "JustifyRight|RightIndent:600|Background:E7E6E6|FontSize:26" };
        string[] totalStyle = new string[] { "LeftIndent:200|Bold", "JustifyRight|RightIndent:600", "Borders:Top|JustifyRight|RightIndent:600", "Borders:Top|JustifyRight|RightIndent:600" };

        tbl.AddRow(new string[] { "Fixed Costs", "", "Annual", "Monthly" }, headerStyle);

        double fixedExpenses = 0;
        double management = questions.BySectionSum("Management Salaries (Annual)");
        tbl.AddRow(AddBreakEvenRow("Total Management Salaries", management), detailStyle);
        fixedExpenses += management;

        tbl.AddRow(AddBreakEvenRow("Minimum Hourly Labor", YearlyHourlyCosts * minLabor * .01), detailStyle);
        fixedExpenses += YearlyHourlyCosts * minLabor * .01;

        double totalGrossPay = management + (YearlyHourlyCosts * minLabor * .01);
        double totalBenefits = 0;
        double benefitsPct = questions.BySectionSum("Employee Benefits", 0, 5);
        double yearlyBenefits = benefitsPct * .01 * totalGrossPay;
        totalBenefits += yearlyBenefits;

        double benefitsMonthly = questions.BySectionSum("Employee Benefits", 5, 1);
        double numberEmployees = questions.BySectionSum("Employee Benefits", 6, 1);
        yearlyBenefits = benefitsMonthly * numberEmployees * 12;
        totalBenefits += yearlyBenefits;

        benefitsMonthly = questions.BySectionSum("Employee Benefits", 7);
        yearlyBenefits = benefitsMonthly * 12;
        totalBenefits += yearlyBenefits;

        tbl.AddRow(AddBreakEvenRow("Employee Benefits", totalBenefits), detailStyle); 
        fixedExpenses += totalBenefits;

        string[] controllableSections = new string[] { "Direct Operating Expenses", "Music & Entertainment", "Marketing", "Utilities", "General & Administrative", "Repairs and Maintenance" };
        List<Question> expenses = questions.FindAll(delegate(Question q)
        {
            return controllableSections.Contains(q.Section);
        });

        double expenseSum = 0;
        double sectionSum = 0;
        double creditCardPct = 0;
        for (int i = 0, ii = expenses.Count; i < ii; i++)
        {
            if (expenses[i].Title == "Credit Card Charges")
            {
                double ccPct = 0, ccRate = 0;
                double.TryParse(questions.ByTitle("Percentages of Credit Card Sales"), out ccPct);
                double.TryParse(questions.ByTitle("Average Discount Percentage"), out ccRate);
                creditCardPct = ccPct * .01 * ccRate * .01;
                continue;
            }
            int val = 0;
            if (int.TryParse(expenses[i].Answer.Text, out val))
            {
                sectionSum += val;
                expenseSum += val;
            }
            if (i >= ii - 1 || expenses[i].Section != expenses[i + 1].Section)
            {
                tbl.AddRow(AddBreakEvenRow(expenses[i].Section, sectionSum * 12), detailStyle);
                sectionSum = 0;
            }
        }
        fixedExpenses += expenseSum * 12;


        expenses = questions.FindAll(delegate(Question q) { return q.Section == "Occupancy Costs"; });
        expenseSum = 0;
        for (int i = 0, ii = expenses.Count; i < ii; i++)
        {
            if (expenses[i].Title == "Percentage Rent - Percentage amount")
            {
                i++;
                continue;
            }

            int val = 0;
            if (int.TryParse(expenses[i].Answer.Text, out val))
            {
                expenseSum += val;
            }
        }
        fixedExpenses += expenseSum * 12;
        tbl.AddRow(AddBreakEvenRow("Occupancy Costs", expenseSum * 12), detailStyle);

        double principal = Principal;
        double loanRate = .01 * questions.ByTitleSum(new string[] { "Rate %" });
        double term = questions.ByTitleSum(new string[] { "Term (years)" });
        double monthlyPayment = (loanRate / 12 * principal) / (1 - Math.Pow(1 + loanRate / 12, -1 * term * 12));
        if (double.IsNaN(monthlyPayment))
            monthlyPayment = 0;
        double interest = 0;
        double loanPrincipalPayments = 0;
        for (int i = 0; i < 12; i++)
        {
            double monthlyInterest = principal * loanRate / 12;
            interest += monthlyInterest;
            loanPrincipalPayments += (monthlyPayment - monthlyInterest);
            principal -= (monthlyPayment - monthlyInterest);
        }
        fixedExpenses += interest + loanPrincipalPayments;
        tbl.AddRow(AddBreakEvenRow("Interest", interest), detailStyle);
        tbl.AddRow(AddBreakEvenRow("Loan Principal Payments", loanPrincipalPayments), detailStyle);
        tbl.AddRow(AddBreakEvenRow("TOTAL", fixedExpenses), totalStyle);
        tbl.AddRow(new string[] { "", "", "", "" });


        tbl.AddRow(new string[] { "Variable Costs", "", "% of Sales", "Monthly" }, headerStyle);

        double foodCostPct = double.Parse(questions.ByTitle("Food Cost %", "0"));
        double liquorCostPct = double.Parse(questions.ByTitle("Liquor Cost %", "0"));
        double beerCostPct = double.Parse(questions.ByTitle("Beer Cost %", "0"));
        double wineCostPct = double.Parse(questions.ByTitle("Wine Cost %", "0"));
        double yearlyCost = (foodCostPct * WeeklyFood + liquorCostPct * WeeklyLiquor + beerCostPct * WeeklyBeer + wineCostPct * WeeklyWine) / 100 * 52;
        double yearlyCostPct = YearlyTotalSales > 0 ? yearlyCost / YearlyTotalSales : 0;
        double hourlyLaborPct = YearlyTotalSales > 0 ? YearlyHourlyCosts * (1 - minLabor * .01) / YearlyTotalSales : 0;
        benefitsPct = benefitsPct * .01 * hourlyLaborPct;

        double variableCostPct = yearlyCostPct + hourlyLaborPct + benefitsPct + creditCardPct;
        double breakEvenYearlySales = fixedExpenses / (1 - variableCostPct);

        double percentageRent = 0, percentageRentAbove = 0, percentageRentYearly = 0;
        double.TryParse(questions.ByTitle("Percentage Rent - Percentage amount"), out percentageRent);
        percentageRent *= .01;
        double.TryParse(questions.ByTitle("Percentage Rent - On annual sales above"), out percentageRentAbove);
        if (breakEvenYearlySales > percentageRentAbove)
        {
            breakEvenYearlySales = percentageRentAbove + ((fixedExpenses - (percentageRentAbove / (fixedExpenses / (1 - variableCostPct)) * fixedExpenses)) / (1 - variableCostPct - percentageRent));
            percentageRentYearly = (breakEvenYearlySales - percentageRentAbove) * percentageRent;
        }

        double breakEvenMonthlySales = breakEvenYearlySales / 12;
        string[] costOfSalesRow = new string[] { "Cost of Sales", "", ToPercent(yearlyCostPct * 100), (yearlyCostPct * breakEvenMonthlySales).ToString("#,##0;(#,##0)") };
        string[] hourlyLaborRow = new string[] { "Hourly Labor", "", ToPercent(hourlyLaborPct * 100), (hourlyLaborPct * breakEvenMonthlySales).ToString("#,##0;(#,##0)") };
        string[] employeeBenefitsRow = new string[] { "Employee Benefits", "", ToPercent(benefitsPct * 100), (benefitsPct * breakEvenMonthlySales).ToString("#,##0;(#,##0)") };
        string[] ccRow = new string[] { "Credit Card Expense", "", ToPercent(creditCardPct * 100), (creditCardPct * breakEvenMonthlySales).ToString("#,##0;(#,##0)") };

        tbl.AddRow(costOfSalesRow, detailStyle);
        tbl.AddRow(hourlyLaborRow, detailStyle);
        tbl.AddRow(employeeBenefitsRow, detailStyle);
        tbl.AddRow(ccRow, detailStyle);

        tbl.AddRow(new string[] { "TOTAL", "", ToPercent(variableCostPct * 100), (variableCostPct * breakEvenMonthlySales).ToString("#,##0;(#,##0)") }, totalStyle);
        tbl.AddRow(new string[] { "", "", "", "" });

        tbl.AddRow(new string[] { "", "", "Annual", "Monthly" }, headerStyle);
        tbl.AddRow(new string[] { "Percentage Rent", "", (percentageRentYearly).ToString("#,##0"), (percentageRentYearly / 12).ToString("#,##0"), }, detailStyle);
        tbl.AddRow(new string[] { "", "", "", "" });

        tbl.AddRow(new string[] { "", "Annual", "Monthly", "Weekly" }, headerStyle);
        tbl.AddRow(new string[] { "Break even Sales", (breakEvenYearlySales).ToString("#,##0"), (breakEvenMonthlySales).ToString("#,##0"), (breakEvenYearlySales / 52).ToString("#,##0"), }, breakEvenStyle);
        tbl.AddRow(new string[] { "", "", "", "" });


        double weeklyTotal = WeeklyFood + WeeklyLiquor + WeeklyBeer + WeeklyWine;
        double foodPct = weeklyTotal > 0 ? WeeklyFood / weeklyTotal : 0;
        double liquorPct =  weeklyTotal > 0 ? WeeklyLiquor / weeklyTotal : 0;
        double beerPct =  weeklyTotal > 0 ? WeeklyBeer / weeklyTotal : 0;
        double winePct = weeklyTotal > 0 ? WeeklyWine / weeklyTotal : 0;

        tbl.AddRow(new string[] { "Sales Break down", "Annual", "Monthly", "Weekly" }, headerStyle);
        tbl.AddRow(new string[] { "Food", (breakEvenYearlySales * foodPct).ToString("#,##0"), (breakEvenMonthlySales * foodPct).ToString("#,##0"), (breakEvenYearlySales * foodPct / 52).ToString("#,##0"), }, detailStyle);
        tbl.AddRow(new string[] { "Liquor", (breakEvenYearlySales * liquorPct).ToString("#,##0"), (breakEvenMonthlySales * liquorPct).ToString("#,##0"), (breakEvenYearlySales * liquorPct / 52).ToString("#,##0"), }, detailStyle);
        tbl.AddRow(new string[] { "Beer", (breakEvenYearlySales * beerPct).ToString("#,##0"), (breakEvenMonthlySales * beerPct).ToString("#,##0"), (breakEvenYearlySales * beerPct / 52).ToString("#,##0"), }, detailStyle);
        tbl.AddRow(new string[] { "Wine", (breakEvenYearlySales * winePct).ToString("#,##0"), (breakEvenMonthlySales * winePct).ToString("#,##0"), (breakEvenYearlySales * winePct / 52).ToString("#,##0"), }, detailStyle);
        tbl.AddRow(new string[] { "TOTAL", (breakEvenYearlySales).ToString("#,##0"), (breakEvenMonthlySales).ToString("#,##0"), (breakEvenYearlySales / 52).ToString("#,##0"), }, new string[] { "LeftIndent:200|Bold", "Borders:Top|JustifyRight|RightIndent:600", "Borders:Top|JustifyRight|RightIndent:600", "Borders:Top|JustifyRight|RightIndent:600" });

    }

    static string[] SumRows(string title, params string[][] rows)
    {
        string[] returnVal = new string[11];
        returnVal[0] = title;
        for(int i = 1; i < 11; i++)
        {
            double sum = 0;
            foreach(string[] row in rows)
            {
                string rowVal = row[i];
                double sign = 1;
                if (rowVal.Contains("%"))
                    rowVal = rowVal.Replace("%", "");
                if (rowVal.Contains("("))
                {
                    rowVal = rowVal.Replace("(", "").Replace(")", "");
                    sign = -1;
                }
                double val = 0;
                if (double.TryParse(rowVal, out val))
                    sum += sign * val;
            }
            if(i % 2 == 1)
                returnVal[i] = sum.ToString("#,##0;(#,##0)");
            else
                returnVal[i] = ToPercent(sum);
        }
        return returnVal;
    }

    static string[] DifferenceRows(string title, params string[][] rows)
    {
        string[] returnVal = new string[11];
        returnVal[0] = title;
        for (int i = 1; i < 11; i++)
        {
            double sum = 0;
            int j = 0;
            foreach (string[] row in rows)
            {
                string rowVal = row[i];
                double sign = 1;
                if (rowVal.Contains("%"))
                    rowVal = rowVal.Replace("%", "");
                if (rowVal.Contains("("))
                {
                    rowVal = rowVal.Replace("(", "").Replace(")", "");
                    sign = -1;
                }
                double val = 0;
                if (double.TryParse(rowVal, out val))
                {
                    if (j == 0)
                        sum += sign * val;
                    else
                        sum -= sign * val;
                }
                j++;
            }
            if (i % 2 == 1)
                returnVal[i] = sum.ToString("#,##0;(#,##0)");
            else
                returnVal[i] = ToPercent(sum);
        }
        return returnVal;
    }

    static string[] Add5YearRow(string title, double yearlyVal, double pct)
    {
        if (double.IsNaN(yearlyVal))
            yearlyVal = 0;
        if (double.IsNaN(pct))
            pct = 0;
        
        string[] row = new string[11];
        row[0] = title;
        for(int i = 1; i < 11; i++)
        {
            if(i % 2 == 1)
            {
                row[i] = (yearlyVal * Math.Pow(1 + pct / 100, i / 2)).ToString("#,##0;(#,##0)");
            }
            else
            {
                double val = 0;
                string getVal = row[i - 1];
                int sign = 1;
                if (getVal.Contains("(") && getVal.Contains(")"))
                {
                    getVal = getVal.Replace("(", "").Replace(")", "");
                    sign = -1;
                }
                double.TryParse(getVal, out val);
                row[i] = ToPercent(sign * 100 * val / FiveYearSales[(i / 2) - 1]);
            }
        }
        return row;
    }

    static string[] AddIncomeRow(string title, double yearlyVal)
    {
        if (double.IsNaN(yearlyVal))
            yearlyVal = 0;

        string[] row = new string[5];
        row[0] = title;
        row[1] = (yearlyVal / 12).ToString("#,##0;(#,##0)");
        row[2] = PctOfYearlySales(yearlyVal);
        row[3] = yearlyVal.ToString("#,##0;(#,##0)");
        row[4] = PctOfYearlySales(yearlyVal);
        return row;
    }

    static string[] AddBreakEvenRow(string title, double yearlyVal)
    {
        if (double.IsNaN(yearlyVal))
            yearlyVal = 0;

        string[] row = new string[4];
        row[0] = title;
        row[1] = "";
        row[2] = yearlyVal.ToString("#,##0;(#,##0)");
        row[3] = (yearlyVal / 12).ToString("#,##0;(#,##0)");
        return row;
    }

    static void AddExpenseRow(Table tbl, Question question, string[] style)
    {
        double monthly = 0;
        double.TryParse(question.Answer.Text, out monthly);
        double yearly = monthly * 12;
        tbl.AddRow(new string[] { question.Title, monthly.ToString("#,##0"), PctOfYearlySales(yearly), yearly.ToString("#,##0"), PctOfYearlySales(yearly) }, style);
    }

    static string PctOfYearlySales(double val)
    {
        return ToPercent(100 * val / YearlyTotalSales);
    }

    static string ToPercent(double val)
    {
        if (double.IsNaN(val))
            return "0%";

        return val == 100 ? "100%" : val.ToString("0.#;(0.#)") + "%";
    }

    static string GetDay(int i)
    {
        string day = "Monday";
        switch (i)
        {
            case 1:
                day = "Tuesday";
                break;
            case 2:
                day = "Wednesday";
                break;
            case 3:
                day = "Thursday";
                break;
            case 4:
                day = "Friday";
                break;
            case 5:
                day = "Saturday";
                break;
            case 6:
                day = "Sunday";
                break;
        }
        return day;
    }

    static Table NewTable(Body body, int columnCt)
    {
        // Create a table.
        Table tbl = new Table();

        // Set the style and width for the table.
        TableProperties tableProp = new TableProperties();
        TableStyle tableStyle = new TableStyle() { Val = "TableGrid" };

        // Make the table width 100% of the page width.
        TableWidth tableWidth = new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct };

        // Apply
        tableProp.Append(tableStyle, tableWidth);
        tbl.AppendChild(tableProp);

        // Add columns to the table.
        TableGrid tg = new TableGrid();
        for (int i = 0; i < columnCt; i++)
        {
            tg.AppendChild(new GridColumn());
        }
        tbl.AppendChild(tg);
        body.AppendChild(tbl);
        return tbl;
    }
}


public static class BodyPart
{
    public static void AddHeader(this Body body, string text)
    {
        Paragraph para = body.AppendChild(new Paragraph());
        ParagraphProperties paraProperties = para.AppendChild(new ParagraphProperties());

        paraProperties.AppendChild(new Justification() { Val = JustificationValues.Center });

        Run run = para.AppendChild(new Run());
        RunProperties runProperties = run.AppendChild(new RunProperties());

        //runProperties.AppendChild(new Bold() { Val = OnOffValue.FromBoolean(true) });

        runProperties.Append(new FontSize() { Val = new StringValue("56") });

        run.AppendChild(new Text(text));
        run.AppendChild(new Break());
    }

    public static void AddParagraph(this Body body, string text)
    {
        Paragraph para = body.AppendChild(new Paragraph());
        ParagraphProperties paraProperties = para.AppendChild(new ParagraphProperties());

        Run run = para.AppendChild(new Run());
        RunProperties runProperties = run.AppendChild(new RunProperties());
        runProperties.Append(new FontSize() { Val = new StringValue("24") });

        run.AppendChild(new Text(text));
        run.AppendChild(new Break());
    }

    public static void AddLineBreak(this Body body)
    {
        body.AppendChild(new Paragraph(new Run(new Break() )));
    }

    public static void AddPageBreak(this Body body)
    {
        body.AppendChild(new Paragraph(new Run(new Break(){ Type = BreakValues.Page })));
    }
}

public static class TablePart
{
    public static void AddRow(this Table table, string[] cells, string[] styles = null, bool singleSpace = true)
    {
        TableRow tr = new TableRow();
        for (int i = 0; i < cells.Length; i++)
        {
            TableCell tc = new TableCell();
            if(cells[i].Contains("|"))
            {
                Paragraph para = tc.AppendChild(new Paragraph(new ParagraphProperties()));
                Run run = para.AppendChild(new Run(new RunProperties()));

                string[] values = cells[i].Split('|');
                for(int j = 0, jj = values.Length; j < jj; j++)
                {
                    if(j != 0)
                        run.AppendChild(new Break());

                    run.AppendChild(new Text(values[j])); 
                }
            }
            else
                tc.AppendChild(new Paragraph(new ParagraphProperties(), new Run(new RunProperties(), new Text(cells[i]))));

            tr.Append(tc);

            foreach (OpenXmlElement els in tc.Elements())
            {
                if (els.GetType() == typeof(Paragraph))
                {
                    Paragraph para = (Paragraph)els;
                    if(singleSpace)
                        para.ParagraphProperties.AppendChild(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" });
                    else
                        para.ParagraphProperties.AppendChild(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "240", After = "0" });
                }
            }
            if (styles != null && styles.Length > i && !string.IsNullOrEmpty(styles[i]))
            {
                TableCellProperties props = new TableCellProperties();
                tc.Append(props);
                if (styles[i].Contains("LeftIndent"))
                {
                    string value = styles[i].Substring(styles[i].IndexOf("LeftIndent:") + 11);
                    if (value.Contains("|"))
                        value = value.Substring(0, value.IndexOf("|"));
                    props.LeftIndent(value);
                }
                if (styles[i].Contains("RightIndent"))
                {
                    string value = styles[i].Substring(styles[i].IndexOf("RightIndent:") + 12);
                    if (value.Contains("|"))
                        value = value.Substring(0, value.IndexOf("|"));
                    props.RightIndent(value);
                }
                if (styles[i].Contains("TopIndent"))
                {
                    string value = styles[i].Substring(styles[i].IndexOf("TopIndent:") + 10);
                    if (value.Contains("|"))
                        value = value.Substring(0, value.IndexOf("|"));
                    props.TopIndent(value);
                }
                if (styles[i].Contains("Background"))
                {
                    string value = styles[i].Substring(styles[i].IndexOf("Background:") + 11);
                    if (value.Contains("|"))
                        value = value.Substring(0, value.IndexOf("|"));
                    props.BackgroundColor(value);
                }
                if (styles[i].Contains("Bold"))
                    tc.Bold();
                if (styles[i].Contains("JustifyRight"))
                    tc.Right();
                if (styles[i].Contains("JustifyCenter"))
                    tc.Center();
                if(styles[i].Contains("FontSize"))
                {
                    string value = styles[i].Substring(styles[i].IndexOf("FontSize:") + 9);
                    if (value.Contains("|"))
                        value = value.Substring(0, value.IndexOf("|"));
                    tc.FontSize(value);
                }
                if (styles[i].Contains("FontColor"))
                {
                    string value = styles[i].Substring(styles[i].IndexOf("FontColor:") + 10);
                    if (value.Contains("|"))
                        value = value.Substring(0, value.IndexOf("|"));
                    tc.FontColor(value);
                }
                if (styles[i].Contains("VerticalText"))
                {
                    string value = styles[i].Substring(styles[i].IndexOf("VerticalText:") + 13);
                    if (value.Contains("|"))
                        value = value.Substring(0, value.IndexOf("|"));
                    props.VerticalText(value);
                }
                if (styles[i].Contains("Borders"))
                {
                    string value = styles[i].Substring(styles[i].IndexOf("Borders:") + 8);
                    if (value.Contains("|"))
                        value = value.Substring(0, value.IndexOf("|"));
                    props.Borders(value);
                }
                if (styles[i].Contains("VerticalMerge"))
                {
                    string value = styles[i].Substring(styles[i].IndexOf("VerticalMerge:") + 14);
                    if (value.Contains("|"))
                        value = value.Substring(0, value.IndexOf("|"));
                    props.VertMerge(value);
                }
                if (styles[i].Contains("HorizontalMerge"))
                {
                    string value = styles[i].Substring(styles[i].IndexOf("HorizontalMerge:") + 16);
                    if (value.Contains("|"))
                        value = value.Substring(0, value.IndexOf("|"));
                    props.HorizMerge(value);
                }
            }
        }

        table.AppendChild(tr);
    }
}

public static class TableCellProps
{
    public static void LeftIndent(this TableCellProperties prop, string value)
    {
        prop.Append(new TableCellMargin(new LeftMargin { Type = TableWidthUnitValues.Dxa, Width = value }));
    }
    public static void RightIndent(this TableCellProperties prop, string value)
    {
        prop.Append(new TableCellMargin(new RightMargin { Type = TableWidthUnitValues.Dxa, Width = value }));
    }
    public static void TopIndent(this TableCellProperties prop, string value)
    {
        prop.Append(new TableCellMargin(new TopMargin { Type = TableWidthUnitValues.Dxa, Width = value }));
    }
    public static void BackgroundColor(this TableCellProperties prop, string color)
    {
        prop.Append(new Shading() { Color = "auto", Fill = color, Val = ShadingPatternValues.Clear });
    }
    public static void Borders(this TableCellProperties prop, string sides)
    {
        TableCellBorders tblBorders = new TableCellBorders();
        foreach(string side in sides.Split(':'))
        {
            if(side == "Top")
                tblBorders.AppendChild(new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Thick), Color = "000000" });
            if (side == "Right")
                tblBorders.AppendChild(new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Thick), Color = "000000" });
            if (side == "Bottom")
                tblBorders.AppendChild(new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Thick), Color = "000000" });
            if (side == "Left")
                tblBorders.AppendChild(new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Thick), Color = "000000" });

        }
        prop.Append(tblBorders);
    }
    public static void VerticalText(this TableCellProperties prop, string width)
    {
        prop.Append(new TextDirection() { Val = TextDirectionValues.TopToBottomRightToLeft });
        TableRowProperties trProp = new TableRowProperties();
        uint wd = 1500;
        uint.TryParse(width, out wd);
        trProp.Append(new TableRowHeight() { Val = new UInt32Value(wd) });
        prop.Parent.Parent.Append(trProp);
    }
    public static void VertMerge(this TableCellProperties prop, string val)
    {
        if(val == "Continue")
            prop.Append(new VerticalMerge() { Val = MergedCellValues.Continue });
        else
            prop.Append(new VerticalMerge() { Val = MergedCellValues.Restart });
    }
    public static void HorizMerge(this TableCellProperties prop, string val)
    {
        if (val == "Continue")
            prop.Append(new HorizontalMerge() { Val = MergedCellValues.Continue });
        else
            prop.Append(new HorizontalMerge() { Val = MergedCellValues.Restart });
    }
}

public static class TableCellClass
{
    public static void Bold(this TableCell tc)
    {
        foreach (OpenXmlElement els in tc.Elements())
        {

            foreach (OpenXmlElement el in els)
            {
                if (el.GetType() == typeof(Run))
                {
                    Run run = (Run)el;
                    run.RunProperties.AppendChild(new Bold() { Val = OnOffValue.FromBoolean(true) });
                }
            }
        }
    }
    public static void FontSize(this TableCell tc, string value)
    {
        foreach (OpenXmlElement els in tc.Elements())
        {
            foreach (OpenXmlElement el in els)
            {
                if (el.GetType() == typeof(Run))
                {
                    Run run = (Run)el;
                    run.RunProperties.AppendChild(new FontSize() { Val = value });
                }
            }
        }
    }
    public static void FontColor(this TableCell tc, string value)
    {
        foreach (OpenXmlElement els in tc.Elements())
        {
            foreach (OpenXmlElement el in els)
            {
                if (el.GetType() == typeof(Run))
                {
                    Run run = (Run)el;
                    run.RunProperties.AppendChild(new Color() { Val = value });
                }
            }
        }
    }
    public static void Center(this TableCell tc)
    {
        foreach (OpenXmlElement els in tc.Elements())
        {
            if (els.GetType() == typeof(Paragraph))
            {
                Paragraph para = (Paragraph)els;
                para.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Center });
            }
        }
    }
    public static void Right(this TableCell tc)
    {
        foreach (OpenXmlElement els in tc.Elements())
        {
            if (els.GetType() == typeof(Paragraph))
            {
                Paragraph para = (Paragraph)els;
                para.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Right });
            }
        }
    }
}