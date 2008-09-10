// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Mock implementation of the IClass interface
	/// </summary>
	public class MockClass : DefaultClass
	{
		// derive from real DefaultClass. The resolver is free to access any information from the class,
		// and I don't want to have to adjust the mock whenever something in SharpDevelop.Dom changes.
		public MockClass(ICompilationUnit cu, string name)
			: base(cu, name)
		{
		}
		
		public MockClass(IProjectContent pc, string name)
			: base(new DefaultCompilationUnit(pc), name)
		{
		}
	}
}
