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
	public class FromImportPythonModuleCompletionTestFixture
	{
		PythonImportCompletion completion;
		MockProjectContent projectContent;
		
		[SetUp]
		public void Init()
		{
			ParseInformation parseInfo = new ParseInformation();
			projectContent = new MockProjectContent();
			completion = new PythonImportCompletion(projectContent);
		}
		
		[Test]
		public void FromUnknownLibraryNoCompletionItemsReturned()
		{
			ArrayList items = completion.GetCompletionItemsFromModule("unknown");
			Assert.AreEqual(0, items.Count);
		}
		
		[Test]
		public void FromMathLibraryGetCompletionItemsReturnsPiField()
		{
			ArrayList items = completion.GetCompletionItemsFromModule("math");
			IField field = PythonCompletionItemsHelper.FindFieldFromCollection("pi", items);
			Assert.IsNotNull(field);
		}
	}
}
