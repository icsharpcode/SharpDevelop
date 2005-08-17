using System;
using System.Text;

namespace NUnit.Framework
{
	/// <summary>
	/// PlatformAttribute is used to mark a test fixture or an
	/// individual method as applying to a particular platform only.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple=false)]
	public sealed class PlatformAttribute : Attribute
	{
		private string include;
		private string exclude;

		public PlatformAttribute() { }

		public PlatformAttribute( string platforms )
		{
			this.include = platforms;
		}

		/// <summary>
		/// Name of the platform that is needed in order for
		/// a test to run. Multiple platforms may be given,
		/// separated by a comma.
		/// </summary>
		public string Include
		{
			get { return this.include; }
			set { include = value; }
		}

		/// <summary>
		/// Name of the platform to be excluded. Multiple platforms
		/// may be given, separated by a comma.
		/// </summary>
		public string Exclude
		{
			get { return this.exclude; }
			set { this.exclude = value; }
		}
	}
}
