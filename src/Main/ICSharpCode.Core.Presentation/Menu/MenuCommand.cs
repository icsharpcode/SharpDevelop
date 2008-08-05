// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.Core.Presentation
{
	class MenuCommand : CoreMenuItem
	{
		ICommand menuCommand;
		
		public MenuCommand(Codon codon, object caller, bool createCommand) : base(codon, caller)
		{
			if (createCommand) {
				CreateCommand();
			}
		}
		
		protected override bool IsEnabledCore {
			get {
				if (!base.IsEnabledCore)
					return false;
				
				if (menuCommand != null && menuCommand is IMenuCommand) {
					return ((IMenuCommand)menuCommand).IsEnabled;
				} else {
					return true;
				}
			}
		}
		
		void CreateCommand()
		{
			try {
				string link = codon.Properties["link"];
				if (link != null && link.Length > 0) {
					if (MenuService.LinkCommandCreator == null)
						throw new NotSupportedException("MenuCommand.LinkCommandCreator is not set, cannot create LinkCommands.");
					menuCommand = MenuService.LinkCommandCreator(codon.Properties["link"]);
				} else {
					menuCommand = (ICommand)codon.AddIn.CreateObject(codon.Properties["class"]);
				}
				if (menuCommand != null) {
					menuCommand.Owner = caller;
				}
			} catch (Exception e) {
				MessageService.ShowError(e, "Can't create menu command : " + codon.Id);
			}
		}
		
		protected override void OnClick()
		{
			base.OnClick();
			if (menuCommand == null) {
				CreateCommand();
			}
			if (menuCommand != null && IsEnabledCore) {
				menuCommand.Run();
			}
		}
	}
}
