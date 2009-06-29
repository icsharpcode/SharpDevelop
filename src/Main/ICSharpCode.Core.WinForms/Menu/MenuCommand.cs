// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ICSharpCode.Core.WinForms
{
	public class MenuCommand : ToolStripMenuItem, IStatusUpdate
	{
		object caller;
		Codon codon;
		ICommand menuCommand = null;
		string description = "";
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}
		
		public ICommand Command {
			get {
				if (menuCommand == null) {
					CreateCommand();
				}
				return menuCommand;
			}
		}
		
		// HACK: find a better way to allow the host app to process link commands
		public static Converter<string, ICommand> LinkCommandCreator;
		
		void CreateCommand()
		{
			try {
				string link = codon.Properties["link"];
				if (link != null && link.Length > 0) {
					if (LinkCommandCreator == null)
						throw new NotSupportedException("MenuCommand.LinkCommandCreator is not set, cannot create LinkCommands.");
					menuCommand = LinkCommandCreator(codon.Properties["link"]);
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
		
		public MenuCommand(Codon codon, object caller) : this(codon, caller, false)
		{
			
		}
		
		public MenuCommand(Codon codon, object caller, bool createCommand)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller        = caller;
			this.codon         = codon;
			
			if (createCommand) {
				CreateCommand();
			}
			
			UpdateText();
			
			// GesturePlaceHolderRegistry.RegisterPlaceHolder(codon.Properties["class"], StringParser.Parse(codon.Properties["label"]));
			// GesturePlaceHolderRegistry.RegisterUpdateHandler(codon.Properties["class"], delegate {
			//                                                           	ShortcutKeys = GesturePlaceHolderRegistry.GetGestures(codon.Properties["class"])[0];
			//                                            });
			// GesturePlaceHolderRegistry.InvokeUpdateHandlers(codon.Properties["class"]);
			
			// if (codon.Properties.Contains("shortcut")) {
			// 	GesturePlaceHolderRegistry.InvokeUpdateHandlers(codon.Properties["class"]);
			// }
		}
		
		public MenuCommand(string label, EventHandler handler) : this(label)
		{
			this.Click  += handler;
		}
		
		public MenuCommand(string label)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.codon  = null;
			this.caller = null;
			Text = StringParser.Parse(label);
		}
		
		protected override void OnClick(System.EventArgs e)
		{
			base.OnClick(e);
			if (codon != null) {
				if (GetVisible() && Enabled) {
					ICommand cmd = Command;
					if (cmd != null) {
						LoggingService.Info("Run command " + cmd.GetType().FullName);
						cmd.Run();
					}
				}
			}
		}
		
//		protected override void OnSelect(System.EventArgs e)
//		{
//			base.OnSelect(e);
//			StatusBarService.SetMessage(description);
//		}
		
		
		public override bool Enabled {
			get {
				if (codon == null) {
					return base.Enabled;
				}
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				bool isEnabled = failedAction != ConditionFailedAction.Disable;
				
				if (menuCommand != null && menuCommand is IMenuCommand) {
					isEnabled &= ((IMenuCommand)menuCommand).IsEnabled;
				}
				return isEnabled;
			}
		}
		
		bool GetVisible()
		{
			if (codon == null)
				return true;
			else
				return codon.GetFailedAction(caller) != ConditionFailedAction.Exclude;
		}
		
		public virtual void UpdateStatus()
		{
			if (codon != null) {
				if (Image == null && codon.Properties.Contains("icon")) {
					try {
						Image = WinFormsResourceService.GetBitmap(codon.Properties["icon"]);
					} catch (ResourceNotFoundException) {}
				}
				Visible = GetVisible();
			}
		}
		
		public virtual void UpdateText()
		{
			if (codon != null) {
				Text = StringParser.Parse(codon.Properties["label"]);
			}
		}
	}
}
