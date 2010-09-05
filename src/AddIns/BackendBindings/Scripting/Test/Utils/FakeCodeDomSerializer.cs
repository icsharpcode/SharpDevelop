// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class FakeCodeDomSerializer : IScriptingCodeDomSerializer
	{
		public string MethodBodyToReturnFromGenerateMethodBodyCall = String.Empty;
		
		public IDesignerHost HostPassedToGenerateInitializeComponentMethodBody;
		public IDesignerSerializationManager SerializationManagerGenerateInitializeComponentMethodBody;
		public string RootNamespacePassedToGenerateInitializeComponentMethodBody;
		public int InitialIndentPassedToGenerateInitializeComponentMethodBody;
		
		public string GenerateInitializeComponentMethodBody(IDesignerHost host, IDesignerSerializationManager serializationManager, string rootNamespace, int initialIndent)
		{
			HostPassedToGenerateInitializeComponentMethodBody = host;
			SerializationManagerGenerateInitializeComponentMethodBody = serializationManager;
			RootNamespacePassedToGenerateInitializeComponentMethodBody = rootNamespace;
			InitialIndentPassedToGenerateInitializeComponentMethodBody = initialIndent;
			
			return MethodBodyToReturnFromGenerateMethodBodyCall;
		}
	}
}
