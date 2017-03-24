using System;
using Adxstudio.Xrm.Cms;
using Microsoft.Xrm.Sdk;

namespace Site.Controls
{
	public partial class MultiRatingControl : PortalUserControl
	{
		public enum VoteValue
		{
			Yes = 1,
			No = 2,
			Unvoted = 0,
		}

		private const string DefaultRatingType = "rating";
		private const string DefaultInitEvent = "onload";
		private const string DefaultLogicalName = "adx_webpage";

		public bool IsPanel { get; set; }

		public Guid SourceID
		{
			get
			{
				var sourceID = ViewState["SourceID"];

				return sourceID == null ? Guid.Empty : (Guid)sourceID;
			}
			set { ViewState["SourceID"] = value; }
		}

		public IRatingInfo RatingInfo
		{
			get
			{
				var ratingInfo = ViewState["RatingInfo"];
				return ratingInfo == null ? null : ratingInfo as IRatingInfo;
			}
			set { ViewState["RatingInfo"] = value; }
		}

		public string LogicalName
		{
			get
			{
				var logicalName = (string)ViewState["LogicalName"];
				return string.IsNullOrEmpty(logicalName) ? DefaultLogicalName : logicalName;
			}
			set { ViewState["LogicalName"] = value.ToLower(); }
		}

		public string RatingType
		{
			get
			{
				var type = (string)ViewState["RatingType"];
				return string.IsNullOrEmpty(type) ? DefaultRatingType : type;
			}
			set { ViewState["RatingType"] = value.ToLower(); }
		}

		/// <devdoc>
		///    <para>Sets or gets the name of the event ahndler that provides initialization for this control.
		///  Set to OnLoad for Site Node-Level entities, set to OnDataBindsing for enumerated entities provided
		///  by a dataadapter (such as in a listview) </para>
		/// </devdoc>
		public string InitEventName
		{
			get
			{
				var name = (string)ViewState["InitEventName"];
				return string.IsNullOrEmpty(name) ? DefaultInitEvent : name;
			}
			set { ViewState["InitEventName"] = value.ToLower(); }
		}

		public bool EnableRatings
		{
			get
			{
				var value = ViewState["EnableRatings"];

				if (value is bool)
				{
					return (bool) value;
				}

				var sourceReference = GetSourceReference();

				var dataAdapterFactory = new RatingDataAdapterFactory(sourceReference);

				var dataAdapter = dataAdapterFactory.GetAdapter(Portal, Request.RequestContext);

				var ratingsEnabled = dataAdapter.RatingsEnabled;

				ViewState["EnableRatings"] = ratingsEnabled;

				return ratingsEnabled;
			}
			set { ViewState["EnableRatings"] = value; }
		}

		protected string VisitorID
		{
			get { return Context.Profile.UserName; }
		}

		protected override void OnDataBinding(EventArgs e)
		{
			base.OnDataBinding(e);
			if (InitEventName != "ondatabinding") return;
			InitialiseControl();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (InitEventName != "onload") return;
			InitialiseControl(); //otherwise wait for databinding.
		}
		
		protected void InitialiseControl()
		{
			var sourceReference = GetSourceReference();

			var dataAdapterFactory = new RatingDataAdapterFactory(sourceReference);

			var dataAdapter = dataAdapterFactory.GetAdapter(Portal, Request.RequestContext);

			if (RatingInfo == null)
			{
				RatingInfo = dataAdapter.GetRatingInfo();

				if (RatingInfo == null)
				{
					throw new Exception("Error getting Rating Info");
				}
			}

			var userRating = dataAdapter.SelectUserRating();

			var visitorRating = dataAdapter.SelectVisitorRating(VisitorID);

			switch (RatingType)
			{
				case "vote":

					VoteControl.Visible = EnableRatings;
					VoteControl.SourceID = SourceID;
					VoteControl.YesCount = RatingInfo.YesCount;
					VoteControl.NoCount = RatingInfo.NoCount;

					if (userRating != null)
					{
						int userVote;

						if (userRating.MaximumValue == userRating.Value)
						{
							userVote = (int)VoteValue.Yes;
						}
						else if (userRating.MinimumValue == userRating.Value)
						{
							userVote = (int)VoteValue.No;
						}
						else
						{
							userVote = (int)VoteValue.Unvoted;
						}

						VoteControl.UserVote = userVote;
					}
					else if (visitorRating != null)
					{
						int userVote;

						if (visitorRating.MaximumValue == visitorRating.Value)
						{
							userVote = (int)VoteValue.Yes;
						}
						else if (visitorRating.MinimumValue == visitorRating.Value)
						{
							userVote = (int)VoteValue.No;
						}
						else
						{
							userVote = (int)VoteValue.Unvoted;
						}

						VoteControl.UserVote = userVote;
					}

					VoteControl.UpdateDisplay();

					break;

				default:

					RatingControl.Visible = EnableRatings;

					RatingControl.SourceID = SourceID;

					RatingControl.AverageRating = RatingInfo.AverageRating;
					RatingControl.RatingCount = RatingInfo.RatingCount;

					if (userRating != null)
					{
						RatingControl.UserRating = userRating.Value;
					}
					else if (visitorRating != null)
					{
						RatingControl.UserRating = visitorRating.Value;
					}

					RatingControl.UpdateDisplay();

					break;
			}
		}

		private EntityReference GetSourceReference()
		{
			if (SourceID != Guid.Empty && !String.IsNullOrEmpty(LogicalName))
			{
				return new EntityReference(LogicalName, SourceID);
			}
			else //use the Current Entity in the IPortalContext
			{
				return Portal.Entity.ToEntityReference();
			}
		}

		protected void Rating_Changed(object sender, EventArgs e)
		{
			var ratingControl = (Rating)sender;

			if (ratingControl == null) return;

			var rating = ratingControl.UserRating;

			var maxRating = ratingControl.MaxRating;

			var minRating = ratingControl.MinRating;

			var sourceReference = GetSourceReference();

			var dataAdapterFactory = new RatingDataAdapterFactory(sourceReference);

			var dataAdapter = dataAdapterFactory.GetAdapter(Portal, Request.RequestContext);

			dataAdapter.SaveRating(rating, maxRating, minRating, VisitorID);

			RatingInfo = dataAdapter.GetRatingInfo();

			ratingControl.AverageRating = RatingInfo.AverageRating;

			ratingControl.RatingCount = RatingInfo.RatingCount;

			ratingControl.UpdateDisplay();
		}

		protected void Vote_Changed(object sender, EventArgs e)
		{
			var voteControl = (Vote)sender;

			if (voteControl == null) return;

			int rating;

			switch (voteControl.UserVote)
			{
				case 1:
					rating = 1;
					break;
				case 2:
					rating = 0;
					break;
				case 0:
				default:
					rating = 3;
					break;
			}

			var sourceReference = GetSourceReference();

			var dataAdapterFactory = new RatingDataAdapterFactory(sourceReference);

			var dataAdapter = dataAdapterFactory.GetAdapter(Portal, Request.RequestContext);

			if (rating == 3)
			{
				dataAdapter.DeleteUserRating(VisitorID);
			}
			else
			{
				dataAdapter.SaveRating(rating, 1, 0, VisitorID);
			}

			RatingInfo = dataAdapter.GetRatingInfo();

			voteControl.YesCount = RatingInfo.YesCount;

			voteControl.NoCount = RatingInfo.NoCount;

			voteControl.UpdateDisplay();

			Response.Redirect(Request.Url.PathAndQuery);
		}
	}
}