// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.NRefactory.CSharp;

namespace ICSharpCode.NRefactory.Demo
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			CSharpParseButtonClick(null, null);
		}
		
		void CSharpParseButtonClick(object sender, EventArgs e)
		{
			CSharpParser parser = new CSharpParser();
			CompilationUnit cu = parser.Parse(new StringReader(csharpCodeTextBox.Text));
			csharpTreeView.Nodes.Clear();
			foreach (var element in cu.Children) {
				csharpTreeView.Nodes.Add(MakeTreeNode(element));
			}
			SelectCurrentNode(csharpTreeView.Nodes);
		}
		
		TreeNode MakeTreeNode(INode node)
		{
			StringBuilder b = new StringBuilder();
			b.Append(DecodeRole(node.Role, node.Parent != null ? node.Parent.GetType() : null));
			b.Append(": ");
			b.Append(node.GetType().Name);
			bool hasProperties = false;
			foreach (PropertyInfo p in node.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
				if (p.PropertyType == typeof(string) || p.PropertyType.IsEnum) {
					if (!hasProperties) {
						hasProperties = true;
						b.Append(" (");
					} else {
						b.Append(", ");
					}
					b.Append(p.Name);
					b.Append(" = ");
					try {
						object val = p.GetValue(node, null);
						b.Append(val != null ? val.ToString() : "**null**");
					} catch (TargetInvocationException ex) {
						b.Append("**" + ex.InnerException.GetType().Name + "**");
					}
				}
			}
			if (hasProperties)
				b.Append(")");
			TreeNode t = new TreeNode(b.ToString());
			t.Tag = node;
			foreach (INode child in node.Children) {
				t.Nodes.Add(MakeTreeNode(child));
			}
			return t;
		}
		
		string DecodeRole(int role, Type type)
		{
			if (type != null) {
				foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Static)) {
					if (field.FieldType == typeof(int) && (int)field.GetValue(null) == role)
						return field.Name;
				}
			}
			foreach (FieldInfo field in typeof(AbstractNode.Roles).GetFields(BindingFlags.Public | BindingFlags.Static)) {
				if (field.FieldType == typeof(int) && (int)field.GetValue(null) == role)
					return field.Name;
			}
			
			return role.ToString();
		}
		
		bool SelectCurrentNode(TreeNodeCollection c)
		{
			int selectionStart = csharpCodeTextBox.SelectionStart;
			int selectionEnd = selectionStart + csharpCodeTextBox.SelectionLength;
			foreach (TreeNode t in c) {
				INode node = t.Tag as INode;
				if (node != null
				    && selectionStart >= GetOffset(csharpCodeTextBox, node.StartLocation)
				    && selectionEnd <= GetOffset(csharpCodeTextBox, node.EndLocation))
				{
					t.Expand();
					if (!SelectCurrentNode(t.Nodes))
						csharpTreeView.SelectedNode = t;
					return true;
				}
			}
			return false;
		}
		
		void CSharpGenerateCodeButtonClick(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}
		
		int GetOffset(TextBox textBox, DomLocation location)
		{
			return textBox.GetFirstCharIndexFromLine(location.Line - 1) + location.Column - 1;
		}
		
		void CSharpTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			INode node = e.Node.Tag as INode;
			if (node != null) {
				int startOffset = GetOffset(csharpCodeTextBox, node.StartLocation);
				int endOffset = GetOffset(csharpCodeTextBox, node.EndLocation);
				csharpCodeTextBox.Select(startOffset, endOffset - startOffset);
			}
		}
	}
}
