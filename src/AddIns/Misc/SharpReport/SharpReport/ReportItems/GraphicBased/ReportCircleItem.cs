/*
 * Created by SharpDevelop.
 * User: Fabio
 * Date: 09/10/2004
 * Time: 21.43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Drawing;

using SharpReportCore;
using SharpReport.Designer;


namespace SharpReport.ReportItems{
	
	public class ReportCircleItem : BaseCircleItem,SharpReport.Designer.IDesignable{	
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
		
		private bool initDone;
		
		
		public ReportCircleItem() : base(){
			visualControl = new ReportCircleControl();
			this.visualControl.Click += new EventHandler(OnControlSelect);
			this.visualControl.VisualControlChanged += new EventHandler (OnControlChanged);
			this.visualControl.BackColorChanged += new EventHandler (OnControlChanged);
			this.visualControl.FontChanged += new EventHandler (OnControlChanged);
			this.visualControl.ForeColorChanged += new EventHandler (OnControlChanged);
	
			base.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler (BasePropertyChange);
			ItemsHelper.UpdateGraphicControl (this.visualControl,this);
			this.initDone = true;
		}

		#region EventHandling
		
		private void BasePropertyChange (object sender, PropertyChangedEventArgs e){
			if (initDone == true) {
				ItemsHelper.UpdateGraphicBase (this.visualControl,this);
			}
		}
		
		private void OnControlChanged (object sender, EventArgs e) {
			ItemsHelper.UpdateGraphicControl (this.visualControl,this);
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
