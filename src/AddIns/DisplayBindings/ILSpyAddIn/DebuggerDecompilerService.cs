// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.Decompiler.ILAst;
using ICSharpCode.ILSpyAddIn.LaunchILSpy;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Project;
using Mono.Cecil;

namespace ICSharpCode.ILSpyAddIn
{
	// Dummy class to avoid the build errors after updating the ICSharpCode.Decompiler version.
	// TODO: get rid of this & fix debugging decompiled files
	public class DecompileInformation {
		public dynamic LocalVariables;
		public dynamic CodeMappings;
	}
	
	/// <summary>
	/// Stores the decompilation information.
	/// </summary>
	public class DebuggerDecompilerService : IDebuggerDecompilerService
	{
		ILSpyAssemblyResolver resolver;
		
		static DebuggerDecompilerService()
		{
			DebugInformation = new ConcurrentDictionary<int, DecompileInformation>();
			ProjectService.SolutionClosed += delegate {
				DebugInformation.Clear();
				GC.Collect();
			};
		}
		
		internal static IDebuggerDecompilerService Instance { get; private set; }
		
		/// <summary>
		/// Gets or sets the external debug information.
		/// <summary>This constains the code mappings and local variables.</summary>
		/// </summary>
		internal static ConcurrentDictionary<int, DecompileInformation> DebugInformation { get; private set; }
		
		public DebuggerDecompilerService()
		{
			Instance = this;
		}
		
		public Tuple<int, int> DebugStepInformation { get; set; }
		
		public bool CheckMappings(int typeToken)
		{
			DecompileInformation data = null;
			DebugInformation.TryGetValue(typeToken, out data);
			DecompileInformation information = data as DecompileInformation;
			
			if (information == null)
				return false;
			
			if (information.CodeMappings == null)
				return false;
			
			return true;
		}
		
		public void DecompileOnDemand(TypeDefinition type)
		{
			if (type == null)
				return;
			
			if (CheckMappings(type.MetadataToken.ToInt32()))
				return;
			
			try {
				DecompilerContext context = new DecompilerContext(type.Module);
				AstBuilder astBuilder = new AstBuilder(context);
				astBuilder.AddType(type);
				DebuggerTextOutput output = new DebuggerTextOutput(new PlainTextOutput());
				astBuilder.GenerateCode(output);
				
				/*int token = type.MetadataToken.ToInt32();
				var info = new DecompileInformation {
					CodeMappings = astBuilder.CodeMappings,
					LocalVariables = astBuilder.LocalVariables,
					DecompiledMemberReferences = astBuilder.DecompiledMemberReferences
				};
				
				// save the data
				DebugInformation.AddOrUpdate(token, info, (k, v) => info);*/
			} catch {
				return;
			}
		}
		
		public bool GetILAndTokenByLineNumber(int typeToken, int lineNumber, out int[] ilRanges, out int memberToken)
		{
			ilRanges = null;
			memberToken = -1;
			if (!CheckMappings(typeToken))
				return false;
			
			var data = (DecompileInformation)DebugInformation[typeToken];
			var mappings = data.CodeMappings;
			foreach (var key in mappings.Keys) {
				var list = mappings[key];
				var instruction = list.GetInstructionByLineNumber(lineNumber, out memberToken);
				if (instruction == null)
					continue;
				
				ilRanges = new int[] { instruction.ILInstructionOffset.From, instruction.ILInstructionOffset.To };
				memberToken = instruction.MemberMapping.MetadataToken;
				return true;
			}
			
			return false;
		}
		
		public bool GetILAndLineNumber(int typeToken, int memberToken, int ilOffset, out int[] ilRange, out int line, out bool isMatch)
		{
			ilRange = null;
			line = -1;
			isMatch = false;
			
			if (!CheckMappings(typeToken))
				return false;
			
			var data = (DecompileInformation)DebugInformation[typeToken];
			var mappings = data.CodeMappings;
			
			if (!mappings.ContainsKey(memberToken))
				return false;
			
			var map = mappings[memberToken].GetInstructionByTokenAndOffset(memberToken, ilOffset, out isMatch);
			if (map != null) {
				ilRange = map.ToArray(isMatch);
				line = map.SourceCodeLine;
				return true;
			}
			
			return false;
		}
		
		public IEnumerable<string> GetLocalVariables(int typeToken, int memberToken)
		{
			if (DebugInformation == null || !DebugInformation.ContainsKey(typeToken))
				yield break;

			var externalData = DebugInformation[typeToken];
			IEnumerable<ILVariable> list;
			
			if (externalData.LocalVariables.TryGetValue(memberToken, out list)) {
				foreach (var local in list) {
					if (local.IsParameter)
						continue;
					if (string.IsNullOrEmpty(local.Name))
						continue;
					yield return local.Name;
				}
			}
		}
		
		public object GetLocalVariableIndex(int typeToken, int memberToken, string name)
		{
			if (DebugInformation == null || !DebugInformation.ContainsKey(typeToken))
				return null;

			var externalData = DebugInformation[typeToken];
			IEnumerable<ILVariable> list;
			
			if (externalData.LocalVariables.TryGetValue(memberToken, out list)) {
				foreach (var local in list) {
					if (local.IsParameter)
						continue;
					if (local.Name == name)
						return new[] { local.OriginalVariable.Index };
				}
			}
			
			return null;
		}
		
		public IAssemblyResolver GetAssemblyResolver(string assemblyFile)
		{
			if (string.IsNullOrEmpty(assemblyFile))
				throw new ArgumentException("assemblyFile is null or empty");
			
			string folderPath = Path.GetDirectoryName(assemblyFile);
			if (resolver == null)
				return (resolver = new ILSpyAssemblyResolver(folderPath));
			
			if (string.Compare(folderPath, resolver.FolderPath, StringComparison.OrdinalIgnoreCase) != 0)
				return (resolver = new ILSpyAssemblyResolver(folderPath));
			
			return resolver;
		}
	}
}
