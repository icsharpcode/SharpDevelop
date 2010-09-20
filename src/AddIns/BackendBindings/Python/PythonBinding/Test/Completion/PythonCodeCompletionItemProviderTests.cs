// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Completion
{
	[TestFixture]
	public class PythonCodeCompletionItemProviderTests
	{
		[Test]
		public void CreateCompletionItemList_OverriddenInPythonCodeCompletionItemProvider_ReturnsPythonCompletionItemList()
		{
			TestablePythonCodeCompletionItemProvider provider = new TestablePythonCodeCompletionItemProvider();
			PythonCompletionItemList list = provider.CallBaseCreateCompletionItemList() as PythonCompletionItemList;
			
			Assert.IsNotNull(list);
		}
	}
}
