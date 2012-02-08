// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Threading;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class FoldGenerationTimer : DispatcherTimer, IFoldGenerationTimer
	{
		public static readonly TimeSpan TwoSecondInterval = new TimeSpan(0, 0, 2);
		public static readonly TimeSpan DefaultInterval = TwoSecondInterval;
		
		public FoldGenerationTimer()
			: base(DispatcherPriority.Background)
		{
			Interval = DefaultInterval;
		}
		
		public void Dispose()
		{
			Stop();
		}
	}
}
