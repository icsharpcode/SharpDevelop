/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 01.05.2009
 * Zeit: 17:30
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using ICSharpCode.Reports.Addin;

namespace ICSharpCode.Reports.Addin.Test.Designer
{
	
	public class MockDesignerGenerator:IDesignerGenerator
	{
		ReportDesignerView view;
		
		public MockDesignerGenerator()
		{
			
		}
		public System.CodeDom.Compiler.CodeDomProvider CodeDomProvider {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ReportDesignerView ViewContent {
			get {
				return this.view;
			}
		}
		
		public void Attach(ReportDesignerView viewContent)
		{
		this.view = viewContent;
		}
		
		public void Detach()
		{
			throw new NotImplementedException();
		}
		
		public System.Collections.Generic.IEnumerable<ICSharpCode.SharpDevelop.OpenedFile> GetSourceFiles(out ICSharpCode.SharpDevelop.OpenedFile designerCodeFile)
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
		
	}
}
