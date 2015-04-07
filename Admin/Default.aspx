<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/Styles/Bootstrap/css/bootstrap.css" rel="stylesheet" />
    <link href="/Styles/Bootstrap/css/bootstrap-theme.css" rel="stylesheet" />
    <link href="/Styles/jquery-te-1.4.0.css" rel="stylesheet" />
                                                                                    
    <link href="/Styles/Style.css" rel="stylesheet" type="text/css" />
    <link href="/Styles/App.css" rel="stylesheet" type="text/css" />

    <script src="/Scripts/jquery-2.0.3.min.js" type="text/javascript"></script>
    <script src="/Scripts/bootstrap.js"></script>
    <script src="/Scripts/jquery-te-1.4.0.min.js"></script>
    <script src="/Scripts/Helpers.js" type="text/javascript"></script>

    <style>
        .EditDialog
        {
            margin: 1em 3em;
            font-size: 18px;
        }
        .EditDialog div
        {
        }
        .EditDialog input, .EditDialog textarea
        {
            margin: .5em 0;
            width: 300px;
        }
        .EditDialog select
        {
            margin: .5em 1em .5em 0;
        }
        th
        {
            padding: 0 12px;
        }
        td
        {
            border: 1px solid black;
            padding: 0 12px;
        }
    </style>

    <script type="text/javascript">
        var Questions = [];

        $(document).ready(function () {
            
            //$("#ConceptEditDialog textarea").jqte();
            //$("#ConceptEditDialog .jqte_tool.jqte_tool_1 .jqte_tool_label").css("height", "28px");
            //$("#ConceptEditDialog .jqte").css({ width: "700px", margin: "0 auto" });
            //$("#ConceptEditDialog .jqte_toolbar").css({ width: "initial" });
            //$("#ConceptEditDialog .jqte_tool").css({ width: "initial" });
            //$("#ConceptEditDialog .jqte_editor").css({ height: "300px", width: "700px" });

            Get();

            $(".nav.primary div").not(".addQuestion").click(function () {
                if ($(this).hasClass("active"))
                    return;

                $(this).siblings().removeClass("active");
                $(this).addClass("active");

                PopulateSubheaders();
                Get();
                
            });

            $("body").on("click", ".nav.secondary div", function () {
                if ($(this).hasClass("active"))
                    return;

                $(this).siblings().removeClass("active");
                $(this).addClass("active");

                Get();
                $("#Sheet").val($(".nav.secondary div.active").html());
            });

            $(".addQuestion").click(function () {
                $(".modal-dialog").show();
                $(".modal-backdrop").show();
                $(".EditDialog input, .EditDialog textarea").val("");
            });

            $(".saveBtn").click(function () {
                var question = { Id: $("#Id").val() || 0, Type: $("#Type").val(), Sheet: $("#Sheet").val(), Page: $("#Page").val(), Section: $("#Section").val(), ControlType: $("#ControlType").val(), Title: $("#Title").val(), Options: $("#Options").val(), SkipCondition: $("#SkipCondition").val() }
                //var question = { Id: 0 };
                var success = function (questionId) {
                    console.log(questionId);
                    $("#Title").val("");
                    $("#Options").val("");
                    Get();
                };
                Post("SaveQuestion", { question: question }, success)
            });
        });

        function Get() {
            var success = function (questions) {
                Questions = questions;
                PopulateTable();
            };
            Post("Get", { header: $(".nav.primary .active").html(), category: $(".nav.secondary .active").html(), userId:0 }, success);
        }

        function PopulateTable() {
            var table = "<table>";
            table += "<tr><th>Id</th><th>Sheet</th><th>Page</th><th>Section</th><th>Question</th><th>Options</th><th>SkipCondition</th><th></th></tr>";
            $(Questions).each(function () {
                if(this.Type == $(".nav.primary .active").text())
                {
                    table += "<tr>";
                    table += "<td>" + this.Id + "</td>";
                    table += "<td>" + this.Sheet + "</td>";
                    table += "<td>" + this.Page + "</td>";
                    table += "<td>" + this.Section + "</td>";
                    if (this.Title && this.Title.length > 20)
                        table += "<td>" + this.Title.substring(0, 20) + "..." + "</td>";
                    else
                        table += "<td>" + this.Title + "</td>";
                    table += "<td>" + this.Options + "</td>";
                    table += "<td>" + this.SkipCondition + "</td>";
                    table += "<td><div class='btn blueBtn' style='font-size:14px;margin:3px;' onclick='Edit(" + this.Id + ");'>Edit</div></td>";
                    //table += "<td><div class='btn blueBtn' style='font-size:14px;margin:3px;' onclick='Delete(" + this.Id + ");'>Delete</div></td>";
                    table += "</tr>";
                }
            });
            table += "</table>";
            $(".fromDb").html(table);

            $("#Type").val($(".nav.primary div.active").html());
        }

        function PopulateSubheaders() {
            var subheaders = [];
            var header = $(".nav.primary div.active").html();
            if (header == "Concept") {
                subheaders = ["Create Your Concept"];
            }
            else if (header == "Business Plan") {
                subheaders = ["Company Description", "Management Team", "Market Analysis", "Marketing Strategy", "Staffing", "Daily Operations", "Software and Controls", "Other Contol Systems", "Inventory", "Accounting"];
            }
            else if (header == "Financials") {
                subheaders = ["Basic Info", "Capital Budget", "Sales Projection", "Hourly Labor", "Expenses", "Investment", "Print"];
            }

            $(".nav.secondary").html("");
            for (var i = 0; i < subheaders.length; i++) {
                if (i == 0)
                    $(".nav.secondary").append($("<div>", { class: "active", style: "margin-left:0;", text: subheaders[i] }));
                else
                    $(".nav.secondary").append($("<div>", { text: subheaders[i] }));
            }
            
            $('#Sheet').html("");
            $.each(subheaders, function () {
                $('#Sheet').append($('<option>', { value: this }).text(this));
            });
        }

        function Edit(id)
        {
            $(Questions).each(function () {
                if (this.Id == id) {
                    $("#Id").val(this.Id);
                    $("#Sheet").val(this.Sheet);
                    $("#Page").val(this.Page);
                    console.log(this.ControlType);
                    $("#ControlType").val(this.ControlType);
                    $("#Section").val(this.Section);
                    $("#Title").val(this.Title);
                    $("#Options").val(this.Options);
                    $("#SkipCondition").val(this.SkipCondition);
                    return 0;
                }
            });
            $(".modal-dialog").show();
            $(".modal-backdrop").show();
        }

        

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="modal-backdrop"></div>
        <div id="ConceptEditDialog" class="modal-dialog" style="position:absolute;">
            <div class="dialogClose">X</div>
            <h3>Add Question</h3>
            <div class="EditDialog">
                <input type="hidden" id="Id" />
                <select id="Type">
                    <option>Concept</option>
                    <option>Business Plan</option>
                    <option>Financials</option>
                </select>
                <select id="Sheet"></select>
                <select id="ControlType">
                    <option value="1">Textbox</option>
                    <option value="2">Textarea</option>
                    <option value="3">Select</option>
                    <option value="4">Checkbox</option>
                    <option value="5">Radio</option>
                    <option value="6">List</option>
                    <option value="7">MultiTextbox</option>
                </select><br />
                <div style="float:left;width: 140px;margin: .5em 0;">Page</div>
                <input type="text" id="Page" /><br />
                <div style="float:left;width: 140px;margin: .5em 0;">Section Header</div>
                <input type="text" id="Section" /><br />
                <div style="float:left;width: 140px;margin: .5em 0;">Question</div>
                <textarea id="Title"></textarea><br />
                <div style="float:left;width: 140px;margin: .5em 0;">Options</div>
                <input type="text" id="Options" /><br />
                <div style="float:left;width: 140px;margin: .5em 0;">Skip Condition</div>
                <input type="text" id="SkipCondition" /><br />
            </div>
            <div class="saveBtn btn" style="margin: 2em 50px;">Save</div>
        </div>
        <div class="header">
            <div style="width:800px;margin:0 auto;">
                <img src="../img/logoshadow.png" class="logo" />
                <div class="myOwnerBox">My Owner Box</div>
                <div style="float: right;margin: 19px 60px 0 0;">Admin</div>
            </div>
        </div>
        <div class="subheader">
            <div class="nav primary">
                <div class="active" style="margin-left:40px;">Concept</div>
                <div>Business Plan</div>
                <div>Financials</div>
                <div class="addQuestion" style="float:right;">Add Question</div>
            </div>
        </div>
        <div class="main">
            <div class="nav secondary" style="height:inherit;width:inherit;">
                <div class="active" style="margin-left:0;">Create Your Concept</div>
            </div>
            <div class="fromDb" style="margin-top:20px;">
            </div>
        </div>
    </form>
</body>
</html>
