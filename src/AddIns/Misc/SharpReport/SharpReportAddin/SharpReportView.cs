/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 10.01.2005
 * Time: 10:04
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
// one line to give the program's name and an idea of what it does.
// Copyright (C) 2005  peter.forstmeier@t-online.de

using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using SharpReport;
using SharpReport.Designer;
using SharpReportAddin.Commands;
using SharpReportCore;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace SharpReportAddin{
	/// <summary>
	/// Description of the view content
	/// </summary>
	public class SharpReportView : AbstractViewContent,IPrintable,IDisposable
	{
		
		private SharpReportManager reportManager;
		
		private BaseDesignerControl designerControl;
		private SharpReportCore.ReportViewer.PreviewControl reportViewer;
		private	TabControl tabControl;

		
		// SideBar
		private SideTab sideTabItem = null;
		private SideTab sideTabFunctions = null;
		private Panel panel;
		
		private bool disposed;
		
		#region privates
		
		void InitView() {
			try {
				reportManager = new SharpReportManager();
				
				panel = new Panel();
				panel.AutoScroll = true;
				panel.Dock = DockStyle.Fill;
				CreateTabControl();
				SharpReportView.BuildToolBarItems();
				if (PropertyPad.Grid != null) {
					PropertyPad.Grid.SelectedObject = designerControl.ReportModel.ReportSettings;
					PropertyPad.Grid.Refresh();
				}
				
			} catch (Exception) {
				throw;
			}
		}
		
		
		// when the model is build, grap these events, otherwise we get permanent
		// changes of IsDirty
		
		private void SetOnPropertyChangedEvents () {
			try {
				ReportModel model = designerControl.ReportModel;

				model.ReportSettings.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler (OnPropertyChanged);
				model.ReportHeader.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler (OnPropertyChanged);
				model.PageHeader.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler (OnPropertyChanged);
				model.DetailSection.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler (OnPropertyChanged);
				model.PageFooter.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler (OnPropertyChanged);
				model.ReportFooter.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler (OnPropertyChanged);
				//watch for FileName change from Reportsettings as well
				model.ReportSettings.FileNameChanged += new EventHandler(OnModelFileNameChanged);
			} catch (Exception) {
				throw;
			}
		}
		
		#endregion
		
			#region Control
		
		private BaseDesignerControl CreateDesignerControl() {
			BaseDesignerControl ctrl = reportManager.BaseDesignControl;
			ctrl.SuspendLayout();
			ctrl.ReportControl.Width = ctrl.ReportModel.ReportSettings.PageSettings.Bounds.Width;
			ctrl.ReportControl.ObjectSelected +=new EventHandler <EventArgs>(OnObjectSelected);
			ctrl.ReportControl.DesignViewChanged += new ItemDragDropEventHandler (OnItemDragDrop);
			ctrl.DesignerDirty += new System.ComponentModel.PropertyChangedEventHandler (OnPropertyChanged);
			ctrl.ResumeLayout();
			return ctrl;
		}
		
		#endregion
		
		#region SideBar Handling
		
		private static bool SharpReportIsRunning () {
			
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				SharpReportView view = content as SharpReportView;
				if (view != null) {
					return true;
				}
			}
			return false;
		}
		
		private static SharpDevelopSideBar GetSideBar () {
			SideBarView v = (SideBarView)WorkbenchSingleton.Workbench.GetPad (typeof(SideBarView)).PadContent;
			SharpDevelopSideBar sb =(SharpDevelopSideBar) v.Control;
			return sb;
		}
		
		private static void BuildToolBarItems() {
			
			SharpDevelopSideBar	sideBar = SharpReportView.GetSideBar();

			if (!SharpReportView.SharpReportIsRunning()) {
				SharpReport.BuildSideTab buildSideTab = new SharpReport.BuildSideTab (sideBar);
				buildSideTab.CreateSidetabs();
				sideBar.Refresh();
			}
		}
		
		
		private  void RemoveSideBarItem() {
			
			if (!SharpReportView.SharpReportIsRunning()) {
				if (sideTabItem != null) {
					SharpDevelopSideBar	sideBar = SharpReportView.GetSideBar();
					sideBar.Tabs.Remove (sideTabItem);
				}
				
				if (this.sideTabFunctions != null) {
					SharpDevelopSideBar	sideBar = GetSideBar();
					sideBar.Tabs.Remove(this.sideTabFunctions);
				}
				SideBarView v = (SideBarView)WorkbenchSingleton.Workbench.GetPad (typeof(SideBarView)).PadContent;
				SharpDevelopSideBar sb =(SharpDevelopSideBar) v.Control;
				
				SideTab s;
				for (int i = SideBarView.sideBar.Tabs.Count -1; i > 0;i -- ) {
					s = SideBarView.sideBar.Tabs[i];
					if (s.Name.IndexOf("Report") > 0) {
						SideBarView.sideBar.Tabs.Remove(s);
					}
				}
				sb.Refresh();
			}
		}
		
		
		void ShutDownView (object sender,ViewContentEventArgs e) {

			if (e.Content is SharpReportView) {
				WorkbenchSingleton.Workbench.ViewClosed -= new ViewContentEventHandler(ShutDownView);
			}
			
			//Allways hide the pad

			HideExplorer he = new HideExplorer();
			he.Run();
			
			ClearAndRebuildExplorer cmd = new ClearAndRebuildExplorer();
			cmd.Run();

		}
		
		
		#endregion
		
		#region TabControl including Events
		
		void CreateTabControl() {

			this.tabControl = BuildDesignerTab.BuildTabControl();
			
			this.tabControl.SelectedIndexChanged += new EventHandler (OnTabPageChanged);
			this.tabControl.Selecting += new TabControlCancelEventHandler(OnTabPageSelecting);
			
			this.designerControl = CreateDesignerControl();
			this.tabControl.TabPages[0].Controls.Add (this.designerControl);
		
			this.panel.Controls.Add (this.tabControl);
			SetHeadLines();
		}
		
		//We set all captions in one method, so we can react much easier on changing of lanuages
		void SetHeadLines(){
			this.tabControl.TabPages[0].Text = ResourceService.GetString("SharpReport.Design");
			this.tabControl.TabPages[1].Text = ResourceService.GetString("SharpReport.Preview");
			this.tabControl.TabPages[2].Text = "ReportViewer";
			this.OnTabPageChanged (this,EventArgs.Empty);
			this.designerControl.Localise();
		}
		void SetTabTitel (string name) {
			base.TitleName = String.Format(CultureInfo.CurrentCulture,
			                               "{0} [{1}]",name,this.tabControl.SelectedTab.Text);
		}
		
		private void OnTabPageSelecting (object sender,TabControlCancelEventArgs e) {
			switch (e.TabPageIndex) {
				case 0:
					break;
				case 1:
					if (e.TabPage.Controls.Count == 0) {
						e.TabPage.Controls.Add(reportManager.PreviewControl);
					}
					break;
				case 2:
					if (e.TabPage.Controls.Count == 0) {
						this.reportViewer = new SharpReportCore.ReportViewer.PreviewControl();
						this.reportViewer.Dock = DockStyle.Fill;
						this.reportViewer.PageSettings = new System.Drawing.Printing.PageSettings();
						e.TabPage.Controls.Add(this.reportViewer);
						
					}
					break;
			}
		}
		
		private void OnTabPageChanged (object sender, EventArgs e) {
			
			string name;
			if (String.IsNullOrEmpty(base.FileName)) {
				base.UntitledName = GlobalValues.SharpReportPlainFileName;
				base.TitleName = GlobalValues.SharpReportPlainFileName;
				base.FileName = GlobalValues.SharpReportPlainFileName;
				name = base.TitleName;
			} else {
				name = Path.GetFileName (base.FileName);
			}
			SetTabTitel (name);
			switch (tabControl.SelectedIndex) {
				case 0 :
					break;
				case 1:
					RunPreview(true);
					this.tabControl.SelectedTab.Visible = true;
					break;
				case 2:
					FillReportViewer();
					break;
				default:
					
					break;
			}
		}
		
		#endregion
		

		#region Preview handling
	
		private void RunPreview(bool standAlone) {
			base.OnSaving(EventArgs.Empty);
			this.UpdateModelFromExplorer();
			try {
				switch (designerControl.ReportModel.DataModel) {
						case GlobalEnums.PushPullModel.FormSheet : {
							PreviewStandartReport(standAlone);
							break;
						}
						case GlobalEnums.PushPullModel.PullData:{
							PreviewStandartReport(standAlone);
							break;
						}
						case GlobalEnums.PushPullModel.PushData:{
							PreviewPushReport (standAlone);
							break;
						}
					default:
						throw new SharpReportException("Wrong ReportType");
						
				}
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
			
		}
		
		
		
		private void PreviewPushReport (bool standAlone){
			try {
				DataSetFromXSDCommand cmd = new DataSetFromXSDCommand	();
				cmd.Run();
				DataSet ds = cmd.DataSet;
				reportManager.ReportPreviewPushData(designerControl.ReportModel,
				                                    ds,
				                                    standAlone);
			}catch (Exception e){
				MessageService.ShowError (e,e.Message);
			}
		}
		
		
		private void PreviewStandartReport(bool standAlone){
			reportManager.ParametersRequest -= new EventHandler<SharpReportParametersEventArgs> (OnParametersRequest);
			reportManager.ParametersRequest +=  new EventHandler<SharpReportParametersEventArgs>(OnParametersRequest);
			
			reportManager.ReportPreview (designerControl.ReportModel, standAlone);
		}
		
		#endregion
		
		#region ReportViewer
		
		private void FillReportViewer() {
			System.Console.WriteLine("FillReportViewer");
			System.Console.WriteLine("\tsetup pagebuilder{0}",DateTime.Now);
			SharpReportCore.Exporters.PageBuilder pb = reportManager.CreatePageBuilder (designerControl.ReportModel);
			System.Console.WriteLine("\tstart createreport{0}",DateTime.Now);
			pb.CreateReport();
			System.Console.WriteLine("\treport created {0}",DateTime.Now);
			SharpReportCore.Exporters.SinglePage sp= pb.FirstPage;
			System.Console.WriteLine("\tPages {0} SinglePage : Items {0}",pb.Pages.Count,sp.Items.Count);

			this.reportViewer.Pages = pb.Pages;
		}
		#endregion
		
		
		#region Events
		
		///<summary>This Event is called if the Report need's Parameters to run a Query,
		/// you can take this as an example how to react to an request for Parameters
		/// The other posibillity is, to fill/change the ParametersCollection by Code or
		/// just write an small Dialog to enter the Parameter values
		/// </summary>
		
		private void OnParametersRequest (object sender,SharpReportParametersEventArgs e) {
			SqlParametersCollection collection = e.SqlParametersCollection;
			if (collection != null && collection.Count > 0) {
				using (ParameterDialog dlg = new ParameterDialog(e.SqlParametersCollection)){
					DialogResult result = dlg.ShowDialog();
				}
			}
		}
		
		
		
		//Something was dropped on the designer
		
		private void OnItemDragDrop (object sender,ItemDragDropEventArgs e) {
			base.IsDirty = true;
			this.OnPropertyChanged (this,new System.ComponentModel.PropertyChangedEventArgs("Item Dragged"));
		}
		
		private void OnPropertyChanged (object sender,
		                                System.ComponentModel.PropertyChangedEventArgs e) {
			base.IsDirty = true;
			OnObjectSelected (this,EventArgs.Empty);
		}
		
		private void OnModelFileNameChanged (object sender,EventArgs e) {
			base.FileName = designerControl.ReportModel.ReportSettings.FileName;
			if (designerControl.ReportModel.ReportSettings.InitDone) {
				base.IsDirty = true;
				this.OnFileNameChanged(e);
				this.SetTabTitel(Path.GetFileName (base.FileName));
			}
		}
		
		
		private void OnObjectSelected (object sender,EventArgs e) {
			
			if (designerControl.ReportControl.SelectedObject != null) {
				BaseReportObject newobj = designerControl.ReportControl.SelectedObject;
				newobj.ResumeLayout();
				
				if (PropertyPad.Grid != null) {
					PropertyPad.Grid.SelectedObject = designerControl.ReportControl.SelectedObject;
				}
			}
		}
		
		private void UpdateModelFromExplorer () {
			ReportExplorer re = (SharpReportAddin.ReportExplorer)WorkbenchSingleton.Workbench.GetPad(typeof(ReportExplorer)).PadContent;
			re.Update(designerControl.ReportModel);
		}
		#endregion
		
		#region Calls from outside commands
		
		/// <summary>
		/// Set PropertyGrid to ReportSettings
		/// </summary>
		public void ShowReportSettings () {
			if (PropertyPad.Grid != null) {
				PropertyPad.Grid.SelectedObject = designerControl.ReportControl.ReportSettings;
				PropertyPad.Grid.Refresh();
			}
		}
		
		
		/// <summary>
		/// Show's Report in PreviewControl
		/// </summary>
		
		public void OnPreviewClick () {
			reportManager.ParametersRequest -= new EventHandler<SharpReportParametersEventArgs> (OnParametersRequest);
			reportManager.ParametersRequest +=  new EventHandler<SharpReportParametersEventArgs>(OnParametersRequest);

			base.OnSaving(EventArgs.Empty);
			this.RunPreview(false);
		}
		
		/// <summary>
		/// Remove the selected Item from <see cref="BaseDesignerControl"></see>
		/// </summary>
		
		public void RemoveSelectedItem () {
			this.designerControl.RemoveSelectedItem ();
		}
		
		/// <summary>
		/// This Method is called after something has changed like Load a new report
		/// Change Sorting or Grouping etc. to update the View and set the DirtyFlag
		/// </summary>
		/// <param name="setViewDirty">If true, set the DirtyFlag and Fire the PropertyChanged Event</param>
		
		public void UpdateView(bool setViewDirty) {
			this.tabControl.SelectedIndex = 0;
			this.OnTabPageChanged(this,EventArgs.Empty);
			if (setViewDirty) {
				this.OnPropertyChanged (this,new System.ComponentModel.PropertyChangedEventArgs("Fired from UpdateView"));
			}
		}
		
		/// <summary>
		/// Tells the <see cref="BaseDesignerControl"></see> to fire an Event if
		/// something in the report layout changes
		/// </summary>
		
		public void RegisterPropertyChangedEvents () {
			SetOnPropertyChangedEvents();
			this.designerControl.RegisterEvents();
		}
		
		#endregion
		
		#region Propertys
		/// <summary>
		/// Returns the complete Designer
		/// </summary>
		
		public BaseDesignerControl DesignerControl {
			get {
				return designerControl;
			}
		}
		
		///<summary>
		/// returns a ReportManager
		/// </summary>
		
		public SharpReportManager ReportManager {
			get {
				return reportManager;
			}
		}
		
		public bool Disposed {
			get {
				return disposed;
			}
		}
		#endregion
		
		
		#region AbstractViewContent requirements
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the view
		/// </summary>
		public override Control Control {
			get {
				return panel;
			}
		}
		
		public override void RedrawContent() {
			
			SetHeadLines();
		}
		
		/// <summary>
		/// Save's the Report
		/// </summary>
		/// <param name="fileName"></param>
		public override void Save(string fileName) {
			try {
				UpdateModelFromExplorer ();
				designerControl.ReportModel.ReportSettings.FileName = fileName;
				
				if (FileUtility.IsValidFileName(fileName)) {
					OnSaving(EventArgs.Empty);
					reportManager.SaveToFile (fileName);
					base.IsDirty = false;
					OnSaved(new SaveEventArgs(true));
				} else {
					MessageService.ShowError ("<" + fileName + "> invalid Filename");
					base.IsDirty = true;
				}
			} catch (Exception e) {
				MessageService.ShowError(e,"SharpReportView:Save");
				throw;
			}
		}
		
		
		public override void Save() {
			this.Save (designerControl.ReportModel.ReportSettings.FileName);
		}
		
		/// <summary>
		/// Creates a new SharpReportView object
		/// </summary>
		public SharpReportView():base() {
			if (GlobalValues.IsValidPrinter()) {
				InitView();
				this.UpdateView(false);
				
				if (!SharpReportView.SharpReportIsRunning()) {
					WorkbenchSingleton.Workbench.ViewClosed += new ViewContentEventHandler(ShutDownView);
				}
				
			} else {
				MessageService.ShowError(ResourceService.GetString("Sharpreport.Error.NoPrinter"));
			}
		}
		
		
		/// <summary>
		/// Loads a new file into View
		/// </summary>
		/// <param name="fileName">A valid Filename</param>
		public override void Load(string fileName){

			try {
				designerControl.ReportControl.ObjectSelected -= new EventHandler <EventArgs>(OnObjectSelected);
				reportManager.LoadFromFile (fileName);	
				base.FileName = fileName;
				designerControl.ReportModel.ReportSettings.FileName = fileName;
				designerControl.ReportControl.ObjectSelected += new EventHandler <EventArgs>(OnObjectSelected);
				if (PropertyPad.Grid != null) {
					PropertyPad.Grid.SelectedObject = designerControl.ReportModel.ReportSettings;
					PropertyPad.Grid.Refresh();
				}
					
				this.designerControl.ReportModel.ReportSettings.AvailableFieldsCollection = reportManager.AvailableFieldsCollection;
				ShowAndFillExplorer se = new ShowAndFillExplorer();
				se.ReportModel = this.designerControl.ReportModel;
				se.Run();
	
			} catch (Exception e) {
				MessageService.ShowError(e,"SharpReportView:Load");
				throw ;
			}
		}
		
		
		#endregion
		
		#region ICSharpCode.SharpDevelop.Gui.IPrintable interface implementation
		
		public System.Drawing.Printing.PrintDocument PrintDocument {
			get {
				AbstractRenderer renderer;
				if (this.designerControl.ReportModel.DataModel == GlobalEnums.PushPullModel.PushData) {
					DataSetFromXSDCommand cmd = new DataSetFromXSDCommand();
					cmd.Run();
					DataSet dataSet = cmd.DataSet;
					renderer = reportManager.GetRendererForPushDataReports(this.designerControl.ReportModel,
					                                                       dataSet);
					
				} else {
					try {
						renderer = reportManager.GetRendererForStandartReports(this.designerControl.ReportModel);
					} catch (Exception e) {
						MessageService.ShowError (e,"SharpReportManager:ReportPreview");
						return null;
					}
				}
				return renderer.ReportDocument;
			}
		}
		
		
		#endregion
		
		#region IDisposable
		
		public override void Dispose(){
			if (PropertyPad.Grid != null) {
				PropertyPad.Grid.SelectedObject = null;
			}
			
			this.disposed = true;
			RemoveSideBarItem();
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~SharpReportView(){
			Dispose(false);
		}
		
		protected  void Dispose(bool disposing){
			if (disposing) {
				// Free other state (managed objects).
				if (this.reportManager != null) {
					this.reportManager.Dispose();
				}
				if (this.designerControl != null) {
					this.designerControl.Dispose();
				}
				
				if (this.reportViewer != null) {
					this.reportViewer.Dispose();
				}
				
				if (this.tabControl != null) {
					this.tabControl.Dispose();
				}
				if (this.panel != null) {
					this.panel.Dispose();
				}
			}
			
			// Release unmanaged resources.
			// Set large fields to null.
			// Call Dispose on your base class.
			base.Dispose();
		}
		
		#endregion
	}
	
}
