// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
