// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;
using NUnit.Framework;
using ICSharpCode.Scripting.Tests.Utils;

namespace ICSharpCode.Scripting.Tests.Console
{
	[TestFixture]
	public class DisposedScriptingConsoleTests : ScriptingConsoleTestsBase
	{
		[Test]
		public void ConsoleImplementsIDisposable()
		{
			base.CreateConsole();
			Assert.IsNotNull(TestableScriptingConsole as IDisposable);
		}
		
		[Test]
		public void ReadLineReturnsNullWhenConsoleDisposed()
		{
			base.CreateConsole();
			TestableScriptingConsole.Dispose();
			Assert.IsNull(TestableScriptingConsole.ReadLine(0));
		}
	}
}
