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

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of PrimaryViewContent.
	/// </summary>
	public class WorkflowPrimaryViewContent : AbstractViewContent, IHasPropertyContainer
	{
		ViewContentControl control;

		public WorkflowPrimaryViewContent(OpenedFile primaryFile) : base(primaryFile)
		{
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
			
			XomlDesignerLoader loader = null;
			
			// First look for a code separation file.
			// FIXME:	The loader does nont know the project this belongs
			//		 	to as ProjectService.CurrentProject is not set at this point
			//			so look for all possible language files.
			LoggingService.DebugFormatted("Looking for code separation file");
			StringBuilder sb = new StringBuilder(file.FileName);
			bool found = false;
			if (File.Exists(file.FileName + ".cs")) {
				sb.Append(".cs");
				found = true;
			} else if (File.Exists(file.FileName + ".vb")) {
				sb.Append(".vb");
				found = true;
			}
			if (found) {
				string codeFileName =  sb.ToString();
				LoggingService.DebugFormatted("Found code file {0}", codeFileName);
				loader = new XomlCodeSeparationDesignerLoader(this, file.FileName, stream, codeFileName);
			}
			
			// No separation file so the nocode loader will be used.
			if (loader == null)
				loader = new XomlDesignerLoader(this, file.FileName, stream);

			control.LoadWorkflow(loader);
		}
		
		public override void Save(OpenedFile file, Stream stream)
		{
			Debug.Assert(file == this.PrimaryFile);
			
			control.SaveWorkflow(stream);
		}
		
		public void LoadContent(string content)
		{
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
	}
}
