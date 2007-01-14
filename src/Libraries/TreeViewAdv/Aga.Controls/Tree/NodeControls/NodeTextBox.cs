using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

namespace Aga.Controls.Tree.NodeControls
{
	public class NodeTextBox: BaseTextControl
	{
		private const int MinTextBoxWidth = 30;

		private TextBox EditorTextBox
		{
			get
			{
				return CurrentEditor as TextBox;
			}
		}

		public NodeTextBox()
		{
		}

		protected override Size CalculateEditorSize(EditorContext context)
		{
			if (Parent.UseColumns)
				return context.Bounds.Size;
			else
			{
				TextBox textBox = context.Editor as TextBox;
				Size size = GetLabelSize(context.CurrentNode, context.DrawContext);
				int width = Math.Max(size.Width + Font.Height, MinTextBoxWidth); // reserve a place for new typed character
				return new Size(width, size.Height);
			}
		}

		public override void KeyDown(KeyEventArgs args)
		{
			if (args.KeyCode == Keys.F2 && Parent.CurrentNode != null)
			{
				args.Handled = true;
				BeginEdit();
			}
		}

		protected override Control CreateEditor(TreeNodeAdv node)
		{
			TextBox textBox = new TextBox();
			textBox.TextAlign = TextAlign;
			textBox.Text = GetLabel(node);
			textBox.BorderStyle = BorderStyle.FixedSingle;
			textBox.TextChanged += new EventHandler(textBox_TextChanged);
			_label = textBox.Text;
			return textBox;
		}

		private string _label;
		private void textBox_TextChanged(object sender, EventArgs e)
		{
			_label = EditorTextBox.Text;
			Parent.UpdateEditorBounds();
		}

		protected override void DoApplyChanges(TreeNodeAdv node, Control editor)
		{
			string oldLabel = GetLabel(node);
			if (oldLabel != _label)
			{
				SetLabel(node, _label);
				OnLabelChanged();
			}
		}

		public void Cut()
		{
			if (EditorTextBox != null)
				EditorTextBox.Cut();
		}

		public void Copy()
		{
			if (EditorTextBox != null)
				EditorTextBox.Copy();
		}

		public void Paste()
		{
			if (EditorTextBox != null)
				EditorTextBox.Paste();
		}

		public void Delete()
		{
			if (EditorTextBox != null)
			{
				int len = Math.Max(EditorTextBox.SelectionLength, 1);
				if (EditorTextBox.SelectionStart < EditorTextBox.Text.Length)
				{
					int start = EditorTextBox.SelectionStart;
					EditorTextBox.Text = EditorTextBox.Text.Remove(EditorTextBox.SelectionStart, len);
					EditorTextBox.SelectionStart = start;
				}
			}
		}

		public event EventHandler LabelChanged;
		protected void OnLabelChanged()
		{
			if (LabelChanged != null)
				LabelChanged(this, EventArgs.Empty);
		}
	}
}
