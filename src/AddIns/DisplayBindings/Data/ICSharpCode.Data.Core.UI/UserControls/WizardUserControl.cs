// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using ICSharpCode.Data.Core.UI.Windows;

#endregion

namespace ICSharpCode.Data.Core.UI.UserControls
{
    /// <summary>
    /// Interaction logic for WizardUserControl.xaml
    /// </summary>
    public abstract partial class WizardUserControl : UserControl, INotifyPropertyChanged
    {
        #region Fields

        private WizardWindow _wizardWindow = null;
        private bool _isReadyForNextStep = false;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the parent WizardWindow.
        /// </summary>
        public WizardWindow WizardWindow
        {
            get { return _wizardWindow; }
            set { _wizardWindow = value; }
        }

        /// <summary>
        /// Returns the index of this WizardUserControl in a WizardWindow.
        /// </summary>
        public virtual int Index
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns the title of this WizardUserControl.
        /// </summary>
        public virtual string Title
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns if this WizardUserControl is dependent of its predecessor user control.
        /// </summary>
        public virtual bool IsDependentOnPredecessor
        {
            get { return true; }
        }

        /// <summary>
        /// Returns if this WizardUserControl can finish the WizardWindow.
        /// </summary>
        public virtual bool CanFinish
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets it this WizardUserControl is ready for the next step.
        /// </summary>
        public bool IsReadyForNextStep
        {
            get { return _isReadyForNextStep; }
            protected set
            {
                _isReadyForNextStep = value;
                OnPropertyChanged("IsReadyForNextStep");
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Intializes a WizardUserControl.
        /// </summary>
        public WizardUserControl()
        { }

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

        #region Methods

        internal void Activate(bool activatedFromPredecessor)
        {
            if (activatedFromPredecessor && IsDependentOnPredecessor)
            {
                OnActivateFromPredecessor();
            }

            OnActivate();
        }

        public virtual void OnActivateFromPredecessor()
        { }

        public virtual void OnActivate()
        { }

        #endregion
    }
}
