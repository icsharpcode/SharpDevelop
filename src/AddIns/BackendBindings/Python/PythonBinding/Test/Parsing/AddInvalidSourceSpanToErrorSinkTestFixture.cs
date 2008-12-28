// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
