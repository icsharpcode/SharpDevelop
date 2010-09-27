// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.Core.Interfaces;

#endregion

namespace ICSharpCode.Data.Core.DatabaseObjects
{
    public class View : Table, IView
    {
        #region Fields

        private string _query = string.Empty;
        private string _definingQuery = string.Empty;

        #endregion

        #region Properties

        public string Query
        {
            get { return _query; }
            set
            {
                _query = value;
                OnPropertyChanged("Query");
            }
        }

        public string DefiningQuery
        {
            get { return _definingQuery; }
            set
            {
                _definingQuery = value;
                OnPropertyChanged("DefiningQuery");
            }
        }

        #endregion
    }
}
