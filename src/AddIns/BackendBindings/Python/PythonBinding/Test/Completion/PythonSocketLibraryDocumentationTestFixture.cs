// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Modules;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Completion
{
	/// <summary>
	/// Tests that the documentation is taken from the Documentation attribute for the method/field in the
	/// python module classes.
	/// </summary>
	[TestFixture]
	public class PythonSocketLibraryDocumentationTestFixture
	{
		PythonModuleCompletionItems completionItems;
		
		[SetUp]
		public void Init()
		{
			PythonStandardModuleType moduleType = new PythonStandardModuleType(typeof(PythonSocket), "socket");
			completionItems = PythonModuleCompletionItemsFactory.Create(moduleType);
		}
		
		[Test]
		public void DocumentationForCreateConnectionMethodTakenFromDocumentationAttribute()
		{
			string doc =
				"Connect to *address* and return the socket object.\n" +
				"\n" +
				"Convenience function.  Connect to *address* (a 2-tuple ``(host,\nport)``) and return the socket object.  Passing the optional\n" +
				"*timeout* parameter will set the timeout on the socket instance\nbefore attempting to connect.  If no *timeout* is supplied, the\n" +
				"global default timeout setting returned by :func:`getdefaulttimeout`\n" +
				"is used.\n";
			
			IMethod method = PythonCompletionItemsHelper.FindMethodFromCollection("create_connection", completionItems);
			Assert.AreEqual(doc, method.Documentation);
		}
	}
}
