// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;

namespace ICSharpCode.Core.Tests.AddInTreeTests.Tests
{
	[TestFixture]
	public class AddInTreeLoadingTests
	{
		#region AddIn node tests
		[Test]
		public void TestEmptyAddInTreeLoading()
		{
			string addInText = @"<AddIn/>";
			AddIn addIn = AddIn.Load(new StringReader(addInText));
		}
		
		[Test]
		public void TestAddInProperties()
		{
			string addInText = @"
<AddIn name        = 'SharpDevelop Core'
       author      = 'Mike Krueger'
       copyright   = 'GPL'
       url         = 'http://www.icsharpcode.net'
       description = 'SharpDevelop core module'
       version     = '1.0.0'/>";
			AddIn addIn = AddIn.Load(new StringReader(addInText));
			Assert.AreEqual(addIn.Properties["name"], "SharpDevelop Core");
			Assert.AreEqual(addIn.Properties["author"], "Mike Krueger");
			Assert.AreEqual(addIn.Properties["copyright"], "GPL");
			Assert.AreEqual(addIn.Properties["url"], "http://www.icsharpcode.net");
			Assert.AreEqual(addIn.Properties["description"], "SharpDevelop core module");
			Assert.AreEqual(addIn.Properties["version"], "1.0.0");
		}
		#endregion 
		
		#region Runtime section tests
		[Test]
		public void TestEmtpyRuntimeSection()
		{
			string addInText = @"<AddIn><Runtime/></AddIn>";
			AddIn addIn = AddIn.Load(new StringReader(addInText));
		}		
		
		[Test]
		public void TestEmtpyRuntimeSection2()
		{
			string addInText = @"<AddIn> <!-- Comment1 --> <Runtime>  <!-- Comment2 -->    </Runtime> <!-- Comment3 --> </AddIn>";
			AddIn addIn = AddIn.Load(new StringReader(addInText));
		}
		
		[Test]
		public void TestRuntimeSectionImport()
		{
			string addInText = @"
<AddIn>
	<Runtime>
		<Import assembly = 'Test.dll'/>
	</Runtime>
</AddIn>";
			AddIn addIn = AddIn.Load(new StringReader(addInText));
			Assert.AreEqual(1, addIn.Runtimes.Count);
			Assert.AreEqual(addIn.Runtimes[0].Assembly, "Test.dll");
		}
		
		[Test]
		public void TestRuntimeSectionComplexImport()
		{
			string addInText = @"
<AddIn>
	<Runtime>
		<Import assembly = '../bin/SharpDevelop.Base.dll'>
			<Erbauer   name='MyErbauer'   class = 'ICSharpCode.Core.ClassErbauer'/>
			<Auswerter name='MyCompare' class = 'ICSharpCode.Core.CompareCondition'/>
			<Erbauer   name='Test'   class = 'ICSharpCode.Core.ClassErbauer2'/>
			<Auswerter name='Condition2' class = 'Condition2Class'/>
		</Import>
	</Runtime>
</AddIn>";
			AddIn addIn = AddIn.Load(new StringReader(addInText));
			Assert.AreEqual(1, addIn.Runtimes.Count);
			Assert.AreEqual(addIn.Runtimes[0].Assembly, "../bin/SharpDevelop.Base.dll");
			Assert.AreEqual(addIn.Runtimes[0].DefinedErbauer.Count, 2);
			Assert.AreEqual(addIn.Runtimes[0].DefinedErbauer[0]["name"], "MyErbauer");
			Assert.AreEqual(addIn.Runtimes[0].DefinedErbauer[0]["class"], "ICSharpCode.Core.ClassErbauer");
			
			Assert.AreEqual(addIn.Runtimes[0].DefinedErbauer[1]["name"], "Test");
			Assert.AreEqual(addIn.Runtimes[0].DefinedErbauer[1]["class"], "ICSharpCode.Core.ClassErbauer2");
			
			Assert.AreEqual(addIn.Runtimes[0].DefinedConditions.Count, 2);
			Assert.AreEqual(addIn.Runtimes[0].DefinedConditions[0]["name"], "MyCompare");
			Assert.AreEqual(addIn.Runtimes[0].DefinedConditions[0]["class"], "ICSharpCode.Core.CompareCondition");
			
			Assert.AreEqual(addIn.Runtimes[0].DefinedConditions[1]["name"], "Condition2");
			Assert.AreEqual(addIn.Runtimes[0].DefinedConditions[1]["class"], "Condition2Class");
		}
		#endregion
		
		#region Path section tests
		[Test]
		public void TestEmptyPathSection()
		{
			string addInText = @"
<AddIn>
	<Path name = '/Path1'/>
	<Path name = '/Path2'/>
	<Path name = '/Path1/SubPath'/>
</AddIn>";
			AddIn addIn = AddIn.Load(new StringReader(addInText));
			Assert.AreEqual(3, addIn.Paths.Count);
			Assert.IsNotNull(addIn.Paths["/Path1"]);
			Assert.IsNotNull(addIn.Paths["/Path2"]);
			Assert.IsNotNull(addIn.Paths["/Path1/SubPath"]);
		}
		
		[Test]
		public void TestSimpleCodon()
		{
			string addInText = @"
<AddIn>
	<Path name = '/Path1'>
		<Simple id ='Simple' attr='a' attr2='b'/>
	</Path>
</AddIn>";
			AddIn addIn = AddIn.Load(new StringReader(addInText));
			Assert.AreEqual(1, addIn.Paths.Count);
			Assert.IsNotNull(addIn.Paths["/Path1"]);
			Assert.AreEqual(1, addIn.Paths["/Path1"].Codons.Count);
			Assert.AreEqual("Simple", addIn.Paths["/Path1"].Codons[0].Name);
			Assert.AreEqual("Simple", addIn.Paths["/Path1"].Codons[0].Id);
			Assert.AreEqual("a", addIn.Paths["/Path1"].Codons[0].Properties["attr"]);
			Assert.AreEqual("b", addIn.Paths["/Path1"].Codons[0].Properties["attr2"]);
		}
		
		[Test]
		public void TestSubCodons()
		{
			string addInText = @"
<AddIn>
	<Path name = '/Path1'>
		<Sub id='Path2'>
			<Codon2 id='Sub2'/>
		</Sub>
	</Path>
</AddIn>";
			AddIn addIn = AddIn.Load(new StringReader(addInText));
			Assert.AreEqual(2, addIn.Paths.Count);
			Assert.IsNotNull(addIn.Paths["/Path1"]);
			Assert.AreEqual(1, addIn.Paths["/Path1"].Codons.Count);
			Assert.AreEqual("Sub", addIn.Paths["/Path1"].Codons[0].Name);
			Assert.AreEqual("Path2", addIn.Paths["/Path1"].Codons[0].Id);
			
			Assert.IsNotNull(addIn.Paths["/Path1/Path2"]);
			Assert.AreEqual(1, addIn.Paths["/Path1/Path2"].Codons.Count);
			Assert.AreEqual("Codon2", addIn.Paths["/Path1/Path2"].Codons[0].Name);
			Assert.AreEqual("Sub2", addIn.Paths["/Path1/Path2"].Codons[0].Id);
		}
		
		[Test]
		public void TestSimpleCondition()
		{
			string addInText = @"
<AddIn>
	<Path name = '/Path1'>
		<Condition name='Equal' string='a' equal='b'>		
			<Simple id ='Simple' attr='a' attr2='b'/>
		</Condition>
	</Path>
</AddIn>";
			AddIn addIn = AddIn.Load(new StringReader(addInText));
			Assert.AreEqual(1, addIn.Paths.Count, "Paths != 1");
			ExtensionPath path = addIn.Paths["/Path1"];
			Assert.IsNotNull(path);
			Assert.AreEqual(1, path.Codons.Count);
			Codon codon = path.Codons[0];
			Assert.AreEqual("Simple", codon.Name);
			Assert.AreEqual("Simple", codon.Id);
			Assert.AreEqual("a",      codon["attr"]);
			Assert.AreEqual("b",      codon["attr2"]);
			
			// Test for condition.
			Assert.AreEqual(1, codon.Conditions.Length);
			Condition condition = codon.Conditions[0] as Condition;
			Assert.IsNotNull(condition);
			Assert.AreEqual("Equal", condition.Name);
			Assert.AreEqual("a", condition["string"]);
			Assert.AreEqual("b", condition["equal"]);
			
		}
		
		[Test]
		public void TestStackedCondition()
		{
			string addInText = @"
<AddIn>
	<Path name = '/Path1'>
		<Condition name='Equal' string='a' equal='b'>		
			<Condition name='StackedCondition' string='1' equal='2'>		
				<Simple id ='Simple' attr='a' attr2='b'/>
			</Condition>
			<Simple id ='Simple2' attr='a' attr2='b'/>
		</Condition>
			<Simple id ='Simple3' attr='a' attr2='b'/>
	</Path>
</AddIn>";
			AddIn addIn = AddIn.Load(new StringReader(addInText));
			Assert.AreEqual(1, addIn.Paths.Count);
			ExtensionPath path = addIn.Paths["/Path1"];
			Assert.IsNotNull(path);
			
			Assert.AreEqual(3, path.Codons.Count);
			Codon codon = path.Codons[0];
			Assert.AreEqual("Simple", codon.Name);
			Assert.AreEqual("Simple", codon.Id);
			Assert.AreEqual("a",      codon["attr"]);
			Assert.AreEqual("b",      codon["attr2"]);
			
			// Test for condition
			Assert.AreEqual(2, codon.Conditions.Length);
			Condition condition = codon.Conditions[1] as Condition;
			Assert.IsNotNull(condition);
			Assert.AreEqual("Equal", condition.Name);
			Assert.AreEqual("a", condition["string"]);
			Assert.AreEqual("b", condition["equal"]);
			
			condition = codon.Conditions[0] as Condition;
			Assert.IsNotNull(condition);
			Assert.AreEqual("StackedCondition", condition.Name);
			Assert.AreEqual("1", condition["string"]);
			Assert.AreEqual("2", condition["equal"]);
			
			codon = path.Codons[1];
			Assert.AreEqual(1, codon.Conditions.Length);
			condition = codon.Conditions[0] as Condition;
			Assert.IsNotNull(condition);
			Assert.AreEqual("Equal", condition.Name);
			Assert.AreEqual("a", condition["string"]);
			Assert.AreEqual("b", condition["equal"]);
			
			codon = path.Codons[2];
			Assert.AreEqual(0, codon.Conditions.Length);
			
		}
		
		[Test]
		public void TestComplexCondition()
		{
			string addInText = @"
<AddIn>
	<Path name = '/Path1'>
		<ComplexCondition>
			<And>
				<Not><Condition name='Equal' string='a' equal='b'/></Not>
				<Or>
					<Condition name='Equal' string='a' equal='b'/>
					<Condition name='Equal' string='a' equal='b'/>
					<Condition name='Equal' string='a' equal='b'/>
				</Or>
			</And>
			<Simple id ='Simple' attr='a' attr2='b'/>
		</ComplexCondition>
	</Path>
</AddIn>";
			AddIn addIn = AddIn.Load(new StringReader(addInText));
			Assert.AreEqual(1, addIn.Paths.Count);
			ExtensionPath path = addIn.Paths["/Path1"];
			Assert.IsNotNull(path);
			Assert.AreEqual(1, path.Codons.Count);
			Codon codon = path.Codons[0];
			Assert.AreEqual("Simple", codon.Name);
			Assert.AreEqual("Simple", codon.Id);
			Assert.AreEqual("a",      codon["attr"]);
			Assert.AreEqual("b",      codon["attr2"]);
			
			// Test for condition.
			Assert.AreEqual(1, codon.Conditions.Length);
		}
		
		#endregion
	}
}
