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
using System.IO;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Data;



using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

using SharpReport;
using SharpReportCore;
using SharpReport.Designer;

namespace SharpReportAddin{
	/// <summary>
	/// Description of the view content
	/// </summary>
	public class SharpReportView : AbstractViewContent,IPrintable
	{
		
		private SharpReportManager reportManager = null;
		
		private BaseDesignerControl designerControl;
		private	TabControl tabControl;
		private TabPage designerPage;
		private TabPage previewPage;
		
		// SideBar
		private AxSideTab sideTabItem = null;
		private AxSideTab sideTabFunctions = null;
		private Panel panel = new Panel();
	
		#region privates
		
		void InitView() {
			try {
				reportManager = new SharpReportManager();
				panel.AutoScroll = true;
				CreateTabControl();
				BuildToolBarItems();
				base.UntitledName = GlobalValues.SharpReportPlainFileName;
				base.FileName = GlobalValues.SharpReportPlainFileName;
				PropertyPad.Grid.SelectedObject = designerControl.ReportModel.ReportSettings;
				PropertyPad.Grid.Refresh();
			} catch (Exception e) {
				MessageService.ShowError(e,e.Message);
				throw e;
			}
		}
		
		
		// when the model is build, grap these events, otherwise we get permanent
		// changes of IsDirty
		
		private void SetOnPropertyChangedEvents () {
			try {
				ReportModel model = designerControl.ReportModel;
				model.ReportSettings.PropertyChanged += new EventHandler (OnSettingsChanged);
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
		
		///<summary>
		/// We have to tell the FieldsdsExplorer that we have loaded or created a new report
		/// </summary>
		/// 
		private void SetFieldsExplorer() {
			try {
				Type type = typeof(FieldsExplorer);
				SharpReportAddin.FieldsExplorer fe = (SharpReportAddin.FieldsExplorer)WorkbenchSingleton.Workbench.GetPad(type).PadContent;
				if (fe != null) {
					this.designerControl.ReportModel.ReportSettings.AvailableFieldsCollection = reportManager.AvailableFieldsCollection;
					fe.Fill(this.designerControl.ReportModel.ReportSettings);
				} else {
					MessageService.ShowError ("SharpReportView:SetFieldExplorer Unable to create FieldsExplorer");
				}
				
			} catch (Exception e) {
				MessageService.ShowError (e);
			}
		}
		#endregion
		
	
		
		#region SideBar Handling
		
		private bool SharpReportIsRunning () {
			// Did we already exist
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				SharpReportView view = content as SharpReportView;
				if (view != null) {
					return true;
				}
			}
			return false;
		}
		
		private SharpDevelopSideBar GetSideBar () {
			SideBarView v = (SideBarView)WorkbenchSingleton.Workbench.GetPad (typeof(SideBarView)).PadContent;
			SharpDevelopSideBar sb =(SharpDevelopSideBar) v.Control;
			return sb;
		}
		
		private void BuildToolBarItems() {
			
			SharpDevelopSideBar	sideBar = GetSideBar();

			if (!this.SharpReportIsRunning()) {
				SharpReport.BuildSideTab buildSideTab = new SharpReport.BuildSideTab (sideBar);
				buildSideTab.CreateSidetabs();
				sideBar.Refresh();
			}
		}
		
	
		
		
		void RemoveSideBarItem() {

			if (!this.SharpReportIsRunning()) {
				if (sideTabItem != null) {
					SharpDevelopSideBar	sideBar = GetSideBar();
					sideBar.Tabs.Remove (sideTabItem);
				}
				if (this.sideTabFunctions != null) {
					SharpDevelopSideBar	sideBar = GetSideBar();
					sideBar.Tabs.Remove(this.sideTabFunctions);
				}
				SideBarView v = (SideBarView)WorkbenchSingleton.Workbench.GetPad (typeof(SideBarView)).PadContent;
				SharpDevelopSideBar sb =(SharpDevelopSideBar) v.Control;
				
				AxSideTab s;
				for (int i = SideBarView.sideBar.Tabs.Count -1; i > 0;i -- ) {
					s = SideBarView.sideBar.Tabs[i];
					if (s.Name.IndexOf("Report") > 0) {
						SideBarView.sideBar.Tabs.Remove(s);
					}
				}
				sb.Refresh();
			}
		}
		
		#endregion
		
		
		#region Control
		
		void CreateTabControl() {
			tabControl = new TabControl();
			tabControl.SelectedIndexChanged += new EventHandler (OnTabPageChanged);
			
			designerPage = new TabPage();

			designerControl = CreateDesignerControl();
			designerPage.Controls.Add (designerControl);
			//create only the TabPage, no Controls are added
			previewPage = new TabPage ();
			tabControl.TabPages.Add (designerPage);
			tabControl.TabPages.Add (previewPage);
			
			tabControl.Alignment = TabAlignment.Bottom;
			
			panel.Dock = DockStyle.Fill;
			panel.AutoScroll = true;
			
			tabControl.Dock = DockStyle.Fill;

			panel.Controls.Add (tabControl);
			SetHeadLines();
			
		}
		//We set all captions in one method, so we can react much easier on changing of lanuages
		void SetHeadLines(){
			designerPage.Text = ResourceService.GetString("SharpReport.Design");
			previewPage.Text = ResourceService.GetString("SharpReport.Preview");
			this.OnTabPageChanged (this,EventArgs.Empty);
			this.designerControl.Localise();
		}
		
		private BaseDesignerControl CreateDesignerControl() {
			BaseDesignerControl ctrl = reportManager.BaseDesignControl;
			ctrl.ReportControl.Width = ctrl.ReportModel.ReportSettings.PageSettings.Bounds.Width;
			ctrl.ReportControl.AutoScroll = true;
			ctrl.Dock = DockStyle.Fill;
			
			ctrl.ReportControl.ObjectSelected += new SelectedEventHandler (OnObjectSelected);
			ctrl.ReportControl.DesignViewChanged += new ItemDragDropEventHandler (OnItemDragDrop);
			ctrl.DesignerDirty += new System.ComponentModel.PropertyChangedEventHandler (OnPropertyChanged);
			return ctrl;
		}
		
		#endregion
		
		
		#region Preview handling
		private void RunPreview() {
			base.OnSaving(EventArgs.Empty);
			System.Console.WriteLine("View:RunPreview {0}",designerControl.ReportModel.ReportSettings.DataModel);
			try {
				switch (designerControl.ReportModel.ReportSettings.DataModel) {
					case GlobalEnums.enmPushPullModel.FormSheet : {
						PreviewStandartReport();
						break;
					}
					case GlobalEnums.enmPushPullModel.PullData:{
						PreviewStandartReport();
						break;
					}
					case GlobalEnums.enmPushPullModel.PushData:{
						PreviewPushReport ();
						break;
					}
				default:
					throw new SharpReportException("Wrong ReportType");
					
			}
			} catch (Exception e) {
				MessageBox.Show("Error in RunPreview" + e.Message);
			}
			
		}
	
			
		
		private void PreviewPushReport (){
			try {
				using (OpenFileDialog openFileDialog = new OpenFileDialog()){
					openFileDialog.Filter = GlobalValues.XsdFileFilter;
					openFileDialog.DefaultExt = GlobalValues.XsdExtension;
					openFileDialog.AddExtension    = true;
					
					if(openFileDialog.ShowDialog() == DialogResult.OK){
						if (openFileDialog.FileName.Length > 0) {
							DataSet ds = new DataSet();
							ds.ReadXml (openFileDialog.FileName);
							reportManager.ReportPreviewPushData(designerControl.ReportModel,
							                                    ds.Tables[0],
							                                    true);
						}
					}
				}
			}catch (Exception e){
				MessageService.ShowError (e,e.Message);
			}
		}
		
		
		private void PreviewStandartReport(){
			System.Console.WriteLine("View:PreviewStadartreport");
			reportManager.NoData -= new SharpReportEventHandler (OnNoDataForReport);
			reportManager.NoData += new SharpReportEventHandler (OnNoDataForReport);
			
			reportManager.ParametersRequest -= new SharpReportParametersEventHandler (OnParametersRequest);
			reportManager.ParametersRequest += new SharpReportParametersEventHandler (OnParametersRequest);
			reportManager.ReportPreview (designerControl.ReportModel, true);
			
		}
		
		#endregion
		
		#region Events
		void OnNoDataForReport (object sender,SharpReportEventArgs e) {
			DialogResult result = MessageBox.Show("This Report contains no Data. Show it as FormSheet",
			                                      "SharpReport",
			                                      MessageBoxButtons.OKCancel,
			                                      MessageBoxIcon.Warning,
			                                      MessageBoxDefaultButton.Button1);
			switch(result) {
				case DialogResult.OK:
					e.Cancel = false;
					break;
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
			}
		}
		
		///<summary>This Event is called if the Report need's Parameters to run a Query,
		/// you can take this as an example how to react to an request for Parameters
		/// The other posibillity is, to fill/change the ParametersCollection by Code or
		/// just write an small Dialog to enter the Parameter values
		/// </summary>
		
		private void OnParametersRequest (object sender,SharpReportParametersEventArgs e) {
			AbstractParametersCollection collection = e.SqlParametersCollection;
			if (collection != null && collection.Count > 0) {
				using (ParameterInput parameterInput = new ParameterInput (e.ReportName,collection)){
					DialogResult result = parameterInput.ShowDialog();
				}
			}
		}
		
		
		private void OnTabPageChanged (object sender, EventArgs e) {
			string name = Path.GetFileName (base.FileName);
			base.TitleName = name + "[" + tabControl.SelectedTab.Text + "]";
			switch (tabControl.SelectedIndex) {
				case 0 :
					break;
				case 1:
					//Create the preview Control only when needed
					if (tabControl.SelectedTab.Controls.Count == 0 ){
						tabControl.SelectedTab.Controls.Add(reportManager.PreviewControl);
					}
					RunPreview();
					this.previewPage.Visible = true;
					break;
				default:
					
					break;
			}
		}
		
		//Something was dropped on the designer
		private void OnItemDragDrop (object sender,ItemDragDropEventArgs e) {
			base.IsDirty = true;
		}
		
		public void OnPropertyChanged (object sender,
		                               System.ComponentModel.PropertyChangedEventArgs e) {
			base.IsDirty = true;
		}
		
		private void OnSettingsChanged (object sender,EventArgs e) {
			base.IsDirty = true;
		}
		private void OnModelFileNameChanged (object sender,EventArgs e) {
			base.FileName = designerControl.ReportModel.ReportSettings.FileName;
			base.IsDirty = true;
			this.OnFileNameChanged(e);
		}
		
		
		private void OnObjectSelected (object sender,EventArgs e) {
			if (designerControl.ReportControl.SelectedObject != null) {
				PropertyPad.Grid.SelectedObject = designerControl.ReportControl.SelectedObject;
			}
		}
	
		protected override void OnFileNameChanged(System.EventArgs e) {
			base.OnFileNameChanged(e);
		}

		#endregion
		
		#region Calls from outside commands
		
		/// <summary>
		/// Set PropertyGrid to ReportSettings
		/// </summary>
		public void ShowReportSettings () {
			PropertyPad.Grid.SelectedObject = designerControl.ReportControl.ReportSettings;
			PropertyPad.Grid.Refresh();
		}
	
		
		/// <summary>
		/// Show's Report in PreviewControl
		/// </summary>
		
	
		public void OnPreviewClick () {

			reportManager.NoData -= new SharpReportEventHandler (OnNoDataForReport);
			reportManager.NoData += new SharpReportEventHandler (OnNoDataForReport);
		
			reportManager.ParametersRequest -= new SharpReportParametersEventHandler (OnParametersRequest);
			reportManager.ParametersRequest += new SharpReportParametersEventHandler (OnParametersRequest);
			base.OnSaving(EventArgs.Empty);
			reportManager.ReportPreview (designerControl.ReportModel,
			                             false);
		}

		
		/// <summary>
		/// This Method is called after something has changed like Load a new report
		/// Change Sorting or Grouping etc. to update the View and set the DirtyFlag
		/// </summary>
		/// <param name="setViewDirty">If true, set the DirtyFlag and Fire the PropertyChanged Event</param>
		public void UpdateView(bool setViewDirty) {
			this.tabControl.SelectedIndex = 0;
			this.OnTabPageChanged(this,EventArgs.Empty);
			SetOnPropertyChangedEvents();
			this.SetFieldsExplorer();
			if (setViewDirty) {
				this.OnPropertyChanged (this,new System.ComponentModel.PropertyChangedEventArgs("Fired from UpdateView"));
			}
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
				designerControl.ReportControl.ObjectSelected -= new SelectedEventHandler (OnObjectSelected);			
				reportManager.LoadFromFile (fileName);		
				base.FileName = fileName;			
				designerControl.ReportModel.ReportSettings.FileName = fileName;
				designerControl.ReportControl.ObjectSelected += new SelectedEventHandler (OnObjectSelected);
				PropertyPad.Grid.SelectedObject = designerControl.ReportModel.ReportSettings;
				PropertyPad.Grid.Refresh();
				UpdateView(false);
			} catch (Exception e) {
				MessageService.ShowError(e.Message);
				throw ;
			}
		}
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose(){
			try {
				PropertyPad.Grid.SelectedObject = null;
				RemoveSideBarItem();
				this.designerControl.Dispose();
				this.reportManager = null;
			} catch (Exception e){
				MessageService.ShowError(e.Message);
				return;
			}
		}
		#endregion
		
		
		
		#region ICSharpCode.SharpDevelop.Gui.IPrintable interface implementation
		public System.Drawing.Printing.PrintDocument PrintDocument {
			get {
				AbstractRenderer renderer = reportManager.GetRenderer(this.designerControl.ReportModel);
				return renderer.ReportDocument;
			}
		}
		
		
		#endregion
		
	}
	
}
