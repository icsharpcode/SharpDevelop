// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Reflection;
using System.Xml;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public class ReflectionParameter : AbstractParameter
	{
		ParameterInfo parameterInfo;
		public override IReturnType ReturnType {
			get {
				return new ReflectionReturnType(parameterInfo.ParameterType);
			}
			set {
			}
		}
		
		public ReflectionParameter(ParameterInfo parameterInfo)
		{
			this.parameterInfo = parameterInfo;
			Name       = parameterInfo.Name;
			
			if (parameterInfo.IsOut) {
				modifier |= ParameterModifier.Out;
			}
			
			Type type = parameterInfo.ParameterType;
			// TODO read param attribute
			//if (type.IsArray && type != typeof(Array) && Attribute.IsDefined(parameterInfo, typeof(ParamArrayAttribute), true)) {
			//	modifier |= ParameterModifier.Params;
			//}
			
			// seems there is no other way to determine a ref parameter
			if (type.Name.EndsWith("&")) {
				modifier |= ParameterModifier.Ref;
			}
		}
	}
}
