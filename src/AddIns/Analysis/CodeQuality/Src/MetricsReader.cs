using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace ICSharpCode.CodeQualityAnalysis
{
	/// <summary>
	/// Reads neccesery information with Mono.Cecil to calculate code metrics
	/// </summary>
	public class MetricsReader
	{
		IDictionary<MemberReference, string> nameCache;
		IDictionary<MemberReference, string> fullNameCache;
		
		public Module MainModule { get; private set; }
		public ISet<Module> Modules { get; private set; }

		public MetricsReader(string file)
		{
			nameCache = new Dictionary<MemberReference, string>();
			fullNameCache = new Dictionary<MemberReference, string>();
			Modules = new HashSet<Module>();
			ReadAssembly(file);
		}

		/// <summary>
		/// Opens a file as assembly and starts reading MainModule.
		/// </summary>
		/// <param name="file">A file which will be analyzed</param>
		private void ReadAssembly(string file)
		{
			var assembly = AssemblyDefinition.ReadAssembly(file);
			ReadModule(assembly.MainModule);
			// support for additional modules but never seen assembly with more than one
		}

		/// <summary>
		/// Reads a module from assembly.
		/// </summary>
		/// <param name="moduleDefinition">A module which contains information</param>
		private void ReadModule(ModuleDefinition moduleDefinition)
		{
			MainModule = new Module()
			{
				Name = moduleDefinition.Name
			};
			
			Modules.Add(MainModule);

			if (moduleDefinition.HasTypes)
				ReadTypes(MainModule, moduleDefinition.Types);
		}

		/// <summary>
		/// Reads types from module.
		/// </summary>
		/// <param name="module">A module where types will be added</param>
		/// <param name="types">A collection of types</param>
		private void ReadTypes(Module module, Collection<TypeDefinition> types)
		{
			// first add all types, because i will need find depend types

			AddTypes(module, types);

			ReadFromTypes(module, types);
		}

		/// <summary>
		/// Iterates through a collection of types and add them to the module.
		/// </summary>
		/// <param name="module">A module where types will be added</param>
		/// <param name="types">A collection of types</param>
		private void AddTypes(Module module, Collection<TypeDefinition> types)
		{
			foreach (TypeDefinition typeDefinition in types)
			{
				if (typeDefinition.Name != "<Module>")
				{
					var type = CreateType(module, typeDefinition);

					if (typeDefinition.HasNestedTypes)
						AddNestedTypes(type, typeDefinition.NestedTypes);
				}
			}
		}

		/// <summary>
		/// Iterates through a collection of nested types and add them to the parent type.
		/// </summary>
		/// <param name="parentType">A type which is owner of nested types</param>
		/// <param name="types">A collection of nested types</param>
		private void AddNestedTypes(Type parentType, Collection<TypeDefinition> types)
		{
			foreach (TypeDefinition typeDefinition in types)
			{
				if (typeDefinition.Name != "<Module>")
				{
					var type = CreateType(parentType.Namespace.Module, typeDefinition);

					parentType.NestedTypes.Add(type);
					type.DeclaringType = parentType;

					if (typeDefinition.HasNestedTypes)
						AddNestedTypes(type, typeDefinition.NestedTypes);
				}
			}
		}

		/// <summary>
		/// Creates a type. If type exist in namespace which isn't created yet so it will be created.
		/// </summary>
		/// <param name="module">A module where type will be added</param>
		/// <param name="typeDefinition">TypeDefinition which will used to create a type.</param>
		/// <returns>A new type</returns>
		private Type CreateType(Module module, TypeDefinition typeDefinition)
		{
			var type = new Type
			{
				Name = FormatTypeName(typeDefinition),
				FullName = FormatTypeName(typeDefinition, true),
				IsInterface = typeDefinition.IsInterface,
				IsEnum = typeDefinition.IsEnum,
				IsClass = typeDefinition.IsClass,
				IsSealed = typeDefinition.IsSealed,
				IsAbstract = typeDefinition.IsAbstract,
				IsPublic = typeDefinition.IsPublic,
				IsStruct = typeDefinition.IsValueType && !typeDefinition.IsEnum && typeDefinition.IsSealed,
				IsInternal = typeDefinition.IsNotPublic,
				IsDelegate = (typeDefinition.BaseType != null ?
							  typeDefinition.BaseType.FullName == "System.MulticastDelegate" : false),
				IsNestedPrivate = typeDefinition.IsNestedPrivate,
				IsNestedPublic = typeDefinition.IsNestedPublic,
				IsNestedProtected = (!typeDefinition.IsNestedPrivate && !typeDefinition.IsNestedPublic &&
									 typeDefinition.IsNestedFamily)
			};

			// try find namespace
			var nsName = GetNamespaceName(typeDefinition);

			var ns = (from n in module.Namespaces
					  where n.Name == nsName
					  select n).SingleOrDefault();

			if (ns == null)
			{
				ns = new Namespace
				{
					Name = nsName,
					Module = module
				};

				module.Namespaces.Add(ns);
			}

			type.Namespace = ns;
			ns.Types.Add(type);
			return type;
		}

		/// <summary>
		/// Reads fields, events, methods from a type.
		/// </summary>
		/// <param name="module">A module where are types located</param>
		/// <param name="types">A collection of types</param>
		private void ReadFromTypes(Module module, Collection<TypeDefinition> types)
		{
			foreach (TypeDefinition typeDefinition in types)
			{

				if (typeDefinition.Name != "<Module>")
				{
					var type =
						(from n in module.Namespaces
						 from t in n.Types
						 where t.FullName == FormatTypeName(typeDefinition, true)
						 select t).SingleOrDefault();
					
					if (typeDefinition.BaseType != null)
					{
						var baseType = (from n in module.Namespaces
										from t in n.Types
										where (t.FullName == FormatTypeName(typeDefinition.BaseType, true))
										select t).SingleOrDefault();

						type.BaseType = baseType; // if baseType is null so propably inherits from another assembly

						if (typeDefinition.BaseType.IsGenericInstance)
						{
							type.IsBaseTypeGenericInstance = true;
							type.GenericBaseTypes.UnionWith(ReadGenericArguments(type.Namespace.Module,
																				 (GenericInstanceType)typeDefinition.BaseType));
						}
					}

					// looks for implemented interfaces
					if (typeDefinition.HasInterfaces)
					{
						foreach (var ic in typeDefinition.Interfaces)
						{
							var implementedIc = (from n in module.Namespaces
												 from t in n.Types
												 where (t.FullName == FormatTypeName(ic, true))
												 select t).SingleOrDefault();

							if (implementedIc != null)
								type.ImplementedInterfaces.Add(implementedIc);

							if (ic.IsGenericInstance)
							{
								type.GenericBaseTypes.UnionWith(ReadGenericArguments(type.Namespace.Module,
																					 (GenericInstanceType)ic));
							}
						}
					}

					if (typeDefinition.HasFields)
						ReadFields(type, typeDefinition.Fields);

					if (typeDefinition.HasEvents)
						ReadEvents(type, typeDefinition.Events);

					if (typeDefinition.HasMethods)
						ReadMethods(type, typeDefinition.Methods);

					if (typeDefinition.HasNestedTypes)
						ReadFromTypes(module, typeDefinition.NestedTypes);
				}
			}
		}

		/// <summary>
		/// Reads events and add them to the type.
		/// </summary>
		/// <param name="type">A type where events will be added</param>
		/// <param name="events">A collection of events</param>
		private void ReadEvents(Type type, Collection<EventDefinition> events)
		{
			foreach (var eventDefinition in events)
			{
				var e = new Event
				{
					Name = eventDefinition.Name,
					DeclaringType = type
				};

				type.Events.Add(e);

				var declaringType =
					(from n in type.Namespace.Module.Namespaces
					 from t in n.Types
					 where t.FullName == FormatTypeName(eventDefinition.EventType, true)
					 select t).SingleOrDefault();

				e.EventType = declaringType;

				// TODO:check eventDefinition.OtherMethods

				// Mono.Cecil threats Events as regular fields
				// so I have to find a field and set IsEvent to true

				var field =
					(from n in type.Namespace.Module.Namespaces
					 from t in n.Types
					 from f in t.Fields
					 where f.Name == e.Name && f.DeclaringType == e.DeclaringType
					 select f).SingleOrDefault();

				if (field != null)
					field.IsEvent = true;
			}
		}

		/// <summary>
		/// Reads fields and add them to the type.
		/// </summary>
		/// <param name="type">A type where fields will be added</param>
		/// <param name="fields">A collection of fields</param>
		private void ReadFields(Type type, Collection<FieldDefinition> fields)
		{
			foreach (FieldDefinition fieldDefinition in fields)
			{
				var field = new Field
				{
					Name = fieldDefinition.Name,
					DeclaringType = type,
					IsPublic = fieldDefinition.IsPublic,
					IsPrivate = fieldDefinition.IsPrivate,
					IsProtected = !fieldDefinition.IsPublic && !fieldDefinition.IsPrivate,
					IsStatic = fieldDefinition.IsStatic,
					IsConstant = fieldDefinition.HasConstant,
					IsReadOnly = fieldDefinition.IsInitOnly
				};

				type.Fields.Add(field);

				var fieldType =
					(from n in type.Namespace.Module.Namespaces
					 from t in n.Types
					 where t.FullName == FormatTypeName(fieldDefinition.FieldType, true)
					 select t).SingleOrDefault();

				if (fieldDefinition.FieldType.IsGenericInstance)
				{
					field.IsGenericInstance = true;
					field.GenericTypes.UnionWith(ReadGenericArguments(type.Namespace.Module,
																	  (GenericInstanceType)fieldDefinition.FieldType));
				}

				field.FieldType = fieldType;
			}
		}

		/// <summary>
		/// Extracts methods and add them to method list for type.
		/// </summary>
		/// <param name="type">A type where methods will be added</param>
		/// <param name="methods">A collection of methods</param>
		private void ReadMethods(Type type, Collection<MethodDefinition> methods)
		{
			foreach (MethodDefinition methodDefinition in methods)
			{
				var method = new Method
				{
					Name = FormatMethodName(methodDefinition),
					DeclaringType = type,
					IsConstructor = methodDefinition.IsConstructor,
					IsPublic = methodDefinition.IsPublic,
					IsPrivate = methodDefinition.IsPrivate,
					IsProtected = !methodDefinition.IsPublic && !methodDefinition.IsPrivate,
					IsStatic = methodDefinition.IsStatic,
					IsSealed = methodDefinition.IsFinal, // not sure if final is sealed
					IsAbstract = methodDefinition.IsAbstract,
					IsSetter = methodDefinition.IsSetter,
					IsGetter = methodDefinition.IsGetter,
					IsVirtual = methodDefinition.IsVirtual
				};

				var returnType =
					(from n in type.Namespace.Module.Namespaces
					 from t in n.Types
					 where t.FullName == FormatTypeName(methodDefinition.ReturnType, true)
					 select t).SingleOrDefault();

				method.ReturnType = returnType; // if null so return type is outside of assembly

				if (methodDefinition.ReturnType.IsGenericInstance)
				{
					method.IsReturnTypeGenericInstance = true;
					method.GenericReturnTypes.UnionWith(ReadGenericArguments(type.Namespace.Module,
																			 (GenericInstanceType) methodDefinition.ReturnType));
				}

				// reading types from parameters
				foreach (var parameter in methodDefinition.Parameters)
				{
					var parameterType =
						(from n in type.Namespace.Module.Namespaces
						 from t in n.Types
						 where t.FullName == FormatTypeName(parameter.ParameterType, true)
						 select t).SingleOrDefault();

					if (parameterType != null)
					{
						var param = new MethodParameter
						{
							ParameterType = parameterType,
							IsIn = parameter.IsIn,
							IsOut = parameter.IsOut,
							IsOptional = parameter.IsOptional,
						};

						// generic parameters
						if (parameter.ParameterType.IsGenericInstance)
						{
							param.IsGenericInstance = true;
							param.GenericTypes = ReadGenericArguments(type.Namespace.Module,
																	  (GenericInstanceType) parameter.ParameterType);
						}

						method.Parameters.Add(param);
					}
				}

				type.Methods.Add(method);
			}

			foreach (MethodDefinition methodDefinition in methods)
			{
				var method = (from m in type.Methods
							  where m.Name == FormatMethodName(methodDefinition)
							  select m).SingleOrDefault();

				if (methodDefinition.Body != null)
				{
					ReadInstructions(method, methodDefinition, methodDefinition.Body.Instructions);
				}
			}
		}

		/// <summary>
		/// Reads method calls by extracting instructions.
		/// </summary>
		/// <param name="method">A method where information will be added</param>
		/// <param name="methodDefinition">A method definition with instructions</param>
		/// <param name="instructions">A collection of instructions</param>
		public void ReadInstructions(Method method, MethodDefinition methodDefinition,
									 Collection<Mono.Cecil.Cil.Instruction> instructions)
		{
			foreach (var instruction in instructions)
			{
				method.Instructions.Add(new Instruction
				                        	{
				                        		DeclaringMethod = method,
												// Operand = instruction.Operand.ToString() // for now operand as string should be enough
				                        	});

				var operand = ReadOperand(instruction);

				if (operand is MethodDefinition)
				{
					var md = operand as MethodDefinition;
					var type = (from n in method.DeclaringType.Namespace.Module.Namespaces
							 from t in n.Types
							 where t.FullName == FormatTypeName(md.DeclaringType, true)
							 select t).SingleOrDefault();

					method.TypeUses.Add(type);

					var findTargetMethod = (from m in type.Methods
											where m.Name == FormatMethodName(md)
											select m).SingleOrDefault();

					if (findTargetMethod != null && type == method.DeclaringType)
						method.MethodUses.Add(findTargetMethod);
				}

				if (operand is FieldDefinition)
				{
					var fd = operand as FieldDefinition;
					var field = (from f in method.DeclaringType.Fields
								 where f.Name == fd.Name
								 select f).SingleOrDefault();

					if (field != null)
						method.FieldUses.Add(field);
				}
			}
		}

		/// <summary>
		/// Reads an instruction operand by recursive calling until non-instruction
		/// operand is found
		/// </summary>
		/// <param name="instruction">An instruction with operand</param>
		/// <returns>An instruction operand</returns>
		public object ReadOperand(Mono.Cecil.Cil.Instruction instruction)
		{
			if (instruction.Operand == null)
				return null;

			var nextInstruction = instruction.Operand as Mono.Cecil.Cil.Instruction;

			if (nextInstruction != null)
				return ReadOperand(nextInstruction);

			return instruction.Operand;
		}

		/// <summary>
		/// Reads generic arguments from type and returns them as a set of types
		/// </summary>
		/// <param name="module">The module where are types located</param>
		/// <param name="genericInstance">The instance type</param>
		/// <returns>A set of types used by generic instance</returns>
		public ISet<Type> ReadGenericArguments(Module module, GenericInstanceType genericInstance)
		{
			var types = new HashSet<Type>();

			foreach (var parameter in genericInstance.GenericArguments)
			{
				var type =
					(from n in module.Namespaces
					 from t in n.Types
					 where t.FullName == FormatTypeName(parameter, true)
					 select t).SingleOrDefault();

				if (type != null) //
					types.Add(type);

				if (parameter.IsGenericInstance)
					types.UnionWith(ReadGenericArguments(module, (GenericInstanceType) parameter));
			}

			return types;
		}

		/// <summary>
		/// Formats method name by adding parameters to it. If there are not any parameters
		/// only empty brackers will be added.
		/// </summary>
		/// <param name="methodDefinition">A method definition with parameters and name</param>
		/// <returns>A name with parameters</returns>
		public string FormatMethodName(MethodDefinition methodDefinition)
		{
			if (nameCache.ContainsKey(methodDefinition))
				return nameCache[methodDefinition];
			
			if (methodDefinition.HasParameters)
			{
				var builder = new StringBuilder();
				var enumerator = methodDefinition.Parameters.GetEnumerator();
				bool hasNext = enumerator.MoveNext();
				while (hasNext)
				{
					builder.Append((enumerator.Current).ParameterType.FullName);
					hasNext = enumerator.MoveNext();
					if (hasNext)
						builder.Append(", ");
				}

				nameCache[methodDefinition] = methodDefinition.Name + "(" + builder + ")";
				return nameCache[methodDefinition];
			}

			nameCache[methodDefinition] = methodDefinition.Name + "()";
			return nameCache[methodDefinition];
		}

		/// <summary>
		/// Formats a specific type name. If type is generic. Brackets <> will be added with proper names of parameters.
		/// </summary>
		/// <param name="type">A type definition with declaring type and name</param>
		/// <returns>A type name</returns>
		public string FormatTypeName(TypeReference type, bool useFullName = false)
		{
			type = type.GetElementType();
			
			var cache = useFullName ? fullNameCache : nameCache;
			
			if (cache.ContainsKey(type))
				return cache[type];
			
			var name = useFullName ? type.FullName : type.Name;

			if (type.IsNested && type.DeclaringType != null)
			{
				cache[type] = FormatTypeName(type.DeclaringType, useFullName) + "+" + name;
				return cache[type];
			}

			if (type.HasGenericParameters)
			{
				var builder = new StringBuilder();
				var enumerator = type.GenericParameters.GetEnumerator();
				bool hasNext = enumerator.MoveNext();
				while (hasNext)
				{
					builder.Append(enumerator.Current.Name);
					hasNext = enumerator.MoveNext();
					if (hasNext)
						builder.Append(",");
				}

				cache[type] = StripGenericName(name) + "<" + builder + ">";
				return cache[type];
			}
			
			return name; // not neeeded to be cached
		}

		/// <summary>
		/// Removes a number of generics parameters. Eg. `3 will be removed from end of name.
		/// </summary>
		/// <param name="name">A name with a number of generics parameters</param>
		/// <returns>A name without generics parameters</returns>
		private string StripGenericName(string name)
		{
			return name.IndexOf('`') != -1 ? name.Remove(name.IndexOf('`')) : name;
		}

		/// <summary>
		/// Gets a namespace name. If type is nested it looks recursively to parent type until finds his namespace.
		/// </summary>
		/// <param name="type">A type definition with namespace</param>
		/// <returns>A namespace</returns>
		private string GetNamespaceName(TypeReference type)
		{
			if (type.IsNested && type.DeclaringType != null)
				return GetNamespaceName(type.DeclaringType);
			
			if (!String.IsNullOrEmpty(type.Namespace))
				return type.Namespace;
			
			return "-";
		}
	}
}
