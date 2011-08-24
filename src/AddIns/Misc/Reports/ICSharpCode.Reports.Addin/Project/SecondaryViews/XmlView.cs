// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace ICSharpCode.Reports.Addin.SecondaryViews
{
	/// <summary>
	/// Description of the view content
	/// </summary>
	public class XmlView : AbstractSecondaryViewContent
	{
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the view
		/// </summary>
		RichTextBox rtb = new RichTextBox();
		IDesignerGenerator generator;
		
		public XmlView(IDesignerGenerator generator,
		               IViewContent content):base(content)
		{
			if (generator == null) {
				throw new ArgumentNullException("generator");
			}
			if (content == null) {
				throw new ArgumentNullException("content");
			}
			this.generator = generator;
			this.TabPageText = "XmlView";
			rtb.Dock = DockStyle.Fill;
			rtb.BackColor = System.Drawing.SystemColors.Desktop;
			rtb.ForeColor = System.Drawing.Color.White;
		}
		
		#region overrides
		
		protected override void LoadFromPrimary()
		{
			this.rtb.Text = generator.ViewContent.ReportFileContent;
		}
		
		protected override void SaveToPrimary()
		{
//			throw new NotImplementedException();
		}
		
		
		public override object Control {
			get {
				return this.rtb;
			}
		}
		
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public sealed override void Dispose()
		{
			try {
				if (this.rtb != null) {
					this.rtb.Dispose();
				}
			} finally {
				base.Dispose();
			}
		}
		
		#endregion
	}
	
}
