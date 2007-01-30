// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.Text;
using System.IO;
using ICSharpCode.SharpDevelop.Gui;
using System.ComponentModel.Design;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;
using System.CodeDom;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
#endregion

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of XomlCodeSeparationDesignerLoader.
	/// </summary>
	public class XomlCodeSeparationDesignerLoader : XomlDesignerLoader
	{
		private string codeFileName;
		
		public XomlCodeSeparationDesignerLoader(IViewContent viewContent, string fileName, Stream stream, string codeFileName) : base(viewContent, fileName, stream)
		{
			this.codeFileName = codeFileName;
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			
			// TODO: Install the Add the additional services into the designer here.
			LoaderHost.AddService(typeof(IMemberCreationService), new MemberCreationService());
			LoaderHost.AddService(typeof(IEventBindingService), new CSharpWorkflowDesignerEventBindingService(LoaderHost,codeFileName));
		}
		
		protected override void Load()
		{
			IWorkflowDesignerEventBindingService srv = LoaderHost.GetService(typeof(IEventBindingService)) as IWorkflowDesignerEventBindingService;
			srv.UpdateCCU();
			
			LoadFromXoml();
			
			LoaderHost.Activate();
		}
      

		
	}
}
