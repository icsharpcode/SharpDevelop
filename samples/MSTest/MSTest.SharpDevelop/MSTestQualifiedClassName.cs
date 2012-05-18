// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.MSTest
{
	public class MSTestQualifiedClassName
	{
		public MSTestQualifiedClassName(string qualifiedClassName)
		{
			ClassName = GetClassName(qualifiedClassName);
		}
		
		string GetClassName(string qualifiedClassName)
		{
			int index = qualifiedClassName.IndexOf(',');
			if (index > 0) {
				return qualifiedClassName.Substring(0, index);
			}
			return qualifiedClassName;
		}
		
		public string ClassName { get; private set; }
		
		public string GetQualifiedMethodName(string methodName)
		{
			return String.Format("{0}.{1}", ClassName, methodName);
		}
	}
}
