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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core.WinForms;
using ICSharpCode.Reports.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of the pad content
	/// </summary>
	internal sealed  class ReportExplorerPad : AbstractPadContent,INotifyPropertyChanged
	{
		private static int viewCount;
		private ExplorerTree explorerTree;
		private static ReportExplorerPad instance;
		private ReportModel reportModel;
		/// <summary>
		/// Creates a new ReportExplorer object
		/// </summary>
		
		
		public ReportExplorerPad():base()
		{
			SD.Workbench.ActiveViewContentChanged += ActiveViewContentChanged;
			SD.Workbench.ViewClosed += ActiveViewClosed;
			this.explorerTree = new ExplorerTree();
			this.explorerTree.MouseDown += new MouseEventHandler(ReportExplorer_MouseDown);
			this.explorerTree.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ReportExplorerPad_PropertyChanged);
			instance = this;
		}

		
		void ReportExplorerPad_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			this.NotifyReportView(e.PropertyName);
		}
		
		#region Setup
		
		
		
		public void AddContent (ReportModel reportModel)
		{
			
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			this.reportModel = reportModel;
			this.explorerTree.ReportModel = this.reportModel;
			ViewCount++;
		}
	
		#endregion
		
		
		void ActiveViewContentChanged(object source, EventArgs e)
		{
			ReportDesignerView vv = SD.Workbench.ActiveViewContent as ReportDesignerView;
			if (vv != null) {
				Console.WriteLine("Explorerpad:ActiveViewContentChanged {0}",vv.TitleName);
			}
		}
		
		private void ActiveViewClosed (object source, ViewContentEventArgs e)
		{
			if (e.Content is ReportDesignerView) {
				Console.WriteLine ("Designer closed");
				                   ViewCount --;
			}
		}
		
		
		#region Mouse
		
		private void ReportExplorer_MouseDown (object sender, MouseEventArgs e)
		{
			AbstractFieldsNode abstrNode =  this.explorerTree.GetNodeAt(e.X, e.Y) as AbstractFieldsNode;
			if (e.Button == MouseButtons.Right) {
				this.explorerTree.SelectedNode = abstrNode;
				if (abstrNode != null) {
					if (abstrNode.ContextMenuAddinTreePath.Length > 0) {
						ContextMenuStrip ctMen = MenuService.CreateContextMenu (this,abstrNode.ContextMenuAddinTreePath);
						ctMen.Show (this.explorerTree, new Point (e.X,e.Y));
					}
				}
			}
		}
		
		#endregion
		
		
		#region publics for Commands
	
		// These public methods are all called from ExplorerCommands
		
		public void ClearNodes () 
		{
			this.explorerTree.ClearSection();
		}
		
		
		public void ToggleOrder ()
		{
			this.explorerTree.ToggleSortOrder();
		}
		
		
		public void RemoveSortNode()
		{
			this.explorerTree.RemoveSortNode();
		}
		
		
		public void RemoveGroupNode()
		{
			this.explorerTree.RemoveGroupNode();
		}
		
		public void RefreshParameters()
		{
			this.explorerTree.BuildTree();
			this.NotifyReportView("Parameters");
		}
		
		#endregion
		
		public static ReportExplorerPad Instance {
			get { return instance; }
		}
		
		
		public static int ViewCount {
			get { return viewCount; }
			set {
				viewCount = value;
				if (viewCount == 0)	{
					Console.WriteLine("Should find a way to close/hide a pad");
				}
			}
		}
		
		
		
		public ReportModel ReportModel 
		{get {return this.reportModel;}}
		

		#region IPropertyChanged
		
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		private  void NotifyReportView(string property)
		{
			if (this.PropertyChanged != null) {
				this.PropertyChanged(this,new System.ComponentModel.PropertyChangedEventArgs(property));                     
			}
		}
		
		#endregion
		
		#region AbstractPadContent
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad
		/// </summary>
		public override object Control 
		{
			get {
				return this.explorerTree;
			}
		}
		
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			SD.Workbench.ActiveViewContentChanged -= ActiveViewContentChanged;
			this.explorerTree.Dispose();
		}
		
		#endregion
	}
}
