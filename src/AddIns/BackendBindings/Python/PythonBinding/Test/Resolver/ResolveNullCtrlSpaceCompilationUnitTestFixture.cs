// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveNullCtrlSpaceCompilationUnitTestFixture
	{
		/// <summary>
		/// Tests that the resolver handles the compilation units
		/// being null.
		/// </summary>
		[Test]
		public void NullCompilationUnit()
		{
			PythonResolver resolver = new PythonResolver();
			ParseInformation parseInfo = new ParseInformation();
			MockProjectContent mockProjectContent = new MockProjectContent();
			ArrayList results = resolver.CtrlSpace(0, 0, parseInfo, String.Empty, ExpressionContext.Namespace);			
			Assert.AreEqual(0, results.Count);
		}
	}
}
