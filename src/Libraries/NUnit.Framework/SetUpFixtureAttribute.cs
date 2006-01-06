using System;

namespace NUnit.Framework
{
	/// <summary>
	/// SetUpFixtureAttribute is used to identify a SetUpFixture
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public sealed class SetUpFixtureAttribute : Attribute
	{
	}
}
