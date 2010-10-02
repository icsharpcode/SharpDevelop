// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Completion
{
	[TestFixture]
	public class PythonCodeCompletionBindingTests
	{
		TestablePythonCodeCompletionBinding completionBinding;
		
		void CreatePythonCodeCompletionBinding()
		{
			completionBinding = new TestablePythonCodeCompletionBinding();
		}
		
		[Test]
		public void InsightHandler_CreateNewCodecompletionBindingInstance_IsSetToPythonInsightWindowHandlerInstance()
		{
			CreatePythonCodeCompletionBinding();
			PythonInsightWindowHandler handler = completionBinding.PythonInsightWindowHandler;
			Assert.IsNotNull(handler);
		}
	}
}
