// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.SharpDevelop.Debugging;
using Mono.Cecil;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// Description of DebuggerDecompilerService.
	/// </summary>
	public class DebuggerDecompilerService : IDebuggerDecompilerService
	{
		private bool CheckMappings(int typeToken)
		{
			object data = null;
			DebuggerService.ExternalDebugInformation.TryGetValue(typeToken, out data);
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
				astBuilder.GenerateCode(new PlainTextOutput());
				
				int token = type.MetadataToken.ToInt32();
				var info = new DecompileInformation {
					CodeMappings = astBuilder.CodeMappings,
					LocalVariables = astBuilder.LocalVariables,
					DecompiledMemberReferences = astBuilder.DecompiledMemberReferences
				};
				
				// save the data
				DebuggerService.ExternalDebugInformation.AddOrUpdate(token, info, (k, v) => info);
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
			
			var data = (DecompileInformation)DebuggerService.ExternalDebugInformation[typeToken];
			var mappings = data.CodeMappings;
			foreach (var key in mappings.Keys) {
				var list = mappings[key];
				var instruction = list.GetInstructionByLineNumber(lineNumber, out memberToken);
				if (instruction == null)
					continue;
				
				ilRanges = new [] { instruction.ILInstructionOffset.From, instruction.ILInstructionOffset.To };
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
			
			var data = (DecompileInformation)DebuggerService.ExternalDebugInformation[typeToken];
			var mappings = data.CodeMappings;
			var map = mappings[memberToken].GetInstructionByTokenAndOffset(memberToken, ilOffset, out isMatch);
			if (map != null) {
				ilRange = map.ToArray(isMatch);
				line = map.SourceCodeLine;
				return true;
			}
			
			return false;
		}
	}
}
