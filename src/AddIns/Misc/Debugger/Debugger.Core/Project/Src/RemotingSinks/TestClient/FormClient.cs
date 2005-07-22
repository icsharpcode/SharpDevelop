// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Remoting;

namespace CustomSinks
{
	public partial class FormClient : Form
	{
		ChatServer chatServer;

		public FormClient()
		{
			InitializeComponent();
			RemotingConfiguration.Configure("Client.exe.config");
			chatServer = new ChatServer();
			chatServer.NewMessage +=
				delegate(object sender, TextMessageEventArgs args)
				{
					richTextBoxChat.AppendText(args.Message + "\r\n");
				};
		}

		private void buttonSend_Click(object sender, EventArgs e)
		{
			chatServer.SendMessage(textBoxInput.Text);
		}
	}
}
