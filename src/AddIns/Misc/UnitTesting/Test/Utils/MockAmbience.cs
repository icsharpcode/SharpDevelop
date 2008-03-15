// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	}
}
