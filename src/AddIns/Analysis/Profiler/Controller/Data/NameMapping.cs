// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
