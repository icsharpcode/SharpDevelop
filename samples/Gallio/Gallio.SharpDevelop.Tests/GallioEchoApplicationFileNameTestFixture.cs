// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.UnitTesting;
using Gallio.SharpDevelop;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace Gallio.SharpDevelop.Tests
{
	[TestFixture]
	public class GallioEchoApplicationFileNameTestFixture
	{
		GallioEchoConsoleApplication app;
		string gallioEchoConsoleFileName;
		MockAddInTree addinTree;
		string addinTreePath;
		
		[SetUp]
		public void Init()
		{
			MockCSharpProject project = new MockCSharpProject();
			SelectedTests tests = new SelectedTests(project);
			
			gallioEchoConsoleFileName = @"d:\gallio\bin\Gallio.Echo.exe";
			addinTreePath = GallioEchoConsoleApplicationFactory.AddInTreePath;
			
			addinTree = new MockAddInTree();
			List<string> items = new List<string>();
			items.Add(gallioEchoConsoleFileName);
			
			addinTree.AddItems<string>(addinTreePath, items);
			
			GallioEchoConsoleApplicationFactory factory = new GallioEchoConsoleApplicationFactory(addinTree);
			app = factory.Create(tests);
		}
		
		[Test]
		public void AddInTreeBuildItemsReturnsGallioEchoConsoleFileName()
		{
			List<string> items = addinTree.BuildItems<string>(addinTreePath, null);
			
			List<string> expectedItems = new List<string>();
			expectedItems.Add(gallioEchoConsoleFileName);

			Assert.AreEqual(expectedItems.ToArray(), items.ToArray());
		}
		
		[Test]
		public void ApplicationFileNameIsTakenFromAddInTree()
		{
			string expectedFileName = gallioEchoConsoleFileName;
			Assert.AreEqual(expectedFileName, app.FileName);
		}
	}
}
