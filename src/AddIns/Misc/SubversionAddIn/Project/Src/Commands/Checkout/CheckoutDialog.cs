// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
