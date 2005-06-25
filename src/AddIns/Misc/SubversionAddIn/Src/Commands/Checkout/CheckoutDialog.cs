using System;
using System.Reflection;
using System.Drawing;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using NSvn.Common;
using NSvn.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.Svn.Commands
{
	/// <summary>
	/// Description of Form1.
	/// </summary>
	public class CheckoutDialog : ExportDialog
	{
		public CheckoutDialog()
		{
			this.Text = "Checkout";
			ControlDictionary["groupBox1"].Text = "Repository location";
			Get<RadioButton>("fromLocalDir").Text = "&Local repository";
			Get<RadioButton>("fromUrl").Checked = true;
			Get<RadioButton>("fromUrl").Text = "&Remote repository";
		}
	}
}
