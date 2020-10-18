using System;
using System.Collections;
using System.Collections.Generic;
using Medella.TdsClient.TDS.Controller;

namespace Medella.TdsClient.Exceptions
{
    public sealed class SqlErrorCollection : ICollection
    {
        // Ideally this would be typed as List<SqlError>, but that would make the non-generic
        // CopyTo behave differently than the full framework (which uses ArrayList), throwing
        // ArgumentException instead of the expected InvalidCastException for incompatible types.
        // Instead, we use List<object>, which makes the non-generic CopyTo behave like
        // ArrayList.CopyTo.
        private readonly List<object> _errors = new List<object>();

        internal SqlErrorCollection()
        {
        }

        public SqlInfoAndError this[int index] => (SqlInfoAndError)_errors[index];

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_errors).CopyTo(array, index);
        }

        public int Count => _errors.Count;

        object ICollection.SyncRoot => this;

        bool ICollection.IsSynchronized => false;

        public IEnumerator GetEnumerator() => _errors.GetEnumerator();

        public void CopyTo(SqlInfoAndError[] array, int index)
        {
            _errors.CopyTo(array, index);
        }

        internal void Add(SqlInfoAndError error)
        {
            _errors.Add(error);
        }
    }
}