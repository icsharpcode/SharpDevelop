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
			
			TestCtrlSpace(fileHeader, fileFooter,
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
			
			TestCtrlSpace(fileHeader, fileFooter,
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
			
			TestCtrlSpace(fileHeader, fileFooter,
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
			
			TestCtrlSpace(fileHeader, fileFooter,
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
			
			TestCtrlSpace(fileHeader, fileFooter,
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
			
			TestCtrlSpace(fileHeader, fileFooter,
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
			
			TestCtrlSpace(fileHeader, fileFooter,
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
			
			TestCtrlSpace(fileHeader, fileFooter,
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
			
			TestCtrlSpace(fileHeader, fileFooter,
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
			
			TestCtrlSpace(fileHeader, fileFooter,
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
			
			TestCtrlSpace(fileHeader, fileFooter,
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
	}
}
