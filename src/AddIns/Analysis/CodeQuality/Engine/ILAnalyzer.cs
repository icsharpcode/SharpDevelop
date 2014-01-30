// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.CodeQuality.Engine.Dom;
using ICSharpCode.Core;
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
				
				try {
					if (operand is MethodReference) {
						var md = ((MethodReference)operand).Resolve();
						if (md != null && assemblies.Contains(md.DeclaringType.Module.Assembly) && mappings.cecilMappings.ContainsKey(md)) {
							if (md.IsGetter || md.IsSetter) {
								var propertyNode = mappings.propertyMappings[(IProperty)mappings.cecilMappings[md]];
								mappings.AddEdge(propertyNode);
							} else if (md.IsAddOn || md.IsRemoveOn || md.IsFire || md.IsOther) {
								var eventNode = mappings.eventMappings[(IEvent)mappings.cecilMappings[md]];
								mappings.AddEdge(eventNode);
							} else {
								var methodNode = mappings.methodMappings[(IMethod)mappings.cecilMappings[md]];
								mappings.AddEdge(methodNode);
							}
						}
					} else if (operand is FieldReference) {
						var fd = ((FieldReference)operand).Resolve();
						if (fd != null && assemblies.Contains(fd.DeclaringType.Module.Assembly) && mappings.cecilMappings.ContainsKey(fd)) {
							var fieldNode = mappings.fieldMappings[(IField)mappings.cecilMappings[fd]];
							mappings.AddEdge(fieldNode);
						}
					}
				} catch (AssemblyResolutionException are) {
					LoggingService.InfoFormatted("CQA: Skipping operand reference: {0}\r\nException:\r\n{1}", operand, are);
				}
			}
		}
		
		object ReadOperand(Instruction instruction)
		{
			while (instruction.Operand is Instruction)
				instruction = (Instruction)instruction.Operand;

			return instruction.Operand;
		}
	}
}
