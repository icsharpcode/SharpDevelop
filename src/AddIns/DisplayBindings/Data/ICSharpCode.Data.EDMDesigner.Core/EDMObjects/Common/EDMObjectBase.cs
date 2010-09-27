// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Interfaces;
using System.ComponentModel;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.ChangeWatcher;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common
{
    public abstract class EDMObjectBase : IEDMObjectBase, INotifyPropertyChanged
    {
        #region Fields

        protected string _name = string.Empty;

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

        public IEDMObjectBase This
        {
            get { return this; }
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

            EDMDesignerChangeWatcher.ObjectChanged(this);
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return _name;
        }

        #endregion
    }
}
