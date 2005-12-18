// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FormsDesigner
{
	public interface IDesignerGenerator
	{
		CodeDomProvider CodeDomProvider {
			get;
		}
		void Attach(FormsDesignerViewContent viewContent);
		void Detach();
		void MergeFormChanges(CodeCompileUnit unit);
		bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out string file, out int position);
		ICollection GetCompatibleMethods(EventDescriptor edesc);
		ICollection GetCompatibleMethods(EventInfo edesc);
	}
}
