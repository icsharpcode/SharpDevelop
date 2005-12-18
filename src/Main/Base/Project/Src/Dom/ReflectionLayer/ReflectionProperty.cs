// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Xml;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public class ReflectionProperty : DefaultProperty
	{
		public ReflectionProperty(PropertyInfo propertyInfo, IClass declaringType) : base(declaringType, propertyInfo.Name)
		{
			this.ReturnType = ReflectionReturnType.Create(this, propertyInfo.PropertyType, false);
			
			CanGet = propertyInfo.CanRead;
			CanSet = propertyInfo.CanWrite;
			
			ParameterInfo[] parameterInfo = propertyInfo.GetIndexParameters();
			if (parameterInfo != null && parameterInfo.Length > 0) {
				// check if this property is an indexer (=default member of parent class)
				foreach (MemberInfo memberInfo in propertyInfo.DeclaringType.GetDefaultMembers()) {
					if (memberInfo == propertyInfo) {
						this.IsIndexer = true;
						break;
					}
				}
				// there are only few properties with parameters, so we can load them immediately
				foreach (ParameterInfo info in parameterInfo) {
					this.Parameters.Add(new ReflectionParameter(info, this));
				}
			}
			
			MethodInfo methodBase = null;
			try {
				methodBase = propertyInfo.GetGetMethod(true);
			} catch (Exception) {}
			
			if (methodBase == null) {
				try {
					methodBase = propertyInfo.GetSetMethod(true);
				} catch (Exception) {}
			}
			
			ModifierEnum modifiers  = ModifierEnum.None;
			if (methodBase != null) {
				if (methodBase.IsStatic) {
					modifiers |= ModifierEnum.Static;
				}
				
				if (methodBase.IsAssembly) {
					modifiers |= ModifierEnum.Internal;
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
				
			} else { // assume public property, if no methodBase could be get.
				modifiers = ModifierEnum.Public;
			}
			this.Modifiers = modifiers;
		}
	}
}
