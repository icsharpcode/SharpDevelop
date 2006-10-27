/*
 * Created by SharpDevelop.
 * User: Fabio
 * Date: 09/10/2004
 * Time: 9.48
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace SharpReport.Designer{
	/// <summary>
	/// Base Class of all Visible Controls like Graphic or textbased Item's
	/// </summary>
	
	public abstract class ReportControlBase : ReportObjectControlBase,
	INotifyPropertyChanged{
		
		private ControlHelper controlHelper;
		
		private const string contextMenuPath = "/SharpReport/ContextMenu/Items";
		
		private bool selected;
		
		
		internal ReportControlBase(){
			InitializeComponent();
			this.SetStyle(ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.ResizeRedraw,
			              true);
			this.UpdateStyles();
			controlHelper = new ControlHelper((Control)this);
		}
		
		private ITracker GetParent {
			get {	
				if (this.Parent is Panel) {
					ITracker t = this.Parent.Parent as ITracker;
					if (t != null) {
						return t;
					} else {
						System.Console.WriteLine("!!!!!!!!! NO TRACKER !!!!");
						return null;
						
					}
				} else {
					ITracker ct = this.Parent as ITracker;
					if (ct != null) {
						return ct;
					}else {
						System.Console.WriteLine("!!!!!!!!! NO TRACKER !!!!");
						return null;
					}
				}
			}
		}
		
		
		private void NotifySelected() {
			
			this.GetParent.SelectedControl = this;
			this.GetParent.InvalidateEx();
			this.OnClick (EventArgs.Empty);
		}
		
		
		private void NotifyUnSelected () {
			this.selected = false;
			this.GetParent.SelectedControl = null;
			this.GetParent.InvalidateEx();
		}
		
	
		private void OnMouseDown (object sender, MouseEventArgs e) {
			ITracker tracker = this.GetParent;
			if (tracker != null) {
				tracker.ClearSelections();
				tracker.RectTracker.m_rect = this.Bounds;
				this.selected = true;
				this.NotifySelected();
				tracker.SelectedControl = this;
			}
			this.selected = true;
		}
		
		
		private void OnMouseUp(object sender, MouseEventArgs e){
			
			if (e.Button == MouseButtons.Right) {
				ContextMenuStrip ctMen = MenuService.CreateContextMenu (this,contextMenuPath);
				ctMen.Show (this,new Point (e.X,e.Y));
				this.NotifyUnSelected();
			}
			
		}
	
		protected void DrawEdges (PaintEventArgs e,Rectangle rectangle) {
			controlHelper.DrawEdges(e,rectangle);
		}
		
		
		protected void DrawEdges (PaintEventArgs e) {
			controlHelper.DrawEdges(e);
		}
		
		
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e){
			base.OnPaint(e);
			if (this.selected) {
				RectTracker tracker = this.GetParent.RectTracker;
				if (tracker != null) {
					tracker.m_rect = this.Bounds;
					tracker.Draw(this.GetParent.DesignSurface.CreateGraphics());
				}
			}
		}
		
		protected override void OnResize(EventArgs e){
			base.OnResize(e);
			this.Invalidate();
		}
		
		protected ControlHelper ControlHelper {
			get {
				return controlHelper;
			}
		}
		
		
		
		protected Rectangle FocusRectangle {
			get {return this.controlHelper.BuildFocusRectangle;}
		}
		
		public bool Selected {
			set {
				selected = value;
			}
		}
		
		#region SharpReportCore.IPropertyChange interface implementation
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		public void NotifyPropertyChanged(string property) {
			if (this.PropertyChanged != null) {
				this.PropertyChanged(this,new System.ComponentModel.PropertyChangedEventArgs (property));
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
			this.SuspendLayout();
			// 
			// ReportControlBase
			// 
			this.Name = "ReportControlBase";
			this.Size = new System.Drawing.Size(292, 56);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
			this.ResumeLayout(false);
		}
		#endregion
	}
}
