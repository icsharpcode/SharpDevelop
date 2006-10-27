// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Reflection;

namespace ICSharpCode.SharpDevelop.Dom.ReflectionLayer
{
	public class ReflectionClass : DefaultClass
	{
		const BindingFlags flags = BindingFlags.Instance  |
			BindingFlags.Static    |
			BindingFlags.NonPublic |
			BindingFlags.DeclaredOnly |
			BindingFlags.Public;
		
		void InitMembers(Type type)
		{
			foreach (Type nestedType in type.GetNestedTypes(flags)) {
				if (!nestedType.IsVisible) continue;
				string name = nestedType.FullName.Replace('+', '.');
				InnerClasses.Add(new ReflectionClass(CompilationUnit, nestedType, name, this));
			}
			
			foreach (FieldInfo field in type.GetFields(flags)) {
				if (!field.IsPublic && !field.IsFamily) continue;
				if (!field.IsSpecialName) {
					Fields.Add(new ReflectionField(field, this));
				}
			}
			
			foreach (PropertyInfo propertyInfo in type.GetProperties(flags)) {
				ReflectionProperty prop = new ReflectionProperty(propertyInfo, this);
				if (prop.IsPublic || prop.IsProtected)
					Properties.Add(prop);
			}
			
			foreach (ConstructorInfo constructorInfo in type.GetConstructors(flags)) {
				if (!constructorInfo.IsPublic && !constructorInfo.IsFamily) continue;
				Methods.Add(new ReflectionMethod(constructorInfo, this));
			}
			
			foreach (MethodInfo methodInfo in type.GetMethods(flags)) {
				if (!methodInfo.IsPublic && !methodInfo.IsFamily) continue;
				if (!methodInfo.IsSpecialName) {
					Methods.Add(new ReflectionMethod(methodInfo, this));
				}
			}
			
			foreach (EventInfo eventInfo in type.GetEvents(flags)) {
				Events.Add(new ReflectionEvent(eventInfo, this));
			}
		}
		
		static bool IsDelegate(Type type)
		{
			return type.IsSubclassOf(typeof(Delegate)) && type != typeof(MulticastDelegate);
		}
		
		static void AddAttributes(IProjectContent pc, IList<IAttribute> list, IList<CustomAttributeData> attributes)
		{
			foreach (CustomAttributeData att in attributes) {
				DefaultAttribute a = new DefaultAttribute(att.Constructor.DeclaringType.FullName);
				foreach (CustomAttributeTypedArgument arg in att.ConstructorArguments) {
					IReturnType type = ReflectionReturnType.Create(pc, null, arg.ArgumentType, false);
					a.PositionalArguments.Add(new AttributeArgument(type, arg.Value));
				}
				foreach (CustomAttributeNamedArgument arg in att.NamedArguments) {
					IReturnType type = ReflectionReturnType.Create(pc, null, arg.TypedValue.ArgumentType, false);
					a.NamedArguments.Add(arg.MemberInfo.Name, new AttributeArgument(type, arg.TypedValue.Value));
				}
				list.Add(a);
			}
		}
		
		internal static void ApplySpecialsFromAttributes(DefaultClass c)
		{
			foreach (IAttribute att in c.Attributes) {
				if (att.Name == "Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute"
				    || att.Name == "Boo.Lang.ModuleAttribute")
				{
					c.ClassType = ClassType.Module;
					break;
				}
			}
		}
		
		public ReflectionClass(ICompilationUnit compilationUnit, Type type, string fullName, IClass declaringType) : base(compilationUnit, declaringType)
		{
			if (fullName.Length > 2 && fullName[fullName.Length - 2] == '`') {
				FullyQualifiedName = fullName.Substring(0, fullName.Length - 2);
			} else {
				FullyQualifiedName = fullName;
			}
			
			this.UseInheritanceCache = true;
			
			try {
				AddAttributes(compilationUnit.ProjectContent, this.Attributes, CustomAttributeData.GetCustomAttributes(type));
			} catch (Exception ex) {
				HostCallback.ShowError("Error reading custom attributes", ex);
			}
			
			// set classtype
			if (type.IsInterface) {
				this.ClassType = ClassType.Interface;
			} else if (type.IsEnum) {
				this.ClassType = ClassType.Enum;
			} else if (type.IsValueType) {
				this.ClassType = ClassType.Struct;
			} else if (IsDelegate(type)) {
				this.ClassType = ClassType.Delegate;
			} else {
				this.ClassType = ClassType.Class;
				ApplySpecialsFromAttributes(this);
			}
			if (type.IsGenericTypeDefinition) {
				foreach (Type g in type.GetGenericArguments()) {
					this.TypeParameters.Add(new DefaultTypeParameter(this, g));
				}
				int i = 0;
				foreach (Type g in type.GetGenericArguments()) {
					AddConstraintsFromType(this.TypeParameters[i++], g);
				}
			}
			
			ModifierEnum modifiers  = ModifierEnum.None;
			
			if (type.IsNestedAssembly) {
				modifiers |= ModifierEnum.Internal;
			}
			if (type.IsSealed) {
				modifiers |= ModifierEnum.Sealed;
			}
			if (type.IsAbstract) {
				modifiers |= ModifierEnum.Abstract;
			}
			
			if (type.IsNestedPrivate ) { // I assume that private is used most and public last (at least should be)
				modifiers |= ModifierEnum.Private;
			} else if (type.IsNestedFamily ) {
				modifiers |= ModifierEnum.Protected;
			} else if (type.IsNestedPublic || type.IsPublic) {
				modifiers |= ModifierEnum.Public;
			} else if (type.IsNotPublic) {
				modifiers |= ModifierEnum.Internal;
			} else if (type.IsNestedFamORAssem || type.IsNestedFamANDAssem) {
				modifiers |= ModifierEnum.Protected;
				modifiers |= ModifierEnum.Internal;
			}
			this.Modifiers = modifiers;
			
			// set base classes
			if (type.BaseType != null) { // it's null for System.Object ONLY !!!
				BaseTypes.Add(ReflectionReturnType.Create(this, type.BaseType, false));
			}
			
			foreach (Type iface in type.GetInterfaces()) {
				BaseTypes.Add(ReflectionReturnType.Create(this, iface, false));
			}
			
			InitMembers(type);
		}
		
		internal void AddConstraintsFromType(ITypeParameter tp, Type type)
		{
			foreach (Type constraint in type.GetGenericParameterConstraints()) {
				if (tp.Method != null) {
					tp.Constraints.Add(ReflectionReturnType.Create(tp.Method, constraint, false));
				} else {
					tp.Constraints.Add(ReflectionReturnType.Create(tp.Class, constraint, false));
				}
			}
		}
	}
	
}
