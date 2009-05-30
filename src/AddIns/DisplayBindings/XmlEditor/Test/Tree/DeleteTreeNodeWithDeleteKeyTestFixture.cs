// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2164 $</version>
// </file>

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
