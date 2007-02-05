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
#endregion

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of SecondaryViewContent.
	/// </summary>
	public class WorkflowDesignerSecondaryViewContent : AbstractSecondaryViewContent, IHasPropertyContainer
	{
		IViewContent viewContent;
		ViewContentControl control;

		#region Constructors
		public WorkflowDesignerSecondaryViewContent(IViewContent primaryViewContent) : base(primaryViewContent)
		{
			this.TabPageText = "Workflow";
			this.viewContent = primaryViewContent;
			control = new ViewContentControl(primaryViewContent);
			
			// HACK to ensure SideBarPad exists! - Normally handled by FormsDesigner
			//PadDescriptor pad = WorkbenchSingleton.Workbench.GetPad(typeof(SideBarView));
			//.pad.CreatePad();
			
		}
		#endregion
		
		#region Property Accessors
		public override Control Control {
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
			control.LoadWorkflow(new NRefactoryDesignerLoader(((ITextEditorControlProvider)viewContent).TextEditorControl, viewContent));
		}
		
		protected override void SaveToPrimary()
		{
			control.UnloadWorkflow();
		}
	}
}
