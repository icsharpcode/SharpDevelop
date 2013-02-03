// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Description of ICodeGenerator.
	/// </summary>
	public interface ICodeGenerator
	{
		void AddAttribute(IEntity target, IAttribute attribute);
		void AddAssemblyAttribute(IAttribute attribute);
		void AddReturnTypeAttribute(IMethod target, IAttribute attribute);
		void InsertEventHandler(ITypeDefinition target, string name, IEvent eventDefinition, bool jumpTo);
	}
}
