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

            FinancialsOverview(wordDocument);
            body.AddPageBreak();
            FinancialsCapitalBudget(wordDocument);
            body.AddPageBreak();
            FinancialsSalesProjection(wordDocument);
        }
    }

    public static void FinancialsOverview(WordprocessingDocument wordDocument)
    {
        Body body = wordDocument.MainDocumentPart.Document.Body;
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

        // Add 3 columns to the table.
        TableGrid tg = new TableGrid(new GridColumn(), new GridColumn(), new GridColumn());
        tbl.AppendChild(tg);

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


        // Add the table to the document
        body.AppendChild(tbl);

    }

    public static void FinancialsCapitalBudget(WordprocessingDocument wordDocument)
    {
        Body body = wordDocument.MainDocumentPart.Document.Body;
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

        // Add 3 columns to the table.
        TableGrid tg = new TableGrid(new GridColumn(), new GridColumn(), new GridColumn());
        tbl.AppendChild(tg);

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


        // Add the table to the document
        body.AppendChild(tbl);
    }

    public static void FinancialsSalesProjection(WordprocessingDocument wordDocument)
    {

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
            TableCell tc = new TableCell(new Paragraph(new ParagraphProperties(), new Run(new RunProperties(), new Text(cells[i]))));
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