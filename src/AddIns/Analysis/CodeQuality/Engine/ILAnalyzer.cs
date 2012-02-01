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
		
		public void Analyze(MethodBody body, NodeBase analyzedNode)
		{
			if (analyzedNode is MethodNode)
				((MethodNode)analyzedNode).CyclomaticComplexity = 0;
			
			if (body == null)
				return;
			
			foreach (var instruction in body.Instructions) {
				// IL cyclomatic complexity
				if (instruction.OpCode.FlowControl == FlowControl.Cond_Branch && analyzedNode is MethodNode)
					((MethodNode)analyzedNode).CyclomaticComplexity++;

				var operand = ReadOperand(instruction);
				
				if (operand is MethodReference) {
					var md = ((MethodReference)operand).Resolve();
					if (md != null && assemblies.Contains(md.DeclaringType.Module.Assembly) && mappings.cecilMappings.ContainsKey(md)) {
						if (md.IsGetter || md.IsSetter) {
							var propertyNode = mappings.propertyMappings[(IProperty)mappings.cecilMappings[md]];
							analyzedNode.AddRelationship(propertyNode);
						} else {
							var methodNode = mappings.methodMappings[(IMethod)mappings.cecilMappings[md]];
							analyzedNode.AddRelationship(methodNode);
						}
					}
				} else if (operand is FieldReference) {
					var fd = ((FieldReference)operand).Resolve();
					if (fd != null && assemblies.Contains(fd.DeclaringType.Module.Assembly) && mappings.cecilMappings.ContainsKey(fd)) {
						var fieldNode = mappings.fieldMappings[(IField)mappings.cecilMappings[fd]];
						analyzedNode.AddRelationship(fieldNode);
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
