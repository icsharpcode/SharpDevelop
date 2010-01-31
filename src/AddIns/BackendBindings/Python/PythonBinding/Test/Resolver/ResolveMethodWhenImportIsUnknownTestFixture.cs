// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveMethodWhenImportIsUnknownTestFixture : ResolveTestFixtureBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			return new ExpressionResult("unknown.methodcall", ExpressionContext.Default);
		}
		
		protected override string GetPythonScript()
		{
			return 
				"from unknown import methodcall\r\n" +
				"unknown.methodcall\r\n" +
				"\r\n";
		}
		
		[Test]
		public void ResolveResultIsNull()
		{
			Assert.IsNull(resolveResult);
		}
	}
}
