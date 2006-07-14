/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 09.07.2006
 * Time: 15:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;

using SharpReport.Designer;
using SharpReportCore;

namespace SharpReport.ReportItems{
	/// <summary>
	/// Description of ReportTableItem.
	/// </summary>
	public class ReportTableItem:TableItem ,IDesignable{
		private ReportTableControl visualControl;
		
			
		#region Constructor
		public ReportTableItem():this (GlobalValues.UnboundName){
			
		}
		
		public ReportTableItem (string tableName):base(tableName) {
			Setup();
		}
		
		private void Setup (){
		    visualControl = new ReportTableControl();
//			ItemsHelper.UpdateBaseFromTextControl (this.visualControl,this);
//			
			this.visualControl.Click += new EventHandler(OnControlSelect);
			this.visualControl.VisualControlChanged += new EventHandler (OnControlChanged);
			this.visualControl.LocationChanged += new EventHandler (OnControlChanged);


//			//Event from Tracker
			this.visualControl.PropertyChanged += new PropertyChangedEventHandler (ControlPropertyChange);
//			
			base.PropertyChanged += new PropertyChangedEventHandler (BasePropertyChange);
//			
			base.Items.Added += OnAdd;
			base.Items.Removed += OnRemove; 
			System.Console.WriteLine("ReporttableItem ctrlName {0}",this.visualControl.Name);
		}
		#endregion
		
		#region List Handling
		private void ChildSelected(object sender, EventArgs e){
			if (Selected != null)
				Selected(sender,e);
		}	
		
		private void ChildPropertyChange (object sender, PropertyChangedEventArgs e){
			if (! base.Suspend) {
				ItemsHelper.UpdateControlFromTextBase (this.visualControl,this);
				this.HandlePropertyChanged(e.PropertyName);
			}
		}
		
		private void OnAdd (object sender, CollectionItemEventArgs<IItemRenderer> e){
			IDesignable des = e.Item as IDesignable;
			if (des != null) {
				this.visualControl.Controls.Add (des.VisualControl);
				des.Selected += ChildSelected;
				des.PropertyChanged += ChildPropertyChange;
			}
		}
		
		private void OnRemove (object sender, CollectionItemEventArgs<IItemRenderer> e){
			
			IDesignable des = e.Item as IDesignable;
			if (des != null) {
				this.visualControl.Controls.Remove(des.VisualControl);
				des.Selected -= ChildSelected;
				this.HandlePropertyChanged("OnChildControlRemoved");
			}
		}
		
		#endregion
		public override string ToString() {
			return this.GetType().Name;
		}
		#region Events from Control
		//Tracker
		
		private void ControlPropertyChange (object sender, PropertyChangedEventArgs e){
			ItemsHelper.UpdateBaseFromTextControl (this.visualControl,this);
			this.HandlePropertyChanged(e.PropertyName);
		}
		
		private void BasePropertyChange (object sender, PropertyChangedEventArgs e){
			ItemsHelper.UpdateControlFromTextBase (this.visualControl,this);
			this.visualControl.DrawBorder = base.DrawBorder;
			this.HandlePropertyChanged(e.PropertyName);
		}
		
		
		private void OnControlChanged (object sender, EventArgs e) {
			this.SuspendLayout();
			ItemsHelper.UpdateBaseFromTextControl (this.visualControl,this);
			
			this.ResumeLayout();
			this.HandlePropertyChanged("OnControlChanged");
		
		}
		
		private void OnAppereanceChanged (object sender, EventArgs e) {
			this.SuspendLayout();
			ItemsHelper.UpdateBaseFromTextControl (this.visualControl,this);
			this.ResumeLayout();
//			UpdateChilds();
			
			this.HandlePropertyChanged("OnControlChanged");
		}
		
		private void OnControlSelect(object sender, EventArgs e){
			if (Selected != null)
				System.Console.WriteLine("fire selected");
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
