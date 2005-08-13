// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Diagnostics;
using System.Text;
using System.ComponentModel.Design;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;

using ICSharpCode.FormDesigner;

namespace ICSharpCode.FormDesigner.Commands
{
	/// <summary>
	/// This is the base class for all designer menu commands
	/// </summary>
	public abstract class AbstractFormDesignerCommand : AbstractMenuCommand
	{
		public abstract CommandID CommandID {
			get;
		}
		
		protected virtual bool CanExecuteCommand(IDesignerHost host)
		{
			return true;
		}
		
		FormDesignerViewContent FormDesigner {
			get {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				if (window == null) {
					return null;
				}
				return window.ActiveViewContent as FormDesignerViewContent;
			}
		}
		public override void Run()
		{
		
			try {
				FormDesignerViewContent formDesigner = FormDesigner;
				if (formDesigner != null && CanExecuteCommand(formDesigner.Host)) {
					IMenuCommandService menuCommandService = (IMenuCommandService)formDesigner.Host.GetService(typeof(IMenuCommandService));
					menuCommandService.GlobalInvoke(CommandID);
				}
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
		}
	}
	
	public class ViewCode : AbstractMenuCommand
	{
		 FormDesignerViewContent FormDesigner {
			get {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				if (window == null) {
					return null;
				}
				return window.ActiveViewContent as FormDesignerViewContent;
			}
		}
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null) {
				return;
			}
			
			FormDesignerViewContent formDesigner = FormDesigner;
			if (formDesigner != null) {
				formDesigner.ShowSourceCode();
				
			}
		}
	}
	
	public class ShowProperties : AbstractMenuCommand
	{
		public override void Run()
		{
			PadDescriptor padContent = WorkbenchSingleton.Workbench.GetPad(typeof(ICSharpCode.SharpDevelop.Gui.PropertyPad));
			if (padContent != null) {
				padContent.BringPadToFront();
			}
		}
	}
	
	public class DesignerVerbSubmenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)owner;
			
			ArrayList items = new ArrayList();
				
			foreach (DesignerVerb verb in menuCommandService.Verbs) {
				items.Add(new ContextMenuCommand(verb));
			}
			
			// add separator at the end of custom designer verbs
			if (items.Count > 0) {
				items.Add(new MenuSeparator());
			}
			
			return (ToolStripItem[])items.ToArray(typeof(ToolStripItem));
		}
		
		class ContextMenuCommand : ICSharpCode.Core.MenuCommand
		{
			DesignerVerb verb;
			
			public ContextMenuCommand(DesignerVerb verb) : base(verb.Text)
			{
				this.Enabled = verb.Enabled;
//				this.Checked = verb.Checked;
				
				this.verb = verb;
				Click += new EventHandler(InvokeCommand);
			}
			
			void InvokeCommand(object sender, EventArgs e)
			{
				try {
					verb.Invoke();
				} catch (Exception ex) {
					MessageService.ShowError(ex);
				}
			}
		}
	}
	
#region Align Commands	
	public class AlignToGrid : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignToGrid;
			}
		}
	}
	
	public class AlignLeft : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignLeft;
			}
		}
	}
	
	public class AlignRight : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignRight;
			}
		}
	}
	
	public class AlignTop : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignTop;
			}
		}
	}
	
	public class AlignBottom : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignBottom;
			}
		}
	}
	
	public class AlignHorizontalCenters : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignHorizontalCenters;
			}
		}
	}
	
	public class AlignVerticalCenters : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignVerticalCenters;
			}
		}
	}
#endregion

#region Make Same Size Commands
	public class SizeToGrid : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SizeToGrid;
			}
		}
	}
	
	public class SizeToControl : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SizeToControl;
			}
		}
	}
	
	public class SizeToControlHeight : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SizeToControlHeight;
			}
		}
	}
	
	public class SizeToControlWidth : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SizeToControlWidth;
			}
		}
	}
#endregion

#region Horizontal Spacing Commands	
	public class HorizSpaceMakeEqual : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.HorizSpaceMakeEqual;
			}
		}
		
		protected override bool CanExecuteCommand(IDesignerHost host)
		{
			ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
			return selectionService.SelectionCount > 1;
		}
	}
	
	public class HorizSpaceIncrease : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.HorizSpaceIncrease;
			}
		}
	}
	
	public class HorizSpaceDecrease : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.HorizSpaceDecrease;
			}
		}
	}
	
	public class HorizSpaceConcatenate : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.HorizSpaceConcatenate;
			}
		}
	}
#endregion
	
#region Vertical Spacing Commands
	public class VertSpaceMakeEqual : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.VertSpaceMakeEqual;
			}
		}
		
		protected override bool CanExecuteCommand(IDesignerHost host)
		{
			ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
			return selectionService.SelectionCount > 1;
		}
		
	}
	
	public class VertSpaceIncrease : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.VertSpaceIncrease;
			}
		}
	}
	
	public class VertSpaceDecrease : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.VertSpaceDecrease;
			}
		}
	}
	
	public class VertSpaceConcatenate : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.VertSpaceConcatenate;
			}
		}
	}
#endregion

#region Center Commands	
	public class CenterHorizontally : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.CenterHorizontally;
			}
		}
	}
	public class CenterVertically : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.CenterVertically;
			}
		}
	}
#endregion
	
#region Order Commands
	public class SendToBack : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SendToBack;
			}
		}
	}
	
	public class BringToFront : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.BringToFront;
			}
		}
	}
#endregion

#region Tray Commands	
	
	public class LineUpIcons : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.LineupIcons;
			}
		}
	}
	
	public class ShowLargeIcons : AbstractCheckableMenuCommand
	{
		FormDesignerViewContent FormDesigner {
			get {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				if (window == null) {
					return null;
				}
				return window.ActiveViewContent as FormDesignerViewContent;
			}
		}
		public override bool IsChecked {
			get {
				ComponentTray tray = Tray;
				if (tray != null) {
					return tray.ShowLargeIcons;
				}
				return false;
			}
			set {
				ComponentTray tray = Tray;
				if (tray != null) {
					tray.ShowLargeIcons = value;
				}
			}
		}
		ComponentTray Tray {
			get {
				FormDesignerViewContent formDesigner = FormDesigner;
				if (formDesigner != null) {
					return formDesigner.Host.GetService(typeof(ComponentTray)) as ComponentTray;
				}
				return null;
				
			}
		}
		public override void Run()
		{
		}
	}
#endregion

#region Global Commands	
	public class LockControls : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.LockControls;
			}
		}
	}
	
	/// <summary>
	/// Displays the tab order mode.
	/// </summary>
	public class ViewTabOrder : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				if (window == null) {
					return false;
				}
				
				FormDesignerViewContent formDesigner = FormDesigner;
				if (formDesigner != null) {
					return formDesigner.IsTabOrderMode;
				}
				return false;
			}
			set {
				SetTabOrder(value);
			}
		}
		FormDesignerViewContent FormDesigner {
			get {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				if (window == null) {
					return null;
				}
				return window.ActiveViewContent as FormDesignerViewContent;
			}
		}
		
		void SetTabOrder(bool show)
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null) {
				return;
			}
			
			FormDesignerViewContent formDesigner = FormDesigner;
			if (formDesigner != null) {
				if (show) {
					formDesigner.ShowTabOrder();
				} else {
					formDesigner.HideTabOrder();
				}
			}
		}
		
		public override void Run()
		{
			 SetTabOrder(!IsChecked);
		}
	}
#endregion

}
