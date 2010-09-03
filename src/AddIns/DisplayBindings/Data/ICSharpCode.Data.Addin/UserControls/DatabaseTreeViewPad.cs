// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
using System.Windows.Forms.Integration;

#endregion

namespace ICSharpCode.DatabaseTools.Addin.UserControls
{
    /// <summary>
    /// Description of DatabaseTreeViewPad.
    /// </summary>
    public class DatabaseTreeViewPad : AbstractPadContent, INotifyPropertyChanged
	{
        #region Fields
        
        private static DatabaseTreeViewPad _instance = null;
        
        private ElementHost _winformElementHost = null;
        private DatabaseTreeView _databaseTreeView = null;
        
        #endregion
		
		/// <summary>
		/// Creates a new ReportExplorer object
		/// </summary>	
		public DatabaseTreeViewPad() : base()
		{
			WorkbenchSingleton.Workbench.ActiveViewContentChanged += ActiveViewContentChanged;
			WorkbenchSingleton.Workbench.ViewClosed += ActiveViewClosed;
			
			_databaseTreeView = new DatabaseTreeView();
			_winformElementHost = new ElementHost();
			_winformElementHost.Child = _databaseTreeView;
			_winformElementHost.Dock = DockStyle.Fill;
			
			_instance = this;
		}
		
		void ActiveViewContentChanged(object source, EventArgs e)
		{

		}
		
		void ActiveViewClosed (object source, ViewContentEventArgs e)
		{

		}
		
		public static DatabaseTreeViewPad Instance {
			get { return _instance; }
		}
		
		#region IPropertyChanged
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		private  void NotifyReportView(string property)
		{
			if (this.PropertyChanged != null) {
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(property));                     
			}
		}
		
		#endregion
		
		#region AbstractPadContent
		
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad
		/// </summary>
		public override Control Control 
		{
			get 
			{
			    return _winformElementHost;
			}
		}
		
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			WorkbenchSingleton.Workbench.ActiveViewContentChanged -= ActiveViewContentChanged;
		}
		
		#endregion
    }
}
