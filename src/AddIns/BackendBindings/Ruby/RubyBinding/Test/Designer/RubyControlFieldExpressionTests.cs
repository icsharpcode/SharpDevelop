// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using IronRuby.Compiler.Ast;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class RubyControlFieldExpressionTests
	{
		[Test]
		public void HasPrefixTest()
		{
			Assert.AreEqual("a", RubyControlFieldExpression.GetPrefix("a.b"));
		}
		
		[Test]
		public void NoDotHasPrefixTest()
		{
			Assert.AreEqual("a", RubyControlFieldExpression.GetPrefix("a"));
		}
		
		[Test]
		public void GetVariableName()
		{
			Assert.AreEqual("abc", RubyControlFieldExpression.GetVariableName("@abc"));
		}
		
		[Test]
		public void VariableNameHasOnlyAtSymbol()
		{
			Assert.AreEqual(String.Empty, RubyControlFieldExpression.GetVariableName("@"));
		}
		
		[Test]
		public void VariableNameIsEmpty()
		{
			Assert.AreEqual(String.Empty, RubyControlFieldExpression.GetVariableName(String.Empty));
		}
		
		[Test]
		public void FullMemberExpression()
		{
			MethodCall call = RubyParserHelper.GetMethodCall("self.a.b.Add()");
			Assert.AreEqual("self.a.b.Add", RubyControlFieldExpression.GetMemberName(call));
		}
		
		[Test]
		public void NullMemberExpression()
		{
			Assert.AreEqual(String.Empty, RubyControlFieldExpression.GetMemberName(null));
		}
			
		[Test]
		public void RubyControlFieldExpressionEquals()
		{
			SimpleAssignmentExpression expression = RubyParserHelper.GetSimpleAssignmentExpression("self.textBox1.Name = \"abc\"");
			RubyControlFieldExpression field1 = RubyControlFieldExpression.Create(expression.Left as AttributeAccess);
			expression = RubyParserHelper.GetSimpleAssignmentExpression("self.textBox1.Name = \"def\"");
			RubyControlFieldExpression field2 = RubyControlFieldExpression.Create(expression.Left as AttributeAccess);
			
			Assert.AreEqual(field1, field2);
		}
		
		[Test]
		public void NullPassedToRubyControlFieldExpressionEquals()
		{
			SimpleAssignmentExpression expression = RubyParserHelper.GetSimpleAssignmentExpression("self.textBox1.Name = \"abc\"");
			RubyControlFieldExpression field = RubyControlFieldExpression.Create(expression.Left as AttributeAccess);
			Assert.IsFalse(field.Equals(null));
		}
		
		[Test]
		public void MethodName()
		{
			string code = "self.menuItem.Items.Add(@fileMenuItem)";
			MethodCall expression = RubyParserHelper.GetMethodCall(code);
			RubyControlFieldExpression field = RubyControlFieldExpression.Create(expression);
			AssertAreEqual(field, "menuItem", "Items", "Add", "self.menuItem.Items");
		}
		
		[Test]
		public void MethodNameWithNoVariableName()
		{
			string code = "self.Items.Add(@fileMenuItem)";
			MethodCall methodCall = RubyParserHelper.GetMethodCall(code);
			RubyControlFieldExpression field = RubyControlFieldExpression.Create(methodCall);
			AssertAreEqual(field, String.Empty, "Items", "Add", "self.Items");
		}
		
		[Test]
		public void SetToolTipMethodCall()
		{
			string code = "@toolTip1.SetToolTip(self, \"Test\")";
			MethodCall expression = RubyParserHelper.GetMethodCall(code);
			RubyControlFieldExpression field = RubyControlFieldExpression.Create(expression);
			AssertAreEqual(field, "toolTip1", String.Empty, "SetToolTip", "@toolTip1");
		}
		
		[Test]
		public void GetMemberNames()
		{
			string[] expected = new string[] { "a", "b" };
			string code = "a.b = 0";
			SimpleAssignmentExpression assignment = RubyParserHelper.GetSimpleAssignmentExpression(code);
			Assert.AreEqual(expected, RubyControlFieldExpression.GetMemberNames(assignment.Left as AttributeAccess));
		}
		
		[Test]
		public void GetMemberNamesForButtonPropertyReference()
		{
			string[] expected = new string[] { "@button1", "Location" };
			string code = "@button1.Location = System::Drawing::Point.new(0, 0)";
			SimpleAssignmentExpression assignment = RubyParserHelper.GetSimpleAssignmentExpression(code);
			Assert.AreEqual(expected, RubyControlFieldExpression.GetMemberNames(assignment.Left as AttributeAccess));
		}
		
		[Test]
		public void GetMemberNamesForFormClientSizePropertyReference()
		{
			string[] expected = new string[] { "self", "ClientSize" };
			string code = "self.ClientSize = System::Drawing::Size.new(300, 400)";
			SimpleAssignmentExpression assignment = RubyParserHelper.GetSimpleAssignmentExpression(code);
			Assert.AreEqual(expected, RubyControlFieldExpression.GetMemberNames(assignment.Left as AttributeAccess));
		}
		
		[Test]
		public void GetMemberNamesForColorReference()
		{
			string[] expected = new string[] { "System", "Drawing", "Color", "Red" };
			string code = "self.BackColor = System::Drawing::Color.Red";
			SimpleAssignmentExpression assignment = RubyParserHelper.GetSimpleAssignmentExpression(code);
			Assert.AreEqual(expected, RubyControlFieldExpression.GetMemberNames(assignment.Right as MethodCall));
		}
		
		[Test]
		public void GetObjectInMethodCall()
		{
			string RubyCode = "@menuStrip1.Items.AddRange(System::Array[System::Windows::Forms::ToolStripItem].new(\r\n" +
						"    [@fileToolStripMenuItem,\r\n" +
						"    @editToolStripMenuItem]))";
			
			MethodCall call = RubyParserHelper.GetMethodCall(RubyCode);
			RubyControlFieldExpression field = RubyControlFieldExpression.Create(call);
			
			using (MenuStrip menuStrip = new MenuStrip()) {
				MockComponentCreator creator = new MockComponentCreator();
				creator.Add(menuStrip, "menuStrip1");
				Assert.AreSame(menuStrip.Items, field.GetMember(creator));
			}
		}
		
		[Test]
		public void GetObjectForUnknownComponent()
		{
			string RubyCode = "@menuStrip1.SuspendLayout()";
			
			MethodCall call = RubyParserHelper.GetMethodCall(RubyCode);
			RubyControlFieldExpression field = RubyControlFieldExpression.Create(call);
			
			using (MenuStrip menuStrip = new MenuStrip()) {
				MockComponentCreator creator = new MockComponentCreator();
				creator.Add(menuStrip, "unknown");
				Assert.IsNull(field.GetMember(creator));
			}
		}
		
		[Test]
		public void GetInstanceObjectInMethodCall()
		{
			string RubyCode = "treeNode1.Nodes.AddRange(System::Array[System::Windows::Forms::TreeNode].new(\r\n" +
						"    [treeNode2]))";
			
			MethodCall callExpression = RubyParserHelper.GetMethodCall(RubyCode);
			RubyControlFieldExpression field = RubyControlFieldExpression.Create(callExpression);
			
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
			string RubyCode = "self.Controls.AddRange(System::Array[System::Windows::Forms::ToolStripItem].new(\r\n" +
						"    [@fileToolStripMenuItem,\r\n" +
						"    @editToolStripMenuItem]))";
			
			MethodCall callExpression = RubyParserHelper.GetMethodCall(RubyCode);
			
			using (Form form = new Form()) {
				Assert.AreSame(form.Controls, RubyControlFieldExpression.GetMember(form, callExpression));
			}
		}
		
		[Test]
		public void LocalVariableInAssignment()
		{
			SimpleAssignmentExpression expression = RubyParserHelper.GetSimpleAssignmentExpression("listViewItem1.TooltipText = \"abc\"");
			RubyControlFieldExpression field = RubyControlFieldExpression.Create(expression.Left as AttributeAccess);
			
			RubyControlFieldExpression expectedField = new RubyControlFieldExpression("TooltipText", "listViewItem1", String.Empty, "listViewItem1.TooltipText");
			Assert.AreEqual(expectedField, field);
		}

		[Test]
		public void LocalVariableCreatingNewInstance()
		{
			SimpleAssignmentExpression expression = RubyParserHelper.GetSimpleAssignmentExpression("listViewItem1 = System::Windows::Forms.ListViewItem.new()");
			RubyControlFieldExpression field = RubyControlFieldExpression.Create(expression.Left as LocalVariable);
			
			RubyControlFieldExpression expectedField = new RubyControlFieldExpression(String.Empty, "listViewItem1", String.Empty, "listViewItem1");
			Assert.AreEqual(expectedField, field);
		}
		
		[Test]
		public void LocalVariableMethodCall()
		{
			string code =
				"listViewItem1 = System::Windows::Forms::ListViewItem.new()\r\n" +
				"listViewItem1.CallMethod()\r\n";
			
			MethodCall expression = RubyParserHelper.GetLastExpression(code) as MethodCall;
			RubyControlFieldExpression field = RubyControlFieldExpression.Create(expression);
		
			RubyControlFieldExpression expectedField = new RubyControlFieldExpression(String.Empty, String.Empty, "CallMethod", "listViewItem1");
			Assert.AreEqual(expectedField, field);
		}
		
		[Test]
		public void LocalVariableIsNotSelfReference()
		{
			SimpleAssignmentExpression expression = RubyParserHelper.GetSimpleAssignmentExpression("listViewItem1.TooltipText = \"abc\"");
			RubyControlFieldExpression field = RubyControlFieldExpression.Create(expression.Left as AttributeAccess);
			Assert.IsFalse(field.IsSelfReference);
		}
		
		[Test]
		public void FieldIsSelfReference()
		{
			SimpleAssignmentExpression expression = RubyParserHelper.GetSimpleAssignmentExpression("self.listView1.TooltipText = \"abc\"");
			RubyControlFieldExpression field = RubyControlFieldExpression.Create(expression.Left as AttributeAccess);
			Assert.IsTrue(field.IsSelfReference);
		}

		[Test]
		public void PrivateClassVariableIsSelfReference()
		{
			SimpleAssignmentExpression expression = RubyParserHelper.GetSimpleAssignmentExpression("@listView1.TooltipText = \"abc\"");
			RubyControlFieldExpression field = RubyControlFieldExpression.Create(expression.Left as AttributeAccess);
			Assert.IsTrue(field.IsSelfReference);
		}
		
		[Test]
		public void GetButtonObjectForSelfReference()
		{
			using (Button button = new Button()) {
				SimpleAssignmentExpression expression = RubyParserHelper.GetSimpleAssignmentExpression("self.button1.Size = System::Drawing::Size.new(10, 10)");
				RubyControlFieldExpression field = RubyControlFieldExpression.Create(expression.Left as AttributeAccess);
								
				Assert.AreEqual(button, field.GetObjectForMemberName(button));
			}
		}

		[Test]
		public void GetButtonObject()
		{
			using (Button button = new Button()) {
				SimpleAssignmentExpression expression = RubyParserHelper.GetSimpleAssignmentExpression("@button1.Size = System::Drawing::Size.new(10, 10)");
				RubyControlFieldExpression field = RubyControlFieldExpression.Create(expression.Left as AttributeAccess);
								
				Assert.AreEqual(button, field.GetObjectForMemberName(button));
			}
		}
		
		[Test]
		public void GetButtonFlatAppearanceObjectForSelfReference()
		{
			using (Button button = new Button()) {
				SimpleAssignmentExpression expression = RubyParserHelper.GetSimpleAssignmentExpression("self.button1.FlatAppearance.BorderSize = 3");
				RubyControlFieldExpression field = RubyControlFieldExpression.Create(expression.Left as AttributeAccess);
								
				Assert.AreEqual(button.FlatAppearance, field.GetObjectForMemberName(button));
			}
		}
		
		[Test]
		public void GetButtonFlatAppearanceObject()
		{
			using (Button button = new Button()) {
				SimpleAssignmentExpression expression = RubyParserHelper.GetSimpleAssignmentExpression("@button1.FlatAppearance.BorderSize = 3");
				RubyControlFieldExpression field = RubyControlFieldExpression.Create(expression.Left as AttributeAccess);
								
				Assert.AreEqual(button.FlatAppearance, field.GetObjectForMemberName(button));
			}
		}
		
		[Test]
		public void GetInvalidTwoLevelDeepButtonPropertyDescriptorForSelfReference()
		{
			using (Button button = new Button()) {
				SimpleAssignmentExpression expression = RubyParserHelper.GetSimpleAssignmentExpression("self.button1.InvalidProperty.BorderSize = 3");
				RubyControlFieldExpression field = RubyControlFieldExpression.Create(expression.Left as AttributeAccess);
								
				Assert.IsNull(field.GetObjectForMemberName(button));
			}
		}
		
		[Test]
		public void NullPropertyValueConversion()
		{
			using (Form form = new Form()) {
				PropertyDescriptor descriptor = TypeDescriptor.GetProperties(form).Find("Text", true);
				Assert.IsNull(RubyControlFieldExpression.ConvertPropertyValue(descriptor, null));
			}
		}
		
		[Test]
		public void ClrMemberMethodCallIsConvertedToActualMemberNames()
		{
			string code =
				"@pictureBox1.clr_member(System::ComponentModel::ISupportInitialize, :BeginInit).call()\r\n";
			
			MethodCall expression = RubyParserHelper.GetMethodCall(code);
			RubyControlFieldExpression field = RubyControlFieldExpression.Create(expression);
		
			RubyControlFieldExpression expectedField = new RubyControlFieldExpression(String.Empty, "pictureBox1", "BeginInit", "@pictureBox1");
			Assert.AreEqual(expectedField, field);
		}
		
		[Test]
		public void UpperCaseClrMemberMethodCallIsConvertedToActualMemberNames()
		{
			string code =
				"@pictureBox1.CLR_MEMBER(System::ComponentModel::ISupportInitialize, :BeginInit).call()\r\n";
			
			MethodCall expression = RubyParserHelper.GetMethodCall(code);
			RubyControlFieldExpression field = RubyControlFieldExpression.Create(expression);
		
			RubyControlFieldExpression expectedField = new RubyControlFieldExpression(String.Empty, "pictureBox1", "BeginInit", "@pictureBox1");
			Assert.AreEqual(expectedField, field);
		}

		
		void AssertAreEqual(RubyControlFieldExpression field, string variableName, string memberName, string methodName, string fullMemberName)
		{
			string expected = "Variable: " + variableName + " Member: " + memberName + " Method: " + methodName + " FullMemberName: " + fullMemberName;
			string actual = "Variable: " + field.VariableName + " Member: " + field.MemberName + " Method: " + field.MethodName + " FullMemberName: " + field.FullMemberName;
			Assert.AreEqual(expected, actual, actual);
		}
	}
}
