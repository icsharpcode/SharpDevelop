<!-- 
	Copyright (c) 2009 Daniel Grunwald
	
	Permission is hereby granted, free of charge, to any person obtaining a copy of this
	software and associated documentation files (the "Software"), to deal in the Software
	without restriction, including without limitation the rights to use, copy, modify, merge,
	publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
	to whom the Software is furnished to do so, subject to the following conditions:
	
	The above copyright notice and this permission notice shall be included in all copies or
	substantial portions of the Software.
	
	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
	PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
	FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
	OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
	DEALINGS IN THE SOFTWARE.
-->
<Window x:Class="$rootnamespace$.AvalonEdit.Sample.Window1"
	xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
	xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
	xmlns:avalonEdit="http://icsharpcode.net/sharpdevelop/avalonedit"
	xmlns:forms="clr-namespace:System.Windows.Forms;assembly=System.Windows.Forms"
	TextOptions.TextFormattingMode="Display"
	Title="AvalonEdit.Sample" Height="500" Width="700"
	>
	<DockPanel>
		<Menu DockPanel.Dock="Top">
			<MenuItem Header="_File">
				<MenuItem Header="_Open..." Click="openFileClick"/>
				<MenuItem Header="_Save" Click="saveFileClick"/>
				<Separator/>
				<MenuItem Header="E_xit" Click="exitClick"/>
			</MenuItem>
			<MenuItem Header="_Edit">
				<MenuItem Header="Cu_t" Command="Redo"/>
				<MenuItem Header="_Copy" Command="Copy"/>
				<MenuItem Header="_Paste" Command="Paste"/>
				<MenuItem Header="_Delete" Command="Delete"/>
				<Separator/>
				<MenuItem Header="_Undo" Command="Undo"/>
				<MenuItem Header="_Redo" Command="Redo"/>
			</MenuItem>
			<MenuItem Header="Fo_rmat">
				<MenuItem Header="_Word Wrap" IsCheckable="True" IsChecked="{Binding ElementName=textEditor,Path=WordWrap}"/>
				<MenuItem Header="Show _Line Numbers" IsCheckable="True" IsChecked="{Binding ElementName=textEditor,Path=ShowLineNumbers}"/>
				<MenuItem Header="Show _End of Line" IsCheckable="True" IsChecked="{Binding ElementName=textEditor,Path=Options.ShowEndOfLine}"/>
			</MenuItem>
		</Menu>
		<ToolBar DockPanel.Dock="Top">
			<CheckBox IsChecked="{Binding ElementName=textEditor,Path=ShowLineNumbers}">
				<TextBlock Width="16" TextAlignment="Center">#</TextBlock>
			</CheckBox>
			<CheckBox IsChecked="{Binding ElementName=textEditor,Path=Options.ShowEndOfLine}">
				<TextBlock Width="16" TextAlignment="Center">¶</TextBlock>
			</CheckBox>
			<ComboBox Name="highlightingComboBox"
				SelectedItem="{Binding SyntaxHighlighting, ElementName=textEditor}"
				ItemsSource="{Binding Source={x:Static avalonEdit:HighlightingManager.Instance}, Path=HighlightingDefinitions}"
				SelectionChanged="HighlightingComboBox_SelectionChanged"/>
		</ToolBar>
		<Grid>
			<Grid.ColumnDefinitions>
				<ColumnDefinition Width="1*"/>
				<ColumnDefinition Width="100"/>
			</Grid.ColumnDefinitions>
			<avalonEdit:TextEditor
				Name="textEditor"
				FontFamily="Consolas"
				FontSize="10pt"
				SyntaxHighlighting="C#"
			>Welcome to AvalonEdit!
			</avalonEdit:TextEditor>
			<GridSplitter Grid.Column="1" Width="4" HorizontalAlignment="Left"/>
			<DockPanel Grid.Column="1" Margin="4 0 0 0">
				<ComboBox Name="propertyGridComboBox" DockPanel.Dock="Top"
				          SelectedIndex="0" SelectionChanged="propertyGridComboBoxSelectionChanged">
					<ComboBoxItem>TextEditor</ComboBoxItem>
					<ComboBoxItem>TextArea</ComboBoxItem>
					<ComboBoxItem>Options</ComboBoxItem>
				</ComboBox>
				<WindowsFormsHost DockPanel.Dock="Right" Name="propertyGridHost">
					<forms:PropertyGrid x:Name="propertyGrid"/>
				</WindowsFormsHost>
			</DockPanel>
		</Grid>
	</DockPanel>
</Window>