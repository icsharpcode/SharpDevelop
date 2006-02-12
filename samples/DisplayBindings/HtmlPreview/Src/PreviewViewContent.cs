/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 19.10.2005
 * Time: 15:53
 */

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace HtmlPreview
{
	/// <summary>
	/// Description of PreviewViewContent.
	/// </summary>
	public class PreviewViewContent : AbstractSecondaryViewContent
	{
		public PreviewViewContent()
		{
		}
		
		WebBrowser browser = new WebBrowser();
		
		#region ICSharpCode.SharpDevelop.Gui.AbstractSecondaryViewContent interface implementation
		public override Control Control {
			get {
				return browser;
			}
		}
		
		public override string TabPageText {
			get {
				return "Preview";
			}
		}
		
		public override void Deselected() {
			browser.DocumentText = "";
			base.Deselected();
		}
		public override void Selected() {
			IViewContent viewContent = this.WorkbenchWindow.ViewContent;
			IEditable editable = (IEditable)viewContent;
			browser.DocumentText = editable.Text;
			base.Selected();
		}
		
		#endregion
		
	}
}
