/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.05.2014
 * Time: 20:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.Reporting.Addin.Commands
{
	/// <summary>
	/// Description of DesignerVerbSubmenuBuilder.
	/// </summary>
	public class DesignerVerbSubmenuBuilder : IMenuItemBuilder
	{
		#region IMenuItemBuilder implementation

		public IEnumerable<object> BuildItems(Codon codon, object owner)
		{
			var menuCommandService = (IMenuCommandService)owner;
			
			var items = new List<ToolStripItem>();
			
			foreach (DesignerVerb verb in menuCommandService.Verbs) {
				Console.WriteLine("{0}",verb.Text);
				items.Add(new ContextMenuCommand(verb));
			}
			
			// add separator at the end of custom designer verbs
			if (items.Count > 0) {
				items.Add(new MenuSeparator());
			}
			
			return items.ToArray();
		}

		#endregion
	}
	
	sealed class ContextMenuCommand : ICSharpCode.Core.WinForms.MenuCommand
		{
			DesignerVerb verb;
			
			public ContextMenuCommand(DesignerVerb verb) : base(verb.Text)
			{
				this.Enabled = verb.Enabled;
//				this.Checked = verb.Checked;
				this.verb = verb;
				Click += InvokeCommand;
			}
			
			void InvokeCommand(object sender, EventArgs e)
			{
				try {
					verb.Invoke();
				} catch (Exception ex) {
					MessageService.ShowException(ex);
				}
			}
		}
}
