using System;
using Xamarin.Forms;


namespace BabyationApp.Controls.Views
{
    /// <summary>
    /// Extented ViewCell to let customize through platform depended renderers
    /// </summary>
	public class ViewCellEx : ViewCell
	{
        /// <summary>
        /// Constructor
        /// </summary>
		public ViewCellEx()
		{
		}

		public static readonly BindableProperty CellBackColorProperty = BindableProperty.Create("CellBackColor", typeof(Color), typeof(ViewCellEx), Color.Transparent);
        /// <summary>
        /// Cells back color
        /// </summary>
        public Color CellBackColor
		{
			get { return (Color)GetValue(CellBackColorProperty); }
			set
			{
                SetValue(CellBackColorProperty, value);

			}
		}
	}
}
