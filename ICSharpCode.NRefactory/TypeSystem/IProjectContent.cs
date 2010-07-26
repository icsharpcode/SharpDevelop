// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Diagnostics.Contracts;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Mutable container of all classes in an assembly.
	/// </summary>
	[ContractClass(typeof(IProjectContentContract))]
	public interface IProjectContent : ITypeResolveContext
	{
		LanguageProperties Language { get; }
	}
	
	[ContractClassFor(typeof(IProjectContent))]
	abstract class IProjectContentContract : ITypeResolveContextContract, IProjectContent
	{
		LanguageProperties IProjectContent.Language {
			get {
				Contract.Ensures(Contract.Result<LanguageProperties>() != null);
				return null;
			}
		}
	}
}
