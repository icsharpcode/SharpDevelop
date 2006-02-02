/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 13.11.2004
 * Time: 22:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;

using SharpReportCore;
using SharpReport.Designer;
using SharpReport.ReportItems;

namespace SharpReport.ReportItems {
	/// <summary>
	/// Description of ReportTextControl.
	/// </summary>
	///<remarks>The <see cref="IDesignable"></see> Interface implementaion is already 
	/// included in the BaseClasses</remarks>
	
	

	public class ReportTextItem : BaseTextItem,SharpReport.Designer.IDesignable{
		
		
		private ReportTextControl visualControl;
		bool initDone;
		
		#region Constructor
		public ReportTextItem() : base(){
			visualControl = new ReportTextControl();

			this.visualControl.Click += new EventHandler(OnControlSelect);
			this.visualControl.VisualControlChanged += new EventHandler (OnControlChanged);
			this.visualControl.BackColorChanged += new EventHandler (OnControlChanged);
			this.visualControl.FontChanged += new EventHandler (OnControlChanged);
			this.visualControl.ForeColorChanged += new EventHandler (OnControlChanged);

			base.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler (BasePropertyChange);
			ItemsHelper.UpdateTextControl (this.visualControl,this);
			this.Text = visualControl.Name;
			GrapFromBase();
			this.initDone = true;
		}
		
		private void GrapFromBase() {
			this.visualControl.SuspendLayout();
			visualControl.StringFormat = base.StandartStringFormat;
			this.visualControl.ResumeLayout();
		}
		
		#endregion
		
		#region overrides
		public override void Dispose() {
			base.Dispose();
			this.visualControl.Dispose();
		}
		
		
		public override string ToString(){
			return this.Name;
		}
		
		#endregion
		
		#region events
		private void BasePropertyChange (object sender, PropertyChangedEventArgs e){
			if (initDone == true) {
				ItemsHelper.UpdateTextBase(this.visualControl,this);
			}
		}
		

		private void OnControlChanged (object sender, EventArgs e) {
			ItemsHelper.UpdateTextControl (this.visualControl,this);
			this.HandlePropertyChanged("OnControlSelected");
		}
		
		public void OnControlSelect(object sender, EventArgs e){
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
		
		public override Font Font {
			get {
				return base.Font;
			}
			set {
				base.Font = value;
				if (this.visualControl != null) {
					this.visualControl.Font = value;
				}
				this.HandlePropertyChanged("Font");
			}
		}
		///<summary>
		/// Holds the text to be displayed
		/// </summary>
		
		public override string Text{
			get {
				return base.Text;
			}
			set {
				base.Text = value;
				if (this.visualControl.Text != value) {
					this.visualControl.Text = value;
					this.visualControl.Refresh();
				}
				this.HandlePropertyChanged("Text");
			}
		}
		
		#region IDesignable
	
		[System.Xml.Serialization.XmlIgnoreAttribute]
		[Browsable(false)]
		public ReportObjectControlBase VisualControl {
			get {
				return visualControl;
			}
		}
	
		public new event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler <EventArgs> Selected;

		#endregion
	}
}
