// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

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
