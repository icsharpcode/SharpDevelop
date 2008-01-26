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
		IReturnType type;
		
		public MockAttribute(string name)
		{
			type = new DefaultReturnType(new MockClass(name));
		}
		
		public IReturnType AttributeType {
			get {
				return type;
			}
		}
		
		public ICompilationUnit CompilationUnit {
			get {
				throw new NotImplementedException();
			}
		}
		
		public DomRegion Region {
			get {
				throw new NotImplementedException();
			}
		}
		
		public AttributeTarget AttributeTarget {
			get {
				throw new NotImplementedException();
			}
		}
		
		public System.Collections.Generic.IList<object> PositionalArguments {
			get {
				throw new NotImplementedException();
			}
		}
		
		public System.Collections.Generic.IDictionary<string, object> NamedArguments {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsFrozen {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void Freeze()
		{
			throw new NotImplementedException();
		}
	}
}
