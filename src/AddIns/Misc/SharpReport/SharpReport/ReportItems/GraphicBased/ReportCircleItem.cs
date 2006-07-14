/*
 * Created by SharpDevelop.
 * User: Fabio
 * Date: 09/10/2004
 * Time: 21.43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.ComponentModel;

using SharpReportCore;
using SharpReport.Designer;


namespace SharpReport.ReportItems{
	
	public class ReportCircleItem : BaseCircleItem,IDesignable{	
	/// <summary>
	/// This Class draws a Circle 
	///All this Graphical Classes derive from <see cref="BaseGraphicItem"></see>
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 07.01.2005 16:14:05
	/// </remarks>
	/// 
	
	private	ReportCircleControl visualControl;
		

	public ReportCircleItem() : base(){
			visualControl = new ReportCircleControl();
			
//			ItemsHelper.UpdateBaseFromGraphicControl (this.visualControl,this);
			
			this.visualControl.Click += new EventHandler(OnControlSelect);
			this.visualControl.VisualControlChanged += new EventHandler (OnControlChanged);
			this.visualControl.LocationChanged += new EventHandler (OnControlChanged);
			this.visualControl.BackColorChanged += new EventHandler (OnControlChanged);
			this.visualControl.FontChanged += new EventHandler (OnControlChanged);
			this.visualControl.ForeColorChanged += new EventHandler (OnControlChanged);
			//Event from Tracker
			this.visualControl.PropertyChanged += new PropertyChangedEventHandler (ControlPropertyChange);
			
			base.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler (BasePropertyChange);
	}

		#region EventHandling
		
		//Tracker
		private void ControlPropertyChange (object sender, PropertyChangedEventArgs e){
			ItemsHelper.UpdateBaseFromTextControl (this.visualControl,this);
			this.HandlePropertyChanged(e.PropertyName);
		}
		
		private void BasePropertyChange (object sender, PropertyChangedEventArgs e){
			ItemsHelper.UpdateControlFromGraphicBase (this.visualControl,this);
			this.HandlePropertyChanged(e.PropertyName);

		}
		
		private void OnControlChanged (object sender, EventArgs e) {
			base.SuspendLayout();
			ItemsHelper.UpdateBaseFromGraphicControl (this.visualControl,this);
			base.ResumeLayout();
			this.HandlePropertyChanged("OnControlChanged");
		}
		
		private void OnControlSelect(object sender, EventArgs e){
			if (Selected != null)
				Selected(this,e);
		}	
		
		/// <summary>
		/// A Property in ReportItem has changed, inform the Designer
		/// to set the View's 'IsDirtyFlag' to true
		/// </summary>
		
		protected void HandlePropertyChanged(string info) {
			if ( !base.Suspend) {
				if (PropertyChanged != null) {
					PropertyChanged (this,new PropertyChangedEventArgs(info));
				}
			}
		}
		
		#endregion
		
		#region overrides
		public override void Render(SharpReportCore.ReportPageEventArgs e) {
			base.Render(e);
		}
		
		public override void Dispose() {
			base.Dispose();
			this.visualControl.Dispose();
		}
		
		public override string ToString() {
			return this.Name;
		}
		#endregion
		
		#region properties
		public override Size Size {
			get {
				return base.Size;
			}
			set {
				base.Size = value;
				if (this.visualControl != null) {
					this.visualControl.Size = value;
				}
				this.HandlePropertyChanged("Size");
			}
		}
		
		public override Point Location {
			get {
				return base.Location;
			}
			set {
				base.Location = value;
				if (this.visualControl != null) {
					this.visualControl.Location = value;
				}
				this.HandlePropertyChanged("Location");
			}
		}
		
		public override Color BackColor {
			get {
				return base.BackColor;
			}
			set {
				base.BackColor = value;
				if (this.visualControl != null) {
					this.visualControl.BackColor = value;
				}
				this.HandlePropertyChanged("Backcolor");
			}
		}
		
		#endregion
		
		#region SharpReport.Designer.IDesignable interface implementation
		
		[System.Xml.Serialization.XmlIgnoreAttribute]
		[Browsable(false)]
		public ReportObjectControlBase VisualControl {
			get {
				return this.visualControl;
			}
		}
		public new event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler <EventArgs> Selected;

		#endregion
	}
	
}
