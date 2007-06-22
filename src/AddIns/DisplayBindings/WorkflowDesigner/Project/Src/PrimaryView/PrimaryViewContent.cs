// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

using WorkflowDesigner.Loaders;

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of PrimaryViewContent.
	/// </summary>
	public class WorkflowPrimaryViewContent : AbstractViewContent, IHasPropertyContainer, IToolsHost
	{
		ViewContentControl control;

		public WorkflowPrimaryViewContent(OpenedFile primaryFile) : base(primaryFile)
		{
			if (primaryFile == null)
				throw new ArgumentNullException("primaryFile");
			
			this.TabPageText = "Workflow";
			control = new ViewContentControl(this);
			
			primaryFile.ForceInitializeView(this); // call Load()
			
		}
		
		public override System.Windows.Forms.Control Control {
			get {
				return control;
			}
		}

		public override void Load(OpenedFile file, Stream stream)
		{
			Debug.Assert(file == this.PrimaryFile);

			control.LoadWorkflow(new XomlDesignerLoader(this, stream));
		}
		
		public override void Save(OpenedFile file, Stream stream)
		{
			Debug.Assert(file == this.PrimaryFile);
			
			control.SaveWorkflow(stream);
		}
		
		public void LoadContent(string content)
		{
			if (content == null)
				throw new ArgumentNullException("content");
			
			XomlDesignerLoader xomlDesignerLoader = new XomlDesignerLoader(this);
			xomlDesignerLoader.Xoml = content;
			control.LoadWorkflow(xomlDesignerLoader);
		}
		
		#region IHasPropertyContainer
		public PropertyContainer PropertyContainer {
			get {
				return control.PropertyContainer;
			}
		}
		#endregion
		
		System.Windows.Forms.Control IToolsHost.ToolsControl {
			get {
				return WorkflowSideTabService.WorkflowSideBar;
			}
		}
	}
}
