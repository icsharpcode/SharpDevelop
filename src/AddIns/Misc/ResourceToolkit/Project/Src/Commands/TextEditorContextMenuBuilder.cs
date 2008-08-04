// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

using Hornung.ResourceToolkit.Gui;
using Hornung.ResourceToolkit.Refactoring;
using Hornung.ResourceToolkit.Resolver;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.TextEditor;

namespace Hornung.ResourceToolkit.Commands
{
	/// <summary>
	/// Builds context menu for editing string resources.
	/// </summary>
	public class TextEditorContextMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			TextEditorControl editor = owner as TextEditorControl;
			
			if (editor == null) {
				return new ToolStripItem[0];
			}
			
			ResourceResolveResult result = ResourceResolverService.Resolve(editor, null);
			if (result != null && result.ResourceFileContent != null && result.Key != null) {
				
				List<ToolStripItem> items = new List<ToolStripItem>();
				MenuCommand cmd;
				
				// add resource (if key does not exist) / edit resource (if key exists)
				if (result.ResourceFileContent.ContainsKey(result.Key)) {
					cmd = new MenuCommand("${res:Hornung.ResourceToolkit.TextEditorContextMenu.EditResource}", this.EditResource);
				} else {
					cmd = new MenuCommand("${res:Hornung.ResourceToolkit.TextEditorContextMenu.AddResource}", this.EditResource);
				}
				cmd.Tag = result;
				items.Add(cmd);
				
				// find references
				cmd = new MenuCommand("${res:SharpDevelop.Refactoring.FindReferencesCommand}", this.FindReferences);
				cmd.Tag = result;
				items.Add(cmd);
				
				// rename
				cmd = new MenuCommand("${res:SharpDevelop.Refactoring.RenameCommand}", this.Rename);
				cmd.Tag = result;
				items.Add(cmd);
				
				
				// put the resource menu items into a submenu
				// with the resource key as title
				ToolStripMenuItem subMenu = new ToolStripMenuItem(result.Key);
				subMenu.DropDownItems.AddRange(items.ToArray());
				return new ToolStripItem[] { subMenu, new MenuSeparator() };
				
			}
			
			return new ToolStripItem[0];
		}
		
		// ********************************************************************************************************************************
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "ICSharpCode.Core.MessageService.ShowWarning(System.String)")]
		void EditResource(object sender, EventArgs e)
		{
			MenuCommand cmd = sender as MenuCommand;
			if (cmd == null) {
				return;
			}
			
			ResourceResolveResult result = cmd.Tag as ResourceResolveResult;
			if (result == null) {
				return;
			}
			
			object value;
			string svalue = null;
			if (result.ResourceFileContent.TryGetValue(result.Key, out value)) {
				svalue = value as string;
				if (svalue == null) {
					MessageService.ShowWarning("${res:Hornung.ResourceToolkit.ResourceTypeNotSupported}");
					return;
				}
			}
			
			EditStringResourceDialog dialog = new EditStringResourceDialog(result.ResourceFileContent, result.Key, svalue, false);
			if (svalue == null) {
				dialog.Text = String.Format(CultureInfo.CurrentCulture, StringParser.Parse("${res:Hornung.ResourceToolkit.CodeCompletion.AddNewDescription}"), result.ResourceFileContent.FileName);
			}
			if (dialog.ShowDialog(WorkbenchSingleton.MainForm) == DialogResult.OK) {
				if (svalue == null) {
					// Add new resource.
					result.ResourceFileContent.Add(dialog.Key, dialog.Value);
				} else {
					// Modify existing resource.
					result.ResourceFileContent.SetValue(result.Key, dialog.Value);
				}
			}
			
		}
		
		// ********************************************************************************************************************************
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Body")]
		void FindReferences(object sender, EventArgs e)
		{
			MenuCommand cmd = sender as MenuCommand;
			if (cmd == null) {
				return;
			}
			
			ResourceResolveResult result = cmd.Tag as ResourceResolveResult;
			if (result == null) {
				return;
			}
			
			// Allow the menu to close
			Application.DoEvents();
			using(AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog(ResourceService.GetString("SharpDevelop.Refactoring.FindReferences"))) {
				FindReferencesAndRenameHelper.ShowAsSearchResults(StringParser.Parse("${res:Hornung.ResourceToolkit.ReferencesToResource}", new string[,] { {"ResourceFileName", System.IO.Path.GetFileName(result.FileName)}, {"ResourceKey", result.Key} }),
				                                                  ResourceRefactoringService.FindReferences(result.FileName, result.Key, monitor));
			}
		}
		
		void Rename(object sender, EventArgs e)
		{
			MenuCommand cmd = sender as MenuCommand;
			if (cmd == null) {
				return;
			}
			
			ResourceResolveResult result = cmd.Tag as ResourceResolveResult;
			if (result == null) {
				return;
			}
			
			// Allow the menu to close
			Application.DoEvents();
			ResourceRefactoringService.Rename(result);
		}
		
	}
}
