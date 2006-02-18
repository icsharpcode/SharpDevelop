// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Microsoft.CSharp;

namespace WrapperGenerator
{
	public class CorSymGenerator: CodeGenerator
	{
		string namespaceToWrap = "Debugger.Interop.CorSym";
		
		public CorSymGenerator(Assembly assembly):base(assembly)
		{
			wrapperNamespace = "Debugger.Wrappers.CorSym";
		}
		
		protected override bool ShouldIncludeType(Type type)
		{
			return type.Namespace == namespaceToWrap &&
			       (type.IsClass || type.IsInterface || type.IsEnum);
		}
		
		protected override Type GetBaseClass(Type type) 
		{
//			Type[] interfaces = type.GetInterfaces();
//			if (interfaces.Length > 0) {
//				return interfaces[0];
//			}
//			if (type.Name.EndsWith("2")) {
//				return type.Assembly.GetType(type.FullName.Remove(type.FullName.Length - 1));
//			}
			return null;
		}
	}
}
