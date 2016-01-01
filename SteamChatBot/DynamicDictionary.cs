using System.Collections.Generic;
using System.Dynamic;

namespace SteamChatBot
{
    public class DynamicDictionary<TValue> : DynamicObject
    {
        private Dictionary<string, TValue> _dictionary;

        public DynamicDictionary()
        {
            _dictionary = new Dictionary<string, TValue>();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            TValue data;
            if (!_dictionary.TryGetValue(binder.Name, out data))
            {
                throw new KeyNotFoundException("Key not found");
            }

            result = (TValue)data;

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (_dictionary.ContainsKey(binder.Name))
            {
                _dictionary[binder.Name] = (TValue)value;
            }
            else
            {
                _dictionary.Add(binder.Name, (TValue)value);
            }

            return true;
        }
    }
}
