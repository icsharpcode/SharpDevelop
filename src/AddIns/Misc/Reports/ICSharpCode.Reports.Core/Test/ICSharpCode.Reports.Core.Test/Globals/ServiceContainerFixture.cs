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
using NUnit.Framework;
using ICSharpCode.Reports.Core.Globals;


namespace ICSharpCode.Reports.Core.Test.Globals
{
	
	public interface ITestInterface
	{
		bool Test();
	}
	
	
	public class MyTestImplementation : ITestInterface
	{
		
		
		public bool Test()
		{
			throw new NotImplementedException();
		}
	}
	
	
	[TestFixture]
	public class ServiceContainerFixture
	{
		[Test]
		public void GetServiceFromContainer()
		{
			var service = ServiceContainer.GetService(typeof(ITestInterface));
			Assert.That (service,Is.Not.Null);
			Assert.That (service,Is.AssignableFrom(typeof(MyTestImplementation)));
			Assert.That (service,Is.AssignableTo (typeof(ITestInterface)));
		}
		
		
		[Test]
		[ExpectedException(typeof(System.ArgumentException))]
		public void ShouldNotAddServiceMoreThanOneTime()
		{
			ServiceContainer.AddService<ITestInterface>(new MyTestImplementation());
		}
			
		
		[Test]
		public void ContainsService()
		{
			bool contains = ServiceContainer.Contains(typeof(ITestInterface));
			Assert.That(contains,Is.True);
		}
		
		
		[Test]
		public void DosNotContainsService()
		{
			bool contains = ServiceContainer.Contains(typeof(IServiceProvider));
			Assert.That(contains,Is.False);
		}	
		
		
		[TestFixtureSetUp]
		public void Init()
		{
			ServiceContainer.InitializeServiceContainer();
			ServiceContainer.AddService<ITestInterface>(new MyTestImplementation());
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
	}
}
