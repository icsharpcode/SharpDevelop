// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using NUnit.Framework;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	[RequiresSTA]
	public class CodeCompletionTests : TextEditorBasedTests
	{
		#region CtrlSpace
		[Test]
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
		public void CtrlSpaceTest21()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button AllowDrop='True' Grid.Row='0' Content='test ";
			string fileFooter = @"' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, false,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(!list.Items.Any());
			              });
		}
		
		[Test]
		public void CtrlSpaceTest22()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Window.Resources>
		<Style TargetType='{x:Type Button}'>
			<Setter Property='";
			string fileFooter = @"'
		</Style>
	</Window.Resources>
	<Grid>
		<Button AllowDrop='True' Grid.Row='0' Content='test ' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              });
		}
		
		[Test]
		public void CtrlSpaceTest23()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Window.Resources>
		<Style TargetType='Button'>
			<Setter Property='";
			string fileFooter = @"'
		</Style>
	</Window.Resources>
	<Grid>
		<Button AllowDrop='True' Grid.Row='0' Content='test ' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              });
		}
		
		[Test]
		public void CtrlSpaceTest24()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Window.Resources>
		<Style TargetType='Button'>
			<Setter Property='AllowDrop' Value='";
			string fileFooter = @"'
		</Style>
	</Window.Resources>
	<Grid>
		<Button AllowDrop='True' Grid.Row='0' Content='test ' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              });
		}
		
		[Test]
		public void CtrlSpaceTest25()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Window.Resources>
		<Style TargetType='{x:Type Button}'>
			<Setter Property='AllowDrop' Value'";
			string fileFooter = @"'
		</Style>
	</Window.Resources>
	<Grid>
		<Button AllowDrop='True' Grid.Row='0' Content='test ' />
	</Grid>
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              });
		}
		
		[Test]
		public void CtrlSpaceTest26()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='clr-namespace:System.Windows;assembly=PresentationFramework'
   	xmlns:c='clr-namespace:System.Windows.Controls;assembly=PresentationFramework'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
	Title='Test' Height='300' Width='300'>
	<c:Grid>
		<c:Button AllowDrop='True' c:Grid.";
			string fileFooter = @" />
	</c:Grid>
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
		public void CtrlSpaceTest27()
		{
			string fileHeader = @"<Window xmlns='clr-namespace:System.Windows;assembly=PresentationFramework'
 xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ";
			string fileFooter = @">
</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	var items = list.Items.Select(item => item.Text).ToArray();
			              	Assert.Contains("x:Class", items);
			              	Assert.Contains("x:Subclass", items);
			              	Assert.Contains("x:ClassModifier", items);
			              	Assert.False(items.Contains("x:FieldModifier"));
			              });
		}
		#endregion
		
		#region KeyPress
		[Test]
		public void TypeAtValueEndingInSpace()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button AllowDrop='True' Grid.Row='0' Content='test ";
			string fileFooter = @"' />
	</Grid>
</Window>";
			
			TestKeyPress(fileHeader, fileFooter, 'a', CodeCompletionKeyPressResult.None,
			             list => {
			             	Assert.AreEqual(0, list.PreselectionLength);
			             	Assert.IsNull(list.SuggestedItem);
			             	Assert.IsTrue(!list.Items.Any());
			             });
		}
		
		[Test]
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
		public void ElementAttributeDotPressTest04()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button";
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
		public void ElementAttributeDotPressTest05()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='clr-namespace:System.Windows;assembly=PresentationFramework'
   	xmlns:c='clr-namespace:System.Windows.Controls;assembly=PresentationFramework'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
	Title='Test' Height='300' Width='300'>
	<c:Grid>
		<c:Button AllowDrop='True' c:Grid";
			string fileFooter = @" />
	</c:Grid>
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
		#endregion
	}
}
