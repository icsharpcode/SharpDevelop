// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Completion
{
	[TestFixture]
	public class ImportEmptyNamespaceCompletionTestFixture
	{
		List<ICompletionEntry> completionItems;
		MockProjectContent projectContent;
		
		[SetUp]
		public void Init()
		{
			projectContent = new MockProjectContent();
			ParseInformation parseInfo = new ParseInformation(new DefaultCompilationUnit(projectContent));
			
			PythonImportCompletion completion = new PythonImportCompletion(projectContent);
			completionItems = completion.GetCompletionItems(String.Empty);
		}
		
		[Test]
		public void StandardMathPythonModuleIsAddedToCompletionItems()
		{
			Assert.Contains(new NamespaceEntry("math"), completionItems);
		}
	}
}
