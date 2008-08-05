// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Windows.Controls;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// A menu item representing an AddIn-Tree element.
	/// </summary>
	class CoreMenuItem : MenuItem
	{
		protected readonly Codon codon;
		protected readonly object caller;
		
		public CoreMenuItem(Codon codon, object caller)
		{
			this.codon = codon;
			this.caller = caller;
			
			if (codon.Properties.Contains("shortcut")) {
				InputGestureText = codon.Properties["shortcut"];
			}
			
			UpdateText();
		}
		
		public void UpdateText()
		{
			if (codon != null) {
				Header = MenuService.ConvertLabel(StringParser.Parse(codon.Properties["label"]));
			}
		}
	}
	
	class MenuCommand : CoreMenuItem
	{
		ICommand menuCommand;
		
		public MenuCommand(Codon codon, object caller, bool createCommand) : base(codon, caller)
		{
			if (createCommand) {
				CreateCommand();
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
			if (menuCommand != null) {
				menuCommand.Run();
			}
		}
	}
}
