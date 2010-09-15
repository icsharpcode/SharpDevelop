// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ICSharpCode.Data.Core.Interfaces;

#endregion

namespace ICSharpCode.Data.Core.DatabaseObjects
{
    public class DatabaseObjectsCollection<T> : ObservableCollection<T>, IDatabaseObjectsCollection where T : IDatabaseObjectBase
    {
        #region Fields

        protected IDatabaseObjectBase _parent = null;

        #endregion

        #region Properties

        public List<IDatabaseObjectBase> NonGenericItems
        {
            get
            { 
                List<IDatabaseObjectBase> items = new List<IDatabaseObjectBase>();
                items.AddRange(this.Cast<IDatabaseObjectBase>());
                return items;
            }
        }

        public int SelectedItemsCount
        {
            get 
            {
                int selectedItemsCount = 0;

                foreach (T item in this)
                {
                    if (item.IsSelected)
                        selectedItemsCount++;
                }

                return selectedItemsCount;
            }
        }

        #endregion

        #region Methods

        public new void Add(T item)
        {
            item.Parent = _parent;
            base.Add(item);
        }

        #endregion

        #region Constructor

        public DatabaseObjectsCollection(IDatabaseObjectBase parent)
        {
            _parent = parent;
        }

        public DatabaseObjectsCollection()
        { }

        #endregion
    }

    public static class DatabaseObjectsCollection
    {
        #region Extension methods

        public static DatabaseObjectsCollection<T> ToDatabaseObjectsCollection<T>(this IEnumerable<T> source) where T : IDatabaseObjectBase
        {
            DatabaseObjectsCollection<T> dest = new DatabaseObjectsCollection<T>(null);

            foreach (T item in source)
                dest.Add(item);

            return dest;
        }

        public static DatabaseObjectsCollection<T> ToDatabaseObjectsCollection<T>(this IEnumerable<T> source, IDatabaseObjectBase parent) where T : IDatabaseObjectBase
        {
            DatabaseObjectsCollection<T> dest = new DatabaseObjectsCollection<T>(parent);

            foreach (T item in source)
                dest.Add(item);

            return dest;
        }

        #endregion
    }
}
