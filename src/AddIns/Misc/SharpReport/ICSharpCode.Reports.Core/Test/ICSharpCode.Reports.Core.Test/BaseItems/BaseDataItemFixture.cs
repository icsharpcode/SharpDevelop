/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 18.01.2009
 * Zeit: 18:01
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using ICSharpCode.Reports.Core.Exporter;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.BaseItems
{
	[TestFixture]
	public class BaseDataItemFixture
	{
		[Test]
		public void DefaultConstructur()
		{
			BaseDataItem bdi = new BaseDataItem();
			Assert.IsNotNull(bdi);
		}
		
		
		[Test]
		public void DefaultSettings()
		{
			BaseDataItem bdi = new BaseDataItem();
			Assert.AreEqual("System.String",bdi.DataType);
			Assert.AreEqual(GlobalValues.UnboundName,bdi.ColumnName);
			Assert.IsTrue(String.IsNullOrEmpty(bdi.BaseTableName));
			Assert.AreEqual("." + GlobalValues.UnboundName,bdi.MappingName);
		}
		
		[Test]
		public void ConstructurWithColumnName()
		{
			BaseDataItem bdi = new BaseDataItem("TestColumn");
			Assert.IsNotNull(bdi);
			Assert.AreEqual("System.String",bdi.DataType);
			Assert.AreEqual("TestColumn",bdi.ColumnName);
			Assert.IsTrue(String.IsNullOrEmpty(bdi.BaseTableName));
			Assert.AreEqual("." + "TestColumn",bdi.MappingName);
		}
		
		[Test]
		public void GetSetColumnName ()
		{
			BaseDataItem bdi = new BaseDataItem();
			bdi.ColumnName = "MyColumn";
			Assert.AreEqual("MyColumn",bdi.ColumnName,"ColumnName should be eqaul <MyColumn>");
		}
		
		[Test]
		public void GetSetTableName ()
		{
			BaseDataItem bdi = new BaseDataItem();
			bdi.BaseTableName = "MyTable";
			Assert.AreEqual("MyTable",bdi.BaseTableName,"ColumnName should be eqaul <MyTable>");
		
		}
		
		
		[Test]
		public void GetSetMappingName ()
		{
			BaseDataItem bdi = new BaseDataItem();
			bdi.BaseTableName = "MyTable";
			bdi.ColumnName = "MyColumn";
			Assert.AreEqual("MyTable" +"." + "MyColumn",bdi.MappingName,"ColumnName should be eqaul <MyTable.MyColumn>");
		
		}
		[Test]
		public void GetSetDataType ()
		{
			BaseDataItem bdi = new BaseDataItem();
			bdi.DataType ="System.String";
			Assert.AreEqual("System.String",bdi.DataType,"DataType should be <System.String>");
		}
		
		
		[Test]
		public void GetSetDBValue ()
		{
			BaseDataItem bdi = new BaseDataItem();
			bdi.DBValue ="DBValue";
			Assert.AreEqual("DBValue",bdi.DBValue,"DataType should be <DBValue>");
		}
		[Test]
		public void GetSetNullValue ()
		{
			BaseDataItem bdi = new BaseDataItem();
			bdi.NullValue ="NullValue";
			Assert.AreEqual("NullValue",bdi.NullValue,"DataType should be <NullValue>");
		}
		
		
		[Test] 
		public void ToStringOverride ()
		{
			BaseDataItem bdi = new BaseDataItem("TestColumn");
			Assert.AreEqual ("BaseDataItem",bdi.ToString(),"ToString override");
		}
		
		#region Exporter
		
		[Test]
		public void ExportColumnIsNotNull ()
		{
			BaseDataItem bt = new BaseDataItem();
			BaseExportColumn bec = bt.CreateExportColumn();
			Assert.IsNotNull(bec);
		}
		
		
		[Test]
		public void TypeofExportShouldBeExportText ()
		{
			BaseTextItem bt = new BaseDataItem();
			BaseExportColumn bec = bt.CreateExportColumn();
			Type t = typeof(ExportText);
			Assert.AreEqual(t,bec.GetType(),"Type should be 'ExportText");
		}
		
		[Test]
		public void TextValueEqualExportedText ()
		{
			BaseDataItem bt = new BaseDataItem();
			bt.Text = "Text";
			ExportText bec = (ExportText)bt.CreateExportColumn();
			bec.Text = bt.Text;
		}
		#endregion
	}
}
