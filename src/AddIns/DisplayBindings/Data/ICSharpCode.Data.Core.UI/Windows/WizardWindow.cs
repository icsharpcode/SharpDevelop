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
