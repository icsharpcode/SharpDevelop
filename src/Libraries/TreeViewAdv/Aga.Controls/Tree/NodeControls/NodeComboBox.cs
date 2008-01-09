using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using System.Drawing.Design;

namespace Aga.Controls.Tree.NodeControls
{
	public class NodeComboBox : BaseTextControl
	{
		#region Properties

		private int _editorWidth = 100;
		[DefaultValue(100)]
		public int EditorWidth
		{
			get { return _editorWidth; }
			set { _editorWidth = value; }
		}

		private List<object> _dropDownItems;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		[Editor(typeof(StringCollectionEditor), typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public List<object> DropDownItems
		{
			get { return _dropDownItems; }
		}

		#endregion

		public NodeComboBox()
		{
			_dropDownItems = new List<object>();
		}

		protected override Size CalculateEditorSize(EditorContext context)
		{
			if (Parent.UseColumns)
				return context.Bounds.Size;
			else
				return new Size(EditorWidth, context.Bounds.Height);
		}

		protected override Control CreateEditor(TreeNodeAdv node)
		{
			ComboBox comboBox = new ComboBox();
			if (DropDownItems != null)
				comboBox.Items.AddRange(DropDownItems.ToArray());
			comboBox.SelectedItem = GetValue(node);
			comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBox.DropDownClosed += new EventHandler(EditorDropDownClosed);
			SetEditControlProperties(comboBox, node);
			return comboBox;
		}

		void EditorDropDownClosed(object sender, EventArgs e)
		{
			EndEdit(true);
		}

		public override void UpdateEditor(Control control)
		{
			(control as ComboBox).DroppedDown = true;
		}

		protected override void DoApplyChanges(TreeNodeAdv node, Control editor)
		{
			SetValue(node, (editor as ComboBox).SelectedItem);
		}

		public override void MouseUp(TreeNodeAdvMouseEventArgs args)
		{
			if (args.Node != null && args.Node.IsSelected) //Workaround of specific ComboBox control behaviour
				base.MouseUp(args);
		}
	}
}
