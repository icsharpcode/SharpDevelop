// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
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
