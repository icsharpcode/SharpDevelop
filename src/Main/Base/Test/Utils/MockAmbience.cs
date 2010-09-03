// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Tests.Utils
{
	/// <summary>
	/// Mock Ambience class.
	/// </summary>
	public class MockAmbience : AbstractAmbience
	{
		public MockAmbience()
		{
		}
		
		public override string Convert(IClass c)
		{
			return String.Empty;
		}
		
		public override string ConvertEnd(IClass c)
		{
			return String.Empty;
		}
		
		public override string Convert(IField c)
		{
			return String.Empty;
		}
		
		public override string Convert(IProperty property)
		{
			return property.Name;
		}
		
		public override string Convert(IEvent e)
		{
			return String.Empty;
		}
		
		public override string Convert(IMethod m)
		{
			return m.Name;
		}
		
		public override string ConvertEnd(IMethod m)
		{
			return String.Empty;
		}
		
		public override string Convert(IParameter param)
		{
			return String.Empty;
		}
		
		public override string Convert(IReturnType returnType)
		{
			return String.Empty;
		}
		
		public override string WrapAttribute(string attribute)
		{
			return String.Empty;
		}
		
		public override string WrapComment(string comment)
		{
			return String.Empty;
		}
		
		public override string GetIntrinsicTypeName(string dotNetTypeName)
		{
			return String.Empty;
		}
		
		public override string ConvertAccessibility(ModifierEnum accessibility)
		{
			return String.Empty;
		}
	}
}
