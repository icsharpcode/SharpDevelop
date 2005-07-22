// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core;

namespace ICSharpCode.FormDesigner.Services
{
	class MenuCommandService : IMenuCommandService
	{
		IServiceContainer serviceContainer;
			
		ArrayList     commands = new ArrayList();
		ArrayList     verbs    = new ArrayList();
		
		Control panel;
		
		public DesignerVerbCollection Verbs {
			get {
				DesignerVerbCollection verbCollection = CreateDesignerVerbCollection();
				verbCollection.AddRange((DesignerVerb[])verbs.ToArray(typeof(DesignerVerb)));
				return verbCollection;
			}
		}
		
		public MenuCommandService(Control panel, IServiceContainer serviceContainer)
		{
			this.panel            = panel;
			this.serviceContainer = serviceContainer;
		}
		
		public void AddCommand(System.ComponentModel.Design.MenuCommand command)
		{
			if (command != null && command.CommandID != null) {
				if (!commands.Contains(command)) {
					this.commands.Add(command);
				}
			}
		}
		
		public void AddVerb(DesignerVerb verb)
		{
			if (verb != null) {
				this.verbs.Add(verb);
			}
		}
		
		public void RemoveCommand(System.ComponentModel.Design.MenuCommand command)
		{
			if (command != null) {
				commands.Remove(command.CommandID);
			}
		}
		
		public void RemoveVerb(DesignerVerb verb)
		{
			if (verb != null) {
				verbs.Remove(verb);
			}
		}
		
		public bool GlobalInvoke(CommandID commandID)
		{
			System.ComponentModel.Design.MenuCommand menuCommand = FindCommand(commandID);
			if (menuCommand == null) {
				return false;
			}
			
			menuCommand.Invoke();
			return true;
		}
		
		public System.ComponentModel.Design.MenuCommand FindCommand(CommandID commandID)
		{
//			if (StringType.StrCmp(MenuUtilities.GetCommandNameFromCommandID(commandID), "", false) == 0 && StringType.StrCmp(commandID.ToString(), "74d21313-2aee-11d1-8bfb-00a0c90f26f7 : 12288", false) == 0) {
//				return MenuUtilities.gPropertyGridResetCommand;
//			}
			
			foreach (System.ComponentModel.Design.MenuCommand menuCommand in commands) {
				if (menuCommand.CommandID == commandID) {
					return menuCommand;
				}
			}
			
			foreach (DesignerVerb verb in Verbs) {
				if (verb.CommandID == commandID) {
					return verb;
				}
			}
			return null;
		}
		
		public void ShowContextMenu(CommandID menuID, int x, int y)
		{
			string contextMenuPath = "/SharpDevelop/FormsDesigner/ContextMenus/";
			
			if (menuID == MenuCommands.ComponentTrayMenu) {
				contextMenuPath += "ComponentTrayMenu";
			} else if (menuID == MenuCommands.ContainerMenu) {
				contextMenuPath += "ContainerMenu";
			} else if (menuID == MenuCommands.SelectionMenu) {
				contextMenuPath += "SelectionMenu";
			} else if (menuID == MenuCommands.TraySelectionMenu) {
				contextMenuPath += "TraySelectionMenu";
			} else {
				throw new Exception();
			}
			Point p = panel.PointToClient(new Point(x, y));
			
			
			MenuService.ShowContextMenu(this, contextMenuPath, panel, p.X, p.Y);
		}
		
		public DesignerVerbCollection CreateDesignerVerbCollection()
		{
			DesignerVerbCollection designerVerbCollection = new DesignerVerbCollection();
			
			ISelectionService selectionService = (ISelectionService)serviceContainer.GetService(typeof(ISelectionService));
			IDesignerHost host = (IDesignerHost)serviceContainer.GetService(typeof(IDesignerHost));
			if (host != null && selectionService != null && selectionService.SelectionCount == 1) {
				IComponent selectedComponent = selectionService.PrimarySelection as Component;
				if (selectedComponent != null) {
					IDesigner designer = host.GetDesigner((IComponent)selectedComponent);
					if (designer != null) {
						designerVerbCollection.AddRange(designer.Verbs);
					}
				}
				
				if (selectedComponent == host.RootComponent) {
					designerVerbCollection.AddRange((DesignerVerb[])this.verbs.ToArray(typeof(DesignerVerb)));
				}
			}
			return designerVerbCollection;
		}
	}
}
