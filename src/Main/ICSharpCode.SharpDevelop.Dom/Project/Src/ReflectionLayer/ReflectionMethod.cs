// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace ICSharpCode.SharpDevelop.Dom.ReflectionLayer
{
	internal class ReflectionMethod : DefaultMethod
	{
		public ReflectionMethod(MethodBase methodBase, ReflectionClass declaringType)
			: base(declaringType, methodBase is ConstructorInfo ? "#ctor" : methodBase.Name)
		{
			if (methodBase is MethodInfo) {
				this.ReturnType = ReflectionReturnType.Create(this, ((MethodInfo)methodBase).ReturnType, false);
			} else if (methodBase is ConstructorInfo) {
				this.ReturnType = DeclaringType.DefaultReturnType;
			}
			
			foreach (ParameterInfo paramInfo in methodBase.GetParameters()) {
				this.Parameters.Add(new ReflectionParameter(paramInfo, this));
			}
			
			if (methodBase.IsGenericMethodDefinition) {
				foreach (Type g in methodBase.GetGenericArguments()) {
					this.TypeParameters.Add(new DefaultTypeParameter(this, g));
				}
				int i = 0;
				foreach (Type g in methodBase.GetGenericArguments()) {
					declaringType.AddConstraintsFromType(this.TypeParameters[i++], g);
				}
			}
			
			if (methodBase.IsStatic) {
				foreach (CustomAttributeData data in CustomAttributeData.GetCustomAttributes(methodBase)) {
					string attributeName = data.Constructor.DeclaringType.FullName;
					if (attributeName == "System.Runtime.CompilerServices.ExtensionAttribute"
					    || attributeName == "Boo.Lang.ExtensionAttribute")
					{
						this.IsExtensionMethod = true;
					}
				}
			}
			ModifierEnum modifiers  = ModifierEnum.None;
			if (methodBase.IsStatic) {
				modifiers |= ModifierEnum.Static;
			}
			if (methodBase.IsPrivate) { // I assume that private is used most and public last (at least should be)
				modifiers |= ModifierEnum.Private;
			} else if (methodBase.IsFamily || methodBase.IsFamilyOrAssembly) {
				modifiers |= ModifierEnum.Protected;
			} else if (methodBase.IsPublic) {
				modifiers |= ModifierEnum.Public;
			} else {
				modifiers |= ModifierEnum.Internal;
			}
			
			if (methodBase.IsVirtual) {
				modifiers |= ModifierEnum.Virtual;
			}
			if (methodBase.IsAbstract) {
				modifiers |= ModifierEnum.Abstract;
			}
			this.Modifiers = modifiers;
		}
	}
}
