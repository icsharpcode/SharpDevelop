// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
