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
using ICSharpCode.SharpDevelop.Dom.CSharp;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveTextBoxFromSystemWindowsFormsImportedAsMyTextBoxTestFixture : ResolveTestFixtureBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			MockClass textBoxClass = new MockClass(projectContent, "System.Windows.Forms.TextBox");
			projectContent.ClassToReturnFromGetClass = textBoxClass;
			projectContent.ClassNameForGetClass = "System.Windows.Forms.TextBox";
			
			return new ExpressionResult("MyTextBox", ExpressionContext.Default);
		}
		
		protected override string GetPythonScript()
		{
			return 
				"from System.Windows.Forms import TextBox as MyTextBox\r\n" +
				"MyTextBox\r\n" +
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
