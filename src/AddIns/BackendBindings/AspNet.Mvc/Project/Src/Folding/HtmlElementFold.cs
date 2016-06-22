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
using ICSharpCode.AvalonEdit.Folding;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class HtmlElementFold : NewFolding
	{
		string elementName = String.Empty;
		
		public string ElementName {
			get { return elementName; }
			set {
				elementName = value;
				UpdateFoldName();
			}
		}
		
		void UpdateFoldName()
		{
			Name = String.Format("<{0}>", elementName);
		}
		
		public int Line { get; set; }
		
		public override bool Equals(object obj)
		{
			var rhs = obj as HtmlElementFold;
			if (rhs != null) {
				return
					(elementName == rhs.ElementName) &&
					(StartOffset == rhs.StartOffset) &&
					(EndOffset == rhs.EndOffset);
			}
			return false;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override string ToString()
		{
			return String.Format(
				"[HtmlElementFold Name='{0}', StartOffset={1}, EndOffset={2}]",
				Name,
				StartOffset,
				EndOffset);
		}
	}
}
