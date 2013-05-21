/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 20.05.2013
 * Time: 18:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.DataManager.Listhandling;
using ICSharpCode.Reporting.DataSource;
using ICSharpCode.Reporting.Items;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.DataSource
{
	[TestFixture]
	public class CollectionHandlingFixture
	{
	
		private ContributorCollection list;

		[Test]
		public void CanInitDataCollection()
		{
			var collectionSource = new CollectionSource	(list,new ReportSettings());
			Assert.That(collectionSource,Is.Not.Null);
		}
		
		
		[Test]
		public void CollectionCountIsEqualToListCount() {
			var collectionSource = new CollectionSource	(list,new ReportSettings());
			Assert.That(collectionSource.Count,Is.EqualTo(list.Count));
		}
		
		
		[Test]
		public void AvailableFieldsEqualContibutorsPropertyCount() {
			var collectionSource = new CollectionSource	(list,new ReportSettings());
			Assert.That(collectionSource.AvailableFields.Count,Is.EqualTo(7));
		}
			
			
		
//http://stackoverflow.com/questions/5378293/simplest-way-to-filter-generic-list		
//http://stackoverflow.com/questions/5378293/simplest-way-to-filter-generic-list		
//http://netmatze.wordpress.com/2012/06/21/implementing-a-generic-iequalitycomparer-and-icomparer-class/		
//		http://blog.velir.com/index.php/2011/02/17/ilistt-sorting-a-better-way/
		[SetUp]
		public void CreateList() {
			var contributorList = new ContributorsList();
			list = contributorList.ContributorCollection;
		}	
	}
}
