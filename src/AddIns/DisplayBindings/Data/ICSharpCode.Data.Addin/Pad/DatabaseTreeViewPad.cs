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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

using ICSharpCode.Data.Core.Interfaces;
using ICSharpCode.Data.Core.UI.UserControls;
using ICSharpCode.Data.EDMDesigner.Core.UI.DisplayBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

#endregion

namespace ICSharpCode.Data.Addin.Pad
{
    /// <summary>
    /// Description of DatabasesTreeViewPad.
    /// </summary>
    public class DatabasesTreeViewPad : AbstractPadContent, INotifyPropertyChanged
	{
        #region Fields
        
        private static DatabasesTreeViewPad _instance = null;
        
        private DatabasesTreeViewUserControl _control = null;
        private DatabasesTreeView _databasesTreeView = null;
       
        #endregion

        #region Properties

        public static DatabasesTreeViewPad Instance 
        {
			get { return _instance; }
		}

        public ObservableCollection<IDatabase> Databases
        {
            get { return _databasesTreeView.Databases; }
        }

		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad
		/// </summary>
		public override object Control 
		{
			get 
			{
			    return _control;
			}
		}

        #endregion

        #region Constructor

        /// <summary>
		/// Creates a new ReportExplorer object
		/// </summary>	
		public DatabasesTreeViewPad() : base()
		{
			SD.Workbench.ActiveViewContentChanged += ActiveViewContentChanged;
			SD.Workbench.ViewClosed += ActiveViewClosed;

            _control = new DatabasesTreeViewUserControl();
			_databasesTreeView = new DatabasesTreeView();
            _databasesTreeView.AdditionalNodes.Add(CSDLDatabaseTreeViewAdditionalNode.Instance);
            DockPanel.SetDock(_databasesTreeView, Dock.Top);
            _control.Content.Children.Add(_databasesTreeView);
			
			_instance = this;
        }

        #endregion

        #region Event handlers

        private void ActiveViewContentChanged(object source, EventArgs e)
		{

		}
		
		private void ActiveViewClosed (object source, ViewContentEventArgs e)
		{

        }

        #endregion

        #region Methods

		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			SD.Workbench.ActiveViewContentChanged -= ActiveViewContentChanged;
		}

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
