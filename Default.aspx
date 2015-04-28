<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>My Owner Box</title>
    <meta name="description" content="How to Start a Restaurant. Restaurant Business Plans and Checklists. MyOwnerBox.com" />
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=0"/>
    <link rel="icon" type="image/png" href="img/favicon.png">
    <link href="Styles/Style.css?i=3" rel="stylesheet" type="text/css" />
    <link href="Styles/Mobile.css?i=3" rel="stylesheet" type="text/css" />

    <script src="Scripts/jquery-2.0.3.min.js" type="text/javascript"></script>
    <script src="Scripts/Helpers.js" type="text/javascript"></script>
    <script type="text/javascript">
        var currentStep = 0;
        var marginLeft = "-300px";
        var prices = [9, 19, 29, 39];
        var price = prices[Math.floor(Math.random() * 4)];

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
                Annual = $(this).hasClass("annual");
                NextQuestion();
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

            $(".questions").on("click", ".creditCard img", function (event) {
                $(".creditCard img").attr("src", "img/UncheckedRadio.png");
                $(this).attr("src", "img/CheckedRadio.png");
                var pricing = $(this).hasClass("monthly") ? "$" + price + " per month" : "$" + Math.floor(price / 2) * 12 + " per year";
                $(".creditCard .price").html(pricing);

            });

            $(".forgotPassword").click(function () {
                $(".loginDialog").hide();
                $(".forgotDialog").show();
            });
        });

        function NextQuestion()
        {
            if(currentStep == 9)
            {
                if (!ValidateSignUp())
                    return;
            }

            var lastQuestion = currentStep % 2
            var lastQuestionId = "question" + lastQuestion;
            $("#" + lastQuestionId).animate({ left: "-100%" }, 500, function () {
                $("#" + lastQuestionId).hide();
                $("#" + lastQuestionId).css("left", "120%");
                $("#" + lastQuestionId).html("");
            });

            var subject = "User Click";
            var body = $("#" + lastQuestionId + " .question span").html() + "<br/><br/>";

            $(".answer input[type='text']").each(function () {
                body += $(this).val() + "   ";
            });

            body = EscapeString(body);
            SendEmail(subject, body);

            currentStep++;
            var questionId = "question" + currentStep % 2;

            var question = "";
            var template = '<div class="question"><span>{{Question}}</span></div><div class="answer"><input type="text" style="font-size:20px;width: 240px;" /><a class="button nextBtn" >Next</a></div>';
            switch(currentStep)
            {
                case 1:
                    question = "Good place to start.<br /> What do you want to call it?";
                    template = template.replace("{{Question}}", question);
                    template = template.replace("<input", "<input placeholder='Restaurant Name' ");
                    break;
                case 2:
                    restaurantName = $(".answer input").val();
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
                    question = "Here's how it works:";
                    template = '<div class="question"><span>{{Question}}</span></div><div class="answer" style="padding:30px;"><img src="img/lightbulbMoney.png" style="height:60px;margin: 4px 20px 0 0;" /><h2 style="padding-right: 20px;font-weight: normal;font-size: 22px;margin-bottom: 32px;">You’ve got a great idea for a restaurant but you don’t know how to get started.</h2><a class="button nextBtn" style="margin-left:300px;" >Next</a></div>';
                    template = template.replace("{{Question}}", question);
                    break;
                case 6:
                    question = "Here's how it works:";
                    template = '<div class="question"><span>{{Question}}</span></div><div class="answer" style="padding:30px;"><img src="img/businessplan.png" style="height: 110px;margin: 4px 18px 0 0;" /><h2 style="padding-right: 20px;font-weight: normal;font-size: 22px;margin-bottom: 32px;">The first thing you need to do is put together a business plan that you can take to investors or use to open your restaurant.</h2><a class="button nextBtn" style="margin-left:300px;" >Next</a></div>';
                    template = template.replace("{{Question}}", question);
                    break;
                case 7:
                    question = "Here's how it works:";
                    template = '<div class="question"><span>{{Question}}</span></div><div class="answer" style="padding:30px;"><img src="img/logoBlue.png" style="height:60px;margin: 4px 12px 40px 0;" /><h2 style="padding-right: 20px;font-weight: normal;font-size: 22px;margin-bottom: 32px;">My Owner Box is the fastest, easiest way to put together your restaurant business plan.</h2><a class="button nextBtn" style="margin-left:300px;" >Next</a></div>';
                    template = template.replace("{{Question}}", question);
                    break;
                case 8:
                    template = '<div class="tagline"><span>Ready to Get Started?</span></div><a class="pitchButton" onclick="NextQuestion()">Yes, I am</a></div>';
                    break;
                case 9:
                    template = '<div class="question"><span>Get Started Now</span></div><div class="answer signUp">';
                    template += '<div>Name</div><input type="text" id="Name" />';
                    template += '<div>Email</div><input type="text" id="Email" />';
                    template += '<div>Password</div><input type="password" id="Password" />';
                    template += '<div>Confirm Password</div><input type="password" id="ConfirmPassword" />';
                    template += '<a class="button nextBtn" style="margin-left: 16px;" >Get Started</a><img style="height: 40px;margin-left: 330px;margin-top:-51px;display:none;" src="http://shop.skype.com/i/images/ab-test/u12_normal_2.gif" /></div>';
                    break;
                default:
                    window.href.location = "/App";

                //case 8:
                //    $(".opening").hide();
                //    template = '<div class="tagline"><span>Ready to Get Started?</span></div><a class="pitchButton" onclick="GoToApp()">Yes, I am</a><img style="height:64px;margin-top:15px;margin-left: 252px;display:none;" src="http://www.entrepreneur.com/iglobal/loading.gif" /></div>';
                //    break;
                //default:
                //    window.href.location = "/App";


                //case 8:
                //    template = '<div class="tagline"><span>Ready to Get Started?</span></div><a class="pitchButton" onclick="NextQuestion()">Yes, I am</a></div>';
                //    break;
                //case 9:
                //    template = '<div class="question" style="margin-top:-70px;"><span></span></div><div class="answer price" style="margin-top: 70px;padding: 0 15px 10px 0;"><div style="padding: 23px 4px 16px 35px;float: left;border-right: 1px solid #aaa;"><div style="margin: -18px 0 10px -18px;color: #F19F00;font-weight: bold;">GET STARTED TODAY</div><span style="font-size:2em;vertical-align: top;margin-left: 24px;">$</span><span id="annualPrice" style="font-size:4em;">{{Price}}</span><br /><span style="font-size: .9em;margin-left: 26px;">One time fee</span></div><div class="startToday"><div class="button buyBtn annual">Start Now</div><div class="underMinute">&nbsp;Creating your restaurant business plan</div></div></div>';
                //    template = template.replace("{{Price}}", price);
                //    break;
                //case 10:
                //    template = '<div class="question"><span>Sign Up Now</span></div><div class="answer signUp">';
                //    template += '<div>Name</div><input type="text" id="Name" />';
                //    template += '<div>Email</div><input type="text" id="Email" />';
                //    template += '<div>Password</div><input type="password" id="Password" />';
                //    template += '<div>Confirm Password</div><input type="password" id="ConfirmPassword" />';
                //    template += '<a class="button nextBtn" style="margin-left: 16px;" >Continue</a></div>';
                //    break;
                //case 11:
                //    template = '<div class="question"><span>Sign Up Now</span></div><div class="answer signUp creditCard">';
                //    //template += '<div class="annualHeader">Annual Subscription - $' + Math.floor(price / 2) + '/month (billed $'+ Math.floor(price / 2) * 12 + '/yr)</div>';
                //    //if (Annual)
                //    //{
                //    //    template += '<div class="price">$' + Math.floor(price / 2) * 12 + ' per year</div>';
                //    //    template += '<div style="margin-bottom:8px;"><img class="annual" src="img/CheckedRadio.png" />Annual<img class="monthly" src="img/UncheckedRadio.png" style="margin-left:24px;" />Monthly</div>';
                //    //}
                //    //else
                //    //{
                //    //    template += '<div class="price">$' + price + ' per month</div>';
                //    //    template += '<div style="margin-bottom:8px;"><img class="annual" src="img/UncheckedRadio.png" />Annual<img class="monthly" src="img/CheckedRadio.png" style="margin-left:24px;" />Monthly</div>';
                //    //}

                //    //template += '<div class="price">$' + price + '</div>';
                //    //var name = User.Name;
                //    //template += '<div>Cardholder Name</div><input type="text" id="Name" value="' + name + '" />';
                //    template += '<input type="text" id="CardNumber" placeholder="Credit Card Number" />';
                //    //template += '<br/><div style="width:155px;float:left;">&nbsp;&nbsp;MM&nbsp;&nbsp;&nbsp;/&nbsp;&nbsp;&nbsp;&nbsp;YY</div><div>CVC</div>';
                //    template += '<br/><input type="text" id="Month" style="width:36px;margin-right:8px;" placeholder="MM" /><input type="text" id="Year" placeholder="YY" style="width:32px;" /><input type="text" id="CVC" placeholder="CVC" style="width:50px;margin:0 0 1.2em 23px" />';
                //    template += '<div style="margin-left: 12px;font-size: 1.15em;">$' + price + ' one time charge</div>';
                //    template += '<div class="error"></div>';
                //    template += '<br/><a class="button nextBtn" >Create My Account</a>';
                //    template += '<img class="loading" style="margin-left:12px;display:none;" src="http://shop.skype.com/i/images/ab-test/u12_normal_2.gif" />';
                //    template += '</div>';
                //    break;

                //case 9:
                //    question = "Perfect. We are currently in private beta.<br/>Enter your email for an invitation<br/> to My Owner Box.";
                //    template = template.replace("{{Question}}", question);
                //    template = template.replace("<input", "<input placeholder='Email' ");
                //    template = template.replace("Next</a></div>", "Request</a></div>");
                //    break;
                //default:
                //    question = "Thank you for signing up.<br/>We will email you soon."
                //    template = '<div class="question"><span>{{Question}}</span></div>';
                //    template = template.replace("{{Question}}", question);
                //    break;
            }

            $("body").css("overflow-x", "hidden");

            $("#" + questionId).html(template);
            $("#" + questionId).show();

            ////var margin = (+$(".questions").css("width").substring(0, $(".questions").css("width").length - 2)) / -2;
            $("#" + questionId).css("margin-left", marginLeft);

            $("#" + questionId).animate({ left: "50%" }, 500, function () {
                $("body").css("overflow-x", "");
                $(".answer input").first().focus();
            });
        }

        function GoToApp() {
            $(".pitchButton").fadeOut();
            $(".questions img").fadeIn(1000);
            window.location.href = "/App";
        }

        function ValidateSignUp()
        {
            $(".signUp input").removeClass("error");
            var valid = true;
            if (!$("#Name").val()) {
                $("#Name").addClass("error");
                valid = false;
            }
            if (!$("#Email").val()) {
                $("#Email").addClass("error");
                valid = false;
            }
            if (!$("#Password").val()) {
                $("#Password").addClass("error");
                valid = false;
            }
            if (!$("#ConfirmPassword").val()) {
                $("#ConfirmPassword").addClass("error");
                valid = false;
            }
            if ($("#Password").val() != $("#ConfirmPassword").val()) {
                $("#Password").addClass("error");
                $("#ConfirmPassword").addClass("error");
                valid = false;
            }
            if (!valid)
                return false;
            else
                User = { Name: $("#Name").val(), Email: $("#Email").val(), Password: $("#Password").val() };

            $(".signUp img").show();
            $(".button").hide();

            var success = function (error) {
                if (error) {
                    $(".error").html(error);
                    $(".signUp img").hide();
                    $(".button").show();
                }
                else {
                    window.location.href = "/App";
                }
            };

            Post("CreateUser", { user: User, restaurantName: restaurantName }, success);
        }

        function ValidateCC()
        {
            if ($(".disabled").length)
                return;

            $(".error").html("");
            $(".signUp input").removeClass("error");
            var valid = true;
            if (!$("#CardNumber").val()) {
                $("#CardNumber").addClass("error");
                valid = false;
            }
            if (!$("#Month").val()) {
                $("#Month").addClass("error");
                valid = false;
            }
            if (!$("#Year").val()) {
                $("#Year").addClass("error");
                valid = false;
            }
            if (!$("#CVC").val()) {
                $("#CVC").addClass("error");
                valid = false;
            }
            if (!valid)
                return;

            User.Annual = $("img.annual").attr("src") == "img/CheckedRadio.png";

            var creditCard = { CardNumber: $("#CardNumber").val(), CardExpirationMonth: $("#Month").val(), CardExpirationYear: $("#Year").val(), Cvc: $("#CVC").val(), Price: price };
            User.CreditCard = creditCard;

            var success = function (error) {
                if (error) {
                    $(".error").html(error);
                    $(".loading").hide();
                    $(".button").removeClass("disabled");
                }
                else
                {
                    window.location.href = "/App";
                }
            };

            $(".loading").show();
            $(".button").addClass("disabled");

            Post("CreateUser", { user: User, restaurantName: restaurantName }, success);
        }

        function SendEmail(subject, body) {
            body += "%3Cbr/%3E%3Cbr/%3EPrice $" + price;
            body += "%3Cbr/%3E%3Cbr/%3E" + new Date();

            Post("SendEmail", { subject: subject, body: body });
        }
        
        function OpenLogin()
        {
            $(".modal-backdrop").show();
            $(".loginDialog").show();
        }

        function Login()
        {
            var success = function (error) {
                if (error) {
                    $(".loginDialog .error").html(error);
                }
                else {
                    window.location.href = "/App";
                }
            };
            Post("Login", { email: $("#LoginEmail").val(), password: $("#LoginPassword").val() }, success);
        }

        function SendPassword()
        {
            var success = function (error) {
                if (error) {
                    $(".forgotDialog .error").html(error);
                }
                else {
                    $(".forgotDialog .error").html("Your password has been sent.");
                    $(".forgotDialog .error").removeClass("error");
                }
            };
            Post("SendPassword", { email: $("#ForgotPasswordEmail").val() }, success);
        }

    </script>
</head>
<body>
<form id="form1" runat="server">
    <div class="modal-backdrop" style="height:150%"></div>
    <div class="loginDialog">
        <div class="dialogClose" onclick="$('.loginDialog').hide();$('.modal-backdrop').hide();">X</div>
        <h3>Log In</h3>
        <div>Email</div><input type="text" id="LoginEmail" />
        <div>Password</div><input type="password" id="LoginPassword" />
        <div class="forgotPassword">Forgot your password?</div>
        <div class="error" style="margin: -12px 0 12px;"></div>
        <a class="button" onclick="Login();">Log In</a>
    </div>  
    <div class="forgotDialog">
        <div class="dialogClose" onclick="$('.forgotDialog').hide();$('.modal-backdrop').hide();">X</div>
        <h3>Forgot Password</h3>
        <div>Enter your email to recover your password.</div><input type="text" id="ForgotPasswordEmail" />
        <div class="error" style="margin: 6px 0 -6px;"></div>
        <br />
        <a class="button" onclick="SendPassword();">Send Email</a>
    </div>   
    <header class="mainnav">
         <ul>
            <li><a href="#">HOME</a></li>
            <li><a href="#howitworks">ABOUT</a></li>
            <li><a href="#contact">CONTACT</a></li>
            <li class="loginBtn"><a onclick="OpenLogin();">LOG IN</a></li>
        </ul>
    </header>
    <div class="pitch">
        <div class="logoDiv">
            <img src="img/logo.png" />
        </div>
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
        <h2 style="margin-bottom:2em;"><span style="color:#F19F00;"><i>My Owner Box</i></span>&nbsp;&nbsp;Plan a restaurant today!</h2>
        <div class="step">
            <img src="img/lightbulbMoney.png" />
            <div class="horizontal"></div>
            <div>A well-conceived, professional restaurant business plan is your greatest single asset for turning your restaurant dreams into reality. It's the key to convincing anyone to invest money, make a loan, lease space, essentially do business with you prior to opening.
                <br /><br />
                Zach Wolff<br />
                Restaurant Consultant</div>
        </div>
        <div class="step">
            <img src="img/graph.png" />
            <div class="horizontal"></div>
            <div>Having a sound business plan was the single most important ingredient in making my new business a reality.
            <br /><br />
            Kellie Reed<br />
            The Topaz <br />
            Santa Rosa, CA
            </div>
        </div>
        <div class="step">
            <img src="img/logoBlue.png" />
            <div class="horizontal"></div>
            <div>The business plan is the most important thing I look at when considering a small business loan. Without a well thought-out business plan, I have a difficult time granting the loan.
                <br /><br />
                Joe Herman<br />
                Small Business Lender</div>
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
