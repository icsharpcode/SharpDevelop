// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.IO;
using System.ServiceModel.Description;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ServiceReferenceProxyGenerator : IServiceReferenceProxyGenerator
	{		
		IServiceReferenceCodeDomBuilder codeDomBuilder;
		ICodeDomProvider codeDomProvider;
		
		public ServiceReferenceProxyGenerator(ICodeDomProvider codeDomProvider)
			: this(codeDomProvider, new ServiceReferenceCodeDomBuilder())
		{
		}
		
		public ServiceReferenceProxyGenerator(
			ICodeDomProvider codeDomProvider,
			IServiceReferenceCodeDomBuilder codeDomBuilder)
		{
			this.codeDomProvider = codeDomProvider;
			this.codeDomBuilder = codeDomBuilder;
		}
		
		public string ServiceReferenceNamespace { get; set; }
		
		public void GenerateProxy(MetadataSet metadata, string proxyFileName)
		{
			CodeCompileUnit compileUnit = codeDomBuilder.GenerateCompileUnit(metadata);
			GenerateProxy(compileUnit, proxyFileName);
		}
		
		void GenerateProxy(CodeCompileUnit compileUnit, string fileName)
		{
			codeDomProvider.GenerateCodeFromCompileUnit(compileUnit, fileName);
		}
	}
}
