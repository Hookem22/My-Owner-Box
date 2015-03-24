using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Word_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string header = "";
        if (!string.IsNullOrEmpty(Request.QueryString["header"]))
        {
            header = Request.QueryString["header"];
        }
        
        using (MemoryStream mem = new MemoryStream())
        {
            if (header == "Concept")
            {
                WordDoc.PrintConcept(mem);
            }
            else
            {
                WordDoc.Financials(mem);
            }

            // Stream it down to the browser
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + "Testdoc3.docx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            mem.Position = 0;
            mem.CopyTo(Response.OutputStream);
            Response.Flush();
            Response.End();
        }
        
        // Create a document by supplying the filepath. 
        //using (WordprocessingDocument wordDocument =
        //    WordprocessingDocument.Create(@"C:\Test.doc", WordprocessingDocumentType.Document))
        //{
        //    // Add a main document part. 
        //    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

        //    // Create the document structure and add some text.
        //    mainPart.Document = new Document();
        //    Body body = mainPart.Document.AppendChild(new Body());
        //    Paragraph para = body.AppendChild(new Paragraph());
        //    Run run = para.AppendChild(new Run());
        //    run.AppendChild(new Text("Create text in body - CreateWordprocessingDocument"));
        //}


        //using (var stream = new MemoryStream())
        //{
        //    using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
        //    {
        //        MainDocumentPart mainPart = doc.AddMainDocumentPart();

        //        new Document(new Body()).Save(mainPart);

        //        Body body = mainPart.Document.Body;
        //        body.Append(new Paragraph(
        //                    new Run(
        //                        new Text("Hello World!"))));

        //        //mainPart.Document.Save();
        //    }

        //    stream.Position = 0;
        //    stream.CopyTo(Response.OutputStream);
        //    Response.Flush();
        //    Response.End();
        //}
        
        
        //byte[] result = null;
        //byte[] templateBytes = System.IO.File.ReadAllBytes(wordTemplate);
        //using (MemoryStream templateStream = new MemoryStream())
        //{
        //    templateStream.Write(templateBytes, 0, (int)templateBytes.Length);
        //    using (WordprocessingDocument doc = WordprocessingDocument.Open(templateStream, true))
        //    {
        //        MainDocumentPart mainPart = doc.MainDocumentPart;           
        //        ...         
        //        mainPart.Document.Save();
        //        templateStream.Position = 0;
        //        using (MemoryStream memoryStream = new MemoryStream())
        //        {
        //            templateStream.CopyTo(memoryStream);
        //            result = memoryStream.ToArray();
        //        }
        //    }
        //}


        //using (MemoryStream mem = new MemoryStream())
        //{
        //    // Create Document
        //    using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(mem, WordprocessingDocumentType.Document, true))
        //    {
        //        // Add a main document part. 
        //        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

        //        new Document(new Body()).Save(mainPart);

        //        Body body = mainPart.Document.Body;
        //        body.Append(new Paragraph(
        //                    new Run(
        //                        new Text("Hello World!"))));

        //        mainPart.Document.Save();
        //        // Stream it down to the browser

        //        // THIS IS PROBABLY THE CRUX OF THE MATTER <---
        //        Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        //        Response.AppendHeader("Content-Disposition", "attachment;filename=HelloWorld.docx");
        //        mem.Position = 0;
        //        mem.CopyTo(Response.OutputStream);
        //        Response.Flush();
        //        Response.End();
        //    }

        //}
        
        //byte[] byteArray = File.ReadAllBytes("Test.docx");
        //using (MemoryStream mem = new MemoryStream())
        //{
        //    mem.Write(byteArray, 0, (int)byteArray.Length);
        //    using (WordprocessingDocument wordDoc =
        //        WordprocessingDocument.Open(mem, true))
        //    {
        //        // Modify the document as necessary.
        //        // For this example, we insert a new paragraph at the
        //        // beginning of the document.
        //        wordDoc.MainDocumentPart.Document.Body.InsertAt(
        //            new Paragraph(
        //                new Run(
        //                    new Text("Newly inserted paragraph."))), 0);
        //    }
        //    // At this point, the memory stream contains the modified document.
        //    // We could write it back to a SharePoint document library or serve
        //    // it from a web server.

        //    // In this example, we serialize back to the file system to verify
        //    // that the code worked properly.
        //    using (FileStream fileStream = new FileStream("Test2.docx",
        //        System.IO.FileMode.CreateNew))
        //    {
        //        mem.WriteTo(fileStream);
        //    }
        //}
    }
}