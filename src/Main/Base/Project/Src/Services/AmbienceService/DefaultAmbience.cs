// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Description of DefaultAmbience.
	/// </summary>
	internal class DefaultAmbience : IAmbience
	{
		public ConversionFlags ConversionFlags { get; set; }
		
		public string ConvertEntity(ICSharpCode.NRefactory.TypeSystem.IEntity e, ICSharpCode.NRefactory.TypeSystem.ITypeResolveContext context)
		{
			return e.Name;
		}
		
		public string ConvertType(ICSharpCode.NRefactory.TypeSystem.IType type)
		{
			return type.Name;
		}
		
		public string ConvertType(ICSharpCode.NRefactory.TypeSystem.ITypeReference type, ICSharpCode.NRefactory.TypeSystem.ITypeResolveContext context)
		{
			return type.Resolve(context).Name;
		}
		
		public string ConvertVariable(ICSharpCode.NRefactory.TypeSystem.IVariable variable, ICSharpCode.NRefactory.TypeSystem.ITypeResolveContext context)
		{
			return variable.Name;
		}
		
		public string WrapAttribute(string attribute)
		{
			return attribute;
		}
		
		public string WrapComment(string comment)
		{
			return "// " + comment;
		}
	}
}
