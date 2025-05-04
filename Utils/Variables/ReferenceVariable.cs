using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainEngine_v2.Utils.Variables
{
    public class ReferenceVariable<T> where T : struct
    {
        private T _value;

        public ReferenceVariable() { }

        public ReferenceVariable(T value)
        {
            _value = value;
        }

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                _event?.Invoke(value);
            }
        }

        public override string? ToString()
        {
            return _value.ToString();
        }

        private event Action<T>? _event;

        public void   RegisterCallback(Action<T> action) => _event += action;

        public void UnregisterCallback(Action<T> action) => _event -= action;
    }
}
