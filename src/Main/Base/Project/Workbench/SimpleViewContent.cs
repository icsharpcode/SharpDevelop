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

namespace ICSharpCode.SharpDevelop.Workbench
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
