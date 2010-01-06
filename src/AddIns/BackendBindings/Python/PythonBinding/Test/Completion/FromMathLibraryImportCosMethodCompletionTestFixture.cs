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

namespace PythonBinding.Tests.Completion
{
	/// <summary>
	/// With dot completion if the from import statement has an a identifier then no
	/// completion items should be returned.
	///
	/// For example pressing '.' after the following should not show any completion items:
	///
	/// "from math import cos"
	/// </summary>
	[TestFixture]
	public class FromMathLibraryImportCosMethodCompletionTestFixture
	{
		PythonImportExpression importExpression;
		PythonImportModuleResolveResult resolveResult;
		MockProjectContent projectContent;
		
		[SetUp]
		public void Init()
		{
			string code = "from math import cos";
			importExpression = new PythonImportExpression(code);
			resolveResult = new PythonImportModuleResolveResult(importExpression);
			
			projectContent = new MockProjectContent();
		}
		
		[Test]
		public void NoCompletionItemsReturned()
		{
			ArrayList items = resolveResult.GetCompletionData(projectContent);
			Assert.AreEqual(0, items.Count);
		}
	}
}
