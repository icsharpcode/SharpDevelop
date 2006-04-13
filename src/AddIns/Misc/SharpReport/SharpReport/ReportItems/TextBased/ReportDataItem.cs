/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 13.11.2004
 * Time: 22:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.ComponentModel;

using SharpReportCore;
using SharpReport.Designer;

namespace SharpReport.ReportItems{
	/// <summary>
	/// This class reads a Column from a DataSource
	/// </summary>
	
	public class ReportDataItem : BaseDataItem ,IDesignable{
								
		
		private ReportDbTextControl visualControl;
//		bool initDone;
		
		#region Constructors
		
		public ReportDataItem() : this(String.Empty){
		}
		
		public ReportDataItem(string columnName):base(columnName){
			Setup();
		}
		#endregion
		
		#region SetUp
		private void Setup(){
			visualControl = new ReportDbTextControl();
			
			this.visualControl.Text = base.ColumnName;
			visualControl.StringFormat = base.StandartStringFormat;
			this.Text = base.ColumnName;
			
			ItemsHelper.UpdateBaseFromTextControl (this.visualControl,this);

			this.visualControl.Click += new EventHandler(OnControlSelect);
			this.visualControl.VisualControlChanged += new EventHandler (OnControlChanged);
			this.visualControl.BackColorChanged += new EventHandler (OnControlChanged);
			this.visualControl.FontChanged += new EventHandler (OnControlChanged);
			this.visualControl.ForeColorChanged += new EventHandler (OnControlChanged);

			base.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler (BasePropertyChange);
		}
		
		#endregion
		
		#region overrides
		public override string ToString(){
			return this.Name;
		}
		
		#endregion
		
		#region events's
		private void BasePropertyChange (object sender, PropertyChangedEventArgs e){
			ItemsHelper.UpdateControlFromTextBase(this.visualControl,this);
			this.HandlePropertyChanged(e.PropertyName);
		}
		

		private void OnControlChanged (object sender, EventArgs e) {
			base.SuspendLayout();
			ItemsHelper.UpdateBaseFromTextControl (this.visualControl,this);
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
		

		
		#region Property's
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
		
		/*
		#region IDisposable
		public override void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~ReportDataItem()
		{
			Dispose(false);
		}
		
		protected override void Dispose(bool disposing){
			try {
				if (disposing) {
				// Free other state (managed objects).
				if (this.visualControl != null) {
					this.visualControl.Dispose();
				}
			}
			} finally {
				base.Dispose();
			}
		}
		#endregion
		*/
	}
}
