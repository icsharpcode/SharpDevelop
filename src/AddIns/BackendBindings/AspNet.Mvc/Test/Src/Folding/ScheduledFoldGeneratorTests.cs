// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc.Folding;
using NUnit.Framework;
using Rhino.Mocks;

namespace AspNet.Mvc.Tests.Folding
{
	[TestFixture]
	public class ScheduledFoldGeneratorTests
	{
		IFoldGenerationTimer fakeTimer;
		ScheduledFoldGenerator scheduledFoldGenerator;
		IFoldGenerator fakeFoldGenerator;
		int foldsGeneratedCount;
		
		void CreateScheduledFoldGenerator()
		{
			fakeTimer = MockRepository.GenerateStub<IFoldGenerationTimer>();
			fakeFoldGenerator = MockRepository.GenerateStub<IFoldGenerator>();
			scheduledFoldGenerator = new ScheduledFoldGenerator(fakeFoldGenerator, fakeTimer);
		}
		
		void RegisterFoldGenerationCounter()
		{
			foldsGeneratedCount = 0;
			fakeFoldGenerator.Stub(generator => generator.GenerateFolds())
				.WhenCalled(generator => foldsGeneratedCount++);
		}
		
		
		void FireTimer()
		{
			fakeTimer.Raise(timer => timer.Tick += null, fakeTimer, new EventArgs());
		}
		
		[Test]
		public void Constructor_FoldGeneratorPassedToBeScheduled_FoldsUpdatedBeforeTimerFires()
		{
			CreateScheduledFoldGenerator();
			
			fakeFoldGenerator.AssertWasCalled(generator => generator.GenerateFolds());
		}
		
		[Test]
		public void Constructor_FoldGeneratorPassedToBeScheduled_FoldGenerationTimerIsStarted()
		{
			CreateScheduledFoldGenerator();
			
			fakeTimer.AssertWasCalled(timer => timer.Start());
		}
		
		[Test]
		public void Dispose_FoldGeneratorPassedToBeScheduled_WrappedFoldGeneratorIsDisposed()
		{
			CreateScheduledFoldGenerator();
			scheduledFoldGenerator.Dispose();
			
			fakeFoldGenerator.AssertWasCalled(generator => generator.Dispose());
		}
		
		[Test]
		public void Dispose_FoldGeneratorPassedToBeScheduled_FoldGenerationTimerIsDisposed()
		{
			CreateScheduledFoldGenerator();
			scheduledFoldGenerator.Dispose();
			
			fakeTimer.AssertWasCalled(timer => timer.Dispose());
		}
		
		[Test]
		public void Constructor_TimerFires_FoldsAreUpdated()
		{
			CreateScheduledFoldGenerator();
			RegisterFoldGenerationCounter();
			
			FireTimer();
			
			Assert.AreEqual(1, foldsGeneratedCount);
		}
		
		[Test]
		public void Dispose_TimerFiresAfterDisposed_FoldsAreNotUpdated()
		{
			CreateScheduledFoldGenerator();
			RegisterFoldGenerationCounter();
			scheduledFoldGenerator.Dispose();
			
			FireTimer();
			
			Assert.AreEqual(0, foldsGeneratedCount);
		}
	}
}
