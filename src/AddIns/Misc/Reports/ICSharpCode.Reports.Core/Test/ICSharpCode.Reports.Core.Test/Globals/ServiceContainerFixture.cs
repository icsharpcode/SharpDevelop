/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 11.02.2011
 * Time: 20:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
