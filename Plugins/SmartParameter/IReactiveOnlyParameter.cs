using System;

namespace Core.Utility.SmartParameter
{
    public interface IReactiveOnlyParameter
    {
        public event Action<float> OnCurrentChanged;

        public event Action<float> OnFlatChanged;

        public event Action<float> OnLimitChanged;

        public event Action<float> OnRefillChanged;

        public event Action<float, float> OnAnyChanged;
    }
}