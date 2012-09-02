// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AvalonEdit.Highlighting.Xshd
{
	/// <summary>
	/// A property in an Xshd file.
	/// </summary>
	[Serializable]
	public class XshdProperty : XshdElement
	{
		/// <summary>
		/// Gets/sets the name.
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// Gets/sets the value.
		/// </summary>
		public string Value { get; set; }
		
		/// <summary>
		/// Creates a new XshdColor instance.
		/// </summary>
		public XshdProperty()
		{
		}
		
		/// <inheritdoc/>
		public override object AcceptVisitor(IXshdVisitor visitor)
		{
			return null;
//			return visitor.VisitProperty(this);
		}
	}
}
