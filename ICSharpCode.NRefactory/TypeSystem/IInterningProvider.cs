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
		/// Intern the specified string.
		/// </summary>
		string InternString(string s);
		
		/// <summary>
		/// Interns the specified object.
		/// The object must implement <see cref="ISupportsInterning"/>, otherwise it will be returned without being interned.
		/// </summary>
		T InternObject<T>(T obj);
		
		IList<T> InternObjectList<T>(IList<T> list);
	}
	
	[ContractClassFor(typeof(IInterningProvider))]
	abstract class IInterningProviderContract : IInterningProvider
	{
		string IInterningProvider.InternString(string s)
		{
			Contract.Ensures((Contract.Result<string>() == null) == (s == null));
			Contract.Ensures(string.IsNullOrEmpty(Contract.Result<string>()) == string.IsNullOrEmpty(s));
			return s;
		}
		
		T IInterningProvider.InternObject<T>(T obj)
		{
			Contract.Ensures((Contract.Result<T>() == null) == (obj == null));
			return obj;
		}
		
		IList<T> IInterningProvider.InternObjectList<T>(IList<T> list)
		{
			Contract.Ensures((Contract.Result<IList<T>>() == null) == (list == null));
			return list;
		}
	}
}
