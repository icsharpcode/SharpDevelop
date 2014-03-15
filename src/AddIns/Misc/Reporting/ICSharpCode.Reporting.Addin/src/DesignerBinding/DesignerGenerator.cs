/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 22.02.2014
 * Time: 19:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Core;
using ICSharpCode.Reporting.Addin.Views;

namespace ICSharpCode.Reporting.Addin.DesignerBinding
{
	/// <summary>
	/// Description of DesignerGenerator.
	/// </summary>
	public class DesignerGenerator:IDesignerGenerator
	{
		DesignerView viewContent;
		
		public DesignerGenerator()
		{
			LoggingService.Info("Create DesignerGenerator");
		}
		
		#region IDesignerGenerator implementation
		
		public void Attach(DesignerView viewContent)
		{
			if (viewContent == null)
				throw new ArgumentNullException("viewContent");
			LoggingService.Info("DesignerGenerator:Attach");
			this.viewContent = viewContent;
		}
		
		public void Detach()
		{
			throw new NotImplementedException();
		}
		
		public System.Collections.Generic.IEnumerable<ICSharpCode.SharpDevelop.Workbench.OpenedFile> GetSourceFiles(out ICSharpCode.SharpDevelop.Workbench.OpenedFile designerCodeFile)
		{
			throw new NotImplementedException();
		}
		
		public void MergeFormChanges(System.CodeDom.CodeCompileUnit unit)
		{
			throw new NotImplementedException();
		}
		
		public bool InsertComponentEvent(System.ComponentModel.IComponent component, System.ComponentModel.EventDescriptor edesc, string eventMethodName, string body, out string file, out int position)
		{
			throw new NotImplementedException();
		}
		
		public System.CodeDom.Compiler.CodeDomProvider CodeDomProvider {
			get {
				throw new NotImplementedException();
			}
		}
		
		public DesignerView ViewContent {
			get {return viewContent;}
			
		}
		
		#endregion
	}
}
