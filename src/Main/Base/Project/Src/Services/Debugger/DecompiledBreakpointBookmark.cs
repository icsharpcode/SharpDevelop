// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Debugging;
using Mono.Cecil;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	public class DecompiledBreakpointBookmark : BreakpointBookmark
	{
		public DecompiledBreakpointBookmark(MemberReference member, int ilFrom, int ilTo, FileName fileName, TextLocation location, BreakpointAction action, string scriptLanguage, string script) : base(fileName, location, action, scriptLanguage, script)
		{
			this.MemberReference = member;
			this.ILFrom = ilFrom;
			this.ILTo = ILTo;
		}
		
		public int ILFrom {
			get; set;
		}
		
		public int ILTo {
			get; set;
		}
		
		MemberReference memberReference;
		
		public MemberReference MemberReference {
			get {
				if (memberReference != null)
					return memberReference;
				
				// reload from filename
				ReaderParameters readerParameters = new ReaderParameters();
				// Use new assembly resolver instance so that the AssemblyDefinitions can be garbage-collected
				// once the code is decompiled.
				readerParameters.AssemblyResolver = new DefaultAssemblyResolver();
				
				string fileName = FileName.ToString();
				int index = fileName.IndexOf(",");
				string assemblyFile = fileName.Substring(0, index);
				string fullTypeName = fileName.Substring(index + 1, fileName.Length - index - 1);
				
				ModuleDefinition module = ModuleDefinition.ReadModule(assemblyFile, readerParameters);
				TypeDefinition typeDefinition = module.GetType(fullTypeName);
				if (typeDefinition == null)
					throw new InvalidOperationException("Could not find type");
				memberReference = typeDefinition;
				return memberReference;
			}
			private set { memberReference = value; }
		}
	}
}
