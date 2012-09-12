// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Core
{	
	/// <summary>
	/// Interface for AnalyticsMonitorService.
	/// </summary>
	/// <remarks>Implementations of this interface must be thread-safe.</remarks>
	[SDService("SD.AnalyticsMonitor", FallbackImplementation = typeof(AnalyticsMonitorFallback))]
	public interface IAnalyticsMonitor
	{
		/// <summary>
		/// Tracks an exception that has occurred.
		/// </summary>
		void TrackException(Exception exception);
		
		/// <summary>
		/// Tracks a feature use.
		/// </summary>
		/// <param name="featureName">Name of the feature</param>
		/// <param name="activationMethod">Method used to 'activate' the feature (e.g. Menu, Toolbar, Shortcut, etc.)</param>
		/// <returns>Object that can be used to 'end' the feature use, if measuring time spans is desired.</returns>
		IAnalyticsMonitorTrackedFeature TrackFeature(string featureName, string activationMethod = null);
		
		/// <summary>
		/// Tracks a feature use.
		/// </summary>
		/// <param name="featureClass">Class containing the feature</param>
		/// <param name="featureName">Name of the feature</param>
		/// <param name="activationMethod">Method used to 'activate' the feature (e.g. Menu, Toolbar, Shortcut, etc.)</param>
		/// <returns>Object that can be used to 'end' the feature use, if measuring time spans is desired.</returns>
		IAnalyticsMonitorTrackedFeature TrackFeature(Type featureClass, string featureName = null, string activationMethod = null);
	}
	
	/// <summary>
	/// Allows marking the end-time of feature uses.
	/// </summary>
	/// <remarks>Implementations of this interface must be thread-safe.</remarks>
	public interface IAnalyticsMonitorTrackedFeature
	{
		void EndTracking();
	}
	
	sealed class AnalyticsMonitorFallback : IAnalyticsMonitor, IAnalyticsMonitorTrackedFeature
	{
		public void TrackException(Exception exception)
		{
		}
		
		public IAnalyticsMonitorTrackedFeature TrackFeature(string featureName, string activationMethod)
		{
			return this;
		}
		
		public IAnalyticsMonitorTrackedFeature TrackFeature(Type featureClass, string featureName, string activationMethod)
		{
			return this;
		}
		
		void IAnalyticsMonitorTrackedFeature.EndTracking()
		{
		}
	}
}
