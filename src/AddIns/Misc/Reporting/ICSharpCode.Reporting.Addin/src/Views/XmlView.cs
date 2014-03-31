/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 17.03.2014
 * Time: 20:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.Reporting.Addin.DesignerBinding;


namespace ICSharpCode.Reporting.Addin.Views
{
	/// <summary>
	/// Description of XmlView.
	/// </summary>
	class XmlView : AbstractSecondaryViewContent
	{
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the view
		/// </summary>
		RichTextBox richTextBox = new RichTextBox();
		IDesignerGenerator generator;
		
		public XmlView(IDesignerGenerator generator,IViewContent content):base(content){
			if (generator == null) {
				throw new ArgumentNullException("generator");
			}
			if (content == null) {
				throw new ArgumentNullException("content");
			}
			this.generator = generator;
			this.TabPageText = "XmlView";
			richTextBox.Dock = DockStyle.Fill;
			richTextBox.BackColor = System.Drawing.SystemColors.Desktop;
			richTextBox.ForeColor = System.Drawing.Color.White;
		}
		
		#region overrides
		
		protected override void LoadFromPrimary()
		{
			richTextBox.Text = generator.ViewContent.ReportFileContent;
		}
		
		protected override void SaveToPrimary()
		{
//			throw new NotImplementedException();
		}
		
		
		public override object Control {
			get {
				return richTextBox;
			}
		}
		
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public sealed override void Dispose()
		{
			try {
				if (this.richTextBox != null) {
					this.richTextBox.Dispose();
				}
			} finally {
				base.Dispose();
			}
		}
		
		#endregion
	}
}
