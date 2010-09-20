// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			base.SetLocalizedTitle(text);
		}
		
		public new string TitleName {
			get { return base.TitleName; }
			set { base.TitleName = value; } // make setter public
		}
	}
}
