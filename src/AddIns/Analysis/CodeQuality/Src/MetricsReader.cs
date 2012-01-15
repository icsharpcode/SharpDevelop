// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
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
		CecilLoader loader;
		ICompilation compilation;
		Dictionary<string, NamespaceNode> namespaceMappings;
		Dictionary<ITypeDefinition, TypeNode> typeMappings;
		Dictionary<IMethod, MethodNode> methodMappings;
		Dictionary<IField, FieldNode> fieldMappings;
		Dictionary<MemberReference, IEntity> cecilMappings;
		AssemblyDefinition assemblyDefinition;
		
		public MetricsReader(string fileName)
		{
			loader = new CecilLoader(true) { IncludeInternalMembers = true };
			namespaceMappings = new Dictionary<string, NamespaceNode>();
			typeMappings = new Dictionary<ITypeDefinition, TypeNode>();
			methodMappings = new Dictionary<IMethod, MethodNode>();
			fieldMappings = new Dictionary<IField, FieldNode>();
			cecilMappings = new Dictionary<MemberReference, IEntity>();
			
			compilation = new SimpleCompilation(loader.LoadAssemblyFile(fileName));
			
			// TODO load referenced assemblies into compilation.
			
			Assembly = new AssemblyNode(compilation.MainAssembly.AssemblyName);
			assemblyDefinition = loader.GetCecilObject(compilation.MainAssembly.UnresolvedAssembly);
			
			foreach (var type in compilation.MainAssembly.GetAllTypeDefinitions()) {
				ReadType(type);
				
				foreach (IMethod method in type.Methods) {
					ReadMethod(method);
				}
				
				foreach (IProperty property in type.Properties) {
					if (property.CanGet)
						ReadMethod(property.Getter);
					if (property.CanSet)
						ReadMethod(property.Setter);
				}
				
				foreach (IField field in type.Fields) {
					ReadField(field);
				}
			}
			
			foreach (var method in methodMappings.Values) {
				ReadInstructions(method, method.Method, loader.GetCecilObject((IUnresolvedMethod)method.Method.UnresolvedMember));
			}
			
			Assembly.namespaces = namespaceMappings.Values;
		}
		
		NamespaceNode ReadNamespace(string namespaceName)
		{
			NamespaceNode ns;
			if (!namespaceMappings.TryGetValue(namespaceName, out ns))
				namespaceMappings.Add(namespaceName, ns = new NamespaceNode(Assembly, namespaceName));
			return ns;
		}
		
		TypeNode ReadType(ITypeDefinition type)
		{
			if (type == null)
				return null;
			TypeNode t;
			if (typeMappings.TryGetValue(type, out t))
				return t;
			cecilMappings.Add(loader.GetCecilObject(type.Parts[0]), type);
			NamespaceNode ns = ReadNamespace(type.Namespace);
			typeMappings.Add(type, t = new TypeNode(type, ns));
			ns.types.Add(t);
			if (type.DeclaringTypeDefinition != null) {
				ReadType(type.DeclaringTypeDefinition).nestedTypes.Add(t);
			}
			return t;
		}
		
		MethodNode ReadMethod(IMethod method)
		{
			MethodNode m;
			TypeNode t = ReadType(method.DeclaringTypeDefinition);
			if (!methodMappings.ContainsKey(method)) {
				m = new MethodNode(method, t, ReadType(method.ReturnType.GetDefinition()));
				t.methods.Add(m);
				methodMappings.Add(method, m);
				cecilMappings.Add(loader.GetCecilObject((IUnresolvedMethod)method.UnresolvedMember), method);
			} else {
				m = methodMappings[method];
			}
			return m;
		}
		
		FieldNode ReadField(IField field)
		{
			FieldNode f;
			TypeNode t = ReadType(field.DeclaringTypeDefinition);
			if (!fieldMappings.ContainsKey(field)) {
				f = new FieldNode(field, t, ReadType(field.ReturnType.GetDefinition()));
				t.fields.Add(f);
				fieldMappings.Add(field, f);
				cecilMappings.Add(loader.GetCecilObject((IUnresolvedField)field.UnresolvedMember), field);
			} else {
				f = fieldMappings[field];
			}
			return f;
		}
		
		void ReadInstructions(MethodNode m, IMethod method, MethodDefinition cecilObject)
		{
			if (!cecilObject.HasBody)
				return;
			
			foreach (var instruction in cecilObject.Body.Instructions) {
				m.instructions.Add(new Instruction { DeclaringMethod = m, Operand = (instruction.Operand ?? "").ToString() });

				// IL cyclomatic complexity
				if (instruction.OpCode.FlowControl == FlowControl.Cond_Branch)
					m.CyclomaticComplexity++;

				var operand = ReadOperand(instruction);

				if (operand is MethodDefinition && ((MethodDefinition)operand).DeclaringType.Module.Assembly == assemblyDefinition) {
					var md = (MethodDefinition)operand;
					if (md.DeclaringType.Name == "" || md.DeclaringType.Name.StartsWith("<")) {
						// TODO : Handle generated members
					} else {
						var opMethod = (IMethod)cecilMappings[md];
						var methodNode = methodMappings[opMethod];
						m.typeUses.Add(methodNode.DeclaringType);
						m.methodUses.Add(methodNode);
					}
				}

				if (operand is FieldDefinition && ((FieldDefinition)operand).DeclaringType.Module.Assembly == assemblyDefinition) {
					var fd = (FieldDefinition)operand;
					if (fd.DeclaringType.Name == "" || fd.DeclaringType.Name.StartsWith("<")) {
						// TODO : Handle generated members
					} else {
						var field = (IField)cecilMappings[fd];
						var fieldNode = fieldMappings[field];
						m.fieldUses.Add(fieldNode);
					}
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
		
		public AssemblyNode Assembly { get; private set; }
	}
}
