// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ICSharpCode.UsageDataCollector
{
	[DataContract]
	sealed class UsageDataMessage
	{
		[DataMember]
		public Guid UserID;
		
		[DataMember]
		public List<UsageDataSession> Sessions = new List<UsageDataSession>();
		
		public UsageDataSession FindSession(long sessionID)
		{
			foreach (UsageDataSession s in Sessions) {
				if (s.SessionID == sessionID)
					return s;
			}
			throw new ArgumentException("Session not found.");
		}
	}
	
	[DataContract]
	sealed class UsageDataSession
	{
		[DataMember]
		public long SessionID;
		
		[DataMember]
		public DateTime StartTime;
		
		[DataMember]
		public DateTime? EndTime;
		
		[DataMember]
		public List<UsageDataEnvironmentProperty> EnvironmentProperties = new List<UsageDataEnvironmentProperty>();
		
		[DataMember]
		public List<UsageDataFeatureUse> FeatureUses = new List<UsageDataFeatureUse>();
		
		[DataMember]
		public List<UsageDataException> Exceptions = new List<UsageDataException>();
	}
	
	[DataContract]
	sealed class UsageDataEnvironmentProperty
	{
		[DataMember]
		public string Name;
		
		[DataMember]
		public string Value;
	}
	
	[DataContract]
	sealed class UsageDataFeatureUse
	{
		[DataMember]
		public DateTime Time;
		
		[DataMember]
		public DateTime? EndTime;
		
		[DataMember]
		public string FeatureName;
		
		[DataMember]
		public string ActivationMethod;
	}
	
	[DataContract]
	sealed class UsageDataException
	{
		[DataMember]
		public DateTime Time;
		
		[DataMember]
		public string ExceptionType;
		
		[DataMember]
		public string StackTrace;
	}
}
