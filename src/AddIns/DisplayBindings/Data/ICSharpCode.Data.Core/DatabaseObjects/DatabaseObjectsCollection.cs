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
