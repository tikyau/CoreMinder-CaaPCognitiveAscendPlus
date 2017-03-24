<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/WebForms.master" AutoEventWireup="true" CodeBehind="GiftList.aspx.cs" Inherits="Site.Areas.WeChat.Pages.GiftList" %>

<%@ Register Src="~/Controls/ChildNavigation.ascx" TagName="ChildNavigation" TagPrefix="site" %>
<%@ Register Src="~/Controls/Comments.ascx" TagName="Comments" TagPrefix="site" %>
<%@ OutputCache CacheProfile="User" %>

<%--<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <adx:Snippet SnippetName="Social Share Widget Code Page Bottom" EditType="text" DefaultText="" HtmlTag="Div" runat="server" />
    <adx:CrmEntityListView runat="server" ID="ListView" CssClass="table-responsive" ListCssClass="table table-striped" ClientIDMode="Static" AlternatingRowStyle-CssClass="alternate-row" SelectMode="Multiple" LanguageCode="<%$ SiteSetting: Language Code, 0 %>" PortalName="<%$ SiteSetting: Language Code %>"></adx:CrmEntityListView>      
</asp:Content>--%>

<asp:Content ContentPlaceHolderID="EntityControls" runat="server">
    <style type="text/css">
        .form-style-10 {
            width: initial;
            padding: 30px;
            margin: 40px auto;
            background: #FFF;
            border-radius: 10px;
            -webkit-border-radius: 10px;
            -moz-border-radius: 10px;
            box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.13);
            -moz-box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.13);
            -webkit-box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.13);
        }

            .form-style-10 .inner-wrap {
                padding: 30px;
                background: #F8F8F8;
                border-radius: 6px;
                margin-bottom: 15px;
            }

            .form-style-10 h1 {
                background: #2A88AD;
                padding: 20px 30px 15px 30px;
                margin: -30px -30px 30px -30px;
                border-radius: 10px 10px 0 0;
                -webkit-border-radius: 10px 10px 0 0;
                -moz-border-radius: 10px 10px 0 0;
                color: #fff;
                text-shadow: 1px 1px 3px rgba(0, 0, 0, 0.12);
                font: normal 30px "Roboto","Helvetica Neue",Helvetica,Arial,sans-serif;
                -moz-box-shadow: inset 0px 2px 2px 0px rgba(255, 255, 255, 0.17);
                -webkit-box-shadow: inset 0px 2px 2px 0px rgba(255, 255, 255, 0.17);
                box-shadow: inset 0px 2px 2px 0px rgba(255, 255, 255, 0.17);
                border: 1px solid #257C9E;
            }

                .form-style-10 h1 > span {
                    display: block;
                    margin-top: 2px;
                    font: 13px Arial, Helvetica, sans-serif;
                }

            .form-style-10 label {
                display: block;
                font: 13px Arial, Helvetica, sans-serif;
                color: #888;
                margin-bottom: 15px;
            }

            .form-style-10 input[type="text"],
            .form-style-10 input[type="date"],
            .form-style-10 input[type="datetime"],
            .form-style-10 input[type="email"],
            .form-style-10 input[type="number"],
            .form-style-10 input[type="search"],
            .form-style-10 input[type="time"],
            .form-style-10 input[type="url"],
            .form-style-10 input[type="password"],
            .form-style-10 textarea,
            .form-style-10 select {
                display: block;
                box-sizing: border-box;
                -webkit-box-sizing: border-box;
                -moz-box-sizing: border-box;
                width: 100%;
                padding: 8px;
                border-radius: 6px;
                -webkit-border-radius: 6px;
                -moz-border-radius: 6px;
                border: 2px solid #fff;
                box-shadow: inset 0px 1px 1px rgba(0, 0, 0, 0.33);
                -moz-box-shadow: inset 0px 1px 1px rgba(0, 0, 0, 0.33);
                -webkit-box-shadow: inset 0px 1px 1px rgba(0, 0, 0, 0.33);
            }

            .form-style-10 .section {
                font: normal 20px 'Bitter', serif;
                color: #2A88AD;
                margin-bottom: 5px;
            }

                .form-style-10 .section span {
                    background: #2A88AD;
                    padding: 5px 10px 5px 10px;
                    position: absolute;
                    border-radius: 50%;
                    -webkit-border-radius: 50%;
                    -moz-border-radius: 50%;
                    border: 4px solid #fff;
                    font-size: 14px;
                    margin-left: -45px;
                    color: #fff;
                    margin-top: -3px;
                }

            .form-style-10 input[type="button"],
            .form-style-10 input[type="submit"] {
                background: #2A88AD;
                padding: 8px 20px 8px 20px;
                border-radius: 5px;
                -webkit-border-radius: 5px;
                -moz-border-radius: 5px;
                color: #fff;
                text-shadow: 1px 1px 3px rgba(0, 0, 0, 0.12);
                font: normal 30px "Roboto","Helvetica Neue",Helvetica,Arial,sans-serif;
                -moz-box-shadow: inset 0px 2px 2px 0px rgba(255, 255, 255, 0.17);
                -webkit-box-shadow: inset 0px 2px 2px 0px rgba(255, 255, 255, 0.17);
                box-shadow: inset 0px 2px 2px 0px rgba(255, 255, 255, 0.17);
                border: 1px solid #257C9E;
                font-size: 15px;
            }

                .form-style-10 input[type="button"]:hover,
                .form-style-10 input[type="submit"]:hover {
                    background: #2A6881;
                    -moz-box-shadow: inset 0px 2px 2px 0px rgba(255, 255, 255, 0.28);
                    -webkit-box-shadow: inset 0px 2px 2px 0px rgba(255, 255, 255, 0.28);
                    box-shadow: inset 0px 2px 2px 0px rgba(255, 255, 255, 0.28);
                }

            .form-style-10 .privacy-policy {
                float: right;
                width: 250px;
                font: 12px Arial, Helvetica, sans-serif;
                color: #4D4D4D;
                margin-top: 10px;
                text-align: right;
            }
    </style>   
       	<div class="table-responsive">
			<asp:GridView ID="GiftListView" runat="server" CssClass="table table-striped" GridLines="None" AlternatingRowStyle-CssClass="alternate-row" AllowSorting="true"
			OnSorting="AcceptedGiftList_Sorting" OnRowDataBound="GiftList_OnRowDataBound" >
				<EmptyDataTemplate>
					<adx:Snippet runat="server" SnippetName="gifts/view/empty" DefaultText="There are no gifts for the selected filter." Editable="true" EditType="html"/>
				</EmptyDataTemplate>
			</asp:GridView>
		</div>
    <site:ChildNavigation ID="ChildNavigation1" ShowDescriptions="true" runat="server" />
    <site:Comments RatingType="vote" EnableRatings="False" runat="server" />
</asp:Content>
