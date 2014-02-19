// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CSharpBinding.FormattingStrategy;

namespace CSharpBinding.OptionPanels
{
	/// <summary>
	/// Marker interface for group or option container.
	/// It doesn't need to have any members.
	/// </summary>
	internal interface IFormattingItemContainer
	{
	}
	
	/// <summary>
	/// Represents a container item for other container items in formatting editor list
	/// </summary>
	[ContentProperty("Children")]
	internal class FormattingGroupContainer : DependencyObject, IFormattingItemContainer
	{
		readonly ObservableCollection<IFormattingItemContainer> children = new ObservableCollection<IFormattingItemContainer>();
		
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(FormattingGroupContainer),
			                            new FrameworkPropertyMetadata());
		
		public string Text {
			get { return (string) GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		
		public ObservableCollection<IFormattingItemContainer> Children
		{
			get {
				return children;
			}
		}
	}
	
	/// <summary>
	/// Represents a container for formatting options.
	/// </summary>
	[ContentProperty("Children")]
	internal class FormattingOptionContainer : DependencyObject, IFormattingItemContainer
	{
		readonly ObservableCollection<FormattingOption> children = new ObservableCollection<FormattingOption>(); 
		
		public ObservableCollection<FormattingOption> Children
		{
			get {
				return children;
			}
		}
	}
	
	/// <summary>
	/// Represents a single formatting option in formatting editor.
	/// </summary>
	internal class FormattingOption : DependencyObject
	{
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(FormattingOption),
			                            new FrameworkPropertyMetadata());
		public string Text {
			get { return (string) GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		
		public string Option
		{
			get;
			set;
		}
	}
	
	/// <summary>
	/// Interaction logic for CSharpFormattingEditor.xaml
	/// </summary>
	public partial class CSharpFormattingEditor : UserControl
	{
		public CSharpFormattingEditor()
		{
			InitializeComponent();
			
			// rootEntries object is only the root container, its children should be shown directly
			var rootEntries = this.Resources["rootEntries"] as FormattingGroupContainer;
			if (rootEntries != null) {
				this.DataContext = rootEntries.Children;
			}
		}
		
		public static readonly DependencyProperty OptionsContainerProperty =
			DependencyProperty.Register("OptionsContainer", typeof(CSharpFormattingOptionsContainer), typeof(CSharpFormattingEditor),
			                            new FrameworkPropertyMetadata(OnOptionsContainerPropertyChanged));
		
		public CSharpFormattingOptionsContainer OptionsContainer {
			get { return (CSharpFormattingOptionsContainer)GetValue(OptionsContainerProperty); }
			set { SetValue(OptionsContainerProperty, value); }
		}
		
		static void OnOptionsContainerPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
//			var editor = o as CSharpFormattingEditor;
//			if (editor != null) {
//				editor.BuildOptionItems();
//				editor.DataContext = editor.rootEntries;
//			}
		}

		/*void BuildOptionItems()
		{
			rootEntries.Clear();
			rootEntries.AddRange(
				new IFormattingItemContainer[]
				{
					new FormattingGroupContainer { Text = "Indentation", Children = new [] { new FormattingOptionContainer {
								Children = new [] {
									new FormattingOption(OptionsContainer) { Option = "IndentNamespaceBody", Text = "Indent namespace body" },
									new FormattingOption(OptionsContainer) { Option = "IndentClassBody", Text = "Indent class body" },
									new FormattingOption(OptionsContainer) { Option = "IndentInterfaceBody", Text = "Indent interface body" },
									new FormattingOption(OptionsContainer) { Option = "IndentStructBody", Text = "Indent struct body" },
									new FormattingOption(OptionsContainer) { Option = "IndentEnumBody", Text = "Indent enum body" },
									new FormattingOption(OptionsContainer) { Option = "IndentMethodBody", Text = "Indent method body" },
									new FormattingOption(OptionsContainer) { Option = "IndentPropertyBody", Text = "Indent property body" },
									new FormattingOption(OptionsContainer) { Option = "IndentEventBody", Text = "Indent event body" },
									new FormattingOption(OptionsContainer) { Option = "IndentBlocks", Text = "Indent blocks" },
									new FormattingOption(OptionsContainer) { Option = "IndentSwitchBody", Text = "Indent switch body" },
									new FormattingOption(OptionsContainer) { Option = "IndentCaseBody", Text = "Indent case body" },
									new FormattingOption(OptionsContainer) { Option = "IndentBreakStatements", Text = "Indent break statements" },
									new FormattingOption(OptionsContainer) { Option = "AlignEmbeddedUsingStatements", Text = "Align embedded using statements" },
									new FormattingOption(OptionsContainer) { Option = "AlignEmbeddedIfStatements", Text = "Align embedded if statements" },
									new FormattingOption(OptionsContainer) { Option = "AlignElseInIfStatements", Text = "Align else in if statements" },
									new FormattingOption(OptionsContainer) { Option = "AutoPropertyFormatting", Text = "Auto property formatting" },
									new FormattingOption(OptionsContainer) { Option = "SimplePropertyFormatting", Text = "Simple property formatting" },
									new FormattingOption(OptionsContainer) { Option = "EmptyLineFormatting", Text = "Empty line formatting" },
									new FormattingOption(OptionsContainer) { Option = "IndentPreprocessorDirectives", Text = "Indent preprocessor directives" },
									new FormattingOption(OptionsContainer) { Option = "AlignToMemberReferenceDot", Text = "Align to member reference dot" },
								}
							}
						}
					},
					new FormattingGroupContainer { Text = "Braces", Children = new [] { new FormattingOptionContainer {
								Children = new [] {
									new FormattingOption(OptionsContainer) { Option = "NamespaceBraceStyle", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "ClassBraceStyle", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "InterfaceBraceStyle", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "StructBraceStyle", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "StructBraceStyle", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "EnumBraceStyle", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "MethodBraceStyle", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "AnonymousMethodBraceStyle", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "ConstructorBraceStyle", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" },
								}
							}
						}
					},
					new FormattingGroupContainer { Text = "New lines", Children = new [] { new FormattingOptionContainer {
								Children = new [] {
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" }
								}
							}
						}
					},
					new FormattingGroupContainer { Text = "Spaces",
						Children = new [] {
							new FormattingGroupContainer { Text = "Methods", Children = new [] { new FormattingOptionContainer {
										Children = new [] {
											new FormattingOption(OptionsContainer) { Option = "", Text = "-" }
										}
									}
								}
							},
							new FormattingGroupContainer { Text = "Method calls", Children = new [] { new FormattingOptionContainer {
										Children = new [] {
											new FormattingOption(OptionsContainer) { Option = "", Text = "-" }
										}
									}
								}
							},
							new FormattingGroupContainer { Text = "Fields", Children = new [] { new FormattingOptionContainer {
										Children = new [] {
											new FormattingOption(OptionsContainer) { Option = "", Text = "-" }
										}
									}
								}
							},
							new FormattingGroupContainer { Text = "Local variables", Children = new [] { new FormattingOptionContainer {
										Children = new [] {
											new FormattingOption(OptionsContainer) { Option = "", Text = "-" }
										}
									}
								}
							},
							new FormattingGroupContainer { Text = "Constructors", Children = new [] { new FormattingOptionContainer {
										Children = new [] {
											new FormattingOption(OptionsContainer) { Option = "", Text = "-" }
										}
									}
								}
							},
							new FormattingGroupContainer { Text = "Indexers", Children = new [] { new FormattingOptionContainer {
										Children = new [] {
											new FormattingOption(OptionsContainer) { Option = "", Text = "-" }
										}
									}
								}
							},
							new FormattingGroupContainer { Text = "Delegates", Children = new [] { new FormattingOptionContainer {
										Children = new [] {
											new FormattingOption(OptionsContainer) { Option = "", Text = "-" }
										}
									}
								}
							},
							new FormattingGroupContainer { Text = "Statements", Children = new [] { new FormattingOptionContainer {
										Children = new [] {
											new FormattingOption(OptionsContainer) { Option = "", Text = "-" }
										}
									}
								}
							},
							new FormattingGroupContainer { Text = "Operators", Children = new [] { new FormattingOptionContainer {
										Children = new [] {
											new FormattingOption(OptionsContainer) { Option = "", Text = "-" }
										}
									}
								}
							},
							new FormattingGroupContainer { Text = "Brackets", Children = new [] { new FormattingOptionContainer {
										Children = new [] {
											new FormattingOption(OptionsContainer) { Option = "", Text = "-" }
										}
									}
								}
							}
						}
					},
					new FormattingGroupContainer { Text = "Blank lines", Children = new [] { new FormattingOptionContainer {
								Children = new [] {
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" }
								}
							}
						}
					},
					new FormattingGroupContainer { Text = "Keep formatting", Children = new [] { new FormattingOptionContainer {
								Children = new [] {
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" }
								}
							}
						}
					},
					new FormattingGroupContainer { Text = "Wrapping", Children = new [] { new FormattingOptionContainer {
								Children = new [] {
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" }
								}
							}
						}
					},
					new FormattingGroupContainer { Text = "Using declarations", Children = new [] { new FormattingOptionContainer {
								Children = new [] {
									new FormattingOption(OptionsContainer) { Option = "", Text = "-" }
								}
							}
						}
					}
				}
			);
		}*/
	}
}