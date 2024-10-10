using System;
using UnityEngine;

namespace Core.Utility.SmartParameter
{
    [Serializable]
    public class CustomParameter : IParameter, IReadonlyParameter, IReactiveOnlyParameter
    {
        #region Parameters API

        [field: SerializeField] public float DefaultValue { get; private set; }

        public float CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                AvoidBreakingLimit();
                OnCurrentChanged?.Invoke(_currentValue);
                OnAnyChanged?.Invoke(_currentValue, _limitValue);
            }
        }

        public event Action<float> OnCurrentChanged;

        public float FlatValue
        {
            get => _flatValue;
            set
            {
                _flatValue = value;
                PushLimitByMult();
                OnFlatChanged?.Invoke(_flatValue);
                OnAnyChanged?.Invoke(_currentValue, _limitValue);
            }
        }

        public event Action<float> OnFlatChanged;

        public float LimitValue
        {
            get => _limitValue;
            set
            {
                _limitValue = value;
                AvoidBreakingLimit();
                OnLimitChanged?.Invoke(_limitValue);
                OnAnyChanged?.Invoke(_currentValue, _limitValue);
            }
        }

        public event Action<float> OnLimitChanged;

        public float Multiplier
        {
            get => _multiplier;
            set
            {
                _multiplier = value;
                PushLimitByMult();
                OnAnyChanged?.Invoke(_currentValue, _limitValue);
            }
        }

        /// <summary>
        /// Return Current and Max values
        /// </summary>
        public event Action<float, float> OnAnyChanged;

        public float RefillPerSecond
        {
            get => _refillPerSecond;
            set
            {
                _refillPerSecond = value;
                OnRefillChanged?.Invoke(_refillPerSecond);
            }
        }

        public event Action<float> OnRefillChanged;

        #endregion Parameters API

        #region Parameters

        [SerializeField] private bool _canBeNegative = true;

        [SerializeField] private float _currentValue = 0;

        [SerializeField] private float _flatValue = 0;

        [SerializeField] private float _limitValue = 0;

        [SerializeField] private float _multiplier = 1f;

        [SerializeField] private float _refillPerSecond = 0;

        [field: SerializeField] public bool CanRefillNow { get; set; } = true;

        #endregion Parameters

        #region IParameter

        public float GetValue()
        {
            return CurrentValue;
        }

        public bool TrySubtract(float amount)
        {
            if (amount < 0) return false;
            if (!_canBeNegative && CurrentValue - amount < 0) return false;

            CurrentValue -= amount;
            AvoidBreakingLimit();

            return true;
        }

        public bool TryAdd(float amount)
        {
            if (amount < 0) return false;

            CurrentValue += amount;
            AvoidBreakingLimit();
            return true;
        }

        #endregion IParameter

        public CustomParameter()
        {
            DefaultValue = 100;
            _limitValue = 100;
            _refillPerSecond = 0;
            _multiplier = 1;
        }

        public CustomParameter(float defaultValue, float limitValue, float refillPerSecond = 0, float multiplier = 1f)
        {
            DefaultValue = defaultValue;
            _limitValue = limitValue;
            _refillPerSecond = refillPerSecond;
            _multiplier = multiplier;
            ResetValueToMax();
        }

        public void InvokeAllForcibly()
        {
            OnFlatChanged?.Invoke(_flatValue);
            OnCurrentChanged?.Invoke(_currentValue);
            OnLimitChanged?.Invoke(_limitValue);
            OnRefillChanged?.Invoke(_refillPerSecond);
            OnAnyChanged?.Invoke(_currentValue, _limitValue);
        }

        public void RefillSecond()
        {
            Refill(1f);
        }

        public void RefillMiliSecond()
        {
            Refill(0.1f);
        }

        public float GetDifferenceWithDefault()
        {
            return CurrentValue / DefaultValue;
        }

        public void ResetValueToMax()
        {
            PushLimitByMult();
            CurrentValue = LimitValue;
        }

        private void Refill(float time)
        {
            if (!CanRefillNow) return;

            if (CurrentValue >= LimitValue)
            {
                CurrentValue = LimitValue;
                return;
            }
            CurrentValue += RefillPerSecond * time;
        }

        private void PushLimitByMult()
        {
            LimitValue = FlatValue * Multiplier;
        }

        [ContextMenu(nameof(AvoidBreakingLimit))]
        private void AvoidBreakingLimit()
        {
            if (CurrentValue <= LimitValue) return;
            CurrentValue = LimitValue;
        }
    }
}