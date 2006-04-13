/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 10.01.2005
 * Time: 22:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SharpReport;
using SharpReportCore;



namespace SharpReport.Designer
{
	/// <summary>
	/// Description of BaseDesignerControl.
	/// </summary>
	public class BaseDesignerControl : System.Windows.Forms.UserControl,SharpReport.Designer.IVisitor
	{
		private SharpReport.Designer.Report reportControl;
		private Ruler.ctrlRuler ctrlRuler1;

		private System.Drawing.GraphicsUnit graphicsUnit;
		
		private ReportModel reportModel;
		
		public event PropertyChangedEventHandler DesignerDirty;
		
		
		
		public BaseDesignerControl()
		{
			InitializeComponent();
			
			this.SetStyle(ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.ResizeRedraw,
			              true);
			this.UpdateStyles();
			
			this.graphicsUnit = System.Drawing.GraphicsUnit.Millimeter;
			reportModel = new ReportModel (graphicsUnit);
			this.ReportControl.ReportSettings = reportModel.ReportSettings;
		}
		#region public'c
		
		public void Localise() {
			reportControl.Localise();
		}
		
		public void RegisterEvents () {
			this.reportControl.SectionChanged += new EventHandler<SectionChangedEventArgs> (this.ReportControlSectionChanged);
			this.reportControl.SizeChanged += new System.EventHandler(this.ReportControlSizeChanged);
			this.reportControl.DesignViewChanged += new SharpReport.Designer.ItemDragDropEventHandler(this.ReportControlDesignViewChanged);
		}
		
		public void RemoveSelectedItem () {
			if (this.SelectedObject == null) {
				return;
			}
			BaseReportItem item = this.SelectedObject as BaseReportItem;
				
			if ((item.Parent == this.SelectedSection)|| (item.Parent == null)){
				this.SelectedSection.Items.Remove (item);
			} else {
				IContainerItem con = item.Parent as IContainerItem;
				if (con != null) {
					con.Items.Remove (item);
				}
			}
		}
		
		
		#endregion
		
		#region overrides
		
		protected override void Dispose(bool disposing ) {
			if( disposing ){
				this.reportControl = null;
				this.ctrlRuler1 = null;
			}
			base.Dispose( disposing );
		}
		
		protected override void OnResize(System.EventArgs e) {
			this.ctrlRuler1.Width = reportControl.Width;
		}
		
		#endregion
		
		#region events
		void ReportControlSizeChanged(object sender, System.EventArgs e){
			this.ctrlRuler1.Width = reportControl.Width;
			this.ctrlRuler1.Invalidate();
			NotifyPropertyChanged(this.Name + "ReportControlSizeChanged");
		}
		
		void ReportControlSectionChanged (object sender,SectionChangedEventArgs e) {
			NotifyPropertyChanged(this.Name + "ReportControlSectionChanged");
		}
		
		void ReportControlDesignViewChanged(object sender, SharpReport.Designer.ItemDragDropEventArgs e){
			NotifyPropertyChanged(this.Name + "ReportControlDesignViewChanged");
		}
		
		#endregion
		
		#region privates
		void NotifyPropertyChanged(string info) {
			if (DesignerDirty != null) {
				DesignerDirty (this,new PropertyChangedEventArgs(info));
			}
		}
		#endregion
		
		#region SharpReport.DelegatesInterfaces.IVisitor interface implementation
		public void Accept(IDesignerVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
		
		
		#region property's
		
		public ReportModel ReportModel {
			get {
				
				reportModel.ReportSettings = reportControl.ReportSettings;
				reportModel.SectionCollection.Clear();
				foreach (ReportSection section in reportControl.SectionCollection) {
					reportModel.SectionCollection.Add (section);
				}
				return reportModel;
			}
			set {
				this.reportModel = value;
				reportControl.ReportSettings = this.reportModel.ReportSettings;
				reportControl.SectionCollection = this.ReportModel.SectionCollection;
			}
		}
		
		
		public SharpReport.Designer.Report ReportControl {
			get {
				return reportControl;
			}
		}
		public BaseReportObject SelectedObject
		{
			get { return this.reportControl.SelectedObject; }
		}
		
		
		public ReportSection SelectedSection {
			get {
				return this.reportControl.SelectedSection;
			}
		}
		
		
		public ReportSectionCollection SectionsCollection {
			get {
				return this.reportControl.SectionCollection;
			}
			set {
				this.reportControl.SectionCollection = value;
			}
		}

		public System.Drawing.GraphicsUnit GraphicsUnit {
			get {
				return graphicsUnit;
			}
			set {
				graphicsUnit = value;
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
			this.ctrlRuler1 = new Ruler.ctrlRuler();
			this.reportControl = new SharpReport.Designer.Report();
			this.SuspendLayout();
			// 
			// ctrlRuler1
			// 
			this.ctrlRuler1.BackColor = System.Drawing.SystemColors.Window;
			this.ctrlRuler1.Direction = Ruler.ctrlRuler.enmDirection.enmHorizontal;
			this.ctrlRuler1.DrawFrame = false;
			this.ctrlRuler1.EndValue = 210;
			this.ctrlRuler1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.ctrlRuler1.LeftMargin = 0;
			this.ctrlRuler1.Location = new System.Drawing.Point(32, 8);
			this.ctrlRuler1.MarginColor = System.Drawing.Color.Empty;
			this.ctrlRuler1.Name = "ctrlRuler1";
			this.ctrlRuler1.RightMargin = 0;
			this.ctrlRuler1.ScaleUnit = System.Drawing.GraphicsUnit.Millimeter;
			this.ctrlRuler1.Size = new System.Drawing.Size(568, 24);
			this.ctrlRuler1.StartValue = 0;
			this.ctrlRuler1.TabIndex = 0;
			// 
			// reportControl
			// 
			this.reportControl.AllowDrop = true;
			this.reportControl.Location = new System.Drawing.Point(8, 40);
			this.reportControl.Name = "reportControl";
			this.reportControl.Size = new System.Drawing.Size(592, 400);
			this.reportControl.TabIndex = 1;
//			this.reportControl.SectionChanged += new EventHandler<SectionChangedEventArgs> (this.ReportControlSectionChanged);
//			this.reportControl.SizeChanged += new System.EventHandler(this.ReportControlSizeChanged);
//			this.reportControl.DesignViewChanged += new SharpReport.Designer.ItemDragDropEventHandler(this.ReportControlDesignViewChanged);
			// 
			// BaseDesignerControl
			// 
			this.AutoScroll = true;
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.Controls.Add(this.reportControl);
			this.Controls.Add(this.ctrlRuler1);
			this.Name = "BaseDesignerControl";
			this.Size = new System.Drawing.Size(608, 440);
			this.ResumeLayout(false);
		}
		#endregion
		
		
	}
}
