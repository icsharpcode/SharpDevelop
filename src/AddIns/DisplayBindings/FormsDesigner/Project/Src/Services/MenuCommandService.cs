// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using CommandID = System.ComponentModel.Design.CommandID;
using MenuCommand = System.ComponentModel.Design.MenuCommand;

namespace ICSharpCode.FormsDesigner.Services
{
	public class MenuCommandService : System.ComponentModel.Design.MenuCommandService
	{
		ICommandProvider commandProvider;
		
		public MenuCommandService(ICommandProvider commandProvider, IServiceProvider serviceProvider) : base(serviceProvider)
		{
			this.commandProvider = commandProvider;
			commandProvider.InitializeGlobalCommands(this);
		}

		public override void ShowContextMenu(CommandID menuID, int x, int y)
		{
			commandProvider.ShowContextMenu(menuID, x, y);
		}
	}
}
