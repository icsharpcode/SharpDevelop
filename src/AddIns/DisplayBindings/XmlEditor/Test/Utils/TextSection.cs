// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace XmlEditor.Tests.Utils
{
	public class TextSection
	{
		int offset;
		int length;
		
		public TextSection(int offset, int length)
		{
			this.offset = offset;
			this.length = length;
		}
		
		public override bool Equals(object obj)
		{
			TextSection rhs = obj as TextSection;
			return (rhs.offset == offset) && (rhs.length == length);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
