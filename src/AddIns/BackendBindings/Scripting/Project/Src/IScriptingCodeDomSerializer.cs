// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

namespace ICSharpCode.Scripting
{
	public interface IScriptingCodeDomSerializer
	{
		string GenerateInitializeComponentMethodBody(IDesignerHost host, IDesignerSerializationManager serializationManager, string rootNamespace, int initialIndent);
		string GenerateInitializeComponentMethodBody(IDesignerHost host, IDesignerSerializationManager serializationManager);
	}
}
