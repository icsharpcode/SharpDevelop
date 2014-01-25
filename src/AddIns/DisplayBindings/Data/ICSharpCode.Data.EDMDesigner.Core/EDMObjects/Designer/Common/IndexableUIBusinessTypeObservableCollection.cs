// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
