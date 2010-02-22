// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
