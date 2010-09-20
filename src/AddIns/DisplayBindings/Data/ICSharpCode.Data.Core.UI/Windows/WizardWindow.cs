// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

using ICSharpCode.Data.Core.UI.UserControls;

#endregion

namespace ICSharpCode.Data.Core.UI.Windows
{
    public class WizardWindow : Window
    {
        #region Fields

        private WizardWindowInnards _wizardWindowInnards = null;

        #endregion

        #region Properties

        public WizardWindowInnards WizardWindowInnards
        {
            get { return _wizardWindowInnards; }
        }

        #endregion

        #region Constructor

        public WizardWindow()
        {
            _wizardWindowInnards = new WizardWindowInnards(this);
            Content = _wizardWindowInnards;
            Closing += new CancelEventHandler(WizardWindow_Closing);
        }

        #endregion

        #region Methods

        public void AddWizardUserControl<T>() where T : WizardUserControl
        {
            T newWizardUserControl = Activator.CreateInstance<T>();
            newWizardUserControl.WizardWindow = this;
            _wizardWindowInnards.WizardUserControls.Add(newWizardUserControl);
        }

        public virtual void OnFinished()
        {
        	if (!DialogResult.HasValue) 
        		DialogResult = true;
        }

        #endregion

        #region Event handlers

        private void WizardWindow_Closing(object sender, CancelEventArgs e)
        {
        	if (!DialogResult.HasValue)
        		DialogResult = false;
        }

        #endregion
    }
}
