using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel; 
 
namespace Aga.Controls.Tree.NodeControls
{
	public class NodeDecimalTextBox : NodeTextBox
	{
		private bool _allowDecimalSeperator = true;
		[DefaultValue(true)]
		public bool AllowDecimalSeperator
		{
			get { return _allowDecimalSeperator; }
			set { _allowDecimalSeperator = value; }
		}

		private bool _allowNegativeSign = true;
		[DefaultValue(true)]
		public bool AllowNegativeSign
		{
			get { return _allowNegativeSign; }
			set { _allowNegativeSign = value; }
		}

		protected NodeDecimalTextBox()
		{
		}

		protected override TextBox CreateTextBox()
		{
			NumericTextBox textBox = new NumericTextBox();
			textBox.AllowDecimalSeperator = AllowDecimalSeperator;
			textBox.AllowNegativeSign = AllowNegativeSign;
			return textBox;
		}

		protected override void DoApplyChanges(TreeNodeAdv node, Control editor)
		{
			SetValue(node, (editor as NumericTextBox).DecimalValue);
		}
	}
}
