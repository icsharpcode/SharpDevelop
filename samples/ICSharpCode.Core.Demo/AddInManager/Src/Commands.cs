// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.AddInManager
{
	public class ShowCommand : AbstractMenuCommand
	{
		#if STANDALONE
		static bool resourcesRegistered;
		#endif
		
		public override void Run()
		{
			#if STANDALONE
			if (!resourcesRegistered) {
				resourcesRegistered = true;
				ResourceService.RegisterStrings("ICSharpCode.AddInManager.StringResources", typeof(ShowCommand).Assembly);
			}
			#endif
			ManagerForm.ShowForm();
		}
	}
	
	public class AddInManagerAddInStateConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			string states = condition.Properties["states"];
			string action = ((AddInControl)caller).AddIn.Action.ToString();
			foreach (string state in states.Split(',')) {
				if (state == action)
					return true;
			}
			return false;
		}
	}
	
	public class DisableCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ManagerForm.Instance.TryRunAction(((AddInControl)Owner).AddIn, AddInAction.Disable);
		}
	}
	
	public class EnableCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ManagerForm.Instance.TryRunAction(((AddInControl)Owner).AddIn, AddInAction.Enable);
		}
	}
	
	public class AbortInstallCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ManagerForm.Instance.TryRunAction(((AddInControl)Owner).AddIn, AddInAction.Uninstall);
		}
	}
	
	public class AbortUpdateCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ManagerForm.Instance.TryRunAction(((AddInControl)Owner).AddIn, AddInAction.InstalledTwice);
		}
	}
	
	public class UninstallCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ManagerForm.Instance.TryUninstall(((AddInControl)Owner).AddIn);
		}
	}
	
	public class OpenHomepageCommand : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				return ((AddInControl)Owner).AddIn.Properties["url"].Length > 0;
			}
		}
		
		public override void Run()
		{
			#if STANDALONE
			try {
				System.Diagnostics.Process.Start(((AddInControl)Owner).AddIn.Properties["url"]);
			} catch {}
			#else
			FileService.OpenFile(((AddInControl)Owner).AddIn.Properties["url"]);
			#endif
			ManagerForm.Instance.Close();
		}
	}
	
	public class AboutCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			using (AboutForm form = new AboutForm(((AddInControl)Owner).AddIn)) {
				form.ShowDialog(ManagerForm.Instance);
			}
		}
	}
	
	public class OptionsCommand : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				#if !STANDALONE
				AddIn addIn = ((AddInControl)Owner).AddIn;
				if (addIn.Enabled) {
					foreach (KeyValuePair<string, ExtensionPath> pair in addIn.Paths) {
						if (pair.Key.StartsWith("/SharpDevelop/Dialogs/OptionsDialog")) {
							return true;
						}
					}
				}
				#endif
				return false;
			}
		}
		
		public override void Run()
		{
			#if !STANDALONE
			AddIn addIn = ((AddInControl)Owner).AddIn;
			AddInTreeNode dummyNode = new AddInTreeNode();
			foreach (KeyValuePair<string, ExtensionPath> pair in addIn.Paths) {
				if (pair.Key.StartsWith("/SharpDevelop/Dialogs/OptionsDialog")) {
					dummyNode.Codons.AddRange(pair.Value.Codons);
				}
			}
			ICSharpCode.SharpDevelop.Commands.OptionsCommand.ShowTabbedOptions(addIn.Name + " " + ResourceService.GetString("AddInManager.Options"),
			                                                                   dummyNode);
			#endif
		}
	}
}
