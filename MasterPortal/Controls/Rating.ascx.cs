using System;
using System.Globalization;
using System.Web.UI;
using AjaxControlToolkit;

namespace Site.Controls
{
	public partial class Rating : UserControl
	{
		public event EventHandler RatingChanged;

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

		public double AverageRating
		{
			get
			{
				var rating = ViewState["AverageRating"];

				return rating == null ? 0 : (double)rating;
			}
			set
			{
				ViewState["AverageRating"] = value;
			}
		}

		public int RatingCount
		{
			get
			{
				var count = ViewState["RatingCount"];

				return count == null ? 0 : (int)count;
			}
			set
			{
				ViewState["RatingCount"] = value;
			}
		}

		public int UserRating
		{
			get
			{
				var rating = ViewState["UserRating"];

				return rating == null ? 0 : (int)rating;
			}
			set
			{
				ViewState["UserRating"] = value;
			}
		}

		public int MaxRating
		{
			get { return RatingStars.MaxRating; }
		}

		public int MinRating
		{
			get { return 0; }
		}

		public Boolean Enabled
		{
			get
			{
				var enabled = ViewState["Enabled"];

				return enabled == null || Convert.ToBoolean(enabled);
			}
			set
			{
				ViewState["Enabled"] = value;
			}
		}

		protected void Page_Load(object sender, EventArgs args)
		{
			if (IsPostBack) return;

			UpdateDisplay();
		}

		public void UpdateDisplay()
		{
			SetCurrentRating(RatingStars, Math.Min(Convert.ToInt32(Math.Round(AverageRating, MidpointRounding.AwayFromZero)), MaxRating));

			Count.Text = RatingCount.ToString(CultureInfo.InvariantCulture);

			RatingStars.ReadOnly = !Enabled;
		}

		private static void SetCurrentRating(AjaxControlToolkit.Rating ratingControl, int rating)
		{
			if (rating < 0)
			{
				ratingControl.CurrentRating = 0;

				return;
			}

			ratingControl.CurrentRating = (rating > ratingControl.MaxRating) ? ratingControl.MaxRating : rating;
		}

		protected void Rating_Changed(object sender, RatingEventArgs args)
		{
			int rating;

			if (!int.TryParse(args.Value, out rating)) return;

			UserRating = rating;

			UpdateDisplay();

			if (RatingChanged != null)
			{
				RatingChanged(this, EventArgs.Empty);
			}
		}
	}
}