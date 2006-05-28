/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 25.05.2006
 * Time: 09:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace SharpReport.Designer{

	/// <summary>
	/// Description of ContainerControl.
	/// </summary>
	public class ContainerControl:ReportControlBase,ITracker{
		private RectTracker rectTracker = new RectTracker();
		private ReportControlBase selectedControl;
		
		public ContainerControl():base(){
		this.Body.MouseMove += new MouseEventHandler( OnMouseMove);
		this.Body.MouseDown += new MouseEventHandler(OnMouseDown);
		}
		
		private Rectangle GetParentRectangle () {
			return this.Body.ClientRectangle;
		}
		
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e){
			base.OnPaint(e);
		}
		
		#region overrides
	/*
		protected override CreateParams CreateParams{ 
			get { 
				CreateParams cp=base.CreateParams; 
				cp.ExStyle|=0x00000020; //WS_EX_TRANSPARENT 
				return cp; 
			} 
		} 
		*/
		#endregion
		
		private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e){			
		
			if (this.rectTracker == null) {
				return;
			}
			if(e.Button != MouseButtons .Left){
				return;
			}	
			Point pt = this.Body.PointToClient(Cursor.Position);
			
			Rectangle rcForm = GetParentRectangle();
			
			if(rcForm.Contains(pt)){
				Rectangle rcObject;
				if (this.rectTracker.HitTest(pt) == RectTracker.TrackerHit.hitNothing) {
//					System.Console.WriteLine("2");
					this.selectedControl = null;
					this.rectTracker.m_rect = new Rectangle(0,0,0,0);
					// just to demonstrate RectTracker::TrackRubberBand
					RectTracker tracker=new RectTracker();
					if (tracker.TrackRubberBand(this.Body, pt, false)){
						// see if rubber band intersects with the doc's tracker		

						tracker.NormalizeRect(ref tracker.m_rect);
						Rectangle rectIntersect = tracker.m_rect;
						foreach (Control ctrl in this.Body.Controls){
							rcObject = ctrl.Bounds;
							if(tracker.m_rect.Contains(rcObject)){
								
								this.rectTracker.m_rect = rcObject;
								this.selectedControl = (ReportControlBase)ctrl;
								this.selectedControl.Selected = true;
								// MainForm.m_propertyWindow.SetSelectedObject(m_seletedCtrl);
								break;
							}
						}
					}
					else{
						
						// No rubber band, see if the point selects an object.
						
						foreach (Control ctrl in this.Body.Controls){
							rcObject = ctrl.Bounds ;
							if(rcObject.Contains(pt)){
								
								this.rectTracker.m_rect = rcObject;
								this.selectedControl = (ReportControlBase)ctrl;
								this.selectedControl.Selected = true;
//								MainForm.m_propertyWindow.SetSelectedObject(ctrl);
								break;
							}
						}
					}
					if(this.selectedControl == null){
//						NotifySectionClick();
						
//						MainForm.m_propertyWindow.SetSelectedObject(m_Form);
//						m_FormTracker.m_rect = rcForm;
					}
					else{
//						System.Console.WriteLine("6");
//						m_FormTracker.Clear();
						
					}
				}
				else if(this.selectedControl != null){// normal tracking action, when tracker is hit	
//					System.Console.WriteLine("7");
					if (this.rectTracker.Track(this.Body, pt, false,null)) {
						Rectangle rc = this.rectTracker.m_rect;
						this.selectedControl.SetBounds(rc.Left, rc.Top, rc.Width, rc.Height);
					}
				}
			}
			else{
				if(this.selectedControl == null){//select the container form
//					System.Console.WriteLine("9");
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
			
		private void OnMouseMove (object sender, MouseEventArgs e) {
			
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
		
		#region ITracker implementation
		
		public virtual void ClearSelections() {
			base.ControlHelper.Clear(this);
			this.selectedControl = null;
			this.InvalidateEx();
		}
		
		public ReportControlBase SelectedControl {
			set {
				this.selectedControl = value;
			}
		}
		
		public RectTracker RectTracker {
			get {
				return this.rectTracker;
			}
		}
		
		public System.Windows.Forms.Control DesignSurface {
			get {
				return this.Body;
			}
		}
		
		public void InvalidateEx(){
			this.Invalidate();
			
			if (this.Parent ==  null) {
				return;
			}
			Rectangle rc = new Rectangle  (this.Body.Location,this.Body.Size);
			this.Invalidate(rc,true);
			
			if(this.selectedControl != null){
				rc = this.rectTracker.m_rect;
			
				this.selectedControl.SetBounds(rc.Left, rc.Top, rc.Width, rc.Height);
				this.selectedControl.Invalidate();
			}
		}
		
		#endregion
	}
}
