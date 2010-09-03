// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
