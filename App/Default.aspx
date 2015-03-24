<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="App_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>My Owner Box</title>
    <meta name="description" content="How to Start a Restaurant. Restaurant Business Plans and Checklists. MyOwnerBox.com" />
    <%--<meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=0"/>
    <link rel="icon" type="image/png" href="img/fav.png">--%>
    <link href="/Styles/Bootstrap/css/bootstrap.css" rel="stylesheet" />
    <link href="/Styles/Bootstrap/css/bootstrap-theme.css" rel="stylesheet" />
    <link href="/Styles/jquery-te-1.4.0.css" rel="stylesheet" />
                                                                                    
    <link href="/Styles/Style.css" rel="stylesheet" type="text/css" />
    <link href="/Styles/App.css" rel="stylesheet" type="text/css" />

    <script src="/Scripts/jquery-2.0.3.min.js" type="text/javascript"></script>
    <script src="/Scripts/bootstrap.js"></script>
    <script src="/Scripts/jquery-te-1.4.0.min.js"></script>
    <script src="/Scripts/Helpers.js" type="text/javascript"></script>
    <script src="/Scripts/jsPDF/jspdf.js" type="text/javascript"></script>
    <script src="/Scripts/jsPDF/FileSaver.js" type="text/javascript"></script>

    <script type="text/javascript">
        var Questions;
        var currentQuestion = 0;
        var currentUserId = 0;

        $(document).ready(function () {
            currentUserId = +$("#CurrentUserId").val();

            Get();

            $(".nav.primary div").not(".restaurantName").click(function () {
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

                if ($(this).text() == "Print")
                {
                    var header = $(".nav.primary .active").html();
                    window.open("/Word?header=" + header);
                    return;
                }

                $(this).siblings().removeClass("active");
                $(this).addClass("active");

                Get();
            });

            $("body").on("click", ".main .backBtn", function () {
                BackClicked();
            });

            $("body").on("click", ".main .nextBtn", function () {
                NextClicked();
            });

            $(".main").on('keyup', 'input', function (e) {
                if (e.which == 13) {
                    if ($(".glyphicon-plus-sign").length)
                        AddItem();
                    else
                        NextClicked();
                }
            });

            $(".main").on("click", ".glyphicon-plus-sign", function() {
                AddItem();
            });

            $(".main").on("click", ".glyphicon-remove-circle", function () {
                $(this).closest(".added").remove();
            });

            $(".header img").click(function () {
                var success = function (val) {
                    $("#ConceptEditDialog .jqte_editor").html(val);
                    $(".modal-dialog").show();
                    $(".modal-backdrop").show();
                                        
                };
                Post("GetTemplate", { questions: Questions }, success);
            });

            $("#ConceptEditDialog textarea").jqte();
            $("#ConceptEditDialog .jqte_tool.jqte_tool_1 .jqte_tool_label").css("height", "28px");
            $("#ConceptEditDialog .jqte").css({ width: "700px", margin: "0 auto" });
            $("#ConceptEditDialog .jqte_editor").css({ height: "600px" });
            //$("#ConceptEditDialog .jqte_editor").html("hello<br/>world");

            $("#ConceptEditDialog textarea").val("Hello world");

        });

        function Get()
        {
            var success = function (questions) {
                currentQuestion = 0;
                Questions = questions;
                NextScreen();
            };
            Post("Get", { header: $(".nav.primary .active").html(), category: $(".nav.secondary .active").html(), userId:currentUserId }, success);
        }

        function PopulateSubheaders()
        {
            var subheaders = [];
            var header = $(".nav.primary div.active").html();
            if(header == "Concept")
            {
                subheaders = ["Create Your Concept", "Print"];
            }
            else if (header == "Business Plan")
            {
                subheaders = ["Summary", "Location"];
            }
            else if(header == "Financials")
            {
                subheaders = ["Basic Info", "Capital Budget", "Sales Projection", "Hourly Labor", "Other Expenses", "Print" ];
            }

            $(".nav.secondary").html("");
            for (var i = 0; i < subheaders.length; i++) {
                if (i == 0)
                    $(".nav.secondary").append($("<div>", { class: "active", style: "margin-left:0;", text: subheaders[i] }));
                else
                    $(".nav.secondary").append($("<div>", { text: subheaders[i] }));
            }

            $(".nav.secondary div").css("color", "white");
            setTimeout(function () {
                $(".nav.secondary div").css("color", "");
             }, 200);

        }

        function PopulateContent()
        {
            if (!Questions.length) {
                $(".fromDb").html("");
                return;
            }
            if (Questions[currentQuestion].ControlType == 7) { //Multi
                var html = "";
                for (var i = 0; i < Questions.length; i++) {
                    if (i == 0) {
                        html += "<div class='multiTextGroup'><h2>" + Questions[currentQuestion].Section + "</h2>";
                        html += "<div class='multiText'><span>" + Questions[currentQuestion].Title + "</span><input type='text' value='{{Answer}}' /></div>";
                    }
                    else if (currentQuestion + i == Questions.length || Questions[currentQuestion + i].Page != Questions[currentQuestion + i - 1].Page) {
                        break;
                    }
                    else if (Questions[currentQuestion + i].Section != Questions[currentQuestion + i - 1].Section) {
                        html += "</div><div class='multiTextGroup divider'>";
                        html += "<h2>" + Questions[currentQuestion + i].Section + "</h2>";
                        html += "<div class='multiText'><span>" + Questions[currentQuestion + i].Title + "</span><input type='text' value='{{Answer}}' /></div>";
                    }
                    else if (!Questions[currentQuestion + i].Title) {
                        html += "<div style='clear:both;'></div>";
                    }
                    else {
                        html += "<div class='multiText'><span>" + Questions[currentQuestion + i].Title + "</span><input type='text' value='{{Answer}}' /></div>";
                    }
                    var answer = Questions[currentQuestion + i].Answer.Text || "";
                    html = html.replace("{{Answer}}", answer);
                }
                html += "</div>";
                $(".fromDb").html(html);

            }
            else {
                var question = Questions[currentQuestion];
                var html = question.Html;
                var answer = question.Answer.Text || "";
                html = html.replace("{{Answer}}", answer);

                $(".fromDb").html(html);

                if (question.ControlType == 3) { //Select
                    $(".AnswerControl").val(answer);
                }
                else if (question.ControlType == 4) { //Checkbox
                    $(answer.split(", ")).each(function () {
                        $(".AnswerControl[value='" + this + "']").prop("checked", true);
                    });
                }
                else if (question.ControlType == 5) { //Radio
                    $(answer.split(", ")).each(function () {
                        $("input[name='AnswerControl'][value='" + this + "']").prop("checked", true);
                    });
                }
                else if (question.ControlType == 6) { //List
                    $(answer.split(", ").reverse()).each(function () {
                        AddItem(this);
                    });
                }
            }
            
        }

        function BackClicked()
        {
            currentQuestion--;
            for (var i = currentQuestion; i > 0; i--) {
                if (!CheckSkip(Questions[i].SkipCondition)) {
                    break;
                }
                currentQuestion--;
            }
            while (currentQuestion > 0 && Questions[currentQuestion].Page && Questions[currentQuestion].Page == Questions[currentQuestion - 1].Page) {
                currentQuestion--;
            }

            PrevScreen();
        }

        function NextClicked()
        {
            SaveAnswer();
            currentQuestion++;

            for (var i = currentQuestion; i < Questions.length; i++) {
                if (!CheckSkip(Questions[i].SkipCondition)) {
                    break;
                }
                currentQuestion++;
            }
            while (Questions[currentQuestion] && Questions[currentQuestion].Page && Questions[currentQuestion].Page == Questions[currentQuestion - 1].Page) {
                currentQuestion++;
            }

            NextScreen();
        }

        function CheckSkip(skip)
        {
            if (!skip || skip.indexOf("|") < 0 || skip.split("|").length < 2)
                return false;
            
            var title = skip.split("|")[0];
            var answer = skip.split("|")[1];
            for (var i = 0; i < Questions.length; i++) {
                if (Questions[i].Title == title) {
                    return Questions[i].Answer.Text == null || Questions[i].Answer.Text.indexOf(answer) < 0;
                }
            }
        }

        function NextScreen()
        {
            $(".main").animate({ "margin-left": "0" }, 200, function () {
                $(".main").fadeOut(100, function () {
                    PopulateContent();
                    var margin = ($(window).width() - 800) / 2;
                    $(".main").css("margin-left", margin + 100 + "px");
                    $(".main").fadeIn(100, function () {
                        $(".main").css("margin-left", "auto");
                        $("#AnswerControl").focus();
                    });
                });
            });
        }

        function PrevScreen() {
            var margin = ($(window).width() - 800) / 2;
            $(".main").animate({ "margin-left": margin + 100 + "px" }, 200, function () {
                $(".main").fadeOut(100, function () {
                    PopulateContent();
                    $(".main").css("margin-left", "0");
                    $(".main").fadeIn(100, function () {
                        $(".main").css("margin-left", "auto");
                        $("#AnswerControl").focus();
                    });
                });
            });
        }

        function AddItem(item) {
            if (!$(".ListControl").val() && !(item && item.length))
                return;

            if (!item)
                item = $(".ListControl").val();

            var div = $("<div>", { class: "added AnswerControl", text: item });
            $(div).append($("<span>", { class: "glyphicon glyphicon-remove-circle" }));
            $(".glyphicon-plus-sign").after(div);

            $(".ListControl").val("");
            $(".ListControl").attr("placeholder", "");
        }

        function SaveAnswer()
        {
            var answer = "";
            if ($(".multiText input").length > 0) { //Multi
                var answerText = [];
                $(".multiText input").each(function () {
                    answerText.push($(this).val());
                });
                var answerArray = [];
                for (var i = 0, blankQuestions = 0; i < answerText.length; i++) {
                    if (!Questions[currentQuestion + blankQuestions + i].Title) {
                        blankQuestions++;
                    }
                    var question = Questions[currentQuestion + blankQuestions + i];
                    question.Answer.Text = answerText[i];
                    answerArray.push(question.Answer);
                }

                var success = function (answers) {
                    console.log(answers);
                    for (var x = 0; x < Questions.length; x++) {
                        for (var y = 0; y < answers.length; y++) {
                            if (Questions[x].Id == answers[y].QuestionId) {
                                Questions[x].Answer.Id = answers[y].Id;
                            }
                        }
                    }
                };
                Post("SaveAnswers", { answers: answerArray }, success);
                return;
            }


            if ($(".added.AnswerControl").length > 0) { //ListControl
                $(".added.AnswerControl").each(function () {
                    answer += $(this).text() + ", ";
                });
                answer = answer.substring(0, answer.length - 2);
            }
            else if ($(".AnswerControl").length > 1) { //Checkboxes
                $(".AnswerControl:checked").each(function () {
                    answer += $(this).val() + ", ";
                });
                answer = answer.substring(0, answer.length - 2);
            }
            else {
                answer = $("input[name='AnswerControl']:checked").val() || $(".AnswerControl").val();
            }

            var question = Questions[currentQuestion];
            question.Answer.Text = answer;

            //Save to DB
            var success = function (answerId) {
                question.Answer.Id = answerId;
            };
            Post("SaveAnswer", { answer: question.Answer }, success);
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <input type="hidden" runat="server" id="CurrentUserId" />
        <div class="modal-backdrop"></div>
        <div id="ConceptEditDialog" class="modal-dialog">
            <div class="dialogClose">X</div>
            <h3>Edit your Restaurant Concept</h3>
            <textarea></textarea>
            <div class="saveBtn btn" style="margin: 2em 50px;">Save</div>
        </div>
        <div class="header">
            <div style="width:800px;margin:0 auto;">
                <img src="../img/logoshadow.png" class="logo" />
                <div class="myOwnerBox">My Owner Box</div>
                <div style="float: right;margin: 19px 60px 0 0;"><a href="../Word">My Account</a></div>
            </div>
        </div>
        <div class="subheader">
            <div class="nav primary">
                <div class="active" style="margin-left:40px;">Concept</div>
                <div>Business Plan</div>
                <div>Financials</div>
                <div class="restaurantName">Restaurant Name</div>
            </div>
        </div>
        <div class="main">
           <div class="nav secondary">
                <div class="active" style="margin-left:0;">Create Your Concept</div>
                <div>Print</div>
            </div>
            <div class="fromDb" style="margin-left:5%;">
                <div class="multiTextGroup">
                    <h2>Sources of Cash</h2>
                    <div class="multiText">
                        <span>Equity Contributions</span>
                        <input type="text" />
                    </div>
                    <div class="multiText">
                        <span>Loan Financing</span>
                        <input type="text" />
                    </div>
                </div>
                <div class="multiTextGroup divider">
                    <h2>Uses of Cash</h2>
                    <div class="multiText">
                        <span>Land and Building</span>
                        <input type="text" />
                    </div>
                    <div class="multiText">
                        <span>Leasehold Improvements</span>
                        <input type="text" />
                    </div>
                    <div class="multiText">
                        <span>Bar / Kitchen Equipment</span>
                        <input type="text" />
                    </div>
                    <div class="multiText">
                        <span>Bar / Dining Room Furniture</span>
                        <input type="text" />
                    </div>
                    <div class="multiText">
                        <span>Professional Services</span>
                        <input type="text" />
                    </div>
                    <div class="multiText">
                        <span>Organizational & Development</span>
                        <input type="text" />
                    </div>
                    <div class="multiText">
                        <span>Interior Finishes & Equipment</span>
                        <input type="text" />
                    </div>
                    <div class="multiText">
                        <span>Exterior Finishes & Equipment</span>
                        <input type="text" />
                    </div>
                    <div class="multiText">
                        <span>Pre-Opening Expenses</span>
                        <input type="text" />
                    </div>
                    <div class="multiText">
                        <span>Working Capital & Contingency</span>
                        <input type="text" />
                    </div>
                </div>
            </div>
            <div class="btnSection">
                <div class="backBtn btn">Back</div>
                <div class="nextBtn btn">Next</div>
            </div>
        </div>
    </form>
</body>
</html>
