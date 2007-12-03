// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace PythonBinding.Tests.Resolver
{
	/// <summary>
	/// Tests that we can resolve System.Console even when
	/// there is no class. This allows us to get code completion
	/// in a simple python script without any classes being required.
	/// </summary>
	[TestFixture]
	public class ResolveSystemConsoleOutsideClassTestFixture : ResolveSystemConsoleTestFixture
	{
		protected override string GetPythonScript()
		{
			return "import System\r\n" +
					"System.Console\r\n";
		}
		
		/// <summary>
		/// Creates a compilation unit with no classes.
		/// </summary>
		protected override ICompilationUnit CreateCompilationUnit(IProjectContent projectContent)
		{
			return new DefaultCompilationUnit(projectContent);
		}
	}
}
