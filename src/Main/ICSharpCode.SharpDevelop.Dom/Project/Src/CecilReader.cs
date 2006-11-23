// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;

using Mono.Cecil;
using AssemblyName = System.Reflection.AssemblyName;

namespace ICSharpCode.SharpDevelop.Dom
{
	public static class CecilReader
	{
		public static ReflectionProjectContent LoadAssembly(string fileName, ProjectContentRegistry registry)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (registry == null)
				throw new ArgumentNullException("registry");
			LoggingService.Info("Cecil: Load from " + fileName);
			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				AssemblyDefinition asm = AssemblyFactory.GetAssembly(fs);
				List<AssemblyName> referencedAssemblies = new List<AssemblyName>();
				foreach (AssemblyNameReference anr in asm.MainModule.AssemblyReferences) {
					referencedAssemblies.Add(new AssemblyName(anr.FullName));
				}
				return new CecilProjectContent(asm.Name.FullName, fileName, referencedAssemblies.ToArray(), asm.MainModule.Types, registry);
			}
		}
		
		static void AddAttributes(IProjectContent pc, IList<IAttribute> list, CustomAttributeCollection attributes)
		{
			foreach (CustomAttribute att in attributes) {
				DefaultAttribute a = new DefaultAttribute(att.Constructor.DeclaringType.FullName);
				// TODO: add only attributes marked "important", and include attribute arguments
				list.Add(a);
			}
		}
		
		static void AddConstraintsFromType(ITypeParameter tp, GenericParameter g)
		{
			foreach (TypeReference constraint in g.Constraints) {
				if (tp.Method != null) {
					tp.Constraints.Add(CreateType(tp.Class.ProjectContent, tp.Method, constraint));
				} else {
					tp.Constraints.Add(CreateType(tp.Class.ProjectContent, tp.Class, constraint));
				}
			}
		}
		
		/// <summary>
		/// Create a SharpDevelop return type from a Cecil type reference.
		/// </summary>
		internal static IReturnType CreateType(IProjectContent pc, IDecoration member, TypeReference type)
		{
			while (type is ModType) {
				type = (type as ModType).ElementType;
			}
			if (type == null) {
				LoggingService.Warn("CecilReader: Null type for: " + member);
				return VoidReturnType.Instance;
			}
			if (type is ReferenceType) {
				// TODO: Use ByRefRefReturnType
				return CreateType(pc, member, (type as ReferenceType).ElementType);
			} else if (type is ArrayType) {
				return new ArrayReturnType(pc, CreateType(pc, member, (type as ArrayType).ElementType), (type as ArrayType).Rank);
			} else if (type is GenericInstanceType) {
				GenericInstanceType gType = (GenericInstanceType)type;
				IReturnType[] para = new IReturnType[gType.GenericArguments.Count];
				for (int i = 0; i < para.Length; ++i) {
					para[i] = CreateType(pc, member, gType.GenericArguments[i]);
				}
				return new ConstructedReturnType(CreateType(pc, member, gType.ElementType), para);
			} else if (type is GenericParameter) {
				GenericParameter typeGP = type as GenericParameter;
				if (typeGP.Owner is MethodDefinition) {
					IMethod method = member as IMethod;
					if (method != null) {
						if (typeGP.Position < method.TypeParameters.Count) {
							return new GenericReturnType(method.TypeParameters[typeGP.Position]);
						}
					}
					return new GenericReturnType(new DefaultTypeParameter(method, typeGP.Name, typeGP.Position));
				} else {
					IClass c = (member is IClass) ? (IClass)member : (member is IMember) ? ((IMember)member).DeclaringType : null;
					if (c != null && typeGP.Position < c.TypeParameters.Count) {
						if (c.TypeParameters[typeGP.Position].Name == type.Name) {
							return new GenericReturnType(c.TypeParameters[typeGP.Position]);
						}
					}
					return new GenericReturnType(new DefaultTypeParameter(c, typeGP.Name, typeGP.Position));
				}
			} else {
				string name = type.FullName;
				if (name == null)
					throw new ApplicationException("type.FullName returned null. Type: " + type.ToString());
				
				if (name.IndexOf('/') > 0) {
					name = name.Replace('/', '.');
				}
				int typeParameterCount = 0;
				if (name.Length > 2 && name[name.Length - 2] == '`') {
					typeParameterCount = name[name.Length - 1] - '0';
					name = name.Substring(0, name.Length - 2);
				}
				
				IClass c = pc.GetClass(name, typeParameterCount);
				if (c != null) {
					return c.DefaultReturnType;
				} else {
					// example where name is not found: pointers like System.Char*
					// or when the class is in a assembly that is not referenced
					return new GetClassReturnType(pc, name, typeParameterCount);
				}
			}
		}
		
		private sealed class CecilProjectContent : ReflectionProjectContent
		{
			public CecilProjectContent(string fullName, string fileName, AssemblyName[] referencedAssemblies,
			                           TypeDefinitionCollection types, ProjectContentRegistry registry)
				: base(fullName, fileName, referencedAssemblies, registry)
			{
				foreach (TypeDefinition td in types) {
					if ((td.Attributes & TypeAttributes.Public) == TypeAttributes.Public) {
						if ((td.Attributes & TypeAttributes.NestedAssembly) == TypeAttributes.NestedAssembly
						    || (td.Attributes & TypeAttributes.NestedPrivate) == TypeAttributes.NestedPrivate
						    || (td.Attributes & TypeAttributes.NestedFamANDAssem) == TypeAttributes.NestedFamANDAssem)
						{
							continue;
						}
						string name = td.FullName;
						if (name.Length == 0 || name[0] == '<')
							continue;
						if (name.Length > 2 && name[name.Length - 2] == '`')
							name = name.Substring(0, name.Length - 2);
						AddClassToNamespaceListInternal(new CecilClass(this.AssemblyCompilationUnit, null, td, name));
					}
				}
				InitializeSpecialClasses();
			}
		}
		
		private sealed class CecilClass : DefaultClass
		{
			public static bool IsDelegate(TypeDefinition type)
			{
				if (type.BaseType == null)
					return false;
				else
					return type.BaseType.FullName == "System.Delegate"
						|| type.BaseType.FullName == "System.MulticastDelegate";
			}
			
			public CecilClass(ICompilationUnit compilationUnit, IClass declaringType,
			                  TypeDefinition td, string fullName)
				: base(compilationUnit, declaringType)
			{
				this.FullyQualifiedName = fullName;
				this.UseInheritanceCache = true;
				
				AddAttributes(compilationUnit.ProjectContent, this.Attributes, td.CustomAttributes);
				
				// set classtype
				if (td.IsInterface) {
					this.ClassType = ClassType.Interface;
				} else if (td.IsEnum) {
					this.ClassType = ClassType.Enum;
				} else if (td.IsValueType) {
					this.ClassType = ClassType.Struct;
				} else if (IsDelegate(td)) {
					this.ClassType = ClassType.Delegate;
				} else {
					this.ClassType = ClassType.Class;
				}
				if (td.GenericParameters.Count > 0) {
					foreach (GenericParameter g in td.GenericParameters) {
						this.TypeParameters.Add(new DefaultTypeParameter(this, g.Name, g.Position));
					}
					int i = 0;
					foreach (GenericParameter g in td.GenericParameters) {
						AddConstraintsFromType(this.TypeParameters[i++], g);
					}
				}
				
				ModifierEnum modifiers  = ModifierEnum.None;
				
				if (td.IsSealed) {
					modifiers |= ModifierEnum.Sealed;
				}
				if (td.IsAbstract) {
					modifiers |= ModifierEnum.Abstract;
				}
				
				if ((td.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPublic) {
					modifiers |= ModifierEnum.Public;
				} else if ((td.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamily) {
					modifiers |= ModifierEnum.Protected;
				} else if ((td.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamORAssem) {
					modifiers |= ModifierEnum.Protected;
				} else {
					modifiers |= ModifierEnum.Public;
				}
				
				this.Modifiers = modifiers;
				
				// set base classes
				if (td.BaseType != null) {
					BaseTypes.Add(CreateType(this.ProjectContent, this, td.BaseType));
				}
				
				foreach (TypeReference iface in td.Interfaces) {
					BaseTypes.Add(CreateType(this.ProjectContent, this, iface));
				}
				
				ReflectionLayer.ReflectionClass.ApplySpecialsFromAttributes(this);
				
				InitMembers(td);
			}
			
			void InitMembers(TypeDefinition type)
			{
				string defaultMemberName = null;
				foreach (CustomAttribute att in type.CustomAttributes) {
					if (att.Constructor.DeclaringType.FullName == "System.Reflection.DefaultMemberAttribute"
					    && att.ConstructorParameters.Count == 1)
					{
						defaultMemberName = att.ConstructorParameters[0] as string;
					}
				}
				
				foreach (TypeDefinition nestedType in type.NestedTypes) {
					TypeAttributes visibility = nestedType.Attributes & TypeAttributes.VisibilityMask;
					if (visibility == TypeAttributes.NestedPublic || visibility == TypeAttributes.NestedFamily
					    || visibility == TypeAttributes.NestedFamORAssem)
					{
						string name = nestedType.Name;
						int pos = name.LastIndexOf('/');
						if (pos > 0)
							name = name.Substring(pos + 1);
						if (name.Length == 0 || name[0] == '<')
							continue;
						if (name.Length > 2 && name[name.Length - 2] == '`')
							name = name.Substring(0, name.Length - 2);
						name = this.FullyQualifiedName + "." + name;
						InnerClasses.Add(new CecilClass(this.CompilationUnit, this, nestedType, name));
					}
				}
				
				foreach (FieldDefinition field in type.Fields) {
					if (IsVisible(field.Attributes) && !field.IsSpecialName) {
						DefaultField f = new DefaultField(this, field.Name);
						f.Modifiers = TranslateModifiers(field);
						f.ReturnType = CreateType(this.ProjectContent, this, field.FieldType);
						Fields.Add(f);
					}
				}
				
				foreach (PropertyDefinition property in type.Properties) {
					if ((property.GetMethod != null && IsVisible(property.GetMethod.Attributes))
					    || (property.SetMethod != null && IsVisible(property.SetMethod.Attributes)))
					{
						DefaultProperty p = new DefaultProperty(this, property.Name);
						if (this.ClassType == ClassType.Interface) {
							p.Modifiers = ModifierEnum.Public | ModifierEnum.Abstract;
						} else {
							p.Modifiers = TranslateModifiers(property);
						}
						p.ReturnType = CreateType(this.ProjectContent, this, property.PropertyType);
						p.CanGet = property.GetMethod != null;
						p.CanSet = property.SetMethod != null;
						if (p.Name == defaultMemberName) {
							p.IsIndexer = true;
						}
						AddParameters(p, property.Parameters);
						Properties.Add(p);
					}
				}
				
				foreach (EventDefinition eventDef in type.Events) {
					if (eventDef.AddMethod != null && IsVisible(eventDef.AddMethod.Attributes)) {
						DefaultEvent e = new DefaultEvent(this, eventDef.Name);
						if (this.ClassType == ClassType.Interface) {
							e.Modifiers = ModifierEnum.Public | ModifierEnum.Abstract;
						} else {
							e.Modifiers = TranslateModifiers(eventDef);
						}
						e.ReturnType = CreateType(this.ProjectContent, this, eventDef.EventType);
						Events.Add(e);
					}
				}
				
				foreach (MethodDefinition method in type.Constructors) {
					AddMethod(method);
				}
				foreach (MethodDefinition method in type.Methods) {
					if (!method.IsSpecialName) {
						AddMethod(method);
					}
				}
			}
			
			void AddMethod(MethodDefinition method)
			{
				if (IsVisible(method.Attributes)) {
					DefaultMethod m = new DefaultMethod(this, method.IsConstructor ? "#ctor" : method.Name);
					
					if (method.GenericParameters.Count > 0) {
						foreach (GenericParameter g in method.GenericParameters) {
							m.TypeParameters.Add(new DefaultTypeParameter(m, g.Name, g.Position));
						}
						int i = 0;
						foreach (GenericParameter g in method.GenericParameters) {
							AddConstraintsFromType(m.TypeParameters[i++], g);
						}
					}
					
					m.ReturnType = CreateType(this.ProjectContent, m, method.ReturnType.ReturnType);
					if (this.ClassType == ClassType.Interface) {
						m.Modifiers = ModifierEnum.Public | ModifierEnum.Abstract;
					} else {
						m.Modifiers = TranslateModifiers(method);
					}
					AddParameters(m, method.Parameters);
					AddExplicitInterfaceImplementations(method.Overrides, m);
					Methods.Add(m);
				}
			}
			
			void AddExplicitInterfaceImplementations(OverrideCollection overrides, IMember targetMember)
			{
				foreach (MethodReference overrideRef in overrides) {
					if (overrideRef.Name == targetMember.Name && targetMember.IsPublic) {
						continue; // is like implicit interface implementation / normal override
					}
					targetMember.InterfaceImplementations.Add(new ExplicitInterfaceImplementation(
						CreateType(this.ProjectContent, targetMember, overrideRef.DeclaringType),
						overrideRef.Name
					));
				}
			}
			
			void AddParameters(IMethodOrProperty target, ParameterDefinitionCollection plist)
			{
				foreach (ParameterDefinition par in plist) {
					IReturnType pReturnType = CreateType(this.ProjectContent, target, par.ParameterType);
					DefaultParameter p = new DefaultParameter(par.Name, pReturnType, DomRegion.Empty);
					if ((par.Attributes & ParameterAttributes.Out) == ParameterAttributes.Out) {
						p.Modifiers = ParameterModifiers.Out;
					} else if (par.ParameterType is ReferenceType) {
						p.Modifiers = ParameterModifiers.Ref;
					} else {
						p.Modifiers = ParameterModifiers.In;
					}
					if ((par.Attributes & ParameterAttributes.Optional) == ParameterAttributes.Optional) {
						p.Modifiers |= ParameterModifiers.Optional;
					}
					if (p.ReturnType.IsArrayReturnType) {
						foreach (CustomAttribute att in par.CustomAttributes) {
							if (att.Constructor.DeclaringType.FullName == typeof(ParamArrayAttribute).FullName) {
								p.Modifiers |= ParameterModifiers.Params;
							}
						}
					}
					target.Parameters.Add(p);
				}
			}
			
			static bool IsVisible(MethodAttributes att)
			{
				return ((att & MethodAttributes.Public) == MethodAttributes.Public)
					|| ((att & MethodAttributes.Family) == MethodAttributes.Family)
					|| ((att & MethodAttributes.FamORAssem) == MethodAttributes.FamORAssem);
			}
			
			static bool IsVisible(FieldAttributes att)
			{
				return ((att & FieldAttributes.Public) == FieldAttributes.Public)
					|| ((att & FieldAttributes.Family) == FieldAttributes.Family)
					|| ((att & FieldAttributes.FamORAssem) == FieldAttributes.FamORAssem);
			}
			
			static ModifierEnum TranslateModifiers(MethodDefinition method)
			{
				ModifierEnum m = ModifierEnum.None;
				
				if (method.IsStatic)
					m |= ModifierEnum.Static;
				
				if (method.IsAbstract) {
					m |= ModifierEnum.Abstract;
				} else if (method.Overrides.Count > 0) {
					if (method.IsFinal) {
						m |= ModifierEnum.Sealed;
					} else {
						m |= ModifierEnum.Override;
					}
				} else if (method.IsVirtual) {
					m |= ModifierEnum.Virtual;
				}
				
				if ((method.Attributes & MethodAttributes.Public) == MethodAttributes.Public)
					m |= ModifierEnum.Public;
				else
					m |= ModifierEnum.Protected;
				
				return m;
			}
			
			static ModifierEnum TranslateModifiers(PropertyDefinition property)
			{
				return TranslateModifiers(property.GetMethod ?? property.SetMethod);
			}
			
			static ModifierEnum TranslateModifiers(EventDefinition @event)
			{
				return TranslateModifiers(@event.AddMethod);
			}
			
			static ModifierEnum TranslateModifiers(FieldDefinition field)
			{
				ModifierEnum m = ModifierEnum.None;
				
				if (field.IsStatic)
					m |= ModifierEnum.Static;
				
				if (field.IsLiteral)
					m |= ModifierEnum.Const;
				else if (field.IsReadOnly)
					m |= ModifierEnum.Readonly;
				
				if ((field.Attributes & FieldAttributes.Public) == FieldAttributes.Public)
					m |= ModifierEnum.Public;
				else
					m |= ModifierEnum.Protected;
				
				return m;
			}
		}
	}
}
