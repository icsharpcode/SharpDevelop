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
using System.ComponentModel.Design.Serialization;
using System.Workflow.ComponentModel.Compiler;
using System.CodeDom;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
#endregion

namespace WorkflowDesigner.Loaders
{
	/// <summary>
	/// Description of XomlCodeSeparationDesignerLoader.
	/// </summary>
	public class XomlCodeSeparationDesignerLoader : XomlDesignerLoader
	{
		private string codeFileName;
		
		public XomlCodeSeparationDesignerLoader(IViewContent viewContent, Stream stream, string codeFileName) : base(viewContent, stream)
		{
			this.codeFileName = codeFileName;
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			
			// TODO: Install the Add the additional services into the designer here.
			LoaderHost.AddService(typeof(IMemberCreationService), new MemberCreationService(LoaderHost));
			LoaderHost.AddService(typeof(IEventBindingService), new CSharpWorkflowDesignerEventBindingService(LoaderHost,codeFileName));
		}
		
		protected override void DoPerformLoad(IDesignerSerializationManager serializationManager)
		{
			IWorkflowDesignerEventBindingService srv = LoaderHost.GetService(typeof(IEventBindingService)) as IWorkflowDesignerEventBindingService;
			srv.UpdateCodeCompileUnit();
			
			LoadXoml(serializationManager);
		}
      

		
	}
}
