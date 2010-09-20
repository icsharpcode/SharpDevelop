// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Dom;
using System;

namespace UnitTesting.Tests.Utils
{
	public class MockAmbience : IAmbience
	{
		public MockAmbience()
		{
		}
		
		public ConversionFlags ConversionFlags {
			get {
				return ConversionFlags.None;
			}
			set {
			}
		}
		
		public string Convert(ModifierEnum modifier)
		{
			return String.Empty;
		}
		
		public string Convert(IClass c)
		{
			return String.Empty;
		}
		
		public string ConvertEnd(IClass c)
		{
			return String.Empty;
		}
		
		public string Convert(IEntity entity)
		{
			return String.Empty;
		}
		
		public string Convert(IField field)
		{
			return String.Empty;
		}
		
		public string Convert(IProperty property)
		{
			return String.Empty;
		}
		
		public string Convert(IEvent e)
		{
			return String.Empty;
		}
		
		public string Convert(IMethod m)
		{
			return String.Empty;
		}
		
		public string ConvertEnd(IMethod m)
		{
			return String.Empty;
		}
		
		public string Convert(IParameter param)
		{
			return String.Empty;
		}
		
		public string Convert(IReturnType returnType)
		{
			return String.Empty;
		}
		
		public string WrapAttribute(string attribute)
		{
			return String.Empty;
		}
		
		public string WrapComment(string comment)
		{
			return String.Empty;
		}
		
		public string GetIntrinsicTypeName(string dotNetTypeName)
		{
			return String.Empty;
		}
		
		public string ConvertAccessibility(ModifierEnum accessibility)
		{
			return String.Empty;
		}
	}
}
