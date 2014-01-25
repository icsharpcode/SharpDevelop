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
