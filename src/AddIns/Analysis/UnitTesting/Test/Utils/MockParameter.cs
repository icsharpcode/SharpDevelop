// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace UnitTesting.Tests.Utils
{
	public class MockParameter : IParameter
	{
		public MockParameter()
		{
		}
		
		public string Name {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IReturnType ReturnType {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public IList<IAttribute> Attributes {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ParameterModifiers Modifiers {
			get {
				throw new NotImplementedException();
			}
		}
		
		public DomRegion Region {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string Documentation {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsOut {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsRef {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsParams {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsOptional {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int CompareTo(object obj)
		{
			throw new NotImplementedException();
		}
		
		public bool IsFrozen {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void Freeze()
		{
		}
	}
}
