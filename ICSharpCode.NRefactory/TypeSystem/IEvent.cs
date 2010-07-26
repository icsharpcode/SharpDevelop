// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Diagnostics.Contracts;

namespace ICSharpCode.NRefactory.TypeSystem
{
	[ContractClass(typeof(IEventContract))]
	public interface IEvent : IMember
	{
		/// <summary>
		/// Gets the add method.
		/// </summary>
		IMethod AddMethod { get; }
		
		/// <summary>
		/// Gets the remove method.
		/// </summary>
		IMethod RemoveMethod { get; }
		
		/// <summary>
		/// Gets the raise method.
		/// </summary>
		IMethod RaiseMethod { get; }
	}
	
	[ContractClassFor(typeof(IEvent))]
	abstract class IEventContract : IMemberContract, IEvent
	{
		IMethod IEvent.AddMethod {
			get { return null; }
		}
		
		IMethod IEvent.RemoveMethod {
			get { return null; }
		}
		
		IMethod IEvent.RaiseMethod {
			get { return null; }
		}
	}
}
