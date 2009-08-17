// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>


using System;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using NUnit.Framework;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	public class CodeCompletionTests : TextEditorBasedTests
	{
		[Test]
		[STAThread]
		public void CtrlSpaceTest01()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		";
			string fileFooter = @"
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.Contains("Window", items);
			              	Assert.Contains("x:TypeExtension", items);
			              	Assert.Contains("!--", items);
			              	Assert.Contains("/Grid", items);
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest02()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button ";
			string fileFooter = @"/>
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.Contains("AllowDrop", items);
			              	Assert.Contains("Content", items);
			              	Assert.Contains("Grid", items);
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest03()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button ";
			string fileFooter = @" />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.Contains("AllowDrop", items);
			              	Assert.Contains("Content", items);
			              	Assert.Contains("Grid", items);
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest04()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button AllowDrop='";
			string fileFooter = @"' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.AreEqual(2, items.Length);
			              	Assert.Contains("True", items);
			              	Assert.Contains("False", items);
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest05()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<DockPanel>
		<Button AllowDrop='True' DockPanel.Dock='";
			string fileFooter = @"' />
	</DockPanel>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.AreEqual(4, items.Length);
			              	Assert.Contains("Top", items);
			              	Assert.Contains("Bottom", items);
			              	Assert.Contains("Left", items);
			              	Assert.Contains("Right", items);
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest06()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button AllowDrop='True' Grid.";
			string fileFooter = @" />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.Contains("Column", items);
			              	Assert.Contains("Row", items);
			              	Assert.Contains("RowSpan", items);
			              	Assert.Contains("ColumnSpan", items);
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest07()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Grid.ColumnDefinitions>
			";
			string fileFooter = @"
		<Button AllowDrop='True' Grid.Row='0' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.Contains("/Grid.ColumnDefinitions", items);
			              	Assert.Contains("ColumnDefinition", items);
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest08()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Grid.ColumnDefinitions>
			<ColumnDefinition ";
			string fileFooter = @"
		<Button AllowDrop='True' Grid.Row='0' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.Contains("Width", items);
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest09()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Grid.ColumnDefinitions>
			<ColumnDefinition Width='";
			string fileFooter = @"
		<Button AllowDrop='True' Grid.Row='0' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.Contains("*", items);
			              	Assert.Contains("Auto", items);
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest10()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ";
			string fileFooter = @">
	<Grid
		<Button AllowDrop='True' Grid.Row='0' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.Contains("xmlns:", items);
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest11()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:test='";
			string fileFooter = @"'>
	<Grid
		<Button AllowDrop='True' Grid.Row='0' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.Contains("http://schemas.microsoft.com/winfx/2006/xaml/presentation", items);
			              	Assert.Contains("http://schemas.microsoft.com/winfx/2006/xaml", items);
			              	Assert.Contains("http://schemas.openxmlformats.org/markup-compatibility/2006", items);
			              	Assert.Contains("System (mscorlib)", items);
			              	Assert.Contains("ICSharpCode.XamlBinding.Tests", items);
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest12()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button AllowDrop='True' Grid.Row='0' Content='{";
			string fileFooter = @"}' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.Contains("Binding", items);
			              	Assert.Contains("x:Type", items);
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest13()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button AllowDrop='True' Grid.Row='0' Content='{x:Type ";
			string fileFooter = @"}' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.Contains("Button", items);
			              	Assert.Contains("CheckBox", items);
			              	Assert.Contains("Type=", items);
			              	Assert.Contains("TypeName=", items);
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest14()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button AllowDrop='True' Grid.Row='0' Content='{x:Static ";
			string fileFooter = @"}' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.Contains("AlignmentX", items);
			              	Assert.Contains("AlignmentY", items);
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest15()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button AllowDrop='True' Grid.Row='0' Content='{x:Static Align";
			string fileFooter = @"}' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(5, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.Contains("AlignmentX", items);
			              	Assert.Contains("AlignmentY", items);
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest16()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button AllowDrop='True' Grid.Row='0' Content='{x:Static AlignmentX.";
			string fileFooter = @"}' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.Contains("Center", items);
			              	Assert.Contains("Left", items);
			              	Assert.Contains("Right", items);
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest17()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button AllowDrop='True' Grid.Row='0' Content='{x:Static AlignmentX.Le";
			string fileFooter = @"}' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(2, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.Contains("Center", items);
			              	Assert.Contains("Left", items);
			              	Assert.Contains("Right", items);
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest18()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button AllowDrop='True' Grid.Row='0' Content='{x:Type Type=";
			string fileFooter = @"}' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.Contains("Button", items);
			              	Assert.Contains("CheckBox", items);
			              	Assert.Contains("ComboBox", items);
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest19()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button AllowDrop='True' Grid.Row='0' Content='{Binding ";
			string fileFooter = @"}' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	foreach (var item in items)
			              		Assert.IsTrue(item.EndsWith("="));
			              });
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest20()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button AllowDrop='True' Grid.Row='0' Content='{x:Type TypeName=Button, ";
			string fileFooter = @"}' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	foreach (var item in items)
			              		Assert.IsTrue(item.EndsWith("="));
			              });
		}
		
		[Test]
		[STAThread]
		public void ElementAttributeDotPressTest01()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Grid";
			string fileFooter = @"
	</Grid>
</Window>";
			
			TestKeyPress(fileHeader, fileFooter, '.', CodeCompletionKeyPressResult.Completed,
			             list => {
			             	Assert.AreEqual(0, list.PreselectionLength);
			             	Assert.IsNull(list.SuggestedItem);
			             	Assert.IsTrue(list.Items.Any());
			             	var items = list.Items.Select(item => item.Text).ToArray();
			             	Assert.Contains("AllowDrop", items);
			             	Assert.Contains("ColumnDefinitions", items);
			             	Assert.Contains("ShowGridLines", items);
			             	Assert.Contains("Uid", items);
			             	Assert.Contains("Column", items);
			             	Assert.Contains("ColumnSpan", items);
			             });
		}
		
		[Test]
		[STAThread]
		public void ElementAttributeDotPressTest02()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button>
			<Grid";
			string fileFooter = @"
	</Grid>
</Window>";
			
			TestKeyPress(fileHeader, fileFooter, '.', CodeCompletionKeyPressResult.Completed,
			             list => {
			             	Assert.AreEqual(0, list.PreselectionLength);
			             	Assert.IsNull(list.SuggestedItem);
			             	Assert.IsTrue(list.Items.Any());
			             	var items = list.Items.Select(item => item.Text).ToArray();
			             	Assert.Contains("Column", items);
			             	Assert.Contains("ColumnSpan", items);
			             	Assert.Contains("Row", items);
			             });
		}
		
		[Test]
		[STAThread]
		public void ElementAttributeDotPressTest03()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button Grid";
			string fileFooter = @"
	</Grid>
</Window>";
			
			TestKeyPress(fileHeader, fileFooter, '.', CodeCompletionKeyPressResult.Completed,
			             list => {
			             	Assert.AreEqual(0, list.PreselectionLength);
			             	Assert.IsNull(list.SuggestedItem);
			             	Assert.IsTrue(list.Items.Any());
			             	var items = list.Items.Select(item => item.Text).ToArray();
			             	Assert.Contains("Column", items);
			             	Assert.Contains("ColumnSpan", items);
			             	Assert.Contains("Row", items);
			             });
		}
		
		[Test]
		[STAThread]
		public void ElementAttributeDotPressTest04()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button ";
			string fileFooter = @"
	</Grid>
</Window>";
			
			TestKeyPress(fileHeader, fileFooter, '.', CodeCompletionKeyPressResult.Completed,
			             list => {
			             	Assert.AreEqual(0, list.PreselectionLength);
			             	Assert.IsNull(list.SuggestedItem);
			             	Assert.IsFalse(list.Items.Any());
			             });
		}
		
		[Test]
		[STAThread]
		public void LowerThanPressedTest()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		";
			string fileFooter = @"
	</Grid>
</Window>";
			
			TestKeyPress(fileHeader, fileFooter, '<', CodeCompletionKeyPressResult.Completed,
			             list => {
			             	Assert.AreEqual(0, list.PreselectionLength);
			             	Assert.IsNull(list.SuggestedItem);
			             	Assert.IsTrue(list.Items.Any());
			             	var items = list.Items.Select(item => item.Text).ToArray();
			             	Assert.Contains("!--", items);
			             	Assert.Contains("![CDATA[", items);
			             	Assert.Contains("Button", items);
			             	Assert.Contains("CheckBox", items);
			             	Assert.Contains("Grid", items);
			             });
		}
		
		[Test]
		[STAThread]
		public void InCommentPressTest()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<!--";
			string fileFooter = @" -->
	</Grid>
</Window>";
			
			TestKeyPress(fileHeader, fileFooter, '<', CodeCompletionKeyPressResult.None, list => {});
			TestKeyPress(fileHeader, fileFooter, '.', CodeCompletionKeyPressResult.None, list => {});
			TestKeyPress(fileHeader, fileFooter, ':', CodeCompletionKeyPressResult.None, list => {});
			TestKeyPress(fileHeader, fileFooter, '/', CodeCompletionKeyPressResult.None, list => {});
			TestKeyPress(fileHeader, fileFooter, 'a', CodeCompletionKeyPressResult.None, list => {});
		}
		
		[Test]
		[STAThread]
		public void InCDataPressTest()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<![CDATA[";
			string fileFooter = @" ]]>
	</Grid>
</Window>";
			
			TestKeyPress(fileHeader, fileFooter, '<', CodeCompletionKeyPressResult.None, list => {});
			TestKeyPress(fileHeader, fileFooter, '.', CodeCompletionKeyPressResult.None, list => {});
			TestKeyPress(fileHeader, fileFooter, ':', CodeCompletionKeyPressResult.None, list => {});
			TestKeyPress(fileHeader, fileFooter, '/', CodeCompletionKeyPressResult.None, list => {});
			TestKeyPress(fileHeader, fileFooter, 'a', CodeCompletionKeyPressResult.None, list => {});
		}
		
		[Test]
		[STAThread]
		public void InPlainTextPressTest()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		";
			string fileFooter = @"
	</Grid>
</Window>";
			
			TestKeyPress(fileHeader, fileFooter, '.', CodeCompletionKeyPressResult.None, list => {});
			TestKeyPress(fileHeader, fileFooter, ':', CodeCompletionKeyPressResult.None, list => {});
			TestKeyPress(fileHeader, fileFooter, '/', CodeCompletionKeyPressResult.None, list => {});
			TestKeyPress(fileHeader, fileFooter, 'a', CodeCompletionKeyPressResult.None, list => {});
			TestKeyPress(fileHeader, fileFooter, '<', CodeCompletionKeyPressResult.Completed, list => {});
		}
	}
}
