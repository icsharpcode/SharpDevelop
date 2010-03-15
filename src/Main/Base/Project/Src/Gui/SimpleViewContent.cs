// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// A simple view content that does not use any files and simply displays a fixed control.
	/// </summary>
	public class SimpleViewContent : AbstractViewContent
	{
		readonly object content;
		
		public override object Control {
			get {
				return content;
			}
		}
		
		public SimpleViewContent(object content)
		{
			if (content == null)
				throw new ArgumentNullException("content");
			this.content = content;
		}
		
		// make this method public
		/// <inheritdoc/>
		public new void SetLocalizedTitle(string text)
		{
			base.SetLocalizedTitle("${res:StartPage.StartPageContentName}");
		}
		
		public new string TitleName {
			get { return base.TitleName; }
			set { base.TitleName = value; }
		}
	}
}
