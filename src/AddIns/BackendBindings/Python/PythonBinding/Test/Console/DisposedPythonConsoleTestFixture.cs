// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// Tests the disposing of the PythonConsole.
	/// </summary>
	[TestFixture]
	public class DisposedPythonConsoleTestFixture
	{
		[Test]
		public void PythonConsoleImplementsIDisposable()
		{
			PythonConsole console = new PythonConsole(new MockTextEditor(), null);
			Assert.IsNotNull(console as IDisposable);
		}
		
		[Test]
		public void ReadLineReturnsNullWhenConsoleDisposed()
		{
			PythonConsole console = new PythonConsole(new MockTextEditor(), null);
			console.Dispose();
			Assert.IsNull(console.ReadLine(0));
		}
	}
}
