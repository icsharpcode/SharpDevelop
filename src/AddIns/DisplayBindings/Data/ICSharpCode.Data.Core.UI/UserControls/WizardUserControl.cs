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
