// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public class MenuCommand : ToolStripMenuItem, IStatusUpdate
	{
		object caller;
		Codon codon;
		ICommand menuCommand = null;
		string description = "";
		IEnumerable<ICondition> conditions;
		
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
		public static Func<string, ICommand> LinkCommandCreator { get; set; }
		
		/// <summary>
		/// Callback that creates ICommand instances when the new syntax for known WPF commands (command="Copy") is used.
		/// </summary>
		public static Func<AddIn, string, ICommand> KnownCommandCreator { get; set; }
		
		void CreateCommand()
		{
			try {
				string link = codon.Properties["link"];
				string command = codon.Properties["command"];
				if (link != null && link.Length > 0) {
					var callback = LinkCommandCreator;
					if (callback == null)
						throw new NotSupportedException("MenuCommand.LinkCommandCreator is not set, cannot create LinkCommands.");
					menuCommand = callback(link);
				} else if (command != null && command.Length > 0) {
					var callback = KnownCommandCreator;
					if (callback == null)
						throw new NotSupportedException("MenuCommand.KnownCommandCreator is not set, cannot create commands.");
					menuCommand = callback(codon.AddIn, command);
				} else {
					menuCommand = (ICommand)codon.AddIn.CreateObject(codon.Properties["class"]);
				}
				if (menuCommand != null) {
					menuCommand.Owner = caller;
				}
			} catch (Exception e) {
				MessageService.ShowException(e, "Can't create menu command : " + codon.Id);
			}
		}
		
		public MenuCommand(Codon codon, object caller, IEnumerable<ICondition> conditions)
			: this(codon, caller, false, conditions)
		{
			
		}
		
		public static Keys ParseShortcut(string shortcutString)
		{
			Keys shortCut = Keys.None;
			if (shortcutString.Length > 0) {
				try {
					foreach (string key in shortcutString.Split('|')) {
						shortCut  |= (System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), key);
					}
				} catch (Exception ex) {
					MessageService.ShowException(ex);
					return System.Windows.Forms.Keys.None;
				}
			}
			return shortCut;
		}
		
		public MenuCommand(Codon codon, object caller, bool createCommand, IEnumerable<ICondition> conditions)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller      = caller;
			this.codon       = codon;
			this.conditions  = conditions;
			
			if (createCommand) {
				CreateCommand();
			}
			
			UpdateText();
			if (codon.Properties.Contains("shortcut")) {
				ShortcutKeys =  ParseShortcut(codon.Properties["shortcut"]);
			}
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
			this.conditions = Enumerable.Empty<ICondition>();
		}
		
		protected override void OnClick(System.EventArgs e)
		{
			base.OnClick(e);
			if (codon != null) {
				if (GetVisible() && Enabled) {
					ICommand cmd = Command;
					if (cmd != null) {
						AnalyticsMonitorService.TrackFeature(cmd.GetType().FullName, "Menu");
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
				ConditionFailedAction failedAction = Condition.GetFailedAction(conditions, caller);
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
				return Condition.GetFailedAction(conditions, caller) != ConditionFailedAction.Exclude;
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
