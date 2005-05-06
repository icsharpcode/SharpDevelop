// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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
		IProjectContent content;
		
		public override IReturnType ReturnType {
			get {
				return ReflectionReturnType.Create(content, parameterInfo.ParameterType);
			}
			set {
			}
		}
		
		public ReflectionParameter(ParameterInfo parameterInfo, IProjectContent content) : base(parameterInfo.Name)
		{
			this.parameterInfo = parameterInfo;
			this.content = content;
			
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
