// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
