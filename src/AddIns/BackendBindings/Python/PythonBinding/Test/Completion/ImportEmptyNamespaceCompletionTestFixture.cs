// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Completion
{
	[TestFixture]
	public class ImportEmptyNamespaceCompletionTestFixture
	{
		ArrayList completionItems;
		MockProjectContent projectContent;
		
		[SetUp]
		public void Init()
		{
			ParseInformation parseInfo = new ParseInformation();
			projectContent = new MockProjectContent();
			
			PythonImportCompletion completion = new PythonImportCompletion(projectContent);
			completionItems = completion.GetCompletionItems(String.Empty);
		}
		
		[Test]
		public void StandardMathPythonModuleIsAddedToCompletionItems()
		{
			Assert.Contains("math", completionItems);
		}
	}
}
