// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	public class FromSysLibraryImportCompletionItemsTestFixture
	{
		PythonImportCompletion completion;
		MockProjectContent projectContent;
		List<ICompletionEntry> completionItems;
		
		[SetUp]
		public void Init()
		{
			projectContent = new MockProjectContent();
			ParseInformation parseInfo = new ParseInformation(new DefaultCompilationUnit(projectContent));
			completion = new PythonImportCompletion(projectContent);
			completionItems = completion.GetCompletionItemsFromModule("sys");
		}
		
		[Test]
		public void GetCompletionItemsDoesNotReturnPythonHiddenMethods()
		{
			IMethod method = PythonCompletionItemsHelper.FindMethodFromCollection("_getframeImpl", completionItems);
			Assert.IsNull(method);
		}
		
		[Test]
		public void GetCompletionItemsReturns__name__()
		{
			AssertFieldExists("__name__");
		}
		
		void AssertFieldExists(string name)
		{
			IField field = GetFieldFromCompletionItems(name);
			Assert.IsNotNull(field, String.Format("Field '{0}' not found in completion items.", name));
		}
		
		IField GetFieldFromCompletionItems(string name)
		{
			return PythonCompletionItemsHelper.FindFieldFromCollection(name, completionItems);
		}

		[Test]
		public void GetCompletionItemsReturns__package__()
		{
			AssertFieldExists("__package__");
		}
		
		[Test]
		public void GetCompletionItemsReturns__stderr__()
		{
			AssertFieldExists("__stderr__");
		}
		
		[Test]
		public void GetCompletionItemsReturns__stdin__()
		{
			AssertFieldExists("__stdin__");
		}
		
		[Test]
		public void GetCompletionItemsReturns__stdout__()
		{
			AssertFieldExists("__stdout__");
		}

		[Test]
		public void GetCompletionItemsReturnsArgv()
		{
			AssertFieldExists("argv");
		}
		
		[Test]
		public void GetCompletionItemsReturnsBuiltInModuleNames()
		{
			AssertFieldExists("builtin_module_names");
		}

		[Test]
		public void GetCompletionItemsReturnsExecutable()
		{
			AssertFieldExists("executable");
		}
		
		[Test]
		public void GetCompletionItemsReturnsExecPrefix()
		{
			AssertFieldExists("exec_prefix");
		}
		
		[Test]
		public void GetCompletionItemsReturnsExitMethod()
		{
			IMethod method = PythonCompletionItemsHelper.FindMethodFromCollection("exit", completionItems);
			Assert.IsNotNull(method);
		}
		
		[Test]
		public void GetCompletionItemsReturnsFlags()
		{
			AssertFieldExists("flags");
		}

		[Test]
		public void GetCompletionItemsReturnsHexVersion()
		{
			AssertFieldExists("hexversion");
		}
		
		[Test]
		public void GetCompletionItemsReturnsLastType()
		{
			AssertFieldExists("last_type");
		}
		
		[Test]
		public void GetCompletionItemsReturnsLastValue()
		{
			AssertFieldExists("last_value");
		}
		
		[Test]
		public void GetCompletionItemsReturnsLastTraceback()
		{
			AssertFieldExists("last_traceback");
		}
		
		[Test]
		public void GetCompletionItemsReturnsMetaPath()
		{
			AssertFieldExists("meta_path");
		}
		
		[Test]
		public void GetCompletionItemsReturnsModules()
		{
			AssertFieldExists("modules");
		}
		
		[Test]
		public void GetCompletionItemsReturnsPath()
		{
			AssertFieldExists("path");
		}
		
		[Test]
		public void GetCompletionItemsReturnsPathHooks()
		{
			AssertFieldExists("path_hooks");
		}
		
		[Test]
		public void GetCompletionItemsReturnsPathImporterCache()
		{
			AssertFieldExists("path_importer_cache");
		}
		
		[Test]
		public void GetCompletionItemsReturnsPs1()
		{
			AssertFieldExists("ps1");
		}
		
		[Test]
		public void GetCompletionItemsReturnsPs2()
		{
			AssertFieldExists("ps2");
		}
		
		[Test]
		public void GetCompletionItemsReturnsPy3kWarning()
		{
			AssertFieldExists("py3kwarning");
		}
		
		[Test]
		public void GetCompletionItemsReturnsDontWriteByteCode()
		{
			AssertFieldExists("dont_write_byte_code");
		}
		
		[Test]
		public void GetCompletionItemsReturnsStdErr()
		{
			AssertFieldExists("stderr");
		}
		
		[Test]
		public void GetCompletionItemsReturnsStdIn()
		{
			AssertFieldExists("stdin");
		}
		
		[Test]
		public void GetCompletionItemsReturnsStdOut()
		{
			AssertFieldExists("stdout");
		}
		
		[Test]
		public void GetCompletionItemsReturnsVersion()
		{
			AssertFieldExists("version");
		}
		
		[Test]
		public void GetCompletionItemsReturnsVersionInfo()
		{
			AssertFieldExists("version_info");
		}
		
		[Test]
		public void GetCompletionItemsReturnsWarnOptions()
		{
			AssertFieldExists("warnoptions");
		}
	}
}
