// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.Common
{
    public class IndexableUIBusinessTypeObservableCollection<K, T> : ObservableCollection<T>
        where T : UIBusinessType<K>
        where K : EDMObjectBase, INotifyPropertyChanged
    {
        private Dictionary<K, T> _elementByKey;

        public IndexableUIBusinessTypeObservableCollection()
        {
            CollectionChanged +=
                (sender, e) =>
                {
                    if (e.NewItems != null)
                        foreach (T newItem in e.NewItems)
                            if ( ! ElementByKey.ContainsKey(newItem.BusinessInstance))
                                ElementByKey.Add(newItem.BusinessInstance, newItem);
                    if (e.OldItems != null)
                        foreach (T oldItem in e.OldItems)
                            ElementByKey.Remove(oldItem.BusinessInstance);
                };
        }
        public IndexableUIBusinessTypeObservableCollection(IEnumerable<T> values)
            : this()
        {
            foreach (T value in values)
                Add(value);
        }

        private Dictionary<K, T> ElementByKey
        {
            get
            {
                if (_elementByKey == null)
                    _elementByKey = new Dictionary<K, T>();
                return _elementByKey;
            }
        }

        public T this[K business]
        {
            get
            {
                if (ElementByKey.ContainsKey(business))
                    return ElementByKey[business];
                return null;
            }
            set
            {
                if (ElementByKey.ContainsKey(business))
                {
                    if (ElementByKey[business] == value)
                        return;
                    throw new InvalidOperationException();
                }
                ElementByKey.Add(business, value);
            }
        }

        public T GetAndAddIfNotExist(K business, Func<K, T> createNew)
        {
            var value = this[business];
            if (value == null)
            {
                value = createNew(business);
                ElementByKey.Add(business, value);
                Add(value);
            }
            return value;
        }

        public void Refresh()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
