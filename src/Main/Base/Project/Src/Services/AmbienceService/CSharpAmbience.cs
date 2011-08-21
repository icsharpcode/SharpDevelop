// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// C# ambience.
	/// </summary>
	public class CSharpAmbience : IAmbience
	{
		public ConversionFlags ConversionFlags { get; set; }
		
		public string ConvertEntity(IEntity e)
		{
			throw new NotImplementedException();
		}
		
		public string ConvertParameter(IVariable v)
		{
			throw new NotImplementedException();
		}
		
		public string ConvertType(IType type)
		{
			throw new NotImplementedException();
		}
		
		public string ConvertAccessibility(Accessibility accessibility)
		{
			throw new NotImplementedException();
		}
		
		public string WrapAttribute(string attribute)
		{
			throw new NotImplementedException();
		}
		
		public string WrapComment(string comment)
		{
			throw new NotImplementedException();
		}
		
		public string GetIntrinsicTypeName(string dotNetTypeName)
		{
			throw new NotImplementedException();
		}
	}
}
