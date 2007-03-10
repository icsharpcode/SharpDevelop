// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.WpfDesign.Designer
{
	static class SharedInstances
	{
		internal static readonly object BoxedTrue = true;
		internal static readonly object BoxedFalse = false;
		internal static readonly object[] EmptyObjectArray = new object[0];
		internal static readonly DesignItem[] EmptyDesignItemArray = new DesignItem[0];
		
		internal static object Box(bool value)
		{
			return value ? BoxedTrue : BoxedFalse;
		}
	}
}
