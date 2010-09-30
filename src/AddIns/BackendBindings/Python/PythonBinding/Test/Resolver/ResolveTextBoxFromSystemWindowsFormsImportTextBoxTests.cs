// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveTextBoxFromSystemWindowsFormsImportTextBoxTests : ResolveTestsBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			MockClass textBoxClass = new MockClass(projectContent, "System.Windows.Forms.TextBox");
			projectContent.SetClassToReturnFromGetClass("System.Windows.Forms.TextBox", textBoxClass);
			
			return new ExpressionResult("TextBox", ExpressionContext.Default);
		}
		
		protected override string GetPythonScript()
		{
			return 
				"from System.Windows.Forms import TextBox\r\n" +
				"TextBox\r\n" +
				"\r\n";
		}
			
		[Test]
		public void ResolveResultIsTypeResolveResult()
		{
			Assert.IsTrue(resolveResult is TypeResolveResult);
		}
		
		[Test]
		public void ResolveResultResolveClassNameIsTextBox()
		{
			Assert.AreEqual("TextBox", TypeResolveResult.ResolvedClass.Name);
		}
		
		TypeResolveResult TypeResolveResult {
			get { return (TypeResolveResult)resolveResult; }
		}
	}
}
