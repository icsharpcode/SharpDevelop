using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace AvalonDock
{
    public class ManagedContentCollection<T> : ReadOnlyObservableCollection<T> where T : ManagedContent
    {
        internal ManagedContentCollection(DockingManager manager)
            : base(new ObservableCollection<T>())
        {
            Manager = manager;
        }


        /// <summary>
        /// Get associated <see cref="DockingManager"/> object
        /// </summary>
        public DockingManager Manager { get; private set; }

        /// <summary>
        /// Override collection changed event to setup manager property on <see cref="ManagedContent"/> objects
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (T cntAdded in e.NewItems)
                    cntAdded.Manager = Manager;
            }
            
            base.OnCollectionChanged(e);
        }

        /// <summary>
        /// Add a content to the list
        /// </summary>
        /// <param name="contentToAdd"></param>
        internal void Add(T contentToAdd)
        {
            if (!Items.Contains(contentToAdd))
                Items.Add(contentToAdd);
        }

        internal void Remove(T contentToRemove)
        {
            Items.Remove(contentToRemove);
        }
    }
}
