/*
 * Created by SharpDevelop.
 * Date: 09/10/2004
 * Time: 9.12
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;


using ICSharpCode.Core;

using SharpReportCore;

using SharpReport.ReportItems;


namespace SharpReport.Designer{
	/// <summary>
	/// Description of Report1.
	/// </summary>
	public class Report : System.Windows.Forms.UserControl
	{
		private SharpReport.Designer.ReportHeader visualReportHeader;
		private SharpReport.Designer.ReportPageHeader visualPageHeader;
		private SharpReport.Designer.ReportFooter visualFooter;
		private SharpReport.Designer.ReportPageFooter visualPageFooter;
		private SharpReport.Designer.ReportDetail visualDetail;
	

		
		// Generic selected object in report
		private IBaseRenderer	selectedObject;
		
		// Section selected in report
		private ReportSection	selectedSection;
		
		private ReportSection header;
		private ReportSection pageHeader;
		private ReportSection detail;
		private ReportSection footer;
		private ReportSection pageFooter;
		
		private ReportSectionCollection sectionCollection;
		
		private ReportSettings reportSettings;
		
		private NameService nameService;
		
		[EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
		
		public event SelectedEventHandler ObjectSelected;
		
		public event EventHandler <SectionChangedEventArgs> SectionChanged;
		public event  ItemDragDropEventHandler DesignViewChanged;

		public Report(){
			InitializeComponent();
			this.SetStyle(ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.ResizeRedraw,
			              true);
			this.UpdateStyles();
			InitSectionCollection();
			Localise();
			this.nameService = new NameService();
		}
		
		public void Localise() {
			this.visualReportHeader.Caption = ResourceService.GetString("SharpReport.Designer.ReportHeader");
			this.visualPageHeader.Caption = ResourceService.GetString("SharpReport.Designer.PageHeader");
			this.visualDetail.Caption = ResourceService.GetString("SharpReport.Designer.DetailSection");
			this.visualPageFooter.Caption = ResourceService.GetString("SharpReport.Designer.PageFooter");
			this.visualFooter.Caption = ResourceService.GetString("SharpReport.Designer.ReportFooter");
		}
		
		protected override void Dispose(bool disposing) {
			if( disposing ){
				
			}
			this.visualReportHeader.Dispose();
			this.visualPageHeader.Dispose();
			this.visualDetail.Dispose();
			this.visualPageFooter.Dispose();
			this.visualFooter.Dispose();
			
			base.Dispose(disposing);
		}
		
		
		private void InitSectionCollection () {
			sectionCollection = new ReportSectionCollection();
			header = new ReportSection( visualReportHeader);
			pageHeader = new ReportSection(visualPageHeader);
			detail = new ReportSection(visualDetail);
			pageFooter = new ReportSection(visualPageFooter);
			footer = new ReportSection(visualFooter);
	
			header.Name = header.VisualControl.GetType().Name;
			pageHeader.Name = pageHeader.VisualControl.GetType().Name;
			detail.Name = detail.VisualControl.GetType().Name;
			footer.Name = footer.VisualControl.GetType().Name;
			pageFooter.Name = pageFooter.VisualControl.GetType().Name;
	
			sectionCollection.Add(header);
			sectionCollection.Add(pageHeader);
			sectionCollection.Add(detail);
			sectionCollection.Add(pageFooter);
			sectionCollection.Add(footer);
	
			header.Selected += new SelectedEventHandler(this.SectionSelected);
			pageHeader.Selected += new SelectedEventHandler(this.SectionSelected);
			detail.Selected += new SelectedEventHandler(this.SectionSelected);
			footer.Selected += new SelectedEventHandler(this.SectionSelected);
			pageFooter.Selected += new SelectedEventHandler(this.SectionSelected);
			
			header.ItemSelected += new SelectedEventHandler(this.ItemSelected);
			pageHeader.ItemSelected += new SelectedEventHandler(this.ItemSelected);
			detail.ItemSelected += new SelectedEventHandler(this.ItemSelected);
			footer.ItemSelected += new SelectedEventHandler(this.ItemSelected);
			pageFooter.ItemSelected += new SelectedEventHandler(this.ItemSelected);			
			
			//This events are from DragDropp
			visualReportHeader.ReportItemsHandling += new ItemDragDropEventHandler (OnAddReportItem);
			visualPageHeader.ReportItemsHandling += new ItemDragDropEventHandler (OnAddReportItem);
			visualDetail.ReportItemsHandling += new ItemDragDropEventHandler (OnAddReportItem);
			visualPageFooter.ReportItemsHandling += new ItemDragDropEventHandler (OnAddReportItem);
			visualFooter.ReportItemsHandling += new ItemDragDropEventHandler (OnAddReportItem);
		}
	
		private Rectangle SectionClientArea (SharpReport.Designer.ReportSectionControlBase ctrl) {
			Rectangle rect = new Rectangle();
			rect.X = ctrl.Location.X;
			rect.Y = ctrl.Location.Y + ctrl.Head.Height;
			rect.Width = ctrl.Width;
			rect.Height = ctrl.Body.Height;
			return rect;
		}
		
		
		protected void OnAddReportItem (object sender,ItemDragDropEventArgs e) {
			if (e.Action != ItemDragDropEventArgs.enmAction.Add) {
				throw new NotSupportedException ("Only add allowed in Report.OnAddReportItem");
			}
			
			ReportSectionControlBase b = (ReportSectionControlBase)sender;
			
			// Find Selected Section
			if (b != null) {
				selectedSection = (ReportSection)sectionCollection.Find(b.GetType().Name);
			} else {
				throw new NullReferenceException("No Section in Report.OnAddReportItem");
			}
			
			//ReportItem from Factory
			GlobalEnums.ReportItemType rptType = (GlobalEnums.ReportItemType)
				GlobalEnums.StringToEnum(typeof(GlobalEnums.ReportItemType),e.ItemName);
			
			BaseReportItem baseReportItem = null;
			try {
				SharpReport.Designer.IDesignableFactory gf = new SharpReport.Designer.IDesignableFactory();
				
				baseReportItem = gf.Create (rptType.ToString());
				IDesignable iDesignable = baseReportItem as IDesignable;
				
				if (iDesignable != null) {
					iDesignable.Location = e.ItemAtPoint;
					iDesignable.Name = nameService.CreateName(this.selectedSection.Items,
					                                          rptType.ToString());
					
				} else {
					string str = String.Format("<{0}> does not implement IDesignable");
					MessageService.ShowError(str);
				}

			} catch (Exception ee) {
				MessageService.ShowError(ee,ee.Message);
			}
			if (baseReportItem == null) {
				string str = String.Format("Unable to create <0> ",rptType.ToString());
				MessageService.ShowError(str);
			}
			
			//If all went well until now
			if (baseReportItem != null) {
				// set usefull values
				
				try {
					//Insert Item into selectedSection.Items
					if (selectedSection != null) {
						baseReportItem.Parent = selectedSection;
						try {
							IItemRenderer aa = baseReportItem as IItemRenderer;
							if (aa != null) {
								selectedSection.Items.Add(aa);
							} else {
								throw new NullReferenceException("Report:OnAddReportItem ");
							}

						} catch (Exception) {
							throw;
						}
					}
				} catch (Exception ee) {
					throw ee;
				}
			}else {
				throw new Exception("No Item created in Report.OnAddReportItem");
			}
			
			try {
				selectedObject = (IItemRenderer)baseReportItem;
			} catch (Exception ex) {
				MessageService.ShowError(ex,ex.Message);
			}
			
			if (DesignViewChanged != null) {
				DesignViewChanged (this,e);
			}
		}
		
		#region property's		
		
		public ReportSectionCollection SectionCollection
		{
			get {
				return sectionCollection;
			}
			set {
				sectionCollection = value;
			}
		}
		
		public BaseReportObject SelectedObject
		{
			get {return (BaseReportObject)selectedObject; }
		}
		
		public ReportSection SelectedSection
		{
			get { return selectedSection; }
		}
		
		private void OnObjectSelected(EventArgs e)
		{
			if (ObjectSelected != null)
				ObjectSelected(this, e);
		}
		
		
		public ReportSettings ReportSettings {
			get {
				return reportSettings;
			}
			set {
				reportSettings = value;
			}
		}
		#endregion
		
		#region events
		
		void SectionSelected(object sender, System.EventArgs e){
			ReportSection section = (ReportSection)sender;
			selectedSection = section;
			selectedObject = (IBaseRenderer)section;
			OnObjectSelected(e);
			
		}
		void ItemSelected(object sender, System.EventArgs e){
			selectedObject = (IBaseRenderer)sender;
			OnObjectSelected(e);
		}
		
		
		/// <summary>
		/// Fire if Size of Section changes
		/// </summary>
		/// <param name="sender">this</param>
		/// <param name="e">SharpReport.Designer.SectionChangedEventArgs</param>
		void SectionSizeChanged(object sender, SharpReport.Designer.SectionChangedEventArgs e){
			if (SectionChanged != null) {
				Rectangle [] rects = new Rectangle[5];
				rects[0] = SectionClientArea (this.visualReportHeader);
				rects[1] = SectionClientArea (this.visualPageHeader);
				rects[2] = SectionClientArea(this.visualDetail);
				rects[3] = SectionClientArea(this.visualPageFooter);
				rects[4] = SectionClientArea(this.visualFooter);
				SectionChanged (this,new SectionChangedEventArgs (selectedSection,rects));
			}
		}
		#endregion
		
		
		
	
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.visualDetail = new SharpReport.Designer.ReportDetail();
			this.visualPageFooter = new SharpReport.Designer.ReportPageFooter();
			this.visualFooter = new SharpReport.Designer.ReportFooter();
			this.visualPageHeader = new SharpReport.Designer.ReportPageHeader();
			this.visualReportHeader = new SharpReport.Designer.ReportHeader();
			this.SuspendLayout();
			// 
			// visualDetail
			// 
			this.visualDetail.AllowDrop = true;
			this.visualDetail.BackColor = System.Drawing.SystemColors.Control;
			this.visualDetail.Caption = "ReportSectionControlBase";
			this.visualDetail.Dock = System.Windows.Forms.DockStyle.Top;
			this.visualDetail.Location = new System.Drawing.Point(0, 140);
			this.visualDetail.Name = "visualDetail";
			this.visualDetail.Size = new System.Drawing.Size(400, 72);
			this.visualDetail.StringAlignment = System.Drawing.StringAlignment.Near;
			this.visualDetail.TabIndex = 3;
			this.visualDetail.SectionChanged += new EventHandler<SectionChangedEventArgs> (this.SectionSizeChanged);
			// 
			// visualPageFooter
			// 
			this.visualPageFooter.BackColor = System.Drawing.SystemColors.Control;
			this.visualPageFooter.Caption = "ReportSectionControlBase";
			this.visualPageFooter.Dock = System.Windows.Forms.DockStyle.Top;
			this.visualPageFooter.Location = new System.Drawing.Point(0, 212);
			this.visualPageFooter.Name = "visualPageFooter";
			this.visualPageFooter.Size = new System.Drawing.Size(400, 68);
			this.visualPageFooter.StringAlignment = System.Drawing.StringAlignment.Near;
			this.visualPageFooter.TabIndex = 6;
			this.visualDetail.SectionChanged += new EventHandler<SectionChangedEventArgs> (this.SectionSizeChanged);
			// 
			// visualFooter
			// 
			this.visualFooter.BackColor = System.Drawing.SystemColors.Control;
			this.visualFooter.Caption = "ReportSectionControlBase";
			this.visualFooter.Dock = System.Windows.Forms.DockStyle.Top;
			this.visualFooter.Location = new System.Drawing.Point(0, 280);
			this.visualFooter.Name = "visualFooter";
			this.visualFooter.Size = new System.Drawing.Size(400, 76);
			this.visualFooter.StringAlignment = System.Drawing.StringAlignment.Near;
			this.visualFooter.TabIndex = 7;
			this.visualDetail.SectionChanged += new EventHandler<SectionChangedEventArgs> (this.SectionSizeChanged);
			// 
			// visualPageHeader
			// 
			this.visualPageHeader.BackColor = System.Drawing.SystemColors.Control;
			this.visualPageHeader.Caption = "ReportSectionControlBase";
			this.visualPageHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.visualPageHeader.Location = new System.Drawing.Point(0, 56);
			this.visualPageHeader.Name = "visualPageHeader";
			this.visualPageHeader.Size = new System.Drawing.Size(400, 84);
			this.visualPageHeader.StringAlignment = System.Drawing.StringAlignment.Near;
			this.visualPageHeader.TabIndex = 1;
			this.visualDetail.SectionChanged += new EventHandler<SectionChangedEventArgs> (this.SectionSizeChanged);
			// 
			// visualReportHeader
			// 
			this.visualReportHeader.BackColor = System.Drawing.SystemColors.Control;
			this.visualReportHeader.Caption = "ReportSectionControlBase";
			this.visualReportHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.visualReportHeader.Location = new System.Drawing.Point(0, 0);
			this.visualReportHeader.Name = "visualReportHeader";
			this.visualReportHeader.Size = new System.Drawing.Size(400, 56);
			this.visualReportHeader.StringAlignment = System.Drawing.StringAlignment.Near;
			this.visualReportHeader.TabIndex = 0;
			this.visualDetail.SectionChanged += new EventHandler<SectionChangedEventArgs> (this.SectionSizeChanged);
			// 
			// Report
			// 
			this.AllowDrop = true;
			this.Controls.Add(this.visualFooter);
			this.Controls.Add(this.visualPageFooter);
			this.Controls.Add(this.visualDetail);
			this.Controls.Add(this.visualPageHeader);
			this.Controls.Add(this.visualReportHeader);
			this.Name = "Report";
			this.Size = new System.Drawing.Size(400, 392);
			this.ResumeLayout(false);
		}
		#endregion
		
	}
}
