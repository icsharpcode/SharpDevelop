// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.Editor;
using NUnit.Framework;

namespace ICSharpCode.AvalonEdit.Document
{
	[TestFixture]
	public class ChangeTrackingTest
	{
		[Test]
		public void NoChanges()
		{
			TextDocument document = new TextDocument("initial text");
			ITextSource snapshot1 = document.CreateSnapshot();
			ITextSource snapshot2 = document.CreateSnapshot();
			Assert.AreEqual(0, snapshot1.Version.CompareAge(snapshot2.Version));
			Assert.AreEqual(0, snapshot1.Version.GetChangesTo(snapshot2.Version).Count());
			Assert.AreEqual(document.Text, snapshot1.Text);
			Assert.AreEqual(document.Text, snapshot2.Text);
		}
		
		[Test]
		public void ForwardChanges()
		{
			TextDocument document = new TextDocument("initial text");
			ITextSource snapshot1 = document.CreateSnapshot();
			document.Replace(0, 7, "nw");
			document.Insert(1, "e");
			ITextSource snapshot2 = document.CreateSnapshot();
			Assert.AreEqual(-1, snapshot1.Version.CompareAge(snapshot2.Version));
			TextChangeEventArgs[] arr = snapshot1.Version.GetChangesTo(snapshot2.Version).ToArray();
			Assert.AreEqual(2, arr.Length);
			Assert.AreEqual("nw", arr[0].InsertedText);
			Assert.AreEqual("e", arr[1].InsertedText);
			
			Assert.AreEqual("initial text", snapshot1.Text);
			Assert.AreEqual("new text", snapshot2.Text);
		}
		
		[Test]
		public void BackwardChanges()
		{
			TextDocument document = new TextDocument("initial text");
			ITextSource snapshot1 = document.CreateSnapshot();
			document.Replace(0, 7, "nw");
			document.Insert(1, "e");
			ITextSource snapshot2 = document.CreateSnapshot();
			Assert.AreEqual(1, snapshot2.Version.CompareAge(snapshot1.Version));
			TextChangeEventArgs[] arr = snapshot2.Version.GetChangesTo(snapshot1.Version).ToArray();
			Assert.AreEqual(2, arr.Length);
			Assert.AreEqual("", arr[0].InsertedText);
			Assert.AreEqual("initial", arr[1].InsertedText);
			
			Assert.AreEqual("initial text", snapshot1.Text);
			Assert.AreEqual("new text", snapshot2.Text);
		}
	}
}
