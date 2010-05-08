/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 14.10.2007
 * Zeit: 18:40
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Reports.Addin.Commands
{
	
	
	/// <summary>
	/// This is the base class for all designer menu commands
	/// </summary>
	public abstract class AbstractFormsDesignerCommand : AbstractMenuCommand
	{
		public abstract CommandID CommandID {
			get;
		}
		
		protected virtual bool CanExecuteCommand(IDesignerHost host)
		{
			return true;
		}
		
		protected static ReportDesignerView ReportDesigner {
			get {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				if (window == null) {
					return null;
				}
				return window.ActiveViewContent as ReportDesignerView;
			}
		}
		
		
		public override void Run()
		{
			ReportDesignerView formDesigner = ReportDesigner;
			if (formDesigner != null && CanExecuteCommand(formDesigner.Host)) {
				IMenuCommandService menuCommandService = (IMenuCommandService)formDesigner.Host.GetService(typeof(IMenuCommandService));
				menuCommandService.GlobalInvoke(CommandID);
			}
		}

		internal virtual void CommandCallBack(object sender, EventArgs e)
		{
			this.Run();
		}
	}

	public class ViewCode : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.ViewCode;
			}
		}
		
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null) {
				return;
			}
			
			ReportDesignerView formDesigner = AbstractFormsDesignerCommand.ReportDesigner;
			if (formDesigner != null) {
				formDesigner.ShowSourceCode();
			}
		}
	}

	public class ShowProperties : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.PropertiesWindow;
			}
		}

		public override void Run()
		{
			PadDescriptor padContent = WorkbenchSingleton.Workbench.GetPad(typeof(ICSharpCode.SharpDevelop.Gui.PropertyPad));
			if (padContent != null) {
				padContent.BringPadToFront();
			}
		}
	}
	
	
	public class TogglePageMargin:AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get { return null; }
		}
		
		public override void Run()
		{
			ReportDesignerView formDesigner = AbstractFormsDesignerCommand.ReportDesigner;
			if (formDesigner != null) {
				formDesigner.TogglePageMargin();
			}
		}
		
	}
	
	public class DesignerVerbSubmenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)owner;
			
			List<ToolStripItem> items = new List<ToolStripItem>();
			
			foreach (DesignerVerb verb in menuCommandService.Verbs) {
				System.Console.WriteLine("{0}",verb.Text);
				items.Add(new ContextMenuCommand(verb));
			}
			
			// add separator at the end of custom designer verbs
			if (items.Count > 0) {
				items.Add(new MenuSeparator());
			}
			
			return items.ToArray();
		}
		
		class ContextMenuCommand : ICSharpCode.Core.WinForms.MenuCommand
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
	/*
	public class AlignToGrid : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignToGrid;
			}
		}
	}
	
	public class AlignLeft : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignLeft;
			}
		}
	}
	
	public class AlignRight : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignRight;
			}
		}
	}
	
	public class AlignTop : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignTop;
			}
		}
	}
	
	public class AlignBottom : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignBottom;
			}
		}
	}
	
	public class AlignHorizontalCenters : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignHorizontalCenters;
			}
		}
	}
	
	public class AlignVerticalCenters : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignVerticalCenters;
			}
		}
	}
	*/
	#endregion

	#region Make Same Size Commands
	/*
	public class SizeToGrid : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SizeToGrid;
			}
		}
	}
	
	public class SizeToControl : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SizeToControl;
			}
		}
	}
	
	public class SizeToControlHeight : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SizeToControlHeight;
			}
		}
	}
	
	public class SizeToControlWidth : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SizeToControlWidth;
			}
		}
	}
	*/
	#endregion

	#region Horizontal Spacing Commands
	/*
	public class HorizSpaceMakeEqual : AbstractFormsDesignerCommand
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
	
	public class HorizSpaceIncrease : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.HorizSpaceIncrease;
			}
		}
	}
	
	public class HorizSpaceDecrease : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.HorizSpaceDecrease;
			}
		}
	}
	
	public class HorizSpaceConcatenate : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.HorizSpaceConcatenate;
			}
		}
	}
	*/
	#endregion
	
	#region Vertical Spacing Commands
	/*
	public class VertSpaceMakeEqual : AbstractFormsDesignerCommand
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
	
	public class VertSpaceIncrease : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.VertSpaceIncrease;
			}
		}
	}
	
	public class VertSpaceDecrease : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.VertSpaceDecrease;
			}
		}
	}
	
	public class VertSpaceConcatenate : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.VertSpaceConcatenate;
			}
		}
	}
	*/
	#endregion

	#region Center Commands
	/*
	public class CenterHorizontally : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.CenterHorizontally;
			}
		}
	}
	public class CenterVertically : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.CenterVertically;
			}
		}
	}
	*/
	#endregion
	
	#region Order Commands
	/*
	public class SendToBack : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SendToBack;
			}
		}
	}
	
	public class BringToFront : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.BringToFront;
			}
		}
	}
	*/
	#endregion

	#region Tray Commands
	/*
	public class LineUpIcons : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.LineupIcons;
			}
		}
	}
	
	public class ShowLargeIcons : AbstractCheckableMenuCommand
	{
		ReportDesignerView FormDesigner {
			get {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				if (window == null) {
					return null;
				}
				return window.ActiveViewContent as ReportDesignerView;
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
				ReportDesignerView formDesigner = FormDesigner;
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
	*/
	#endregion

	#region Global Commands
	/*
	public class LockControls : AbstractFormsDesignerCommand
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
				ReportDesignerView formDesigner = FormDesigner;
				if (formDesigner != null) {
					return formDesigner.IsTabOrderMode;
				}
				return false;
			}
			set {
				SetTabOrder(value);
			}
		}
		ReportDesignerView FormDesigner {
			get {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				if (window == null) {
					return null;
				}
				return window.ActiveViewContent as ReportDesignerView;
			}
		}
		
		void SetTabOrder(bool show)
		{
			ReportDesignerView formDesigner = FormDesigner;
			if (formDesigner != null) {
				if (show) {
					formDesigner.ShowTabOrder();
				} else {
					formDesigner.HideTabOrder();
				}
			}
		}
	}
	*/
	#endregion
}
