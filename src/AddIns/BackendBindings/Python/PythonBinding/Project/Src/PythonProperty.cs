// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using Microsoft.Scripting;

namespace ICSharpCode.PythonBinding
{
	public class PythonProperty : DefaultProperty
	{
		public PythonProperty(IClass declaringType, string name, SourceLocation location)
			: base(declaringType, name)
		{
			GetPropertyRegion(location);
			declaringType.Properties.Add(this);
		}
		
		void GetPropertyRegion(SourceLocation location)
		{
			int line = location.Line;
			int column = location.Column;
			Region = new DomRegion(line, column, line, column);
		}
	}
}
