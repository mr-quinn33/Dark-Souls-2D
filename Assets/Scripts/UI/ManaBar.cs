namespace Assets.Scripts.UI
{
    internal sealed class ManaBar : StatsBar
    {
        private protected override void Start()
        {
            base.Start();
            SetMaxValue(character.MaxMana);
            SetValue(character.Mana);
            character.OnMaxManaUpdated += SetMaxValue;
            character.OnManaUpdated += SetValue;
        }

        private void OnDisable()
        {
            character.OnMaxManaUpdated -= SetMaxValue;
            character.OnManaUpdated -= SetValue;
        }
    }
}