/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 01.03.2006
 * Time: 14:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.ComponentModel;

using SharpReport.Designer;
using SharpReportCore;

namespace SharpReport.ReportItems
{
	
	/// <summary>
	/// Description of ReportTableItem.
	/// </summary>
	public class ReportRowItem : RowItem ,IDesignable{
		private ReportRowControl visualControl;

		
		#region Constructor
		public ReportRowItem():this (GlobalValues.UnboundName){
		}
		
		public ReportRowItem (string tableName):base(tableName) {
			Setup();
		}
		
		#endregion
		
		#region Setup
		private void Setup(){
			
			visualControl = new ReportRowControl();
			ItemsHelper.UpdateBaseFromTextControl (this.visualControl,this);
			
			this.visualControl.Click += new EventHandler(OnControlSelect);
			this.visualControl.VisualControlChanged += new EventHandler (OnControlChanged);
			this.visualControl.FontChanged += new EventHandler (OnControlChanged);
			this.visualControl.ForeColorChanged += new EventHandler (OnControlChanged);
			this.visualControl.BackColorChanged += new EventHandler (OnAppereanceChanged);
			
			base.PropertyChanged += new PropertyChangedEventHandler (BasePropertyChange);
			
			base.Items.Added += OnAdd;
			base.Items.Removed += OnRemove;
		}
		#endregion
		
		
		#region Events for Childs
		private void ChildSelected(object sender, EventArgs e){
			if (Selected != null)
				Selected(sender,e);
		}	
		
		private void OnChildControlChanged (object sender, EventArgs e) {
			ItemsHelper.UpdateBaseFromTextControl (this.visualControl,this);
			this.HandlePropertyChanged("OnChildControlChanged");
		}
		
		private void ChildPropertyChange (object sender, PropertyChangedEventArgs e){
			if (! base.Suspend) {
				ItemsHelper.UpdateControlFromTextBase (this.visualControl,this);
				this.HandlePropertyChanged(e.PropertyName);
			}
		}
		
		#endregion
		
		private void UpdateChilds () {
			foreach (BaseReportItem br in this.Items) {
				br.BackColor = this.BackColor;
				IDesignable des = br as IDesignable;
				if (des != null) {
					des.VisualControl.BackColor = this.BackColor;
				}
			}
		}
		
		#region EventHandling for this Class
		
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
			UpdateChilds();
			
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
		
	/*
		#region IDisposable
		public override void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~ReportRowItem()
		{
			Dispose(false);
		}
		
		protected override void Dispose(bool disposing){
			try {
				if (disposing) {
					
				}
			} finally {
				if (this.visualControl != null) {
					this.visualControl.Dispose();
					this.visualControl = null;
				}
				base.Dispose();
			}
		}
		#endregion
*/
	}
}
