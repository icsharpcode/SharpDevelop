// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Diagnostics.Contracts;

namespace ICSharpCode.NRefactory.TypeSystem
{
	[ContractClass(typeof(INamedElementContract))]
	public interface INamedElement
	{
		/// <summary>
		/// Gets the fully qualified name of the class the return type is pointing to.
		/// </summary>
		/// <returns>
		/// "System.Int32" for int[]<br/>
		/// "System.Collections.Generic.List" for List&lt;string&gt;
		/// </returns>
		string FullName {
			get;
		}
		
		/// <summary>
		/// Gets the short name of the class the return type is pointing to.
		/// </summary>
		/// <returns>
		/// "Int32" or "int" (depending how the return type was created) for int[]<br/>
		/// "List" for List&lt;string&gt;
		/// </returns>
		string Name {
			get;
		}
		
		/// <summary>
		/// Gets the namespace of the class the return type is pointing to.
		/// </summary>
		/// <returns>
		/// "System" for int[]<br/>
		/// "System.Collections.Generic" for List&lt;string&gt;
		/// </returns>
		string Namespace {
			get;
		}
		
		/// <summary>
		/// Gets the full dotnet name of the return type. The DotnetName is used for the
		/// documentation tags.
		/// </summary>
		/// <returns>
		/// "System.Int[]" for int[]<br/>
		/// "System.Collections.Generic.List{System.String}" for List&lt;string&gt;
		/// </returns>
		string DotNetName {
			get;
		}
	}
	
	[ContractClassFor(typeof(INamedElement))]
	abstract class INamedElementContract : INamedElement
	{
		string INamedElement.FullName {
			get {
				Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
				return null;
			}
		}
		
		string INamedElement.Name {
			get {
				Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
				return null;
			}
		}
		
		string INamedElement.Namespace {
			get {
				Contract.Ensures(Contract.Result<string>() != null);
				return null;
			}
		}
		
		string INamedElement.DotNetName {
			get {
				Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
				return null;
			}
		}
	}
}
