// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
