// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

using ICSharpCode.SharpDevelop;

namespace ICSharpCode.FormsDesigner
{
	public interface IDesignerGenerator
	{
		CodeDomProvider CodeDomProvider {
			get;
		}
		void Attach(FormsDesignerViewContent viewContent);
		void Detach();
		FormsDesignerViewContent ViewContent { get; }
		/// <summary>
		/// Gets the OpenedFile for the file which contains the code to be modified by the forms designer.
		/// This method must never return null. If it cannot find that file, it must throw an exception.
		/// </summary>
		OpenedFile DetermineDesignerCodeFile();
		void MergeFormChanges(CodeCompileUnit unit);
		bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out string file, out int position);
		ICollection GetCompatibleMethods(EventDescriptor edesc);
	}
}
