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
