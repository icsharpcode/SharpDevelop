// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.Profiler.Controller.Data
{
	/// <summary>
	/// In-memory representation of a name mapping.
	/// </summary>
	public class NameMapping
	{
		internal static readonly string[] EmptyParameterList = new string[0];
		
		int id;
		string returnType;
		string name;
		IList<string> parameters;
		
		/// <summary>
		/// Creates a new dummy NameMapping containing a valid ID only.
		/// </summary>
		public NameMapping(int id)
			: this(id, null, null, null)
		{
		}
		
		/// <summary>
		/// Creates a new NameMapping with valid data.
		/// </summary>
		public NameMapping(int id, string returnType, string name, IList<string> parameters)
		{
			this.id = id;
			this.returnType = returnType ?? "";
			this.name = name ?? "";
			if (parameters != null)
				this.parameters = Array.AsReadOnly(parameters.ToArray());
			else
				this.parameters = EmptyParameterList;
		}
		
		/// <summary>
		/// The internally used ID to identify the name mapping.
		/// </summary>
		public int Id { get { return id; } }
		
		/// <summary>
		/// The return type of the method.
		/// </summary>
		public string ReturnType { get { return returnType; } }
		
		/// <summary>
		/// The name of the method (including full namespace and class).
		/// </summary>
		public string Name { get { return name; } }
		
		/// <summary>
		/// List of parameters.
		/// </summary>
		public IList<string> Parameters { get { return parameters; } }
	}
}