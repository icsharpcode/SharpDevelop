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
	public class MockAttribute : IAttribute
	{
		string name = String.Empty;
		
		public MockAttribute()
		{
		}
		
		public MockAttribute(string name)
		{
			this.name = name;
		}
		
		public AttributeTarget AttributeTarget {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public int CompareTo(object obj)
		{
			throw new NotImplementedException();
		}
	}
}
