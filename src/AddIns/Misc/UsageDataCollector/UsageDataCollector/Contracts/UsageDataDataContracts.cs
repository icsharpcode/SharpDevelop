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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ICSharpCode.UsageDataCollector.Contracts
{
	/// <summary>
	/// Represents a message used to send data to the server.
	/// </summary>
	[DataContract]
	public sealed class UsageDataMessage
	{
		/// <summary>
		/// Unique identifier per user.
		/// We need this to distinguish between 1 user using a feature 100 times, or 100 users using a feature 1 time.
		/// </summary>
		[DataMember]
		public Guid UserID;

		/// <summary>
		/// Gets the sessions stored in this message.
		/// </summary>
		[DataMember]
		public List<UsageDataSession> Sessions = new List<UsageDataSession>();
		
		/// <summary>
		/// Finds the session with the specified session ID.
		/// </summary>
		/// <exception cref="ArgumentException">No session with the specified ID was found.</exception>
		public UsageDataSession FindSession(long sessionID)
		{
			foreach (UsageDataSession s in Sessions) {
				if (s.SessionID == sessionID)
					return s;
			}
			throw new ArgumentException("Session not found.");
		}
	}
	
	/// <summary>
	/// A UsageData session.
	/// </summary>
	[DataContract]
	public sealed class UsageDataSession
	{
		/// <summary>
		/// ID of the session, usually unique per user (unless the user restores a backup of the database).
		/// Could be used to detect duplicate uploads.
		/// </summary>
		[DataMember]
		public long SessionID;
		
		/// <summary>
		/// Timestamp when the session was started.
		/// </summary>
		[DataMember]
		public DateTime StartTime;
		
		/// <summary>
		/// Timestamp when the session finished.
		/// Nullable because the end time might not be recorded if the application crashed.
		/// </summary>
		[DataMember]
		public DateTime? EndTime;
		
		/// <summary>
		/// List of environment properties associated with the session.
		/// </summary>
		[DataMember]
		public List<UsageDataEnvironmentProperty> EnvironmentProperties = new List<UsageDataEnvironmentProperty>();
		
		/// <summary>
		/// List of feature uses associated with the session.
		/// </summary>
		[DataMember]
		public List<UsageDataFeatureUse> FeatureUses = new List<UsageDataFeatureUse>();
		
		/// <summary>
		/// List of exceptions associated with the session.
		/// </summary>
		[DataMember]
		public List<UsageDataException> Exceptions = new List<UsageDataException>();
	}
	
	/// <summary>
	/// A property storing a value about the environment (App version, OS Version etc.).
	/// These are stored per session because the user might use different versions of an application in parallel,
	/// or have multiple OS installations pointing to the same database (e.g. when SharpDevelop is used on an USB stick).
	/// </summary>
	[DataContract]
	public sealed class UsageDataEnvironmentProperty
	{
		/// <summary>
		/// Name of the property.
		/// </summary>
		[DataMember]
		public string Name;
		
		/// <summary>
		/// Value of the property.
		/// </summary>
		[DataMember]
		public string Value;
	}
	
	/// <summary>
	/// Represents a feature being used.
	/// </summary>
	[DataContract]
	public sealed class UsageDataFeatureUse
	{
		/// <summary>
		/// The time when the feature was used.
		/// </summary>
		[DataMember]
		public DateTime Time;
		
		/// <summary>
		/// End time of the feature use. Nullable because for some features, the no time span will be recorded.
		/// Also, for features where the time span usually is recorded, crashes might cause the end time to be missing
		/// in some cases.
		/// </summary>
		[DataMember]
		public DateTime? EndTime;
		
		/// <summary>
		/// Name of the feature.
		/// </summary>
		[DataMember]
		public string FeatureName;
		
		/// <summary>
		/// How the feature was activated (Menu, Toolbar, Shortcut, etc.)
		/// </summary>
		[DataMember]
		public string ActivationMethod;
	}
	
	/// <summary>
	/// Represents an exception that was recorded.
	/// Only ExceptionType and StackTrace are available because those don't contain personal information.
	/// The exception message might e.g. contain names of the file the user is working on.
	/// </summary>
	[DataContract]
	public sealed class UsageDataException
	{
		/// <summary>
		/// The time when the exception occurred.
		/// </summary>
		[DataMember]
		public DateTime Time;
		
		/// <summary>
		/// The fully qualified typename of the exception.
		/// </summary>
		[DataMember]
		public string ExceptionType;
		
		/// <summary>
		/// The stack trace of the exception. The 'at'/'in' words are potentially localized.
		/// </summary>
		[DataMember]
		public string StackTrace;
	}
}
