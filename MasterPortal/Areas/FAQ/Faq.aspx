<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/WebFormsContent.master" AutoEventWireup="true" CodeBehind="Faq.aspx.cs" Inherits="Site.Areas.FAQ.Faq" %>
<%@ Import Namespace="Adxstudio.Xrm.Web.Mvc.Html" %>



<asp:Content ID="Content2" ContentPlaceHolderID="Breadcrumbs" runat="server">
    <ul class="breadcrumb"><li><a href="/">Home</a></li>
        <li class="active">FAQ</li>
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageHeader" runat="server">
    <h1>FAQ</h1>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server" EnablePageMethods='true'>
    <div id="searchBox">
      <div class="content-panel panel panel-default">
          <div class="panel-heading">Search</div>
          <div class="panel-body">
            <div class="form-group">
                <input name="" type="text" id="txtSearch" class="form-control" placeholder="Enter the question">
            </div>
            <div class="form-group">
                <button class="form-control" id="btnSearch"  type='button'> Search </button>
                <!-- button must must the attribute which is button, after do that the page will not reload-->
            </div>
          </div>
        </div>
    </div>

    <div id="searchResult">
        <%--<p>
            <strong>Q:  Can partners continue to transact Adxstudio Portals deals?</strong>
        </p>
        <p>A: Reseller partners will continue to use Adxstudio agreement forms until Adxstudio licensing is incorporated into the Microsoft Volume Licensing program (currently targeted for the Spring Release). </p>
        --%>
        <hr />
    </div>

    <!-- bot container start -->
    <button id="btnBar" type="button" class="btn btn-primary">Live Chat</button>
    <div id="botContainer">  
        <iframe id="botiframe"  src='https://webchat.botframework.com/embed/demo0303?s=hBywCv5Vxe4.cwA.C3Q.-aeXDZK4L5g9Du53ltwLhV_U2OJq9_mwypwvHN9PHQ8'></iframe>
    </div>
    <!-- bot container end -->
<script>

    $("#btnSearch").on('click', function () {
        var baseId = "726c117c-ad74-4bf4-9911-2a14aadd3b75";
        var subKey = "ca93418621c04d07811966b37a23e4ee";
        var query = $("#txtSearch").val();
        $("#txtSearch").val("");
        $.ajax({
            "async": true,
            "crossDomain": true,
            "url": "https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/" + baseId + "/generateAnswer",
            "method": "POST",
            "headers": {
                "ocp-apim-subscription-key": subKey,
                "content-type": "application/json",
                "cache-control": "no-cache"
            },
            "data": "{'question':'" + query + "',\"top\":3};"
        })
          .done(function (data) {
                $("#searchResult").text("");
                $("#searchResult").append("<h3><b>Your question</b> : " + query + "</h3><br>");
                for (i = 0; i < data.answers.length; i++) {
                    console.log(data.answers[i].score); 
                    console.log(data.answers[i].answer);                   
                    $("#searchResult").append("<p><b>A</b> : " + data.answers[i].answer + "</p><br>");
                    $("#searchResult").append("<b>Score</b> : " + data.answers[i].score + "<hr>");
                }
              //console.log(data.answers["0"].answer);
              
          })
          .fail(function (err) {
              console.log(err);
              alert(error);
          });
    })


    $("#txtSearch").keypress(function (e) {
        if (e.keyCode == 13) {//  13 enter          
            $("#btnSearch").click();
            return false;// must return false, so the page will not reload********
        }
    })
</script>
<style>
    /*bot Container start*/
    #botContainer {
        position:fixed;
        right:0;
        bottom:-448px;
        z-index:9999;
        background-color:white;
    }

    #botiframe {
        position:relative;
        right:0;
        height: 480px;
        width: 402px;
        }

    #btnBar{
      position:fixed;
      bottom:2px;
      right:420px;
      z-index:9999;
      padding-bottom: 6px;
      padding-top: 6px;
    }
    
    #searchResult > hr{
      border: 0;
      height: 1px;
      background: #333;
      background-image: linear-gradient(to right, #ccc, #333, #ccc);
    }
    /*bot Container end*/
</style>

</asp:Content>




