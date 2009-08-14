#region Usings

using System;
using System.Collections.Generic;
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
        { }

        #endregion
    }
}
