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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common
{
    public class EventedObservableCollection<T> : ObservableCollection<T>
    {
        #region Events

        public event Action<T> ItemAdded;
        public event Action<T> ItemRemoved;

        #endregion

        #region Constructor

        public EventedObservableCollection()
        {
        }

        #endregion

        #region Methods

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if (e.NewItems != null)
                foreach (T newItem in e.NewItems)
                    OnItemAdded(newItem);
            if (e.OldItems != null)
                foreach (T oldItem in e.OldItems)
                    OnItemRemoved(oldItem);
        }

        protected virtual void OnItemAdded(T item)
        {
            if (ItemAdded != null)
                ItemAdded(item);
        }

        protected virtual void OnItemRemoved(T item)
        {
            if (ItemRemoved != null)
                ItemRemoved(item);
        }

        #endregion
    }

    public static class EventedObservableCollection
    {
        #region Extension methods

        public static EventedObservableCollection<T> ToEventedObservableCollection<T>(this IEnumerable<T> source)
        {
            EventedObservableCollection<T> dest = new EventedObservableCollection<T>();

            foreach (T item in source)
                dest.Add(item);

            return dest;
        }

        #endregion
    }
}
