// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.UnitTesting;
using Gallio.Runner.Events;

namespace Gallio.SharpDevelop.Tests.Utils
{
	public class MockTestRunnerEvents : ITestRunnerEvents
	{
		#pragma warning disable 67
		
		public event EventHandler<Gallio.Runtime.Logging.LogEntrySubmittedEventArgs> LogEntrySubmitted;
		public event EventHandler<Gallio.Runner.Events.MessageReceivedEventArgs> MessageReceived;
		public event EventHandler<InitializeStartedEventArgs> InitializeStarted;
		public event EventHandler<InitializeFinishedEventArgs> InitializeFinished;
		public event EventHandler<DisposeStartedEventArgs> DisposeStarted;
		public event EventHandler<DisposeFinishedEventArgs> DisposeFinished;
		public event EventHandler<ExploreStartedEventArgs> ExploreStarted;
		public event EventHandler<ExploreFinishedEventArgs> ExploreFinished;
		public event EventHandler<RunStartedEventArgs> RunStarted;
		public event EventHandler<RunFinishedEventArgs> RunFinished;
		public event EventHandler<TestDiscoveredEventArgs> TestDiscovered;
		public event EventHandler<AnnotationDiscoveredEventArgs> AnnotationDiscovered;
		public event EventHandler<TestStepStartedEventArgs> TestStepStarted;
		public event EventHandler<TestStepFinishedEventArgs> TestStepFinished;
		public event EventHandler<TestStepLifecyclePhaseChangedEventArgs> TestStepLifecyclePhaseChanged;
		public event EventHandler<TestStepMetadataAddedEventArgs> TestStepMetadataAdded;
		public event EventHandler<TestStepLogAttachEventArgs> TestStepLogAttach;
		public event EventHandler<TestStepLogStreamWriteEventArgs> TestStepLogStreamWrite;
		public event EventHandler<TestStepLogStreamEmbedEventArgs> TestStepLogStreamEmbed;
		public event EventHandler<TestStepLogStreamBeginSectionBlockEventArgs> TestStepLogStreamBeginSectionBlock;
		public event EventHandler<TestStepLogStreamBeginMarkerBlockEventArgs> TestStepLogStreamBeginMarkerBlock;
		public event EventHandler<TestStepLogStreamEndBlockEventArgs> TestStepLogStreamEndBlock;
		
		#pragma warning restore 67
	
		public void FireTestStepFinishedEvent(string name)
		{
			TestStepFinishedEventArgs e = CreateTestStepFinishedEventArgs(name);
			FireTestStepFinishedEvent(e);
		}
		
		TestStepFinishedEventArgs CreateTestStepFinishedEventArgs(string testName)
		{
			GallioTestStepFinishedEventArgsFactory factory = new GallioTestStepFinishedEventArgsFactory();
			return factory.Create(testName);
		}
		
		void FireTestStepFinishedEvent(TestStepFinishedEventArgs e)
		{
			if (TestStepFinished != null) {
				TestStepFinished(this, e);
			}
		}
		
		public void FireTestStepFinishedEventForNonTestCase(string testName)
		{
			TestStepFinishedEventArgs e = CreateTestStepFinishedEventArgs(testName);
			e.Test.IsTestCase = false;
			FireTestStepFinishedEvent(e);
		}
		
		public void FireDisposeStartedEvent()
		{
			if (DisposeStarted != null) {
				DisposeStarted(this, new DisposeStartedEventArgs());
			}
		}
	}
}
