// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.ComponentModel;
using ICSharpCode.Data.Core.Interfaces;

#endregion

namespace ICSharpCode.Data.Core.DatabaseObjects
{
    public class DatabaseObjectBase : IDatabaseObjectBase, INotifyPropertyChanged
    {
        #region Fields

        protected string _name = string.Empty;
        protected IDatabaseObjectBase _parent = null;
        protected bool _isSelected = true;
         
        #endregion

        #region Properties

        public virtual string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public IDatabaseObjectBase Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                OnPropertyChanged("Parent");
            }
        }

        public virtual bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            } 
        }

        #endregion
    }

    public class DatabaseObjectBase<T> : DatabaseObjectBase, IDatabaseObjectBase<T> where T : IDatabaseObjectBase
    {
        #region Fields

        private DatabaseObjectsCollection<T> _items = null;

        #endregion

        #region Properties

        public DatabaseObjectsCollection<T> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        #endregion

        #region Constructor

        public DatabaseObjectBase() : base()
        {
            _items = new DatabaseObjectsCollection<T>(this);
        }

        #endregion
    }
}
