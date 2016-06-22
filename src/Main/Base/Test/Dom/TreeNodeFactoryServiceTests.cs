// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using ICSharpCode.TreeView;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Dom
{
	[TestFixture]
	public class TreeNodeFactoryServiceTests
	{
		interface IBase {}
		interface I1 : IBase {}
		interface I2 : IBase {}
		class Base : IBase {}
		class C1 : I1 {}
		class C2 : I2 {}
		class Both : I1, I2 {}
		
		class MockTreeNodeFactory : ITreeNodeFactory
		{
			readonly Type type;
			
			public MockTreeNodeFactory(Type type)
			{
				this.type = type;
			}
			
			Type ITreeNodeFactory.GetSupportedType(object model)
			{
				if (type.IsInstanceOfType(model))
					return type;
				else
					return null;
			}
			
			SharpTreeNode ITreeNodeFactory.CreateTreeNode(object model)
			{
				if (type.IsInstanceOfType(model))
					return new MockTreeNode(type);
				else
					return null;
			}
		}
		
		class MockTreeNode : SharpTreeNode
		{
			object model;
			
			public MockTreeNode(object model)
			{
				this.model = model;
			}
			
			protected override object GetModel()
			{
				return model;
			}
		}
		
		ITreeNodeFactory CreateService(params Type[] types)
		{
			return new TreeNodeFactoryService(types.Select(t => new MockTreeNodeFactory(t)).ToList());
		}
		
		[Test]
		public void DoesNotSupportNull()
		{
			var service = CreateService(typeof(I1), typeof(I2));
			Assert.IsNull(service.GetSupportedType(null));
		}
		
		[Test]
		public void CannotCreateNodeFromNull()
		{
			var service = CreateService(typeof(I1), typeof(I2));
			Assert.IsNull(service.CreateTreeNode(null));
		}
		
		[Test]
		public void UnsupportedObject()
		{
			var service = CreateService(typeof(I1), typeof(I2));
			Assert.IsNull(service.GetSupportedType(new object()));
			Assert.IsNull(service.CreateTreeNode(new object()));
		}
		
		[Test]
		public void TwoCandidates_FirstOneWins()
		{
			var service = CreateService(typeof(I1), typeof(I2));
			Assert.AreEqual(typeof(I1), service.GetSupportedType(new Both()));
			Assert.AreEqual(typeof(I1), service.CreateTreeNode(new Both()).Model);
		}
		
		[Test]
		public void Derived_Wins_Over_Base()
		{
			var service = CreateService(typeof(IBase), typeof(I1));
			Assert.AreEqual(typeof(I1), service.GetSupportedType(new C1()));
			Assert.AreEqual(typeof(I1), service.CreateTreeNode(new C1()).Model);
		}
		
		[Test]
		public void Base_Loses_To_Derived()
		{
			var service = CreateService(typeof(I1), typeof(IBase));
			Assert.AreEqual(typeof(I1), service.GetSupportedType(new C1()));
			Assert.AreEqual(typeof(I1), service.CreateTreeNode(new C1()).Model);
		}
		
		[Test]
		public void ThreeCandidates_FirstElibleWins()
		{
			// IBase is ineligible because I1 and I2 both derive from it, so I1 gets chosen
			var service = CreateService(typeof(IBase), typeof(I1), typeof(I2));
			Assert.AreEqual(typeof(I1), service.GetSupportedType(new Both()));
			Assert.AreEqual(typeof(I1), service.CreateTreeNode(new Both()).Model);
		}
		
	}
}
