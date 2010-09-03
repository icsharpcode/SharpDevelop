// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using IronRuby.Compiler.Ast;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests that a local variable can be deserialized. Local variables are used when adding
	/// ListViewItems to a ListView.
	/// </summary>
	[TestFixture]
	public class DeserializeLocalVariableTestFixture
	{
		string RubyCode = "self.Items.AddRange(System::Array[System::Windows::Forms::ListViewItem].new([listViewItem1]))";
		
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

			MethodCall callExpression = RubyParserHelper.GetMethodCall(RubyCode);
			
			mockDesignerLoaderHost = new MockDesignerLoaderHost();
			typeResolutionService = mockDesignerLoaderHost.TypeResolutionService;
			RubyCodeDeserializer deserializer = new RubyCodeDeserializer(componentCreator);
			deserializedObject = deserializer.Deserialize(callExpression.Arguments.Expressions[0]);
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
