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
	public class DisposedRubyConsoleTestFixture : RubyConsoleTestsBase
	{
		[Test]
		public void RubyConsoleImplementsIDisposable()
		{
			base.CreateRubyConsole();
			Assert.IsNotNull(TestableRubyConsole as IDisposable);
		}
		
		[Test]
		public void ReadLineReturnsNullWhenConsoleDisposed()
		{
			base.CreateRubyConsole();
			TestableRubyConsole.Dispose();
			Assert.IsNull(TestableRubyConsole.ReadLine(0));
		}
	}
}
