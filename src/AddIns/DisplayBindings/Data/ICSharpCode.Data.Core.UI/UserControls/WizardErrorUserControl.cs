#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace ICSharpCode.Data.Core.UI.UserControls
{
    public class WizardErrorUserControl : WizardUserControl
    {
        #region Fields

        private Exception _exception = null;

        #endregion

        #region Properties

        public override sealed bool CanFinish
        {
            get { return false; }
        }

        public Exception Exception 
        {
            get { return _exception; }
            set 
            { 
                _exception = value; 
                OnPropertyChanged("Exception"); 
            }
        }

        public int PreviousIndex { get; set; }   

        #endregion
    }
}
