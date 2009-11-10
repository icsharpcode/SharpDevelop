// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
