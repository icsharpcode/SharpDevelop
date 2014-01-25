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
using System.Runtime.Serialization;

namespace ICSharpCode.Profiler.Controller.Data
{
	/// <summary>
	/// Thrown when the database used to internally store the usage data is incompatible with the DB version
	/// expected by the UsageDataCollector library.
	/// </summary>
	[Serializable]
	public class IncompatibleDatabaseException : Exception
	{
		/// <summary>
		/// Expected database version.
		/// </summary>
		public Version ExpectedVersion { get; set; }
		
		/// <summary>
		/// Actual database version.
		/// </summary>
		public Version ActualVersion { get; set; }
		
		/// <summary>
		/// Creates a new IncompatibleDatabaseException instance.
		/// </summary>
		public IncompatibleDatabaseException() {}
		
		/// <summary>
		/// Creates a new IncompatibleDatabaseException instance.
		/// </summary>
		public IncompatibleDatabaseException(Version expectedVersion, Version actualVersion)
			: base("Expected DB version " + expectedVersion + " but found " + actualVersion)
		{
			this.ExpectedVersion = expectedVersion;
			this.ActualVersion = actualVersion;
		}
		
		/// <summary>
		/// Deserializes an IncompatibleDatabaseException instance.
		/// </summary>
		protected IncompatibleDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			if (info != null) {
				this.ExpectedVersion = (Version)info.GetValue("ExpectedVersion", typeof(Version));
				this.ActualVersion = (Version)info.GetValue("ActualVersion", typeof(Version));
			}
		}
		
		/// <inheritdoc/>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			if (info != null) {
				info.AddValue("ExpectedVersion", ExpectedVersion, typeof(Version));
				info.AddValue("ActualVersion", ActualVersion, typeof(Version));
			}
		}
	}
}
