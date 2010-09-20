// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	/// <summary>
	/// Tests the CommandLineHistory class.
	/// </summary>
	[TestFixture]
	public class EmptyCommandLineHistoryTestFixture
	{
		CommandLineHistory history;
		
		[SetUp]
		public void Init()
		{
			history = new CommandLineHistory();
		}

		[Test]
		public void CurrentCommandLineIsNull()
		{
			Assert.IsNull(history.Current);
		}
		
		[Test]
		public void MoveNextReturnsFalse()
		{
			Assert.IsFalse(history.MoveNext());
		}
		
		[Test]
		public void MovePreviousReturnsFalse()
		{
			Assert.IsFalse(history.MovePrevious());
		}
	}
}
