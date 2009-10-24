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
		MemberInfo member;
		string name;
		Type parameterType;
		int position;
		
		public override MemberInfo Member {
			get { return member; }
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override Type ParameterType {
			get { return parameterType; }
		}
		
		public override int Position {
			get { return position; }
		}
		
		public DebugParameterInfo(MemberInfo member, string name, Type parameterType, int position)
		{
			this.member = member;
			this.name = name;
			this.parameterType = parameterType;
			this.position = position;
		}
			
		//		public virtual ParameterAttributes Attributes { get; }
		//		public virtual object DefaultValue { get; }		
		//		public virtual object RawDefaultValue { get; }
		//		
		//		public virtual object[] GetCustomAttributes(bool inherit);
		//		public virtual object[] GetCustomAttributes(Type attributeType, bool inherit);
		//		public virtual Type[] GetOptionalCustomModifiers();
		//		public virtual Type[] GetRequiredCustomModifiers();
		//		public virtual bool IsDefined(Type attributeType, bool inherit);
		
		public override string ToString()
		{
			return this.ParameterType + " " + this.Name;
		}
	}
}
