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
		public ReflectionParameter(ParameterInfo parameterInfo, IMember member) : base(parameterInfo.Name)
		{
			this.ReturnType = ReflectionReturnType.Create(member, parameterInfo.ParameterType, false);
			
			Type type = parameterInfo.ParameterType;
			
			if (parameterInfo.IsOut) {
				modifier = ParameterModifiers.Out;
			} else if (type.Name.EndsWith("&")) {
				// seems there is no other way to determine a ref parameter
				modifier = ParameterModifiers.Ref;
			}
			
			if (parameterInfo.IsOptional) {
				modifier |= ParameterModifiers.Optional;
			}
			if (type.IsArray && type != typeof(Array)) {
				foreach (CustomAttributeData data in CustomAttributeData.GetCustomAttributes(parameterInfo)) {
					if (data.Constructor.DeclaringType.FullName == typeof(ParamArrayAttribute).FullName) {
						modifier |= ParameterModifiers.Params;
						break;
					}
				}
			}
		}
	}
}
