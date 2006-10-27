// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using NoGoop.ObjBrowser.Panels;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.Dialogs
{
	public class GettingStartedDialog : Dialog
	{
		protected CheckBox          _checkBox;
		protected RichTextBox           _textBox;

		public GettingStartedDialog() : base(!INCLUDE_BUTTONS)
		{
			Text = StringParser.Parse("${res:ComponentInspector.GettingStartedDialog.Title}");

			String descText;

			/*if (ObjectBrowser.License.
				HasProduct(NogoopLicense.PRODUCT_COM_COMPINSP))
			{*/
				Height = 400;
				descText = StringParser.Parse("${res:ComponentInspector.GettingStartedDialog.Information}");
			/*}
			else
			{
				Height = 350;
				descText = 
					"Open a control or application assembly using the File menu. "
					+ "\n\n"

					+"Once the assembly is open, it appears in the "
					+ "Assemblies/Types tab, and its controls appear in "
					+ "the Controls tab.\n\n"

					+"To create an object, navigate down the above trees to "
					+ "an item shown in bold.  Drag this to the Design Surface "
					+ "or Objects panel on the left.\n\n"

					+"You may then inspect or invoke members of the created "
					+ "object by right-clicking the member in the Objects tree. "
					+ "If its a Control, you may move/resize/embed it "
					+ "in another Control on the design surface.";

			}*/

			_textBox = Utils.MakeDescText(descText, this);
			_textBox.Dock = DockStyle.Fill;
			Controls.Add(_textBox);

			Label l = new Label();
			l.Dock = DockStyle.Fill;
			Controls.Add(l);

			Panel bottomPanel = new Panel();
			bottomPanel.Dock = DockStyle.Bottom;

			_checkBox = new CheckBox();
			_checkBox.Dock = DockStyle.Left;
			_checkBox.FlatStyle = FlatStyle.System;
			_checkBox.Checked = true;
			_checkBox.Text = StringParser.Parse("${res:ComponentInspector.GettingStartedDialog.ShowThisDialogCheckBox}");
			_checkBox.TextAlign = ContentAlignment.TopLeft;
			_checkBox.CheckAlign = ContentAlignment.TopLeft;
			_checkBox.Width = 100;
			_checkBox.CheckedChanged += new EventHandler(GetStartChecked);
			bottomPanel.Controls.Add(_checkBox);
			
			ButtonPanel bp = new ButtonPanel(this, !ButtonPanel.CANCEL);
			bp.Dock = DockStyle.Right;
			bottomPanel.Controls.Add(bp);
			bottomPanel.Height = Utils.BUTTON_HEIGHT;
			Controls.Add(bottomPanel);
		}

		void GetStartChecked(Object sender, EventArgs args)
		{
			ComponentInspectorProperties.ShowGettingStartedDialog = _checkBox.Checked;
			//Preferences.GetPrefItem(Preferences.SHOW_GET_START).Value = _checkBox.Checked;
		}
	}
}
