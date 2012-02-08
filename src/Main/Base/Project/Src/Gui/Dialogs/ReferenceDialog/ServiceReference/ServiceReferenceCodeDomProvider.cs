// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ServiceReferenceCodeDomProvider : ICodeDomProvider
	{
		IProject project;
		CodeDomProvider codeDomProvider;
		
		public ServiceReferenceCodeDomProvider(IProject project)
		{
			this.project = project;
		}
		
		public string FileExtension {
			get { return CodeDomProvider.FileExtension; }
		}
		
		CodeDomProvider CodeDomProvider {
			get {
				if (codeDomProvider == null) {
					codeDomProvider = project.CreateCodeDomProvider();
				}
				return codeDomProvider;
			}
		}
		
		public void GenerateCodeFromCompileUnit(CodeCompileUnit compileUnit, string fileName)
		{
			using (var writer = new StreamWriter(fileName)) {
				CodeDomProvider.GenerateCodeFromCompileUnit(compileUnit, writer, null);
			}
		}
	}
}
