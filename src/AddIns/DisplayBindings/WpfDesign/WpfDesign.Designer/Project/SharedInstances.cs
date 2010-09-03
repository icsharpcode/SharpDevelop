// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
