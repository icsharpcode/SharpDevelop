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
		public const string SEPARATOR = ","; // don't use '.'
			
		MemberReference memberReference;
		string assemblyFile;
		
		public DecompiledBreakpointBookmark(FileName fileName, Location location, BreakpointAction action, string scriptLanguage, string script) 
			: base(fileName, location, action, scriptLanguage, script)
		{
			
		}
		
		public MemberReference MemberReference {
			get { return memberReference; }
		}
		
		public MemberReference GetMemberReference(IAssemblyResolver resolver)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");
			
			if (memberReference != null)
				return memberReference;
			
			// reload from filename
			ReaderParameters readerParameters = new ReaderParameters();
			// Use new assembly resolver instance so that the AssemblyDefinitions can be garbage-collected
			// once the code is decompiled.
			readerParameters.AssemblyResolver = resolver;
			
			string typeName;
			if (GetAssemblyAndType(FileName.ToString(), out assemblyFile, out typeName)) {
				ModuleDefinition module = ModuleDefinition.ReadModule(assemblyFile, readerParameters);
				TypeDefinition typeDefinition = module.GetType(typeName);
				if (typeDefinition == null)
					throw new InvalidOperationException("Could not find type");
				memberReference = typeDefinition;
			}
			
			return memberReference;
		}
		
		/// <summary>
		/// Gets the assembly file and the type from the file name.
		/// </summary>
		/// <returns><c>true</c>, if the operation succeded; <c>false</c>, otherwise.</returns>
		public static bool GetAssemblyAndType(string fileName, out string assemblyFile, out string typeName)
		{
			if (string.IsNullOrEmpty(fileName) || !fileName.Contains(",")) {
				assemblyFile = null;
				typeName = null;
				return false;
			}
			
			int index = fileName.IndexOf(SEPARATOR);
			assemblyFile = fileName.Substring(0, index);
			typeName = fileName.Substring(index + 1, fileName.Length - index - 4);
			return true;
		}
	}
}
