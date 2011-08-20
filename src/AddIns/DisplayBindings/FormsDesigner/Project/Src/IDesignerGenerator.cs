// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace ICSharpCode.FormsDesigner
{
	public interface IDesignerGenerator
	{
		void MergeFormChanges(CodeCompileUnit unit);
		bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out string file, out int position);
		ICollection GetCompatibleMethods(EventDescriptor edesc);
		void NotifyFormRenamed(string newName);
		Type CodeDomProviderType { get; }
		CodeDomProvider CreateCodeDomProvider();
	}
}
