// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Parser;
using Mono.Cecil;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// Description of ILSpyFullParseInformation.
	/// </summary>
	public class ILSpyFullParseInformation : ParseInformation
	{
		SyntaxTree syntaxTree;
		// Required to make WeakReferences work properly as cache policy.
		AssemblyDefinition[] neededAssemblies;

		public ILSpyFullParseInformation(ILSpyUnresolvedFile unresolvedFile, ITextSourceVersion parsedVersion, SyntaxTree syntaxTree, IEnumerable<AssemblyDefinition> neededAssemblies)
			: base(unresolvedFile, parsedVersion, true)
		{
			this.syntaxTree = syntaxTree;
			this.neededAssemblies = neededAssemblies.ToArray();
		}
		
		public SyntaxTree SyntaxTree { get { return syntaxTree; } }
	}
}
