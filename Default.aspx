<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>My Owner Box</title>
    <meta name="description" content="How to Start a Restaurant. Restaurant Business Plans and Checklists. MyOwnerBox.com" />
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=0"/>
<%--    <link rel="icon" type="image/png" href="img/fav.png">--%>
    <link href="Styles/Style.css?i=3" rel="stylesheet" type="text/css" />
    <link href="Styles/Mobile.css?i=3" rel="stylesheet" type="text/css" />

    <script src="Scripts/jquery-2.0.3.min.js" type="text/javascript"></script>
    <script src="Scripts/Helpers.js" type="text/javascript"></script>
    <script type="text/javascript">
        var currentStep = 0;
        var marginLeft = "-300px";
        //var prices = [29, 35, 39, 45, 49, 55, 59, 65, 69, 75, 79, 85, 89, 95, 99];
        var price = 45; //prices[Math.floor(Math.random() * 11)];

        $(document).ready(function () {
            var mobParam = getParameterByName("id");
            var isMobile = mobilecheck() || tabletCheck() || mobParam == "m";
            if (isMobile)
            {
                $("body").addClass("Mobile");
                marginLeft = "-45%";
            }

            $('.pitchButton').click(function () {
                var questionId = "question0";
                $(".opening").animate({ left: "-100%" }, 500, function () { });

                $("body").css("overflow-x", "hidden");

                $("#" + questionId).show();

                //var margin = (+$(".questions").css("width").substring(0, $(".questions").css("width").length - 2)) / -2;
                $("#" + questionId).css("margin-left", marginLeft);

                $("#" + questionId).animate({ left: "50%" }, 500, function () {
                    $("body").css("overflow-x", "");
                });

                var subject = "Yes they do";
                var body = "Clicked Yes, I do";
                SendEmail(subject, body);
            });

            $(".questions").on("click", ".answer .nextBtn", function () {
                NextQuestion();
            });

            $(".questions").on("click", ".answer .blueBtn", function (event) {
                NextQuestion($(this).html());
            });

            $(".questions").on("click", ".answer .buyBtn", function (event) {
                var type = "MyOwnerBox Purchase: ";
                type += $(this).hasClass("annual") ? "Annual" : "Monthly";
                NextQuestion(type);
            });

            $(".questions").on('keyup', 'input[type=text]', function (e) {
                if (e.which == 13)
                    NextQuestion();
            });

            $(".contact .button").click(function () {
                $('.contact h2').show();
                var subject = EscapeString($(".contact #Subject").val());
                var body = EscapeString($(".contact #Body").val());
                SendEmail(subject, body);
            });
        });

        function NextQuestion(type)
        {
            var lastQuestion = currentStep % 2
            var lastQuestionId = "question" + lastQuestion;
            $("#" + lastQuestionId).animate({ left: "-100%" }, 500, function () {
                $("#" + lastQuestionId).hide();
                $("#" + lastQuestionId).css("left", "120%");
            });

            var subject = "User Click";
            var body = $("#" + lastQuestionId + " .question span").html() + "<br/><br/>";
            if (type)
                body += type;
            else
                body += $("#" + lastQuestionId + " input").val();

            body = EscapeString(body);
            SendEmail(subject, body);

            currentStep++;
            var questionId = "question" + currentStep % 2;

            var question = "";
            var template = '<div class="question"><span>{{Question}}</span></div><div class="answer"><input type="text" style="font-size:20px;width: 240px;" /><a class="button nextBtn" >Next</a></div>';
            switch(currentStep)
            {
                case 1:
                    //question = "Good place to start.<br/> So what kind of restaurant is this?";
                    //template = '<div class="question"><span>{{Question}}</span></div><div class="answer" style="padding: 2em 0;text-align: center;width:470px;"><a class="blueBtn">Brick and Morter</a><a class="blueBtn" style="padding: 5px 65px;">Trailer</a></div>';
                    //template = template.replace("{{Question}}", question);
                    //break;
                    question = "Good place to start.<br /> What do you want to call it?";
                    template = template.replace("{{Question}}", question);
                    template = template.replace("<input", "<input placeholder='Restaurant Name' ");
                    break;
                case 2:
                    question = "Cool! What kind of food are you making?";
                    template = template.replace("{{Question}}", question);
                    template = template.replace("<input", "<input placeholder='Italian, Tex Mex, BBQ, etc.' ");
                    break;
                case 3:
                    question = "That sounds good,<br/> what city will you be in?";
                    template = template.replace("{{Question}}", question);
                    template = template.replace("<input", "<input placeholder='City, State' ");
                    break;
                case 4:
                    question = "This is sounding awesome.<br/>Hypothetically, when should we open?";
                    template = template.replace("{{Question}}", question);
                    template = template.replace("<input", "<input placeholder='Month/Year' ");
                    break;
                case 5:
                    question = "Here's how it works:<br/>We're going to walk you through.";
                    template = '<div class="question"><span>{{Question}}</span></div><div class="answer"><h2>The fastest, easiest way to start your restaurant</h2><ul><li>1.) <span style="font-weight:bold;">Organize</span> your ideas</li><li>2.) <span style="font-weight:bold;">Step by Step</span> guides</li><li>3.) <span style="font-weight:bold;">Create</span> your concept and business plan</li></ul><a class="button nextBtn" >Next</a></div>';
                    template = template.replace("{{Question}}", question);
                    break;
                //case 6:
                //    question = "Here's how it works:<br/>We're going to help you get answers.";
                //    template = '<div class="question"><span>{{Question}}</span></div><div class="answer withImg"><h2></h2><img src="img/yelp.png" /><a class="button" >Next</a></div>';
                //    template = template.replace("{{Question}}", question);
                //    break;
                case 6:
                    question = "At the end, you'll end up<br/>with a pitch and a plan.";
                    template = '<div class="question"><span>{{Question}}</span></div><div class="answer withImg"><img src="img/businessplan.png" /><a class="button nextBtn" >Next</a></div>';
                    template = template.replace("{{Question}}", question);
                    break;
                case 7:
                    template = '<div class="tagline"><span>Ready to Get Started?</span></div><a class="pitchButton" onclick="NextQuestion()">Yes, I am</a></div>';
                    break;
                case 8:
                    template = '<div class="answer price" style="margin-top:-60px;"><div style="padding: 23px 4px 23px 35px;float: left;border-right: 1px solid #aaa;"><div style="margin: -18px 0 8px -18px;color: #F19F00;font-weight: bold;">SAVE 50% ANNUALLY</div><span style="font-size:2em;vertical-align: top;">$</span><span id="annualPrice" style="font-size:4em;">{{AnnualPrice}}</span><span>/mo</span><br /><span style="font-size: .9em;">Billed Annually</span></div><div class="startToday"><div class="button buyBtn annual">Start Today</div><div class="underMinute">&nbsp;Sign up in under a minute</div></div></div><div class="answer price" style="border:none;"><div style="padding: 28px 38px;float: left;border-right: 1px solid #aaa;"><span style="font-size:2em;vertical-align: top;">$</span><span id="monthlyPrice" style="font-size:3.8em;">{{MonthlyPrice}}</span><span>/mo</span><br /><span style="font-size: .9em;">Billed Monthly</span></div><div class="startToday"><div class="button buyBtn" style="background: white;color: #F19F00;border: 2px solid #F19F00;">Start Today</div><div class="underMinute">&nbsp;Sign up in under a minute</div></div></div>';
                    template = template.replace("{{MonthlyPrice}}", price).replace("{{AnnualPrice}}", Math.floor(price / 2));
                    break;
                case 9:
                    question = "Perfect. We are currently in private beta.<br/>Enter your email for an invitation<br/> to My Owner Box.";
                    template = template.replace("{{Question}}", question);
                    template = template.replace("<input", "<input placeholder='Email' ");
                    template = template.replace("Next</a></div>", "Request</a></div>");
                    break;
                default:
                    question = "Thank you for signing up.<br/>We will email you soon."
                    template = '<div class="question"><span>{{Question}}</span></div>';
                    template = template.replace("{{Question}}", question);
                    break;
            }

            $("body").css("overflow-x", "hidden");

            $("#" + questionId).html(template);
            $("#" + questionId).show();

            ////var margin = (+$(".questions").css("width").substring(0, $(".questions").css("width").length - 2)) / -2;
            $("#" + questionId).css("margin-left", marginLeft);

            $("#" + questionId).animate({ left: "50%" }, 500, function () {
                $("body").css("overflow-x", "");
            });

        }

        function SendEmail(subject, body) {
            body += "%3Cbr/%3E%3Cbr/%3EPrice $" + price;
            body += "%3Cbr/%3E%3Cbr/%3E" + new Date();

            Post("Default.aspx/SendEmail", { subject: subject, body: body });
        }

    </script>
</head>
<body>
<form id="form1" runat="server">
    <header class="mainnav">
         <ul>
            <li><a href="#">HOME</a></li>
            <li><a href="#howitworks">ABOUT</a></li>
            <li><a href="#contact">CONTACT</a></li>
        </ul>
    </header>
    <div class="pitch">
        <div class="logoDiv">
            <img src="img/logo.png" />
        </div>
<%--        Prices
        <div class="questions">
            <div class="answer price" style="margin-top:-60px;">
                <div style="padding: 23px 4px 23px 35px;float: left;border-right: 1px solid #aaa;"><div style="margin: -18px 0 8px -18px;color: #F19F00;font-weight: bold;">SAVE 50% ANNUALLY</div><span style="font-size:2em;vertical-align: top;">$</span><span id="annualPrice" style="font-size:4em;">19</span><span>/mo</span><br /><span style="font-size: .9em;">Billed Annually</span></div>
                <div style="float:left;padding: 15px 0 0 63px;"><div class="button" style="width: 120px;font-size:1.5em;line-height:2em;margin-bottom: .2em;">Start Today</div><div>&nbsp;Sign up in under a minute</div></div>
            </div>
            <div class="answer price" style="border:none;">
                <div style="padding: 28px 38px;float: left;border-right: 1px solid #aaa;"><span style="font-size:2em;vertical-align: top;">$</span><span id="monthlyPrice" style="font-size:3.8em;">39</span><span>/mo</span><br /><span style="font-size: .9em;">Billed Monthly</span></div>
                <div style="float:left;padding: 15px 0 0 63px;"><div class="button" style="width: 120px;font-size:1.5em;line-height:2em;margin-bottom: .2em;background: white;color: #F19F00;border: 2px solid #F19F00;">Start Today</div><div>&nbsp;Sign up in under a minute</div></div>
            </div>
        </div>--%>
        <div class="opening" style="position:relative;">
            <div class="tagline">
                <span>Want to Start a Restaurant?</span>
            </div>
            <a class="pitchButton">
                Yes, I do
            </a>
        </div>
        <div id="question0" class="questions">
            <div class="question">
                <span>Awesome, let's get started.<br/> So what kind of restaurant is this?</span>
            </div>
            <div class="answer whatType">
                <a class="blueBtn">Brick and Mortar</a>
                <a class="blueBtn" style="padding: 5px 65px;">Trailer</a>
            </div>
        </div>   
        <div id="question1" class="questions">
            <div class="question">
                <span>Awesome, let's get started.<br /> What do you want to call it?</span>
            </div>
            <div class="answer">
                <input type="text" style="font-size:20px;width: 240px;" />
                <a class="button">Next</a>
            </div>
        </div> 

    </div>
    <div id="howitworks" class="arrow-up"></div>
    <div class="howitworks">
        <h2 style="margin-bottom:2em;"><span style="color:#F19F00;"><i>How My Owner Box Works</i></span>&nbsp;&nbsp;Plan a restaurant today!</h2>
        <div class="step">
            <img src="img/lightbulb.png" />
            <div class="horizontal"></div>
            <div>You’ve got an idea for a restaurant</div>
        </div>
        <div class="step">
            <img src="img/questionMark.png" />
            <div class="horizontal"></div>
            <div>You don’t know how to get started</div>
        </div>
        <div class="step">
            <img src="img/logoBlue.png" />
            <div class="horizontal"></div>
            <div>Let’s walk through it together</div>
        </div>
    </div>
    <div class="quote">
        "My Owner Box saved us a lot of time and money and was incredibly helpful for getting our restaurant off the ground."
    </div>
    <br />
    <div id="contact" class="contact">
        <h2>Contact Us</h2>
        <div class="horizontal" style="margin: -4px 15% 4px;"></div>
        <input id="Subject" type="text" placeholder="Subject" />
        <textarea id="Body" placeholder="Give us your feedback!" rows="4" ></textarea>
        <br />
        <br />
        <div class="button" style="margin:0 auto;" >SUBMIT</div><h2 style="float:left;display: none;width:100%;text-align:center;"><span style="color:#F19F00;"><i>Thanks for contacting us. Your email has been sent.</i></span></h2>
        <div class="footer">
            <span></span>
        </div>
    </div>
    </form>

<%--<a title="Web Statistics" href="http://clicky.com/100820445"><img alt="Web Statistics" src="//static.getclicky.com/media/links/badge.gif" border="0" /></a>--%>
<script type="text/javascript">
    var clicky_site_ids = clicky_site_ids || [];
    clicky_site_ids.push(100820445);
    (function () {
        var s = document.createElement('script');
        s.type = 'text/javascript';
        s.async = true;
        s.src = '//static.getclicky.com/js';
        (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(s);
    })();
</script>
<noscript><p><img alt="Clicky" width="1" height="1" src="//in.getclicky.com/100820445ns.gif" /></p></noscript>
</body>
</html>
