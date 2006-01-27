/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 20.12.2004
 * Time: 23:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

using SharpReportCore;
using SharpReport.ReportItems;

namespace SharpReport.Designer{
	/// <summary>
	/// BaseControl for <see cref="ReportSection"></see>	
	/// </summary>
	/// 
	
	public abstract class ReportSectionControlBase :ReportObjectControlBase
	{
		private System.Windows.Forms.Label titleLabel;
		private System.Windows.Forms.Panel titlePanel;
		private System.Windows.Forms.Panel splitPanel;
		private Ruler.ctrlRuler ctrlRuler1;
		private System.Windows.Forms.Panel bodyPanel;
		
		private string caption = String.Empty;
		
		private bool mouseDown;
		private bool dragAllowed;
		
		private int  currentY;
		
		public event SelectedEventHandler ItemSelected;
		public event ItemDragDropEventHandler ItemDragDrop;
		public event EventHandler <SectionChangedEventArgs> SectionChanged;
		
		internal ReportSectionControlBase()
		{
			InitializeComponent();
			this.SetStyle(ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.ResizeRedraw,
			              true);
			this.UpdateStyles();	
			caption = this.Name;
		}
		
		
		void ReportSectionLoad(object sender, System.EventArgs e)
		{
			titleLabel.Text = this.GetType().Name;
		}
		
		void BodyPanelSizeChanged(object sender, System.EventArgs e){
			this.Size  = new Size (this.Size.Width,this.bodyPanel.Height + this.titlePanel.Height + this.splitPanel.Height);
		}
		
		
		private void BodyPanelPaint(object sender, PaintEventArgs e) {
			Graphics g = e.Graphics;
			g.Clear(this.Body.BackColor);
			
			if (dragAllowed) {
//				using (Pen p = new Pen (Color.Gray)) {
//				    p.Width = 5;
//				    g.DrawRectangle(p,2,2,this.Width - 4, this.Height -4);
//				}
			}
			System.Windows.Forms.ControlPaint.DrawGrid (e.Graphics,
			                                            this.Body.ClientRectangle,
			                                            GlobalValues.GridSize,
			                                            Color.Gray);
		}
		
		
		
		private void TitelLabelPaint(object sender, PaintEventArgs e) {
			e.Graphics.Clear (this.BackColor);
			using (Brush brush = new SolidBrush(Color.Black)) {
			       	e.Graphics.DrawString (caption,
			                       this.Font,
			                       brush,
			                       new PointF(0,0));
			}
		}
		
		void SplitPanelMouseDown(object sender, System.Windows.Forms.MouseEventArgs e){
			mouseDown = true;			
			currentY = e.Y;
		}
		
		
		void SplitPanelMouseMove(object sender, System.Windows.Forms.MouseEventArgs e){
//			System.Console.WriteLine("MoseMove");
			if (mouseDown)
			{
//				if (this.Height + (e.Y - currentY) > titlePanel.Height)
//					this.Height = this.Height + (e.Y - currentY);
			}
		}
		
		void SplitPanelMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (mouseDown){
				this.Height = this.Height + (e.Y - currentY);
				if (SectionChanged != null) {
					SectionChanged (this,new SectionChangedEventArgs (null,null));
				}
				mouseDown = false;
			}
		}
		
		
		#region propertys

		public override Control Body
		{
			get { return bodyPanel; }
		}
		
		public  Control Head
		{
			get { 
				return this.titlePanel;
			}
		}
		
		public string Caption {
			get {
				return caption;
			}
			set {
				caption = value;
				this.titleLabel.Text = caption;
			}
		}
		
		#endregion
		
		#region events

		void FiredDragDropItem (string item,Point pointAt) {
			if (ItemSelected != null) {
				ItemSelected (this,new EventArgs());
			}
			if (ItemDragDrop != null) {
				ItemDragDropEventArgs ea = new ItemDragDropEventArgs (ItemDragDropEventArgs.enmAction.Add,
				                                                      pointAt,
				                                                      item);
				ItemDragDrop (this,ea);
			}
		}
		
		#endregion
		
		#region overrides
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
			using (Pen p = new Pen(Color.Gray,5)){
				e.Graphics.DrawRectangle(p, 0, 0, this.Width - 1, this.Height - 1);
			}
		}
		
		protected override void OnResize (EventArgs e) {
			base.OnResize (e);
		}
	
		
		#endregion
		
		#region dragdrop
		protected bool CheckDragElement (System.Windows.Forms.DragEventArgs e) {
			bool drag = false;
			if (e.Data.GetDataPresent(typeof(System.String))){
				Object item = (object)e.Data.GetData(typeof(System.String));
				string str = Convert.ToString(item);
				try {
					GlobalEnums.StringToEnum (typeof(GlobalEnums.ReportItemType),str);
					e.Effect = DragDropEffects.Copy;
					drag = true;
				} catch (Exception ex) {
					e.Effect = DragDropEffects.None;
					drag = false;
					throw ex;
				} finally {
					
				}
			} else {
				e.Effect = DragDropEffects.None;
				drag = false;
			}
			return drag;
		}
		
		void BodyPanelDragDrop(object sender, System.Windows.Forms.DragEventArgs e){
			if (e.Data.GetDataPresent(typeof(System.String))){
				Object item = (object)e.Data.GetData(typeof(System.String));
				FiredDragDropItem (Convert.ToString(item),
				                  this.Body.PointToClient (Cursor.Position));
			}
			
		}
		
		void BodyPanelDragEnter(object sender, System.Windows.Forms.DragEventArgs e){
//		System.Console.WriteLine("BodyPanelDragEnter");
			dragAllowed = CheckDragElement(e);
			this.Body.Invalidate();
		}
		
		void BodyPanelDragLeave(object sender, System.EventArgs e){
			dragAllowed = false;
			this.Body.Invalidate();
		}
		
		private void BodyPanelDragOver(object sender, System.Windows.Forms.DragEventArgs e){
			System.Console.WriteLine("ReportSectionControlBase:BodyDragOver");
//			if (this.bodyPanel.Controls.Count > 0) {		
//				if (PerformHitTest()) {
//					
//				} else {
////					System.Console.WriteLine("\t\t no Hit");
//					
//				}
//				
//			}
			
			if (dragAllowed) {
				e.Effect = DragDropEffects.Copy;
			} else {
				e.Effect = DragDropEffects.None;
			}
		}
		
		/*
		bool PerformHitTest() {
		
			Point p = this.bodyPanel.PointToClient(Control.MousePosition);
			for (int i = 0; i < this.bodyPanel.Controls.Count; i++) {
				Control c = this.bodyPanel.Controls[i];
				Rectangle r = new Rectangle(c.Left,c.Top,c.ClientRectangle.Width,c.ClientRectangle.Height);
				if (r.Contains (p)) {
					this.ItemSelected(c,EventArgs.Empty);
					return true;
				}
			}
			
			return false;	
		}
		*/
		
		#endregion
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.bodyPanel = new System.Windows.Forms.Panel();
			this.ctrlRuler1 = new Ruler.ctrlRuler();
			this.splitPanel = new System.Windows.Forms.Panel();
			this.titlePanel = new System.Windows.Forms.Panel();
			this.titleLabel = new System.Windows.Forms.Label();
			this.titlePanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// bodyPanel
			// 
			this.bodyPanel.AllowDrop = true;
			this.bodyPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
						| System.Windows.Forms.AnchorStyles.Left) 
						| System.Windows.Forms.AnchorStyles.Right)));
			this.bodyPanel.BackColor = System.Drawing.SystemColors.Window;
			this.bodyPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.bodyPanel.Location = new System.Drawing.Point(24, 20);
			this.bodyPanel.Name = "bodyPanel";
			this.bodyPanel.Size = new System.Drawing.Size(408, 129);
			this.bodyPanel.TabIndex = 6;
			this.bodyPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.BodyPanelDragEnter);
			this.bodyPanel.SizeChanged += new System.EventHandler(this.BodyPanelSizeChanged);
			this.bodyPanel.DragLeave += new System.EventHandler(this.BodyPanelDragLeave);
			this.bodyPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.BodyPanelPaint);
			this.bodyPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.BodyPanelDragDrop);
			this.bodyPanel.DragOver += new System.Windows.Forms.DragEventHandler(this.BodyPanelDragOver);
			// 
			// ctrlRuler1
			// 
			this.ctrlRuler1.BackColor = System.Drawing.SystemColors.Window;
			this.ctrlRuler1.Direction = Ruler.ctrlRuler.enmDirection.enmVertikal;
			this.ctrlRuler1.DrawFrame = false;
			this.ctrlRuler1.EndValue = 210;
			this.ctrlRuler1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.ctrlRuler1.LeftMargin = 0;
			this.ctrlRuler1.Location = new System.Drawing.Point(0, 20);
			this.ctrlRuler1.MarginColor = System.Drawing.Color.Empty;
			this.ctrlRuler1.Name = "ctrlRuler1";
			this.ctrlRuler1.RightMargin = 0;
			this.ctrlRuler1.ScaleUnit = System.Drawing.GraphicsUnit.Millimeter;
			this.ctrlRuler1.Size = new System.Drawing.Size(24, 128);
			this.ctrlRuler1.StartValue = 0;
			this.ctrlRuler1.TabIndex = 1;
			// 
			// splitPanel
			// 
			this.splitPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.splitPanel.Cursor = System.Windows.Forms.Cursors.HSplit;
			this.splitPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitPanel.Location = new System.Drawing.Point(0, 150);
			this.splitPanel.Name = "splitPanel";
			this.splitPanel.Size = new System.Drawing.Size(432, 4);
			this.splitPanel.TabIndex = 7;
			this.splitPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SplitPanelMouseUp);
			this.splitPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SplitPanelMouseMove);
			this.splitPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SplitPanelMouseDown);
			// 
			// titlePanel
			// 
			this.titlePanel.BackColor = System.Drawing.SystemColors.Control;
			this.titlePanel.Controls.Add(this.titleLabel);
			this.titlePanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.titlePanel.Location = new System.Drawing.Point(0, 0);
			this.titlePanel.Name = "titlePanel";
			this.titlePanel.Size = new System.Drawing.Size(432, 20);
			this.titlePanel.TabIndex = 5;
			// 
			// titleLabel
			// 
			this.titleLabel.Location = new System.Drawing.Point(24, 0);
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Size = new System.Drawing.Size(144, 20);
			this.titleLabel.TabIndex = 3;
			this.titleLabel.Text = "label1";
			this.titleLabel.Paint += new System.Windows.Forms.PaintEventHandler(this.TitelLabelPaint);
			// 
			// ReportSectionControlBase
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.bodyPanel);
			this.Controls.Add(this.titlePanel);
			this.Controls.Add(this.splitPanel);
			this.Controls.Add(this.ctrlRuler1);
			this.Name = "ReportSectionControlBase";
			this.Size = new System.Drawing.Size(432, 154);
			this.Load += new System.EventHandler(this.ReportSectionLoad);
			this.titlePanel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		
		
		
	}
		
}
