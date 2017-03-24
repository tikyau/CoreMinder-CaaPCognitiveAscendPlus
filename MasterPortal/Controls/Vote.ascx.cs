using System;
using System.Web.UI;
using Microsoft.Xrm.Sdk;

namespace Site.Controls
{
	/// <summary>
	/// Thumbs Up & Thumbs Down User Control.
	/// 
	/// YOu MUST set EnableEventValidation="false" in the @Page declaration of your parent .aspx page in order for this control
	/// to function properly.
	/// </summary>
	public partial class Vote : UserControl
	{
		public event EventHandler VoteChanged;

		protected static string VoteYesImageURL = "~/xrm-adx/samples/images/thumb_up.png";

		protected static string VoteNoImageURL = "~/xrm-adx/samples/images/thumb_down.png";

		protected static string SpacerImageURL = "~/xrm-adx/samples/images/spacer.gif";

		public Guid SourceID
		{
			get
			{
				var sourceID = ViewState["SourceID"];

				return sourceID == null ? Guid.Empty : (Guid)sourceID;
			}
			set
			{
				ViewState["SourceID"] = value;
			}
		}

		public int YesCount
		{
			get
			{
				var yesCount = ViewState["YesCount"];
				return yesCount == null ? 0 : Convert.ToInt32(yesCount);
			}
			set
			{
				ViewState["YesCount"] = value;
			}
		}

		public int NoCount
		{
			get
			{
				var noCount = ViewState["NoCount"];
				return noCount == null ? 0 : Convert.ToInt32(noCount);
			}
			set
			{
				ViewState["NoCount"] = value;
			}
		}

		public int UserVote
		{
			get
			{
				var userVote = ViewState["UserVote"];
				return userVote == null ? 0 : Convert.ToInt32(userVote);
			}
			set
			{
				ViewState["UserVote"] = value;
			}
		}

		public Boolean Enabled
		{
			get
			{
				var enabled = ViewState["Enabled"];
				return enabled == null ? true : Convert.ToBoolean(enabled);
			}
			set
			{
				ViewState["Enabled"] = value;
			}
		}

		protected void Page_Load(object sender, EventArgs args)
		{
			if (!Page.IsPostBack)
			{
				UpdateDisplay();
			}
		}

		public void UpdateDisplay()
		{
			VoteYesCount.Text = YesCount.ToString();

			VoteNoCount.Text = NoCount.ToString();

			SetUserVote(UserVote);

			UpdatePanelVotes.Update();

			VoteYesButton.Enabled = Enabled;

			VoteNoButton.Enabled = Enabled;

			User.Visible = Enabled;
		}

		private void SetUserVote(int vote)
		{
			switch (vote)
			{
				case 1:

					VoteButton.ImageUrl = VoteYesImageURL;

					VoteButton.Visible = true;

					VoteButton.Enabled = true;

					VoteButton.ToolTip = "Click here to delete your vote";

					User.Visible = true;

					break;

				case 2:

					VoteButton.ImageUrl = VoteNoImageURL;

					VoteButton.Visible = true;

					VoteButton.Enabled = true;

					VoteButton.ToolTip = "Click here to delete your vote";

					User.Visible = true;

					break;

				default:

					VoteButton.ImageUrl = SpacerImageURL;

					VoteButton.Visible = false;

					VoteButton.Enabled = false;

					VoteButton.ToolTip = "";

					User.Visible = false;

					break;
			}
		}

		protected void VoteYes(object sender, ImageClickEventArgs e)
		{
			UserVote = 1;

			UpdateDisplay();

			if (VoteChanged != null)
			{
				VoteChanged(this, EventArgs.Empty);
			}
		}

		protected void VoteNo(object sender, ImageClickEventArgs e)
		{
			UserVote = 2;

			UpdateDisplay();

			if (VoteChanged != null)
			{
				VoteChanged(this, EventArgs.Empty);
			}
		}

		protected void DeleteVote(object sender, ImageClickEventArgs e)
		{
			UserVote = 0;

			UpdateDisplay();

			if (VoteChanged != null)
			{
				VoteChanged(this, EventArgs.Empty);
			}
		}

	}
}