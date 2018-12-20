using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BabyationApp.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BusyIndicator : ContentView
	{
        public static readonly BindableProperty PadCanvasColorProperty = BindableProperty.Create(
            nameof(PadCanvasColor),
            typeof(Color),
            typeof(BusyIndicator),
            defaultValue: Color.Black);

        public static readonly BindableProperty IndicatorColorProperty = BindableProperty.Create(
            nameof(IndicatorColor),
            typeof(Color),
            typeof(BusyIndicator),
            defaultValue: Color.White);

        public static readonly BindableProperty PadOpacityProperty = BindableProperty.Create(
            nameof(PadOpacity),
            typeof(double),
            typeof(BusyIndicator),
            defaultValue: .5);

        public BusyIndicator()
        {
            InitializeComponent();

            _padCanvas_Boxview.SetBinding(BoxView.BackgroundColorProperty, new Binding(nameof(PadCanvasColor), source: this));
            _padCanvas_Boxview.SetBinding(BoxView.OpacityProperty, new Binding(nameof(PadOpacity), source: this));
            _spinnerIndicator_ActivityIndicator.SetBinding(ActivityIndicator.ColorProperty, new Binding(nameof(IndicatorColor), source: this));
        }

        public Color PadCanvasColor
        {
            get => (Color)GetValue(PadCanvasColorProperty);
            set => SetValue(PadCanvasColorProperty, value);
        }

        public Color IndicatorColor
        {
            get => (Color)GetValue(IndicatorColorProperty);
            set => SetValue(IndicatorColorProperty, value);
        }

        public double PadOpacity
        {
            get => (double)GetValue(PadOpacityProperty);
            set => SetValue(PadOpacityProperty, value);
        }
    }
}