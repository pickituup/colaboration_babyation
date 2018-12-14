using Xamarin.Forms;
using BabyationApp.iOS.Renderers;
using Xamarin.Forms.Platform.iOS;
using BabyationApp.Controls.Views;

[assembly: ExportRenderer(typeof(ViewCell), typeof(ViewCellRendererEx))]
namespace BabyationApp.iOS.Renderers
{
	public class ViewCellRendererEx : ViewCellRenderer
	{
		UIKit.UIView _bgView = null;
		public override UIKit.UITableViewCell GetCell(Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv)
		{
			tv.AllowsSelection = false;
			var cell = base.GetCell(item, reusableCell, tv);
			UIKit.UIColor backColor = Xamarin.Forms.Color.Transparent.ToUIColor();

			var cellEx = item as ViewCellEx;
			if (cellEx != null)
			{
				backColor = cellEx.CellBackColor.ToUIColor();
	
			}

			cell.BackgroundColor = backColor;

			if (_bgView == null)
			{
				_bgView = new UIKit.UIView
				{
					BackgroundColor = backColor
				};
			}
			cell.BackgroundView = _bgView;

			cell.SelectionStyle = UIKit.UITableViewCellSelectionStyle.None;
			switch (item.StyleId)
			{
				case "checkmark":
					cell.Accessory = UIKit.UITableViewCellAccessory.Checkmark;
					break;
				case "detail-button":
					cell.Accessory = UIKit.UITableViewCellAccessory.DetailButton;
					break;
				case "detail-disclosure-button":
					cell.Accessory = UIKit.UITableViewCellAccessory.DetailDisclosureButton;
					break;
				case "disclosure":
				default:
					cell.Accessory = UIKit.UITableViewCellAccessory.None;
					break;
			}
			return cell;
		}

	}
}