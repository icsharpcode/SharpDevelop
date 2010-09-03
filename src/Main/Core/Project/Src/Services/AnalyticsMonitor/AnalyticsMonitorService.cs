// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Core.Services;
using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Allows marking the end-time of feature uses.
	/// </summary>
	/// <remarks>Implementations of this interface must be thread-safe.</remarks>
	public interface IAnalyticsMonitorTrackedFeature
	{
		void EndTracking();
	}
	
	/// <summary>
	/// Allows tracking feature use.
	/// All methods on this class are thread-safe.
	/// </summary>
	public static class AnalyticsMonitorService
	{
		/// <summary>
		/// Tracks an exception that has occurred.
		/// </summary>
		public static void TrackException(Exception exception)
		{
			if (exception == null)
				throw new ArgumentNullException("exception");
			IAnalyticsMonitor monitor = ServiceManager.Instance.GetService<IAnalyticsMonitor>();
			if (monitor != null) {
				monitor.TrackException(exception);
			}
		}
		
		/// <summary>
		/// Tracks a feature use.
		/// </summary>
		/// <param name="featureName">Name of the feature</param>
		/// <returns>Object that can be used to 'end' the feature use, if measuring time spans is desired.</returns>
		public static IAnalyticsMonitorTrackedFeature TrackFeature(string featureName)
		{
			return TrackFeature(featureName, null);
		}
		
		/// <summary>
		/// Tracks a feature use.
		/// </summary>
		/// <param name="featureName">Name of the feature</param>
		/// <param name="activationMethod">Method used to 'activate' the feature (e.g. Menu, Toolbar, Shortcut, etc.)</param>
		/// <returns>Object that can be used to 'end' the feature use, if measuring time spans is desired.</returns>
		public static IAnalyticsMonitorTrackedFeature TrackFeature(string featureName, string activationMethod)
		{
			if (featureName == null)
				throw new ArgumentNullException("featureName");
			
			if (activationMethod != null)
				LoggingService.Debug("Activated feature '" + featureName + "', activation=" + activationMethod);
			else
				LoggingService.Debug("Activated feature '" + featureName + "'");
			
			IAnalyticsMonitor monitor = ServiceManager.Instance.GetService<IAnalyticsMonitor>();
			if (monitor != null) {
				return monitor.TrackFeature(featureName, activationMethod) ?? DummyFeature.Instance;
			} else {
				return DummyFeature.Instance;
			}
		}
		
		sealed class DummyFeature : IAnalyticsMonitorTrackedFeature
		{
			public static readonly DummyFeature Instance = new DummyFeature();
			
			public void EndTracking()
			{
			}
		}
		
		
		/// <summary>
		/// Tracks a feature use.
		/// </summary>
		/// <param name="featureClass">Class containing the feature</param>
		/// <param name="featureName">Name of the feature</param>
		/// <param name="activationMethod">Method used to 'activate' the feature (e.g. Menu, Toolbar, Shortcut, etc.)</param>
		/// <returns>Object that can be used to 'end' the feature use, if measuring time spans is desired.</returns>
		public static IAnalyticsMonitorTrackedFeature TrackFeature(Type featureClass, string featureName = null, string activationMethod = null)
		{
			if (featureClass == null)
				throw new ArgumentNullException("featureClass");
			if (featureName != null)
				return TrackFeature(featureClass.FullName + "/" + featureName, activationMethod);
			else
				return TrackFeature(featureClass.FullName, activationMethod);
		}
	}
}
