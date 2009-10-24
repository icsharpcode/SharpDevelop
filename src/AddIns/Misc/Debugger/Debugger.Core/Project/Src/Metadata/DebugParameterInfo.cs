// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.MetaData;
using ICSharpCode.NRefactory.Ast;
using Mono.Cecil.Signatures;

namespace Debugger.MetaData
{
	public class DebugParameterInfo : System.Reflection.ParameterInfo
	{
		public override MemberInfo Member { get; internal set; }
		public override string Name { get; internal set; }
		public override Type ParameterType { get; internal set; }
		public override int Position { get; internal set; }
			
		//		public virtual ParameterAttributes Attributes { get; }
		//		public virtual object DefaultValue { get; }		
		//		public virtual object RawDefaultValue { get; }
		//		
		//		public virtual object[] GetCustomAttributes(bool inherit);
		//		public virtual object[] GetCustomAttributes(Type attributeType, bool inherit);
		//		public virtual Type[] GetOptionalCustomModifiers();
		//		public virtual Type[] GetRequiredCustomModifiers();
		//		public virtual bool IsDefined(Type attributeType, bool inherit);
	}
}
