// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Console
{
	/// <summary>
	/// Tests the disposing of the RubyConsole.
	/// </summary>
	[TestFixture]
	public class DisposedRubyConsoleTestFixture
	{
		[Test]
		public void RubyConsoleImplementsIDisposable()
		{
			RubyConsole console = new RubyConsole(new MockConsoleTextEditor(), null);
			Assert.IsNotNull(console as IDisposable);
		}
		
		[Test]
		public void ReadLineReturnsNullWhenConsoleDisposed()
		{
			RubyConsole console = new RubyConsole(new MockConsoleTextEditor(), null);
			console.Dispose();
			Assert.IsNull(console.ReadLine(0));
		}
	}
}
