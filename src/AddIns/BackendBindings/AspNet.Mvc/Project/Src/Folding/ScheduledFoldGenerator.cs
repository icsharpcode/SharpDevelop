// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
