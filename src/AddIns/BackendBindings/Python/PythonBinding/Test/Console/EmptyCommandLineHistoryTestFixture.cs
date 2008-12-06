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
