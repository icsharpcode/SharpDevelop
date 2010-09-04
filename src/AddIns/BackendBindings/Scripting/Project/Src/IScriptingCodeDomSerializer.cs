// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

namespace ICSharpCode.Scripting
{
	public interface IScriptingCodeDomSerializer
	{
		string GenerateInitializeComponentMethodBody(IDesignerHost host, IDesignerSerializationManager serializationManager, string rootNamespace, int initialIndent);
	}
}
