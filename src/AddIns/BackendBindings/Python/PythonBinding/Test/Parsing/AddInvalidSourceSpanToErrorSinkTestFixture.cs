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
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using NUnit.Framework;
using PythonBinding.Tests;

namespace PythonBinding.Tests.Parsing
{
	/// <summary>
	/// Tests that the python compiler sink does not throw an exception when an invalid SourceLocation is used.
	/// </summary>
	[TestFixture]
	public class AddInvalidSourceSpanToErrorSinkTestFixture
	{		
		[Test]
		public void AddInvalidSourceSpan()
		{
			IronPython.Hosting.Python.CreateEngine();
			PythonCompilerSink sink = new PythonCompilerSink();
			SourceUnit source = DefaultContext.DefaultPythonContext.CreateSourceUnit(NullTextContentProvider.Null, @"D:\test.py", SourceCodeKind.InteractiveCode);
			sink.Add(source, "Test", SourceSpan.Invalid, 1000, Severity.FatalError);
		}
	}
}
