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

    //public static void Sample(MemoryStream mem)
    //{
    //    // Create Document
    //    using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(mem,
    //    WordprocessingDocumentType.Document, true))
    //    {
    //        // Add a main document part. 
    //        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

    //        // Create the document structure and add some text.
    //        mainPart.Document = new Document();
    //        mainPart.AddParagraph("this sample");

    //    }
    //}

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

            foreach(string text in Concept.GetDoc())
            {
                body.AddParagraph(text);
            }
            

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

            //FinancialsOverview(wordDocument);
            //body.AddPageBreak();
            //FinancialsCapitalBudget(wordDocument);
            //body.AddPageBreak();
            FinancialsSalesProjection(wordDocument);
        }
    }

    public static void FinancialsOverview(WordprocessingDocument wordDocument)
    {
        Body body = wordDocument.MainDocumentPart.Document.Body;

        body.AddHeader("Financials Overview");
        
        Table tbl = NewTable(body, 3);

        tbl.AddRow(new string[] { "SOURCES OF CASH", "", "" }, new string[] { "Bold" });
        tbl.AddRow(new string[] { "Equity Contributions", "400,000", "" }, new string[] { "LeftIndent:400", "JustifyRight|RightIndent:1000" });
        tbl.AddRow(new string[] { "Loan Financing", "677,675", "" }, new string[] { "LeftIndent:400", "JustifyRight|RightIndent:1000" });
        tbl.AddRow(new string[] { "TOTAL SOURCES OF CASH", "", "1,077,675" }, new string[] { "Bold|Background:ABCDEF", "Background:ABCDEF", "Background:ABCDEF|JustifyRight|RightIndent:1000" });
        tbl.AddRow(new string[] { "", "", "" });
        tbl.AddRow(new string[] { "USES OF CASH", "", "" }, new string[] { "Bold" });
        tbl.AddRow(new string[] { "Land & Building", "0", "" }, new string[] { "LeftIndent:400", "JustifyRight|RightIndent:1000" });
        tbl.AddRow(new string[] { "Leasehold Improvements", "400,000", "" }, new string[] { "LeftIndent:400", "JustifyRight|RightIndent:1000" });
        tbl.AddRow(new string[] { "Bar / Kitchen Equipment", "175,000", "" }, new string[] { "LeftIndent:400", "JustifyRight|RightIndent:1000" });
        tbl.AddRow(new string[] { "Bar / Dining Room Furniture", "75,000", "" }, new string[] { "LeftIndent:400", "JustifyRight|RightIndent:1000" });
        tbl.AddRow(new string[] { "Professional Services", "19,500", "" }, new string[] { "LeftIndent:400", "JustifyRight|RightIndent:1000" });
        tbl.AddRow(new string[] { "Organizational & Development", "34,475", "" }, new string[] { "LeftIndent:400", "JustifyRight|RightIndent:1000" });
        tbl.AddRow(new string[] { "Interior Finishes and Equipment", "66,500", "" }, new string[] { "LeftIndent:400", "JustifyRight|RightIndent:1000" });
        tbl.AddRow(new string[] { "Exterior Finishes and Equipment", "48,500", "" }, new string[] { "LeftIndent:400", "JustifyRight|RightIndent:1000" });
        tbl.AddRow(new string[] { "Pre-Opening Expenses", "108,700", "" }, new string[] { "LeftIndent:400", "JustifyRight|RightIndent:1000" });
        tbl.AddRow(new string[] { "Working Capital and Contingency", "150,000", "" }, new string[] { "LeftIndent:400", "JustifyRight|RightIndent:1000" });
        tbl.AddRow(new string[] { "TOTAL USES OF CASH", "", "1,077,675" }, new string[] { "Bold|Background:ABCDEF", "Background:ABCDEF", "Background:ABCDEF|JustifyRight|RightIndent:1000" });

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

        List<Question> questions = Question.Get("Financials", "Capital Budget", 1 /*TODO: User Id*/);
        for (int i = 0, ii = questions.Count; i < ii; i++)
        {
            if(i == 0)
            {
                tbl.AddRow(new string[] { questions[i].Section, "", "" }, header);
            }
            else if(i == questions.Count - 1)
            {
                tbl.AddRow(new string[] { "", "", sum.ToString("#,##0;(#,##0)") }, sumStyle, false);
                break;
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
                questions[i].Answer.Text = val.ToString("#,###;(#,###)");
                sum += val;
            }
            tbl.AddRow(new string[] { questions[i].Title, questions[i].Answer.Text, "" }, odd ? detailOdd : detailEven);

            odd = !odd;

        }
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

        string[] header = new string[] { "FontSize:36|Bold|JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:AAAAAA", "JustifyCenter|Background:AAAAAA", "JustifyCenter|Background:AAAAAA", "JustifyCenter|Background:AAAAAA", "JustifyCenter|Background:AAAAAA", "JustifyCenter|Background:AAAAAA" };
        string[] sumStyle = new string[] { "", "", "Bold" };
        double foodSum = 0;
        double liquorSum = 0;
        double beerSum = 0;
        double wineSum = 0;

        List<Question> questions = Question.Get("Financials", "Sales Projection", 1 /*TODO: User Id*/);
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
        tbl.AddRow(new string[] { "TOTALS", "", "", foodSum.ToString("0.00"), bevSum.ToString("0.00"), (foodSum + bevSum).ToString("0.00") }, new string[] { "Bold", "JustifyCenter|Bold", "JustifyCenter|Bold", "JustifyCenter|Bold", "JustifyCenter|Bold", "JustifyCenter|Bold", "JustifyCenter|Bold" });

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

    static void TableTurns(Body body, double[] breakfastSums, double[] lunchSums, double [] dinnerSums)
    {
        body.AddHeader("Sales Projections - Typical Week");

        Table tbl = NewTable(body, 9);

        string[] header = new string[] { "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF", "JustifyCenter|Background:ABCDEF" };
        string[] sumStyle = new string[] { "", "", "Bold" };
        double foodSum = 0;
        double bevSum = 0;

        List<Question> questions = Question.Get("Financials", "Sales Projection", 1 /*TODO: User Id*/);
        tbl.AddRow(new string[] { "", "", "Table|Turns", "Covers", "Food", "Liquor", "Beer", "Wine", "Total" }, header);
        
        double numberSeats = 150;
        double totalFood = 0;
        double totalLiquor = 0;
        double totalBeer = 0;
        double totalWine = 0;

        for (int i = 0; i < 7; i++)
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

            string[] style = new string[] { "JustifyCenter|Bold", "Bold", "JustifyRight|RightIndent:300", "JustifyRight|RightIndent:300", "JustifyRight|RightIndent:300", "JustifyRight|RightIndent:300", "JustifyRight|RightIndent:300", "JustifyRight|RightIndent:300", "JustifyRight|RightIndent:300", "JustifyRight|RightIndent:300" };

            double turns = 0;
            double.TryParse(questions[i + 40].Answer.Text, out turns);
            double covers = Math.Floor(turns * numberSeats);
            double breakfastFood = covers * breakfastSums[0];
            double breakfastLiquor = covers * breakfastSums[1];
            double breakfastBeer = covers * breakfastSums[2];
            double breakfastWine = covers * breakfastSums[3];
            tbl.AddRow(new string[] { "", "Breakfast", turns.ToString(), covers.ToString(), breakfastFood.ToString("#,##0;(#,##0)"), breakfastLiquor.ToString("#,##0;(#,##0)"), breakfastBeer.ToString("#,##0;(#,##0)"), breakfastWine.ToString("#,##0;(#,##0)"), (breakfastFood + breakfastLiquor + breakfastBeer + breakfastWine).ToString("#,##0;(#,##0)") }, style);

            double.TryParse(questions[i + 47].Answer.Text, out turns);
            covers = Math.Floor(turns * numberSeats);
            double lunchFood = covers * lunchSums[0];
            double lunchLiquor = covers * lunchSums[1];
            double lunchBeer = covers * lunchSums[2];
            double lunchWine = covers * lunchSums[3];
            tbl.AddRow(new string[] { day, "Lunch", turns.ToString(), covers.ToString(), lunchFood.ToString("#,##0;(#,##0)"), lunchLiquor.ToString("#,##0;(#,##0)"), lunchBeer.ToString("#,##0;(#,##0)"), lunchWine.ToString("#,##0;(#,##0)"), (lunchFood + lunchLiquor + lunchBeer + lunchWine).ToString("#,##0;(#,##0)") }, style);

            double.TryParse(questions[i + 54].Answer.Text, out turns);
            covers = Math.Floor(turns * numberSeats);
            double dinnerFood = covers * dinnerSums[0];
            double dinnerLiquor = covers * dinnerSums[1];
            double dinnerBeer = covers * dinnerSums[2];
            double dinnerWine = covers * dinnerSums[3];
            tbl.AddRow(new string[] { "", "Dinner", turns.ToString(), covers.ToString(), dinnerFood.ToString("#,##0;(#,##0)"), dinnerLiquor.ToString("#,##0;(#,##0)"), dinnerBeer.ToString("#,##0;(#,##0)"), dinnerWine.ToString("#,##0;(#,##0)"), (dinnerFood + dinnerLiquor + dinnerBeer + dinnerWine).ToString("#,##0;(#,##0)") }, style);

            totalFood += breakfastFood + lunchFood + dinnerFood;
            totalLiquor += breakfastLiquor + lunchLiquor + dinnerLiquor;
            totalBeer += breakfastBeer + lunchBeer + dinnerBeer;
            totalWine += breakfastWine + lunchWine + dinnerWine;
            tbl.AddRow(new string[] { "", "TOTALS", "", "", (breakfastFood + lunchFood + dinnerFood).ToString("#,##0;(#,##0)"), (breakfastLiquor + lunchLiquor + dinnerLiquor).ToString("#,##0;(#,##0)"), (breakfastBeer + lunchBeer + dinnerBeer).ToString("#,##0;(#,##0)"), (breakfastWine + lunchWine + dinnerWine).ToString("#,##0;(#,##0)"), (breakfastFood + lunchFood + dinnerFood + breakfastLiquor + lunchLiquor + dinnerLiquor + breakfastBeer + lunchBeer + dinnerBeer + breakfastWine + lunchWine + dinnerWine).ToString("#,##0;(#,##0)") }, style);
            tbl.AddRow(new string[] { ""});
        }

        string[] totalStyle = new string[] { "JustifyCenter|Bold|FontSize:28", "", "", "", "JustifyRight|RightIndent:100|FontSize:28", "JustifyRight|RightIndent:100|FontSize:28", "JustifyRight|RightIndent:100|FontSize:28", "JustifyRight|RightIndent:100|FontSize:28", "JustifyRight|RightIndent:100|Bold|FontSize:28" };
        tbl.AddRow(new string[] { "TOTALS", "", "", "", totalFood.ToString(), totalLiquor.ToString(), totalBeer.ToString(), totalWine.ToString(), (totalFood + totalLiquor + totalBeer + totalWine).ToString() }, totalStyle);

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

            if(singleSpace)
            {
                foreach (OpenXmlElement els in tc.Elements())
                {
                    if (els.GetType() == typeof(Paragraph))
                    {
                        Paragraph para = (Paragraph)els;
                        para.ParagraphProperties.AppendChild(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" });
                    }
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
                //if (styles[i].Contains("BorderBottom"))
                //    props.BorderBottom();
            }

            tr.Append(tc);
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
    public static void BackgroundColor(this TableCellProperties prop, string color)
    {
        prop.Append(new Shading() { Color = "auto", Fill = color, Val = ShadingPatternValues.Clear });
    }
    public static void BorderBottom(this TableCellProperties prop)
    {
        TableCellBorders tblBorders = new TableCellBorders();
        tblBorders.AppendChild(new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Thick), Color = "000000" });
        prop.Append(tblBorders);
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