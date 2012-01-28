// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.CodeQuality.Engine.Dom;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ICSharpCode.CodeQuality.Engine
{
	/// <summary>
	/// Description of ILAnalyzer.
	/// </summary>
	public class ILAnalyzer
	{
		AssemblyDefinition[] assemblies;
		AssemblyAnalyzer mappings;
		
		public ILAnalyzer(AssemblyDefinition[] assemblies, AssemblyAnalyzer mappings)
		{
			if (assemblies == null)
				throw new ArgumentNullException("assemblies");
			this.assemblies = assemblies;
			this.mappings = mappings;
		}
		
		public void Analyze(MethodBody body, MethodNode analyzedMethod, out int cyclomaticComplexity)
		{
			cyclomaticComplexity = 0;
			
			if (body == null)
				return;
			
			foreach (var instruction in body.Instructions) {
				// IL cyclomatic complexity
				if (instruction.OpCode.FlowControl == FlowControl.Cond_Branch)
					cyclomaticComplexity++;

				var operand = ReadOperand(instruction);
				
				if (operand is MethodDefinition) {
					var md = (MethodDefinition)operand;
					if (assemblies.Contains(md.DeclaringType.Module.Assembly) && mappings.cecilMappings.ContainsKey(md)) {
						var opMethod = (IMethod)mappings.cecilMappings[md];
						var methodNode = mappings.methodMappings[opMethod];
						analyzedMethod.uses.Add(methodNode);
						methodNode.usedBy.Add(analyzedMethod);
					}
				}
//
//				if (operand is MethodDefinition && ((MethodDefinition)operand).DeclaringType.Module.Assembly == assemblyDefinition) {
//					var md = (MethodDefinition)operand;
//					if (md.DeclaringType.Name == "" || md.DeclaringType.Name.StartsWith("<")) {
//						// TODO : Handle generated members
//					} else {
//						var opMethod = (IMethod)cecilMappings[md];
//						var methodNode = methodMappings[opMethod];
//						m.typeUses.Add(methodNode.DeclaringType);
//						m.methodUses.Add(methodNode);
//					}
//				}
//
//				if (operand is FieldDefinition && ((FieldDefinition)operand).DeclaringType.Module.Assembly == assemblyDefinition) {
//					var fd = (FieldDefinition)operand;
//					if (fd.DeclaringType.Name == "" || fd.DeclaringType.Name.StartsWith("<")) {
//						// TODO : Handle generated members
//					} else {
//						var field = (IField)cecilMappings[fd];
//						var fieldNode = fieldMappings[field];
//						m.fieldUses.Add(fieldNode);
//					}
//				}
			}
		}
		
		public object ReadOperand(Instruction instruction)
		{
			while (instruction.Operand is Instruction)
				instruction = (Instruction)instruction.Operand;

			return instruction.Operand;
		}
	}
}
