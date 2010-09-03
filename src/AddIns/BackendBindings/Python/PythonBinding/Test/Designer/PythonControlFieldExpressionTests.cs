// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using IronPython.Compiler.Ast;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class PythonControlFieldExpressionTests
	{
		[Test]
		public void HasPrefixTest()
		{
			Assert.AreEqual("a", PythonControlFieldExpression.GetPrefix("a.b"));
		}
		
		[Test]
		public void NoDotHasPrefixTest()
		{
			Assert.AreEqual("a", PythonControlFieldExpression.GetPrefix("a"));
		}
		
		[Test]
		public void GetVariableName()
		{
			Assert.AreEqual("abc", PythonControlFieldExpression.GetVariableName("_abc"));
		}
		
		[Test]
		public void VariableNameHasOnlyUnderscore()
		{
			Assert.AreEqual(String.Empty, PythonControlFieldExpression.GetVariableName("_"));
		}
		
		[Test]
		public void VariableNameIsEmpty()
		{
			Assert.AreEqual(String.Empty, PythonControlFieldExpression.GetVariableName(String.Empty));
		}
		
		[Test]
		public void FullMemberExpression()
		{
			CallExpression call = PythonParserHelper.GetCallExpression("self._a.b.Add()");
			Assert.AreEqual("self._a.b.Add", PythonControlFieldExpression.GetMemberName(call.Target as MemberExpression));
		}
		
		[Test]
		public void NullMemberExpression()
		{
			Assert.AreEqual(String.Empty, PythonControlFieldExpression.GetMemberName(null));
		}
		
		[Test]
		public void PythonControlFieldExpressionEquals()
		{
			AssignmentStatement statement = PythonParserHelper.GetAssignmentStatement("self._textBox1.Name = \"abc\"");
			PythonControlFieldExpression field1 = PythonControlFieldExpression.Create(statement.Left[0] as MemberExpression);
			statement = PythonParserHelper.GetAssignmentStatement("self._textBox1.Name = \"def\"");
			PythonControlFieldExpression field2 = PythonControlFieldExpression.Create(statement.Left[0] as MemberExpression);
			
			Assert.AreEqual(field1, field2);
		}
		
		[Test]
		public void NullPassedToPythonControlFieldExpressionEquals()
		{
			AssignmentStatement statement = PythonParserHelper.GetAssignmentStatement("self._textBox1.Name = \"abc\"");
			PythonControlFieldExpression field = PythonControlFieldExpression.Create(statement.Left[0] as MemberExpression);
			Assert.IsFalse(field.Equals(null));
		}
		
		[Test]
		public void MethodName()
		{
			string code = "self.menuItem.Items.Add(self._fileMenuItem)";
			CallExpression expression = PythonParserHelper.GetCallExpression(code);
			PythonControlFieldExpression field = PythonControlFieldExpression.Create(expression);
			AssertAreEqual(field, "menuItem", "Items", "Add", "self.menuItem.Items");
		}
		
		[Test]
		public void MethodNameWithNoVariableName()
		{
			string code = "self.Items.Add(self._fileMenuItem)";
			CallExpression expression = PythonParserHelper.GetCallExpression(code);
			PythonControlFieldExpression field = PythonControlFieldExpression.Create(expression);
			AssertAreEqual(field, String.Empty, "Items", "Add", "self.Items");
		}
		
		[Test]
		public void SetToolTipMethodCall()
		{
			string code = "self._toolTip1.SetToolTip(self, \"Test\")";
			CallExpression expression = PythonParserHelper.GetCallExpression(code);
			PythonControlFieldExpression field = PythonControlFieldExpression.Create(expression);
			AssertAreEqual(field, "toolTip1", "_toolTip1", "SetToolTip", "self._toolTip1");
		}
		
		[Test]
		public void GetMemberNames()
		{
			string[] expected = new string[] { "a", "b" };
			string code = "a.b = 0";
			AssignmentStatement statement = PythonParserHelper.GetAssignmentStatement(code);
			Assert.AreEqual(expected, PythonControlFieldExpression.GetMemberNames(statement.Left[0] as MemberExpression));
		}
		
		[Test]
		public void GetObjectInMethodCall()
		{
			string pythonCode = "self._menuStrip1.Items.AddRange(System.Array[System.Windows.Forms.ToolStripItem](\r\n" +
						"    [self._fileToolStripMenuItem,\r\n" +
						"    self._editToolStripMenuItem]))";
			
			CallExpression callExpression = PythonParserHelper.GetCallExpression(pythonCode);
			PythonControlFieldExpression field = PythonControlFieldExpression.Create(callExpression);
			
			using (MenuStrip menuStrip = new MenuStrip()) {
				MockComponentCreator creator = new MockComponentCreator();
				creator.Add(menuStrip, "menuStrip1");
				Assert.AreSame(menuStrip.Items, field.GetMember(creator));
			}
		}
		
		[Test]
		public void GetObjectForUnknownComponent()
		{
			string pythonCode = "self._menuStrip1.SuspendLayout()";
			
			CallExpression callExpression = PythonParserHelper.GetCallExpression(pythonCode);
			PythonControlFieldExpression field = PythonControlFieldExpression.Create(callExpression);
			
			using (MenuStrip menuStrip = new MenuStrip()) {
				MockComponentCreator creator = new MockComponentCreator();
				creator.Add(menuStrip, "unknown");
				Assert.IsNull(field.GetMember(creator));
			}
		}

		[Test]
		public void GetInstanceObjectInMethodCall()
		{
			string pythonCode = "treeNode1.Nodes.AddRange(System.Array[System.Windows.Forms.TreeNode](\r\n" +
						"    [treeNode2]))";
			
			CallExpression callExpression = PythonParserHelper.GetCallExpression(pythonCode);
			PythonControlFieldExpression field = PythonControlFieldExpression.Create(callExpression);
			
			TreeNode treeNode1 = new TreeNode();
			TreeNode treeNode2 = new TreeNode();
			MockComponentCreator creator = new MockComponentCreator();
			creator.AddInstance(treeNode1, "treeNode1");
			creator.AddInstance(treeNode2, "treeNode2");
			object member = field.GetMember(creator);
			Assert.AreSame(treeNode1.Nodes, member);
		}
		
		[Test]
		public void GetObjectInMethodCallFromSpecifiedObject()
		{
			string pythonCode = "self.Controls.AddRange(System.Array[System.Windows.Forms.ToolStripItem](\r\n" +
						"    [self._fileToolStripMenuItem,\r\n" +
						"    self._editToolStripMenuItem]))";
			
			CallExpression callExpression = PythonParserHelper.GetCallExpression(pythonCode);
			
			using (Form form = new Form()) {
				Assert.AreSame(form.Controls, PythonControlFieldExpression.GetMember(form, callExpression));
			}
		}
		
		[Test]
		public void LocalVariable()
		{
			AssignmentStatement statement = PythonParserHelper.GetAssignmentStatement("listViewItem1.TooltipText = \"abc\"");
			PythonControlFieldExpression field = PythonControlFieldExpression.Create(statement.Left[0] as MemberExpression);
			
			PythonControlFieldExpression expectedField = new PythonControlFieldExpression("TooltipText", "listViewItem1", String.Empty, "listViewItem1.TooltipText");
			Assert.AreEqual(expectedField, field);
		}

		[Test]
		public void LocalVariableIsNotSelfReference()
		{
			AssignmentStatement statement = PythonParserHelper.GetAssignmentStatement("listViewItem1.TooltipText = \"abc\"");
			PythonControlFieldExpression field = PythonControlFieldExpression.Create(statement.Left[0] as MemberExpression);
			Assert.IsFalse(field.IsSelfReference);
		}

		[Test]
		public void FieldIsNotSelfReference()
		{
			AssignmentStatement statement = PythonParserHelper.GetAssignmentStatement("self.listView1.TooltipText = \"abc\"");
			PythonControlFieldExpression field = PythonControlFieldExpression.Create(statement.Left[0] as MemberExpression);
			Assert.IsTrue(field.IsSelfReference);
		}

		[Test]
		public void GetButtonObjectForSelfReference()
		{
			using (Button button = new Button()) {
				AssignmentStatement statement = PythonParserHelper.GetAssignmentStatement("self._button1.Size = System.Drawing.Size(10, 10)");
				PythonControlFieldExpression field = PythonControlFieldExpression.Create(statement.Left[0] as MemberExpression);
								
				Assert.AreEqual(button, field.GetObjectForMemberName(button));
			}
		}

		[Test]
		public void GetButtonObject()
		{
			using (Button button = new Button()) {
				AssignmentStatement statement = PythonParserHelper.GetAssignmentStatement("_button1.Size = System.Drawing.Size(10, 10)");
				PythonControlFieldExpression field = PythonControlFieldExpression.Create(statement.Left[0] as MemberExpression);
								
				Assert.AreEqual(button, field.GetObjectForMemberName(button));
			}
		}
		
		[Test]
		public void GetButtonFlatAppearanceObjectForSelfReference()
		{
			using (Button button = new Button()) {
				AssignmentStatement statement = PythonParserHelper.GetAssignmentStatement("self._button1.FlatAppearance.BorderSize = 3");
				PythonControlFieldExpression field = PythonControlFieldExpression.Create(statement.Left[0] as MemberExpression);
								
				Assert.AreEqual(button.FlatAppearance, field.GetObjectForMemberName(button));
			}
		}
		
		[Test]
		public void GetButtonFlatAppearanceObject()
		{
			using (Button button = new Button()) {
				AssignmentStatement statement = PythonParserHelper.GetAssignmentStatement("_button1.FlatAppearance.BorderSize = 3");
				PythonControlFieldExpression field = PythonControlFieldExpression.Create(statement.Left[0] as MemberExpression);
								
				Assert.AreEqual(button.FlatAppearance, field.GetObjectForMemberName(button));
			}
		}
		
		[Test]
		public void GetInvalidTwoLevelDeepButtonPropertyDescriptorForSelfReference()
		{
			using (Button button = new Button()) {
				AssignmentStatement statement = PythonParserHelper.GetAssignmentStatement("self._button1.InvalidProperty.BorderSize = 3");
				PythonControlFieldExpression field = PythonControlFieldExpression.Create(statement.Left[0] as MemberExpression);
								
				Assert.IsNull(field.GetObjectForMemberName(button));
			}
		}
		
		[Test]
		public void NullPropertyValueConversion()
		{
			using (Form form = new Form()) {
				PropertyDescriptor descriptor = TypeDescriptor.GetProperties(form).Find("Text", true);
				Assert.IsNull(PythonControlFieldExpression.ConvertPropertyValue(descriptor, null));
			}
		}
		
		void AssertAreEqual(PythonControlFieldExpression field, string variableName, string memberName, string methodName, string fullMemberName)
		{
			string expected = "Variable: " + variableName + " Member: " + memberName + " Method: " + methodName + " FullMemberName: " + fullMemberName;
			string actual = "Variable: " + field.VariableName + " Member: " + field.MemberName + " Method: " + field.MethodName + " FullMemberName: " + field.FullMemberName;
			Assert.AreEqual(expected, actual, actual);
		}
	}
}
