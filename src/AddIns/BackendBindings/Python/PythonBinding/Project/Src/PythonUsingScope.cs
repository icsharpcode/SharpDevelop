// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonUsingScope : DefaultUsingScope
	{
		public PythonUsingScope(string fileName)
		{
			NamespaceName = Path.GetFileNameWithoutExtension(fileName);
			Parent = new DefaultUsingScope();
		}
	}
}
