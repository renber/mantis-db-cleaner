using System;
using System.Collections.Generic;
using System.Text;

namespace MantisDBCleaner.Types
{
    class TaskDataStore
    {
        private Dictionary<DataKey, object> values = new Dictionary<DataKey, object>();

        public void Set<T>(DataKey name, T value)
        {
            if (values.ContainsKey(name))
                values.Remove(name);

            values.Add(name, value);
        }

        public T Get<T>(DataKey name)
        {
            return (T)values[name];
        }
    }

    enum DataKey
    {
        KeepProjectIDs
    }
}
