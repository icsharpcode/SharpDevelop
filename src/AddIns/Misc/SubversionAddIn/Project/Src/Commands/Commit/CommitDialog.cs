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
	public class CommitDialog : BaseSharpDevelopForm
	{
		public string LogMessage {
			get {
				return ControlDictionary["logMessageTextBox"].Text;
			}
			set {
				ControlDictionary["logMessageTextBox"].Text = value;
			}
		}
		
		public CommitDialog()
		{
			SetupFromXmlStream(Assembly.GetCallingAssembly().GetManifestResourceStream("CommitDialog.xfrm"));
		}
	}
}
