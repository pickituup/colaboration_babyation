using Xamarin.Forms;

namespace BabyationApp.Effects
{
    public class UnderlineEffect : RoutingEffect
    {

        public const string EffectNamespace = "BabyationApp.Effects.UnderlineEffect";

        public UnderlineEffect()
            : base($"{EffectNamespace}.{nameof(UnderlineEffect)}") { }
    }
}
