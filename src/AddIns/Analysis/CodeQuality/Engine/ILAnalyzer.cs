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
		
		public void Analyze(MethodBody body, MethodNode analyzedMethod)
		{
			analyzedMethod.CyclomaticComplexity = 0;
			
			if (body == null)
				return;
			
			foreach (var instruction in body.Instructions) {
				// IL cyclomatic complexity
				if (instruction.OpCode.FlowControl == FlowControl.Cond_Branch)
					analyzedMethod.CyclomaticComplexity++;

				var operand = ReadOperand(instruction);
				
				if (operand is MethodReference) {
					var md = ((MethodReference)operand).Resolve();
					if (md != null && assemblies.Contains(md.DeclaringType.Module.Assembly) && mappings.cecilMappings.ContainsKey(md)) {
						var methodNode = mappings.methodMappings[(IMethod)mappings.cecilMappings[md.Resolve()]];
						analyzedMethod.AddRelationship(methodNode);
					}
				} else if (operand is FieldReference) {
					var fd = ((FieldReference)operand).Resolve();
					if (fd != null && assemblies.Contains(fd.DeclaringType.Module.Assembly) && mappings.cecilMappings.ContainsKey(fd)) {
						var fieldNode = mappings.fieldMappings[(IField)mappings.cecilMappings[fd]];
						analyzedMethod.AddRelationship(fieldNode);
					}
				}
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
