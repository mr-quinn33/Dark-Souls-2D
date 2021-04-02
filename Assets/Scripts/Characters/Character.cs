using Assets.Scripts.Interfaces;
using Assets.Scripts.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Characters
{
    public abstract class Character : ScriptableObject, IDestructible
    {
        #region Fields
        [Header("Health")]
        [SerializeField] private int maxHealth;
        [SerializeField] private int health;

        [Header("Mana")]
        [SerializeField] private int maxMana;
        [SerializeField] private int mana;

        [Header("Stamina")]
        [SerializeField] private float maxStamina;
        [SerializeField] private float stamina;
        [SerializeField] private float staminaRegen;

        [Header("Time Delay")]
        [SerializeField] private float staminaRegenDelay;
        [SerializeField] private float destroyDelay;

        [Header("Defence")]
        [SerializeField] private int physicalDefence;
        [SerializeField] private int magicalDefence;

        public event Func<float, Animator> OnDestroy;
        public event Action OnStaminaExhausted;
        public event Action<int> OnMaxHealthUpdated;
        public event Action<int> OnHealthUpdated;
        public event Action<int> OnMaxManaUpdated;
        public event Action<int> OnManaUpdated;
        public event Action<float> OnMaxStaminaUpdated;
        public event Action<float> OnStaminaUpdated;

        private CancellationTokenSource cancellationTokenSource;
        #endregion

        #region Properties
        public int MaxHealth
        {
            get => maxHealth;
            private set
            {
                maxHealth = value < 0 ? default : value;
                OnMaxHealthUpdated?.Invoke(maxHealth);
            }
        }

        public int Health
        {
            get => health;
            private set
            {
                if (value <= 0)
                {
                    health = default;
                    Destroy(destroyDelay);
                }
                else
                {
                    health = value > maxHealth ? maxHealth : value;
                }
                OnHealthUpdated?.Invoke(health);
            }
        }

        public int MaxMana
        {
            get => maxMana;
            private set
            {
                maxMana = value < 0 ? default : value;
                OnMaxManaUpdated?.Invoke(maxMana);
            }
        }

        public int Mana
        {
            get => mana;
            private set
            {
                mana = value < 0 ? default : value > maxMana ? maxMana : value;
                OnManaUpdated?.Invoke(mana);
            }
        }

        public float MaxStamina
        {
            get => maxStamina;
            private set
            {
                maxStamina = value < 0f ? default : value;
                OnMaxStaminaUpdated?.Invoke(maxStamina);
            }
        }

        public float Stamina
        {
            get => stamina;
            private set
            {
                if (value < 0f)
                {
                    stamina = default;
                    OnStaminaExhausted?.Invoke();
                }
                else
                {
                    stamina = value > maxStamina ? maxStamina : value;
                }
                OnStaminaUpdated?.Invoke(stamina);
            }
        }

        public float StaminaRegen { get => staminaRegen; private set => staminaRegen = value < 0f ? default : value; }

        public int PhysicalDefence => physicalDefence;

        public int MagicalDefence => magicalDefence;

        public Vector2 Vector { get; set; }

        public enum Directions { Up, Down, Left, Right };

        public Directions Direction => Vector == default ? Directions.Down : Vector.y + Vector.x > 0f
            ? Vector.y - Vector.x > 0f ? Directions.Up : Directions.Right
            : Vector.y - Vector.x > 0f ? Directions.Left : Directions.Down;
        #endregion

        #region Methods
        public Character Clone(Character character)
        {
            character.maxHealth = maxHealth;
            character.health = health;
            character.maxMana = maxMana;
            character.mana = mana;
            character.maxStamina = maxStamina;
            character.stamina = stamina;
            character.staminaRegen = staminaRegen;
            character.staminaRegenDelay = staminaRegenDelay;
            character.destroyDelay = destroyDelay;
            character.physicalDefence = physicalDefence;
            character.magicalDefence = magicalDefence;
            return character;
        }

        #region Increasers

        public void IncreaseMaxHealthOnly(int amount) => MaxHealth += Mathf.Clamp(amount, default, int.MaxValue);

        public void IncreaseMaxHealthAndHealth(int amount)
        {
            MaxHealth += Mathf.Clamp(amount, default, int.MaxValue);
            Health += Mathf.Clamp(amount, default, int.MaxValue);
        }

        public void IncreaseHealth(int amount) => Health += Mathf.Clamp(amount, default, int.MaxValue);

        public void IncreaseMaxManaOnly(int amount) => MaxMana += Mathf.Clamp(amount, default, int.MaxValue);

        public void IncreaseMaxManaAndMana(int amount)
        {
            MaxMana += Mathf.Clamp(amount, default, int.MaxValue);
            Mana += Mathf.Clamp(amount, default, int.MaxValue);
        }

        public void IncreaseMana(int amount) => Mana += Mathf.Clamp(amount, default, int.MaxValue);

        public void IncreaseMaxStamina(float amount)
        {
            MaxStamina += Mathf.Clamp(amount, default, float.MaxValue);
            if (Stamina == MaxStamina)
            {
                DecreaseStaminaAsync(default);
            }
        }

        public void IncreaseStaminaRegen(float amount) => StaminaRegen += Mathf.Clamp(amount, default, float.MaxValue);

        #endregion

        #region Decreasers

        public void DecreaseMaxHealth(int amount)
        {
            int newMaxHealth = MaxHealth - Mathf.Clamp(amount, default, int.MaxValue);
            MaxHealth = newMaxHealth < Health ? (Health = newMaxHealth) : newMaxHealth;
        }

        public void DecreaseHealth(int amount) => Health -= Mathf.Clamp(amount, default, int.MaxValue);

        public void DecreaseMaxMana(int amount)
        {
            int newMaxMana = MaxMana - Mathf.Clamp(amount, default, int.MaxValue);
            MaxMana = newMaxMana < Mana ? (Mana = newMaxMana) : newMaxMana;
        }

        public void DecreaseMana(int amount) => Mana -= Mathf.Clamp(amount, default, int.MaxValue);

        public void DecreaseMaxStamina(float amount)
        {
            float newMaxStamina = MaxStamina - Mathf.Clamp(amount, default, float.MaxValue);
            if (newMaxStamina < Stamina)
            {
                cancellationTokenSource?.Cancel();
                Stamina = MaxStamina = newMaxStamina;
            }
            else
            {
                MaxStamina = newMaxStamina;
            }
        }

        public async void DecreaseStaminaAsync(float amount)
        {
            Stamina -= Mathf.Clamp(amount, default, float.MaxValue);
            try
            {
                await RecoverStaminaAsync(TaskUtility.RefreshTokenSource(ref cancellationTokenSource));
            }
            catch (OperationCanceledException) { }
        }

        public void DecreaseStaminaRegen(float amount) => StaminaRegen -= Mathf.Clamp(amount, default, float.MaxValue);
        #endregion

        public virtual void Destroy(float time) => OnDestroy?.Invoke(time).SetTrigger("Dead");

        private protected async void ResetStats(float time)
        {
            await Task.Delay((int)time * 1000);
            Health = MaxHealth;
            Mana = MaxMana;
            Stamina = MaxStamina;
        }

        private async Task RecoverStaminaAsync(CancellationToken cancellationToken)
        {
            await Task.Delay((int)staminaRegenDelay * 1000);
            while (stamina < maxStamina)
            {
                await Task.Yield();
                cancellationToken.ThrowIfCancellationRequested();
                stamina += staminaRegen * Time.deltaTime;
                OnStaminaUpdated?.Invoke(stamina);
            }
            stamina = maxStamina;
            OnStaminaUpdated?.Invoke(stamina);
        }
        #endregion
    }
}