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
	/// Dummy ambience implementation.
	/// </summary>
	internal class DefaultAmbience : IAmbience
	{
		public ConversionFlags ConversionFlags { get; set; }
		
		public string ConvertEntity(IEntity e)
		{
			return e.Name;
		}
		
		public string ConvertType(IType type)
		{
			return type.Name;
		}
		
		public string ConvertVariable(IVariable variable)
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
		
		public string ConvertConstantValue(object constantValue)
		{
			if (constantValue == null)
				return "null";
			if (constantValue is char)
				return "'" + constantValue + "'";
			if (constantValue is String)
				return "\"" + constantValue + "\"";
			return constantValue.ToString();
		}
	}
}
