namespace Assets.Scripts.UI
{
    internal sealed class StaminaBar : StatsBar
    {
        private protected override void Start()
        {
            base.Start();
            SetMaxValue(character.MaxStamina);
            SetValue(character.Stamina);
            character.OnMaxStaminaUpdated += SetMaxValue;
            character.OnStaminaUpdated += SetValue;
        }

        private void OnDisable()
        {
            character.OnMaxStaminaUpdated -= SetMaxValue;
            character.OnStaminaUpdated -= SetValue;
        }
    }
}