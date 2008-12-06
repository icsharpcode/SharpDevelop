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
	public class CommandLineHistoryTestFixture
	{
		CommandLineHistory history;
		
		[SetUp]
		public void Init()
		{
			history = new CommandLineHistory();
			history.Add("a");
			history.Add("b");
			history.Add("c");
		}
		
		[Test]
		public void LastCommandLineIsNull()
		{
			Assert.IsNull(history.Current);
		}
		
		[Test]
		public void MovePreviousOnce()
		{
			Assert.IsTrue(history.MovePrevious());
		}
		
		[Test]
		public void CurrentAfterMovePrevious()
		{
			history.MovePrevious();
			Assert.AreEqual("c", history.Current);
		}
		
		[Test]
		public void AddLineAfterMovePrevious()
		{
			history.MovePrevious();
			history.MovePrevious();
			history.Add("d");
			
			Assert.IsNull(history.Current);
		}
		
		[Test]
		public void EmptyLineIgnored()
		{
			history.Add(String.Empty);
			history.MovePrevious();
			Assert.AreEqual("c", history.Current);
		}
		
		/// <summary>
		/// After trying to move beyond the end of the list moving previous should not show the last
		/// item again.
		/// </summary>
		[Test]
		public void MovePreviousThenNextTwiceThenPreviousAgain()
		{
			history.MovePrevious();
			history.MoveNext();
			history.MoveNext();
			history.MovePrevious();
			Assert.AreEqual("b", history.Current);
		}
	}
}
