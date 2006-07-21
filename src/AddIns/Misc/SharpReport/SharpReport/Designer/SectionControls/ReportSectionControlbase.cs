/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 20.12.2004
 * Time: 23:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

using SharpReportCore;
using SharpReport.ReportItems;

/// <summary>
/// BaseControl for <see cref="ReportSection"></see>
/// </summary>
/// 

namespace SharpReport.Designer{
	public abstract class ReportSectionControlBase :ReportObjectControlBase,
													ITracker{
													
		private System.Windows.Forms.Panel titlePanel;
		private System.Windows.Forms.Panel splitPanel;
		private Ruler.ctrlRuler ctrlRuler1;
		private System.Windows.Forms.Panel bodyPanel;
		
		private string caption;
		
		private bool mouseDown;
		private bool dragAllowed;
		
		private int  currentY;
		private IDesignableFactory designableFactory;
		private ControlHelper controlHelper;
		private BaseReportItem draggedItem;
		
		public event EventHandler <EventArgs> ItemSelected;
		public event ItemDragDropEventHandler ItemDragDrop;
		public event EventHandler <SectionChangedEventArgs> SectionChanged;
		
		private ReportControlBase selectedControl;
		private RectTracker rectTracker = new RectTracker();
		
		internal ReportSectionControlBase(){
			InitializeComponent();
			this.SetStyle(ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.ResizeRedraw,
			              true);
			this.UpdateStyles();
			caption = this.Name;
			this.designableFactory = new IDesignableFactory();
			this.controlHelper = new ControlHelper(this);
		}
		
		void BodyPanelSizeChanged(object sender, System.EventArgs e){
			this.Size  = new Size (this.Size.Width,this.bodyPanel.Height + this.titlePanel.Height + this.splitPanel.Height);
		}
		
		#region overrides
		
		protected override CreateParams CreateParams{ 
			get { 
				CreateParams cp=base.CreateParams; 
				cp.ExStyle|=0x00000020; //WS_EX_TRANSPARENT 
				return cp; 
			} 
		} 
		
		#endregion
		
		#region ITracker implementation
		
		public void ClearSelections() {
			this.controlHelper.Clear(this.bodyPanel);
			this.selectedControl = null;
			this.InvalidateEx();
		}
		
		public void InvalidateEx() {
			
			this.Invalidate();
			
			if (this.Parent ==  null) {
				return;
			}
			Rectangle rc = new Rectangle  (this.bodyPanel.Location,this.bodyPanel.Size);
			this.Invalidate(rc,true);
			
			if(this.selectedControl != null){
				rc = this.rectTracker.m_rect;
				this.selectedControl.SetBounds(rc.Left, rc.Top, rc.Width, rc.Height);
				this.selectedControl.Invalidate();
			}
			
		}
		
		public ReportControlBase SelectedControl {
			set {
				selectedControl = value;
			}
		}
		
		public RectTracker RectTracker {
			get {
				return this.rectTracker;
			}
		}
		
		public Control DesignSurface {
			get{
				return this.bodyPanel;
			}
		}
		#endregion
		
		
		#region tracker
		
		
		private Rectangle GetParentRectangle(){
			return new Rectangle(new Point(0,0),this.bodyPanel.Size);
		}
		
		
		private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e){			
		
			if (this.rectTracker == null) {
				return;
			}
			if(e.Button != MouseButtons .Left){
				return;
			}	
			Point pt = this.bodyPanel.PointToClient(Cursor.Position);
			
			Rectangle rcForm = GetParentRectangle();

			if(rcForm.Contains(pt)){		
				Rectangle rcObject;
				if (this.rectTracker.HitTest(pt) == RectTracker.TrackerHit.hitNothing) {

					this.selectedControl = null;
					this.rectTracker.m_rect = new Rectangle(0,0,0,0);
					// just to demonstrate RectTracker::TrackRubberBand
					RectTracker tracker=new RectTracker();
					if (tracker.TrackRubberBand(this.bodyPanel, pt, false)){
						// see if rubber band intersects with the doc's tracker		
//                        System.Console.WriteLine("3");
						tracker.NormalizeRect(ref tracker.m_rect);
						Rectangle rectIntersect = tracker.m_rect;
						foreach (Control ctrl in this.bodyPanel.Controls){
							rcObject = ctrl.Bounds;
//				
							if(tracker.m_rect.Contains(rcObject)){
								
								this.rectTracker.m_rect = rcObject;
								this.selectedControl = (ReportControlBase)ctrl;
								this.selectedControl.Selected = true;
								break;
							}
						}
					}
					else{
					
						// No rubber band, see if the point selects an object.
						
						foreach (Control ctrl in this.bodyPanel.Controls){
							rcObject = ctrl.Bounds ;
							if(rcObject.Contains(pt)){
								this.rectTracker.m_rect = rcObject;
								this.selectedControl = (ReportControlBase)ctrl;
								break;
							}
						}
					}
					if(this.selectedControl == null){
						NotifySectionClick();
					}
					else{
//						System.Console.WriteLine("6");
//						m_FormTracker.Clear();
						
					}
				}
				else if(this.selectedControl != null){// normal tracking action, when tracker is hit	
					if (this.rectTracker.Track(this.bodyPanel, pt, false,null)) {
						Rectangle rc = this.rectTracker.m_rect;
						this.selectedControl.SetBounds(rc.Left, rc.Top, rc.Width, rc.Height);
						this.selectedControl.NotifyPropertyChanged("Tracker");
					}
				}

			}
			else{
				if(this.selectedControl == null){//select the container form
					System.Console.WriteLine("9");
//					MainForm.m_propertyWindow.SetSelectedObject(m_Form);
					/*
					if(m_FormTracker.HitTest(pt) == RectTracker.TrackerHit.hitNothing)
					{
						m_FormTracker.m_rect = rcForm;		
					}
					else if(!m_FormTracker.IsEmpty()) 
					{
						m_FormTracker.Track(this, pt, false,null);
					}
					*/
				}
				else{
//					System.Console.WriteLine("10");
//					m_FormTracker.Clear();
				}
			}
			this.InvalidateEx();
			
		}
		
		
		private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e){
			if (this.rectTracker != null) {
				Point mousept=new Point(e.X,e.Y);
				
				if(this.selectedControl != null){
					if(!rectTracker.SetCursor(this,0,mousept))
						this.Cursor=Cursors.Arrow;
				}
				
//			else{
//				if(!m_FormTracker.SetCursor(this,0,mousept))
//					this.Cursor=Cursors.Arrow;
//			}
			}
		}

		#endregion
		
		#region painting
		
		private void BodyPanelPaint(object sender, PaintEventArgs pea) {
			pea.Graphics.Clear(this.Body.BackColor);
			ControlPaint.DrawGrid (pea.Graphics,
			                       this.Body.ClientRectangle,
			                       GlobalValues.GridSize,
			                       Color.Gray);
		}
		
		
		private void OnPaintTitel(object sender, PaintEventArgs pea) {
			pea.Graphics.Clear (this.BackColor);
			using (Brush brush = new SolidBrush(Color.Black)) {
				pea.Graphics.DrawString (caption,
				                         this.Font,
				                         brush,
				                         new PointF(this.bodyPanel.Location.X,0));
			}
		}
		
		#endregion
		
		#region splitter
		void SplitPanelMouseDown(object sender, System.Windows.Forms.MouseEventArgs e){
			mouseDown = true;
			currentY = e.Y;
		}
		
		
		void SplitPanelMouseUp(object sender, System.Windows.Forms.MouseEventArgs mea){
			if (mouseDown){
				this.Height = this.Height + (mea.Y - currentY);
				if (SectionChanged != null) {
					SectionChanged (this,new SectionChangedEventArgs (null,null));
				}
				mouseDown = false;
			}
		}
		
		#endregion
		
		#region propertys

		public override Control Body{
			get { return bodyPanel; }
		}
		
		public  Control Head{
			get {
				return this.titlePanel;
			}
		}
		
		public string Caption {
			set {
				caption = value;
			}
		}
		
		#endregion
		
		#region events
		
		void NotifySectionClick () {
			ClearSelections();
			this.OnClick(EventArgs.Empty);
		}
		
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

		
		#region DragDrop
		
		private string DragObjectToString (DragEventArgs dea) {
			if (dea.Data.GetDataPresent(typeof(System.String))){
				return Convert.ToString (dea.Data.GetData(typeof(System.String)),
				                         CultureInfo.InvariantCulture);
			} else {
				return String.Empty;
			}
		}
		
		private bool CheckDraggedControl (DragEventArgs dea) {
			string str = this.DragObjectToString (dea);
			return this.designableFactory.Contains(str);
		}
		
		private bool CheckDragElement (System.Windows.Forms.DragEventArgs dea) {
			bool drag;
			if (CheckDraggedControl(dea)) {
				dea.Effect = DragDropEffects.Copy;
				drag = true;
			} else {
				dea.Effect = DragDropEffects.None;
				drag = false;
			}
			return drag;
		}
		
		
		
		void BodyPanelDragDrop(object sender, System.Windows.Forms.DragEventArgs dea){
			if (dea.Data.GetDataPresent(typeof(System.String))){
				Object item = (object)dea.Data.GetData(typeof(System.String));
				
				FiredDragDropItem (Convert.ToString(item),
				                   this.Body.PointToClient (Cursor.Position));
			}
			
		}
		
		void BodyPanelDragEnter(object sender, System.Windows.Forms.DragEventArgs dea){
			dragAllowed = CheckDragElement(dea);
			string s = DragObjectToString (dea);
			
			this.draggedItem = 	 this.designableFactory.Create(s);
			
		}
		
		void BodyPanelDragLeave(object sender, System.EventArgs e){
			dragAllowed = false;
			this.Body.Invalidate();
		}
		
		private void BodyPanelDragOver(object sender, System.Windows.Forms.DragEventArgs dea){
			IContainerItem parentControl = this.IsValidContainer(dea);
			if (parentControl != null) {
				if (parentControl.IsValidChild(this.draggedItem)) {
					dea.Effect = DragDropEffects.Copy;
				} else {
					dea.Effect = DragDropEffects.None;
				}
				
			} else {
				if (dragAllowed) {
					dea.Effect = DragDropEffects.Copy;
				} else {
					dea.Effect = DragDropEffects.None;
				}
			}

		}
		
		private IContainerItem IsValidContainer(DragEventArgs dea) {
			Point point = new Point(dea.X,dea.Y);
//			System.Console.WriteLine("ReportSectionControlBase:IsValidContainer");
			for (int i = 0; i < this.bodyPanel.Controls.Count; i++) {
				Control c = this.bodyPanel.Controls[i];
				Rectangle r = c.ClientRectangle;
				
				if (r.Contains(c.PointToClient(point))) {
//					System.Console.WriteLine("\tfound Control {0} ",c.Name);
					IContainerItem ia = c as IContainerItem;
					ContainerControl cc = c as ContainerControl;
					if (cc != null) {
						System.Console.WriteLine("\tContainerControl {0}",cc.Name);
					} else {
//						System.Console.WriteLine("\tNo ContainerControl");
					}
//					if (ia != null) {
//						System.Console.WriteLine("\t\tContainer is {0}",c.Name);
//					} else {
//						System.Console.WriteLine("\t\tNo COntainer{0}",c.Name);
//					}
					return ia;
				}
			}
			return null;
		}
		
		
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
			this.bodyPanel.Size = new System.Drawing.Size(408, 131);
			this.bodyPanel.TabIndex = 6;
			this.bodyPanel.DragOver += new System.Windows.Forms.DragEventHandler(this.BodyPanelDragOver);
			this.bodyPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.BodyPanelDragDrop);
			this.bodyPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.BodyPanelDragEnter);
			this.bodyPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.BodyPanelPaint);
			this.bodyPanel.SizeChanged += new System.EventHandler(this.BodyPanelSizeChanged);
			this.bodyPanel.DragLeave += new System.EventHandler(this.BodyPanelDragLeave);
			this.bodyPanel.MouseMove += new MouseEventHandler(this.OnMouseMove);
			this.bodyPanel.MouseDown += new MouseEventHandler(this.OnMouseDown);
			// 
			// ctrlRuler1
			// 
			this.ctrlRuler1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			                                                               | System.Windows.Forms.AnchorStyles.Left)));
			this.ctrlRuler1.BackColor = System.Drawing.SystemColors.Window;
			this.ctrlRuler1.Direction = Ruler.ctrlRuler.enmDirection.enmVertikal;
			this.ctrlRuler1.DrawFrame = true;
			this.ctrlRuler1.EndValue = 210;
			this.ctrlRuler1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ctrlRuler1.LeftMargin = 0;
			this.ctrlRuler1.Location = new System.Drawing.Point(0, 20);
			this.ctrlRuler1.MarginColor = System.Drawing.Color.Empty;
			this.ctrlRuler1.Name = "ctrlRuler1";
			this.ctrlRuler1.RightMargin = 0;
			this.ctrlRuler1.ScaleUnit = System.Drawing.GraphicsUnit.Millimeter;
			this.ctrlRuler1.Size = new System.Drawing.Size(24, 131);
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
			this.splitPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SplitPanelMouseDown);
			this.splitPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SplitPanelMouseUp);
			// 
			// titlePanel
			// 
			this.titlePanel.BackColor = System.Drawing.SystemColors.Control;
			this.titlePanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.titlePanel.Location = new System.Drawing.Point(0, 0);
			this.titlePanel.Name = "titlePanel";
			this.titlePanel.Size = new System.Drawing.Size(432, 20);
			this.titlePanel.TabIndex = 5;
			this.titlePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintTitel);
			// 
			// UserControl1
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.bodyPanel);
			this.Controls.Add(this.titlePanel);
			this.Controls.Add(this.splitPanel);
			this.Controls.Add(this.ctrlRuler1);
			
			this.Name = "UserControl1";
			this.Size = new System.Drawing.Size(432, 154);
			this.titlePanel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		
		#endregion
	}
	
}
