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
	[TestFixture]
	public class OneItemCommandLineHistoryTestFixture
	{
		CommandLineHistory history;
		
		[SetUp]
		public void Init()
		{
			history = new CommandLineHistory();
			history.Add("a");
		}
		
		[Test]
		public void Current()
		{
			Assert.AreEqual(null, history.Current);
		}
		
		[Test]
		public void MovePrevious()
		{
			Assert.IsTrue(history.MovePrevious());
		}
		
		[Test]
		public void MovePreviousTwice()
		{
			history.MovePrevious();
			Assert.IsFalse(history.MovePrevious());
		}
		
		[Test]
		public void MoveNextFails()
		{
			Assert.IsFalse(history.MoveNext());
		}
		
		[Test]
		public void CurrentAfterMovePrevious()
		{
			history.MovePrevious();
			Assert.AreEqual("a", history.Current);
		}
		
		[Test]
		public void CurrentAfterMovePreviousTwice()
		{
			history.MovePrevious();
			history.MovePrevious();
			Assert.AreEqual("a", history.Current);
		}		
		
		[Test]
		public void MovePreviousThenBack()
		{
			history.MovePrevious();
			Assert.IsFalse(history.MoveNext());
		}

		[Test]
		public void CurrentAfterMovePreviousThenBack()
		{
			history.MovePrevious();
			history.MoveNext();
			Assert.AreEqual("a", history.Current);
		}
				
		[Test]
		public void CurrentAfterMovePreviousTwiceThenBack()
		{
			history.MovePrevious();
			history.MovePrevious();
			history.MoveNext();
			Assert.AreEqual("a", history.Current);
		}
		
		[Test]
		public void MoveNextTwiceThenBack()
		{
			history.MoveNext();
			history.MoveNext();
			Assert.IsTrue(history.MovePrevious());
		}
		
		[Test]
		public void CurrentAfterMoveNextTwiceThenBack()
		{
			MoveNextTwiceThenBack();
			Assert.AreEqual("a", history.Current);
		}
		
		[Test]
		public void IgnoreSameCommandLineEntered()
		{
			history.Add("a");
			history.MovePrevious();
			Assert.IsFalse(history.MovePrevious());
		}
	}
}
