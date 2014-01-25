// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.FormsDesigner.Commands
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
		
		FormsDesignerViewContent FormDesigner {
			get {
				return SD.Workbench.ActiveViewContent as FormsDesignerViewContent;
			}
		}
		public override void Run()
		{
			
			try {
				FormsDesignerViewContent formDesigner = FormDesigner;
				if (formDesigner != null && CanExecuteCommand(formDesigner.Host)) {
					IMenuCommandService menuCommandService = (IMenuCommandService)formDesigner.Host.GetService(typeof(IMenuCommandService));
					menuCommandService.GlobalInvoke(CommandID);
				}
			} catch (Exception e) {
				MessageService.ShowException(e);
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

		FormsDesignerViewContent FormDesigner {
			get {
				IWorkbenchWindow window = SD.Workbench.ActiveWorkbenchWindow;
				if (window == null) {
					return null;
				}
				return window.ActiveViewContent as FormsDesignerViewContent;
			}
		}
		
		public override void Run()
		{
			IWorkbenchWindow window = SD.Workbench.ActiveWorkbenchWindow;
			if (window == null) {
				return;
			}
			
			FormsDesignerViewContent formDesigner = FormDesigner;
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
			PadDescriptor padContent = SD.Workbench.GetPad(typeof(ICSharpCode.SharpDevelop.Gui.PropertyPad));
			if (padContent != null) {
				padContent.BringPadToFront();
			}
		}
	}
	
	public class DesignerVerbSubmenuBuilder : IMenuItemBuilder
	{
		public IEnumerable<object> BuildItems(Codon codon, object owner)
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)owner;
			
			List<ToolStripItem> items = new List<ToolStripItem>();
			
			foreach (DesignerVerb verb in menuCommandService.Verbs) {
				items.Add(new ContextMenuCommand(verb));
			}
			
			// add separator at the end of custom designer verbs
			if (items.Count > 0) {
				items.Add(new MenuSeparator());
			}
			
			return items;
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
					MessageService.ShowException(ex);
				}
			}
		}
	}
	
	#region Align Commands
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
	#endregion

	#region Make Same Size Commands
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
	#endregion

	#region Horizontal Spacing Commands
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
	#endregion
	
	#region Vertical Spacing Commands
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
	#endregion

	#region Center Commands
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
	#endregion
	
	#region Order Commands
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
	#endregion

	#region Tray Commands
	
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
		FormsDesignerViewContent FormDesigner {
			get {
				IWorkbenchWindow window = SD.Workbench.ActiveWorkbenchWindow;
				if (window == null) {
					return null;
				}
				return window.ActiveViewContent as FormsDesignerViewContent;
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
				FormsDesignerViewContent formDesigner = FormDesigner;
				if (formDesigner != null) {
					return formDesigner.Host.GetService(typeof(ComponentTray)) as ComponentTray;
				}
				return null;
				
			}
		}
	}
	#endregion

	#region Global Commands
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
				FormsDesignerViewContent formDesigner = FormDesigner;
				if (formDesigner != null) {
					return formDesigner.IsTabOrderMode;
				}
				return false;
			}
			set {
				SetTabOrder(value);
			}
		}
		FormsDesignerViewContent FormDesigner {
			get {
				IWorkbenchWindow window = SD.Workbench.ActiveWorkbenchWindow;
				if (window == null) {
					return null;
				}
				return window.ActiveViewContent as FormsDesignerViewContent;
			}
		}
		
		void SetTabOrder(bool show)
		{
			FormsDesignerViewContent formDesigner = FormDesigner;
			if (formDesigner != null) {
				if (show) {
					formDesigner.ShowTabOrder();
				} else {
					formDesigner.HideTabOrder();
				}
			}
		}
	}
	#endregion
}
