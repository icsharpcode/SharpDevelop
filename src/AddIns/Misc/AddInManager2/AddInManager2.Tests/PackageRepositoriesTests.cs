// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AddInManager2;
using ICSharpCode.AddInManager2.Model;
using ICSharpCode.AddInManager2.Tests.Fakes;
using ICSharpCode.SharpDevelop;
using NuGet;
using NUnit.Framework;

namespace ICSharpCode.AddInManager2.Tests
{
	[TestFixture]
	[Category("PackageRepositoriesTests")]
	public class PackageRepositoriesTests
	{
		[Test, Description("Configured repositories must be correctly loaded from settings.")]
		public void ReadRepositoriesFromConfiguration()
		{
			FakeAddInManagerSettings settings = new FakeAddInManagerSettings();
			settings.PackageRepositories = new string[]
			{
				@"Repository1=C:\Repositories\Repository1",
				@"Repository2=C:\Repositories\Repository2"
			};
			
			AddInManagerEvents events = new AddInManagerEvents();
			PackageRepositories packageRepositories = new PackageRepositories(events, settings);
			
			var packageSources = packageRepositories.RegisteredPackageSources;
			Assert.That(packageSources, Is.Not.Null);
			Assert.That(packageSources.Count(), Is.GreaterThan(0));
			Assert.That(packageSources.ElementAt(0).Name, Is.EqualTo("Repository1"));
			Assert.That(packageSources.ElementAt(1).Name, Is.EqualTo("Repository2"));
			Assert.That(packageSources.ElementAt(0).Source, Is.EqualTo(@"C:\Repositories\Repository1"));
			Assert.That(packageSources.ElementAt(1).Source, Is.EqualTo(@"C:\Repositories\Repository2"));
		}
		
		[Test, Description("Configured repositories must be correctly saved to settings.")]
		public void SaveRepositoriesToConfiguration()
		{
			FakeAddInManagerSettings settings = new FakeAddInManagerSettings();
			settings.PackageRepositories = new string[]
			{
				@"Repository1=C:\Repositories\Repository1",
				@"Repository2=C:\Repositories\Repository2"
			};
			
			AddInManagerEvents events = new AddInManagerEvents();
			PackageRepositories packageRepositories = new PackageRepositories(events, settings);
			
			List<PackageSource> packageSources = new List<PackageSource>();
			packageSources.Add(new PackageSource(@"C:\Repositories\Repository3", "Repository3"));
			packageSources.Add(new PackageSource(@"C:\Repositories\Repository4", "Repository4"));
			packageRepositories.RegisteredPackageSources = packageSources;
			
			var packageRepositoriesSetting = settings.PackageRepositories;
			Assert.That(packageRepositoriesSetting, Is.Not.Null);
			Assert.That(packageRepositoriesSetting.Count(), Is.GreaterThan(0));
			Assert.That(packageRepositoriesSetting.ElementAt(0), Is.EqualTo(@"Repository3=C:\Repositories\Repository3"));
			Assert.That(packageRepositoriesSetting.ElementAt(1), Is.EqualTo(@"Repository4=C:\Repositories\Repository4"));
		}
	}
}