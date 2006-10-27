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
using System.Drawing;

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
			base.Name = this.visualControl.Name;
		}
		
		private void Setup (){
		    visualControl = new ReportTableControl();

			this.visualControl.Click += new EventHandler(OnControlSelect);
			this.visualControl.VisualControlChanged += new EventHandler (OnControlChanged);
			this.visualControl.LocationChanged += new EventHandler (OnControlChanged);


//			//Event from Tracker
			this.visualControl.PropertyChanged += new PropertyChangedEventHandler (ControlPropertyChange);			
			base.PropertyChanged += new PropertyChangedEventHandler (BasePropertyChange);
//			
			base.Items.Added += OnAdd;
			base.Items.Removed += OnRemove; 
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
			System.Console.WriteLine("TableItem:OnAdd");
			IDesignable des = e.Item as IDesignable;
			
			if (des != null) {
				this.visualControl.Controls.Add (des.VisualControl);
				des.Selected += ChildSelected;
				des.PropertyChanged += ChildPropertyChange;
				System.Console.WriteLine("\t Added <{0}>",e.Item.Name);
			}
		}
		
		private void OnRemove (object sender, CollectionItemEventArgs<IItemRenderer> e){
			System.Console.WriteLine("TableItem:OnRemove");
			IDesignable des = e.Item as IDesignable;
			System.Console.WriteLine("\t <{0}>",des.Name);
			if (des != null) {
				this.visualControl.Controls.Remove(des.VisualControl);
				des.Selected -= ChildSelected;
				this.HandlePropertyChanged("OnChildControlRemoved");
			}
		}
		
		#endregion
		
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
		
		#region overrides
		
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
		
		public override string ToString(){
			return this.GetType().Name;
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
