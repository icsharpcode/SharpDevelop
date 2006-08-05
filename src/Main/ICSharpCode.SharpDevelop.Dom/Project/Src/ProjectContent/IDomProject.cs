// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IDomProject
	{
		string Language { get; }
		string RootNamespace { get; set; }
		string OutputAssemblyFullPath { get; }
	}
}
