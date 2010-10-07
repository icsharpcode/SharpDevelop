// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ICSharpCode.NRefactory.TypeSystem
{
	[ContractClass(typeof(IInterningProviderContract))]
	public interface IInterningProvider
	{
		/// <summary>
		/// Interns the specified object.
		/// The object must implement <see cref="ISupportsInterning"/>, or must be of one of the types
		/// known to the interning provider to use value equality,
		/// otherwise it will be returned without being interned.
		/// </summary>
		T Intern<T>(T obj) where T : class;
		
		IList<T> InternList<T>(IList<T> list) where T : class;
	}
	
	[ContractClassFor(typeof(IInterningProvider))]
	abstract class IInterningProviderContract : IInterningProvider
	{
		T IInterningProvider.Intern<T>(T obj)
		{
			Contract.Ensures((Contract.Result<T>() == null) == (obj == null));
			return obj;
		}
		
		IList<T> IInterningProvider.InternList<T>(IList<T> list)
		{
			Contract.Ensures((Contract.Result<IList<T>>() == null) == (list == null));
			return list;
		}
	}
}
