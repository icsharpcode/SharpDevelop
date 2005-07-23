using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	abstract class VariableListItem: TreeListViewItem
	{
		bool textsInitialized;

		public abstract bool IsValid {
			get;
		}

		public VariableListItem()
		{
			Reset();
		}

		public virtual void PrepareForExpansion()
		{

		}

		public virtual void Reset()
		{
			SubItems.Clear();
			Text = "";
			SubItems.Add("");
			SubItems.Add("");
		}

		public virtual void Refresh()
		{

		}

		protected void SetTexts(string name, string value, string type)
		{
			if (value == SubItems[1].Text || !textsInitialized) {
				// Value has not changed since last setting
				if (SubItems[1].ForeColor != Color.Black) {
					SubItems[1].ForeColor = Color.Black;
					SubItems[1].Font = new Font(SubItems[1].Font, FontStyle.Regular);
				}
			} else {
				// Value has changed since last setting
				if (SubItems[1].ForeColor != Color.Blue) {
					SubItems[1].ForeColor = Color.Blue;
					SubItems[1].Font = new Font(SubItems[1].Font, FontStyle.Bold);
				}
			}

			if (SubItems[0].Text != name) {
				SubItems[0].Text = name;
			}
			if (SubItems[1].Text != value) {
				SubItems[1].Text = value;
			}
			if (SubItems[2].Text != type) {
				SubItems[2].Text = type;
			}

			textsInitialized = true;
		}
	}
}
