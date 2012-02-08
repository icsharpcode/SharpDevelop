// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
