namespace Assets.Scripts.UI
{
    internal sealed class HealthBar : StatsBar
    {
        private protected override void Start()
        {
            base.Start();
            SetMaxValue(character.MaxHealth);
            SetValue(character.Health);
            character.OnMaxHealthUpdated += SetMaxValue;
            character.OnHealthUpdated += SetValue;
        }

        private void OnDisable()
        {
            character.OnMaxHealthUpdated -= SetMaxValue;
            character.OnHealthUpdated -= SetValue;
        }
    }
}