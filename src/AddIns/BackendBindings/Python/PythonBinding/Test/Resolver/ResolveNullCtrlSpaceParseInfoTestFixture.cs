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
	public class ResolveNullCtrlSpaceParseInfoTestFixture
	{
		/// <summary>
		/// Tests that the resolver handles the parse info being null
		/// </summary>
		[Test]
		public void ResolveCtrlSpaceDoesNotThrowExceptionWhenNullParseInfoIsNull()
		{
			PythonResolver resolver = new PythonResolver();
			ArrayList results = resolver.CtrlSpace(0, 0, null, "abc", ExpressionContext.Namespace);			
			Assert.AreEqual(0, results.Count);
		}
	}
}
