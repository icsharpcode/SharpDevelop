// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Core.Services
{
	/// <summary>
	/// Interface for AnalyticsMonitorService.
	/// </summary>
	/// <remarks>Implementations of this interface must be thread-safe.</remarks>
	public interface IAnalyticsMonitor
	{
		void TrackException(Exception exception);
		IAnalyticsMonitorTrackedFeature TrackFeature(string featureName, string activationMethod);
	}
}
