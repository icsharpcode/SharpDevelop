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
