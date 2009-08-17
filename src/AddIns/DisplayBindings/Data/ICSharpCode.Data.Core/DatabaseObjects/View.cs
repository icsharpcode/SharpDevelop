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

        private string _definingQuery = string.Empty;

        #endregion

        #region Properties

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
