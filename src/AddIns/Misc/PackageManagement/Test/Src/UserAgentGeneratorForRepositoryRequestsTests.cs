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
using System.Net;
using ICSharpCode.PackageManagement;
using NuGet;
using NUnit.Framework;
using Rhino.Mocks;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class UserAgentGeneratorForRepositoryRequestsTests
	{
		UserAgentGeneratorForRepositoryRequests generator;
		IPackageRepositoryFactoryEvents repositoryFactoryEvents;
		
		void CreateGenerator()
		{
			repositoryFactoryEvents = MockRepository.GenerateStub<IPackageRepositoryFactoryEvents>();
			generator = new UserAgentGeneratorForRepositoryRequests(repositoryFactoryEvents);
		}
		
		IPackageRepository CreatePackageRepository()
		{
			return MockRepository.GenerateStub<IPackageRepository>();
		}
		
		IHttpClientEvents CreatePackageRepositoryThatImplementsIHttpClientEvents()
		{
			return MockRepository.GenerateMock<IHttpClientEvents, IPackageRepository>();
		}
		
		void FireRepositoryCreatedEvent(IHttpClientEvents clientEvents)
		{
			FireRepositoryCreatedEvent(clientEvents as IPackageRepository);
		}
		
		void FireRepositoryCreatedEvent(IPackageRepository repository)
		{
			var eventArgs = new PackageRepositoryFactoryEventArgs(repository);
			repositoryFactoryEvents.Raise(
				events => events.RepositoryCreated += null,
				repositoryFactoryEvents,
				eventArgs);
		}
		
		WebRequest FireSendingRequestEvent(IHttpClientEvents clientEvents)
		{
			WebRequest request = CreateWebRequest();
			request.Headers = new WebHeaderCollection();
			
			var eventArgs = new WebRequestEventArgs(request);
			clientEvents.Raise(
				events => events.SendingRequest += null,
				clientEvents,
				eventArgs);
			
			return request;
		}
		
		WebRequest CreateWebRequest()
		{
			return MockRepository.GenerateStub<WebRequest>();
		}
		
		[Test]
		public void SendingRequest_UserAgentGeneration_UserAgentSetOnRequest()
		{
			CreateGenerator();
			IHttpClientEvents clientEvents = CreatePackageRepositoryThatImplementsIHttpClientEvents();
			FireRepositoryCreatedEvent(clientEvents);
			
			WebRequest request = FireSendingRequestEvent(clientEvents);
			
			string userAgent = request.Headers[HttpRequestHeader.UserAgent];
			Assert.IsTrue(userAgent.StartsWith("SharpDevelop"), userAgent);
		}
		
		[Test]
		public void RepositoryCreated_RepositoryDoesNotImplementIHttpClientEvents_NullReferenceExceptionNotThrown()
		{
			CreateGenerator();
			IPackageRepository repository = CreatePackageRepository();
			
			FireRepositoryCreatedEvent(repository);
		}
	}
}
