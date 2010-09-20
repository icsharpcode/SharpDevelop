// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using IronPython.Compiler.Ast;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that a local variable can be deserialized. Local variables are used when adding
	/// ListViewItems to a ListView.
	/// </summary>
	[TestFixture]
	public class DeserializeLocalVariableTestFixture
	{
		string pythonCode = "self.Items.AddRange(System.Array[System.Windows.Forms.ListViewItem]([listViewItem1]))";
		
		MockDesignerLoaderHost mockDesignerLoaderHost;
		MockTypeResolutionService typeResolutionService;
		MockComponentCreator componentCreator;
		ListViewItem listViewItem1;
		object deserializedObject;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			componentCreator = new MockComponentCreator();
			listViewItem1 = (ListViewItem)componentCreator.CreateInstance(typeof(ListViewItem), new object[0], "listViewItem1", false);

			CallExpression callExpression = PythonParserHelper.GetCallExpression(pythonCode);
			
			mockDesignerLoaderHost = new MockDesignerLoaderHost();
			typeResolutionService = mockDesignerLoaderHost.TypeResolutionService;
			PythonCodeDeserializer deserializer = new PythonCodeDeserializer(componentCreator);
			deserializedObject = deserializer.Deserialize(callExpression.Args[0].Expression);
		}
		
		[Test]
		public void ListViewItemDeserialized()
		{
			ListViewItem[] array = (ListViewItem[])deserializedObject;
			Assert.AreSame(listViewItem1, array[0]);
		}
		
		[Test]
		public void ListViewItemIsNotNull()
		{
			Assert.IsNotNull(deserializedObject);
		}
	}
}
