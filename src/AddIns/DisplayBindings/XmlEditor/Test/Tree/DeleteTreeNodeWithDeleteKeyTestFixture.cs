// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests that the delete key event is fired so we can 
	/// </summary>
	[TestFixture]
	public class DeleteTreeNodeWithDeleteKeyTestFixture
	{
		bool deleteKeyPressEventFired;
		
		[Test]
		public void DeleteKeyPressed()
		{
			using (DerivedXmlTreeViewControl treeView = new DerivedXmlTreeViewControl()) {
				treeView.DeleteKeyPressed += TreeViewDeleteKeyPressed;
				treeView.CallProcessCmdKey(Keys.Delete);
			}
			Assert.IsTrue(deleteKeyPressEventFired);
		}
		
		void TreeViewDeleteKeyPressed(object sender, EventArgs e)
		{
			deleteKeyPressEventFired = true;
		}
	}
}
