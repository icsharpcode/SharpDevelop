// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
