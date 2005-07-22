// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Xml;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public class ReflectionParameter : DefaultParameter
	{
		ParameterInfo parameterInfo;
		IMember member;
		
		public override IReturnType ReturnType {
			get {
				return ReflectionReturnType.Create(member, parameterInfo.ParameterType, false);
			}
			set {
			}
		}
		
		public ReflectionParameter(ParameterInfo parameterInfo, IMember member) : base(parameterInfo.Name)
		{
			this.parameterInfo = parameterInfo;
			this.member = member;
			
			if (parameterInfo.IsOut) {
				modifier = ParameterModifiers.Out;
			}
			Type type = parameterInfo.ParameterType;
			// TODO read param attribute
			//if (type.IsArray && type != typeof(Array) && Attribute.IsDefined(parameterInfo, typeof(ParamArrayAttribute), true)) {
			//	modifier |= ParameterModifier.Params;
			//}
			
			// seems there is no other way to determine a ref parameter
			if (type.Name.EndsWith("&")) {
				modifier |= ParameterModifiers.Ref;
			}
		}
	}
}
