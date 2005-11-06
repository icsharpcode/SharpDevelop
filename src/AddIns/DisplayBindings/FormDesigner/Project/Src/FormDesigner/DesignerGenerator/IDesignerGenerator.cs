// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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

namespace ICSharpCode.FormDesigner
{
	public interface IDesignerGenerator
	{
		CodeDomProvider CodeDomProvider {
			get;
		}
		void Attach(FormDesignerViewContent viewContent);
		void Detach();
		void MergeFormChanges(CodeCompileUnit unit);
		bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out int position);
		ICollection GetCompatibleMethods(EventDescriptor edesc);
		ICollection GetCompatibleMethods(EventInfo edesc);
	}
}
