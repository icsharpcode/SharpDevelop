// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public string GenerateInitializeComponentMethodBody(IDesignerHost host, IDesignerSerializationManager serializationManager)
		{
			return GenerateInitializeComponentMethodBody(host, serializationManager, String.Empty, 0);
		}
	}
}
