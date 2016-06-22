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

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class ScheduledFoldGenerator : IFoldGenerator
	{
		IFoldGenerator foldGenerator;
		IFoldGenerationTimer timer;
		
		public ScheduledFoldGenerator(IFoldGenerator foldGenerator)
			: this(foldGenerator, new FoldGenerationTimer())
		{
		}
		
		public ScheduledFoldGenerator(
			IFoldGenerator foldGenerator,
			IFoldGenerationTimer timer)
		{
			this.foldGenerator = foldGenerator;
			this.timer = timer;
			
			GenerateFolds();
			timer.Tick += TimerElapsed;
			timer.Start();
		}
		
		void TimerElapsed(object sender, EventArgs e)
		{
			GenerateFolds();
		}
		
		public void GenerateFolds()
		{
			foldGenerator.GenerateFolds();
		}
		
		public void Dispose()
		{
			timer.Tick -= TimerElapsed;
			timer.Dispose();
			foldGenerator.Dispose();
		}
	}
}
