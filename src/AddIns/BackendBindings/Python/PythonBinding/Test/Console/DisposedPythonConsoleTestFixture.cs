// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// Tests the disposing of the PythonConsole.
	/// </summary>
	[TestFixture]
	public class DisposedPythonConsoleTestFixture : PythonConsoleTestsBase
	{
		[Test]
		public void PythonConsoleImplementsIDisposable()
		{
			base.CreatePythonConsole();
			Assert.IsNotNull(TestablePythonConsole as IDisposable);
		}
		
		[Test]
		public void ReadLineReturnsNullWhenConsoleDisposed()
		{
			base.CreatePythonConsole();
			TestablePythonConsole.Dispose();
			Assert.IsNull(TestablePythonConsole.ReadLine(0));
		}
	}
}
