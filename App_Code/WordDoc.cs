using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
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

    public static void InsertTableInDoc(MemoryStream mem)
    {
        // Open a WordprocessingDocument for editing using the filepath.
        using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(mem,
            WordprocessingDocumentType.Document, true))
        {
            // Assign a reference to the existing document body.
            MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

            // Create the document structure and add some text.
            mainPart.Document = new Document();
            Body body = mainPart.Document.AppendChild(new Body());

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

            // Create 1 row to the table.
            TableRow tr1 = new TableRow();

            // Add a cell to each column in the row.
            TableCell tc1 = new TableCell(new Paragraph(new Run(new Text("1"))));
            TableCell tc2 = new TableCell(new Paragraph(new Run(new Text("2"))));
            TableCell tc3 = new TableCell(new Paragraph(new Run(new Text("3"))));
            tr1.Append(tc1, tc2, tc3);

            // Add row to the table.
            tbl.AppendChild(tr1);

            // Add the table to the document
            body.AppendChild(tbl);
        }
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
}