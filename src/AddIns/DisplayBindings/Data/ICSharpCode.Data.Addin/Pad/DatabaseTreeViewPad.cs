// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using ICSharpCode.Data.Core.Interfaces;
using ICSharpCode.Data.Core.UI.UserControls;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Data.EDMDesigner.Core.UI.DisplayBinding;

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
			WorkbenchSingleton.Workbench.ActiveViewContentChanged += ActiveViewContentChanged;
			WorkbenchSingleton.Workbench.ViewClosed += ActiveViewClosed;

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
			WorkbenchSingleton.Workbench.ActiveViewContentChanged -= ActiveViewContentChanged;
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
