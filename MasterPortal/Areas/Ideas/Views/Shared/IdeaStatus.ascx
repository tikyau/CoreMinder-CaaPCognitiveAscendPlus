<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Adxstudio.Xrm.Ideas.IIdea>" %>
<%@ Import Namespace="Site.Areas.Ideas" %>

<span class="label
<%= Model.Status == (int)IdeaStatus.New ? " label-info" : string.Empty %>
<%= Model.Status == (int)IdeaStatus.Inactive ? " label-default" : string.Empty %>
<%= Model.Status == (int)IdeaStatus.Accepted ? " label-primary" : string.Empty %>
<%= Model.Status == (int)IdeaStatus.Completed ? " label-success" : string.Empty %>
<%= Model.Status == (int)IdeaStatus.Rejected ? " label-danger" : string.Empty %>"><%: Enum.GetName(typeof(IdeaStatus), Model.Status)%></span>
