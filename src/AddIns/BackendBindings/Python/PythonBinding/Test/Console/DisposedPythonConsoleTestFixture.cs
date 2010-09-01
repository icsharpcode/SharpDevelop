// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
