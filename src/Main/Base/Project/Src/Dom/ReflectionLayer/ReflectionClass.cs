// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public class ReflectionClass : DefaultClass
	{
		Type type;
		
		BindingFlags flags = BindingFlags.Instance  |
		                     BindingFlags.Static    | 
		                     BindingFlags.NonPublic |
		                     BindingFlags.DeclaredOnly |
		                     BindingFlags.Public;
		
		public override List<IClass> InnerClasses {
			get {
				List<IClass> innerClasses = new List<IClass>();
				foreach (Type nestedType in type.GetNestedTypes(flags)) {
					innerClasses.Add(new ReflectionClass(CompilationUnit, nestedType, this));
				}
				return innerClasses;
			}
		}
		
		public override List<IField> Fields {
			get {
				List<IField> fields = new List<IField>();
				foreach (FieldInfo field in type.GetFields(flags)) {
					IField newField = new ReflectionField(field, this);
					if (!newField.IsInternal) {
						fields.Add(newField);
					}
				}
				return fields;
			}
		}
		
		public override List<IProperty> Properties {
			get {
				List<IProperty> properties = new List<IProperty>();
				foreach (PropertyInfo propertyInfo in type.GetProperties(flags)) {
					ParameterInfo[] p = null;
					
					// we may not get the permission to access the index parameters
					try {
						p = propertyInfo.GetIndexParameters();
					} catch (Exception) {}
					if (p == null || p.Length == 0) {
						properties.Add(new ReflectionProperty(propertyInfo, this));
					}
				}
				
				return properties;
			}
		}
		
		public override List<IIndexer> Indexer {
			get {
				List<IIndexer> indexer = new List<IIndexer>();
				foreach (PropertyInfo propertyInfo in type.GetProperties(flags)) {
					ParameterInfo[] p = null;
					
					// we may not get the permission to access the index parameters
					try {
						p = propertyInfo.GetIndexParameters();
					} catch (Exception) {}
					if (p != null && p.Length != 0) {
						indexer.Add(new ReflectionIndexer(propertyInfo, this));
					}
				}
				
				return indexer;
			}
		}
		
		public override List<IMethod> Methods {
			get {
				List<IMethod> methods = new List<IMethod>();
				
				foreach (ConstructorInfo constructorInfo in type.GetConstructors(flags)) {
					IMethod newMethod = new ReflectionMethod(constructorInfo, this);
					if (!newMethod.IsInternal) {
						methods.Add(newMethod);
					}
				}
				
				foreach (MethodInfo methodInfo in type.GetMethods(flags)) {
					if (!methodInfo.IsSpecialName) {
						IMethod newMethod = new ReflectionMethod(methodInfo, this);
						if (!newMethod.IsInternal) {
							methods.Add(newMethod);
						}
					}
				}
				return methods;
			}
		}
		
		public override List<IEvent> Events {
			get {
				List<IEvent> events = new List<IEvent>();
				
				foreach (EventInfo eventInfo in type.GetEvents(flags)) {
					IEvent newEvent = new ReflectionEvent(eventInfo, this);
					
					if (!newEvent.IsInternal) {
						events.Add(newEvent);
					}
				}
				return events;
			}
		}
		
		public static bool IsDelegate(Type type)
		{
			return type.IsSubclassOf(typeof(Delegate)) && type != typeof(MulticastDelegate);
		}
		
		public override string DocumentationTag {
			get {
				return "T:" + type.FullName;
			}
		}
		
		public ReflectionClass(ICompilationUnit compilationUnit, Type type, IClass declaringType) : base(compilationUnit, declaringType)
		{
			this.type = type;
			FullyQualifiedName = type.FullName.Replace('+', '.');
			
			// set classtype
			if (IsDelegate(type)) {
				this.ClassType = ClassType.Delegate;
				MethodInfo invoke          = type.GetMethod("Invoke");
				ReflectionMethod newMethod = new ReflectionMethod(invoke, this);
				Methods.Add(newMethod);
			} else if (type.IsInterface) {
				this.ClassType = ClassType.Interface;
			} else if (type.IsEnum) {
				this.ClassType = ClassType.Enum;
			} else if (type.IsValueType) {
				this.ClassType = ClassType.Struct;
			} else {
				this.ClassType = ClassType.Class;
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
			} else if (type.IsNestedFamORAssem) {
				modifiers |= ModifierEnum.ProtectedOrInternal;
			} else if (type.IsNestedFamANDAssem) {
				modifiers |= ModifierEnum.Protected;
				modifiers |= ModifierEnum.Internal;
			}
			this.Modifiers = modifiers;
			
			// set base classes
			if (type.BaseType != null) { // it's null for System.Object ONLY !!!
				BaseTypes.Add(type.BaseType.FullName);
			}
			
			// add members
			foreach (Type iface in type.GetInterfaces()) {
				BaseTypes.Add(iface.FullName);
			}
		
		}
	}
}
