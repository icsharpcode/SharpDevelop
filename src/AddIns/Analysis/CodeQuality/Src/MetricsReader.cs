using System;
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
        public Module MainModule { get; private set; } 

        public MetricsReader(string file)
        {
            this.ReadAssembly(file);
        }

        /// <summary>
        /// Opens file as assembly and starts reading MainModule
        /// </summary>
        /// <param name="file"></param>
        private void ReadAssembly(string file)
        {
            var assembly = AssemblyDefinition.ReadAssembly(file);
            ReadModule(assembly.MainModule);
        }

        /// <summary>
        /// Reads main module from assembly
        /// </summary>
        /// <param name="moduleDefinition"></param>
        private void ReadModule(ModuleDefinition moduleDefinition)
        {
            this.MainModule = new Module()
                                  {
                                      Name = moduleDefinition.Name
                                  };

            if (moduleDefinition.HasTypes)
                ReadTypes(MainModule, moduleDefinition.Types);
        }

        /// <summary>
        /// Reads types from module
        /// </summary>
        /// <param name="module"></param>
        /// <param name="types"></param>
        private void ReadTypes(Module module, Collection<TypeDefinition> types)
        {
            // first add all types, because i will need find depend types

            AddTypes(module, types);

            ReadFromTypes(module, types);
        }

        private void AddTypes(Module module, Collection<TypeDefinition> types)
        {
            foreach (TypeDefinition typeDefinition in types)
            {
                if (typeDefinition.Name != "<Module>")
                {
                    var type = new Type()
                    {
                        Name = FormatTypeName(typeDefinition)
                    };

                    // try find namespace
                    var nsName = GetNamespaceName(typeDefinition);

                    var ns = (from n in module.Namespaces
                              where n.Name == nsName
                              select n).SingleOrDefault();

                    if (ns == null)
                    {
                        ns = new Namespace()
                        {
                            Name = nsName,
                            Module = module
                        };

                        module.Namespaces.Add(ns);
                    }

                    type.Namespace = ns;
                    ns.Types.Add(type);

                    if (typeDefinition.HasNestedTypes)
                        AddTypes(module, typeDefinition.NestedTypes);
                }
            }
        }

        private void ReadFromTypes(Module module, Collection<TypeDefinition> types)
        {
            foreach (TypeDefinition typeDefinition in types)
            {

                if (typeDefinition.Name != "<Module>")
                {
                    var type =
                        (from n in module.Namespaces
                         from t in n.Types
                         where (t.Name == FormatTypeName(typeDefinition))
                         select t).SingleOrDefault();


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

        private void ReadEvents(Type type, Collection<EventDefinition> events)
        {
            foreach (var eventDefinition in events)
            {
                var e = new Event()
                            {
                                Name = eventDefinition.Name,
                                Owner = type
                            };

                type.Events.Add(e);

                var declaringType =
                    (from n in type.Namespace.Module.Namespaces
                     from t in n.Types
                     where t.Name == e.Name
                     select t).SingleOrDefault();

                e.EventType = declaringType;

                // Mono.Cecil threats Events as regular fields
                // so I have to find a field and set IsEvent to true

                var field =
                    (from n in type.Namespace.Module.Namespaces
                     from t in n.Types
                     from f in t.Fields
                     where f.Name == e.Name
                     select f).SingleOrDefault();

                if (field != null)
                    field.IsEvent = true;
            }
        }

        /// <summary>
        /// Reads fields and add them to field list for type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fields"></param>
        private void ReadFields(Type type, Collection<FieldDefinition> fields)
        {
            foreach (FieldDefinition fieldDefinition in fields)
            {
                var field = new Field()
                                {
                                    Name = fieldDefinition.Name,
                                    IsEvent = false,
                                    Owner = type
                                };

                type.Fields.Add(field);

                var declaringType =
                        (from n in type.Namespace.Module.Namespaces
                         from t in n.Types
                         where t.Name == FormatTypeName(fieldDefinition.DeclaringType)
                         select t).SingleOrDefault();

                field.FieldType = declaringType;
            }
        }

        /// <summary>
        /// Extracts methods and add them to method list for type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methods"></param>
        private void ReadMethods(Type type, Collection<MethodDefinition> methods)
        {
            foreach (MethodDefinition methodDefinition in methods)
            {
                var method = new Method
                {
                    Name = FormatMethodName(methodDefinition),
                    Owner = type,
                    IsConstructor = methodDefinition.IsConstructor
                };

                var declaringType =
                        (from n in type.Namespace.Module.Namespaces
                         from t in n.Types
                         where t.Name == FormatTypeName(methodDefinition.DeclaringType)
                         select t).SingleOrDefault();

                method.MethodType = declaringType;

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
        /// Reads method calls by extracting instrunctions
        /// </summary>
        /// <param name="method"></param>
        /// <param name="methodDefinition"></param>
        /// <param name="instructions"></param>
        public void ReadInstructions(Method method, MethodDefinition methodDefinition,
            Collection<Instruction> instructions)
        {
            foreach (Instruction instruction in instructions)
            {
                var instr = ReadInstruction(instruction);
                
                if (instr is MethodDefinition)
                {
                    var md = instr as MethodDefinition;
                    var type = (from n in method.MethodType.Namespace.Module.Namespaces
                                from t in n.Types
                                where t.Name == FormatTypeName(md.DeclaringType) &&
                                n.Name == t.Namespace.Name
                                select t).SingleOrDefault();

                    method.TypeUses.Add(type);

                    var findTargetMethod = (from m in type.Methods
                                            where m.Name == FormatMethodName(md)
                                            select m).SingleOrDefault();

                    if (findTargetMethod != null && type == method.MethodType) 
                        method.MethodUses.Add(findTargetMethod);
                }

                if (instr is FieldDefinition)
                {
                    var fd = instr as FieldDefinition;
                    var field = (from f in method.MethodType.Fields
                                where f.Name == fd.Name
                                select f).SingleOrDefault();

                    if (field != null)
                        method.FieldUses.Add(field);
                }

                if (instr != null)
                    Console.WriteLine(instr.GetType().ToString());
            }
        }

        /// <summary>
        /// Reads instruction operand by recursive calling until non-instruction
        /// operand is found 
        /// </summary>
        /// <param name="instruction"></param>
        /// <returns></returns>
        public object ReadInstruction(Instruction instruction)
        {
            if (instruction.Operand == null)
                return null;
            
            var nextInstruction = instruction.Operand as Instruction;

            if (nextInstruction != null)
                return ReadInstruction(nextInstruction);
            else
                return instruction.Operand;
        }

        /// <summary>
        /// Formats method name by adding parameters to it. If there are not any parameters
        /// only empty brackers will be added.
        /// </summary>
        /// <param name="methodDefinition"></param>
        /// <returns></returns>
        public string FormatMethodName(MethodDefinition methodDefinition)
        {
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

                return methodDefinition.Name + "(" + builder + ")";
            }
            else
            {
                return methodDefinition.Name + "()";
            }
        }

        /// <summary>
        /// Formats a specific type name. If type is generic. Brackets <> will be added with proper names of parameters.
        /// </summary>
        /// <param name="typeDefinition"></param>
        /// <returns></returns>
        public string FormatTypeName(TypeDefinition typeDefinition)
        {
            if (typeDefinition.IsNested && typeDefinition.DeclaringType != null)
            {
                return FormatTypeName(typeDefinition.DeclaringType) + "+" + typeDefinition.Name;
            }

            if (typeDefinition.HasGenericParameters)
            {
                var builder = new StringBuilder();
                var enumerator = typeDefinition.GenericParameters.GetEnumerator();
                bool hasNext = enumerator.MoveNext();
                while (hasNext)
                {
                    builder.Append((enumerator.Current).Name);
                    hasNext = enumerator.MoveNext();
                    if (hasNext)
                        builder.Append(",");
                }

                return StripGenericName(typeDefinition.Name) + "<" + builder + ">";
            }

            return typeDefinition.Name; 
        }

        /// <summary>
        /// Removes a number of generics parameters. Eg. `3 will be removed from end of name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string StripGenericName(string name)
        {
            return name.IndexOf('`') != -1 ? name.Remove(name.IndexOf('`')) : name;
        }

        /// <summary>
        /// Gets namespace name. If type is nested it looks recursively to parent type until finds his namespace.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetNamespaceName(TypeDefinition type)
        {
            if (type.IsNested && type.DeclaringType != null)
                return GetNamespaceName(type.DeclaringType);
            
            if (!String.IsNullOrEmpty(type.Namespace))
                return type.Namespace;
            
            return "-";
        }
    }
}
