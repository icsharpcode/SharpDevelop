/*
 * Created by SharpDevelop.
 * User: Fabio
 * Date: 10/10/2004
 * Time: 14.55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace SharpReport.Designer
{
	/// <summary>
	/// All report objects such as Sections and Items
	/// should derive from ReportObjectBase so they can
	/// show theirs properties in the property window
	/// and can have some common members managed easier.
	/// </summary>

	public abstract class ReportObjectControlBase : UserControl	
	{
		private StringAlignment stringAlignment = StringAlignment.Near;
		private StringFormat stringFormat;
	
		/// <summary>
		/// This Event fire's when anything in the VisualControl Design changes like
		/// Size,Location,BackColor etc.
		/// </summary>
		public event EventHandler VisualControlChanged;
		
		#region Constructor
		internal ReportObjectControlBase(){
			InitializeComponent();
			this.SetStyle(ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.ResizeRedraw,
			              true);
			this.UpdateStyles();
		}
		#endregion
		
		protected void OnControlChanged () {
			if ( VisualControlChanged != null ) {
				VisualControlChanged (this,EventArgs.Empty);
			}
		}
	
		#region Properties
		// Some report object such as Sections
		// does not coincide with the "rendered object"
		// so "this" identifies the whole report object
		// and "Body" identifies the rendered control
		public virtual Control Body
		{
			get { 
				return this; 
			}
		}		
		
		/// <summary>
		/// Use the same StringFormat we use in printing the report
		/// </summary>
		public StringFormat StringFormat {
			get {
				return stringFormat;
			}
			set {
				stringFormat = value;
			}
		}
		
		
		/// <summary>
		/// use the same StringAlignment we use in printing
		/// </summary>
		public StringAlignment StringAlignment {
			get {
				return stringAlignment;
			}
			set {
				stringAlignment = value;
				this.Invalidate();
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
			// 
			// ReportObjectBase
			// 
			this.Name = "ReportObjectBase";
			this.Size = new System.Drawing.Size(120, 56);
		}
		#endregion
	
	}
}
