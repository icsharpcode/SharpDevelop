// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

using WorkflowDesigner.Loaders;
#endregion

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of SecondaryViewContent.
	/// </summary>
	public class WorkflowDesignerSecondaryViewContent : AbstractSecondaryViewContent, IHasPropertyContainer, IToolsHost
	{
		ViewContentControl control;

		#region Constructors
		public WorkflowDesignerSecondaryViewContent(IViewContent primaryViewContent) : base(primaryViewContent)
		{
			this.TabPageText = "Workflow";
			control = new ViewContentControl(primaryViewContent);
		}
		#endregion
		
		#region Property Accessors
		public override object Content {
			get {
				return control;
			}
		}
		
		public PropertyContainer PropertyContainer {
			get {
				return control.PropertyContainer;
			}
		}
		#endregion


		protected override void LoadFromPrimary()
		{
			control.LoadWorkflow(new CodeDesignerLoader(this.PrimaryViewContent));
		}
		
		protected override void SaveToPrimary()
		{
			control.UnloadWorkflow();
		}
		
		System.Windows.Forms.Control IToolsHost.ToolsControl {
			get {
				return WorkflowSideTabService.WorkflowSideBar;
			}
		}
	}
}
