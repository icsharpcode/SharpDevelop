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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml;

using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.Services;
using ICSharpCode.WpfDesign.Designer.Xaml;
using NUnit.Framework;

namespace ICSharpCode.WpfDesign.Tests.Designer
{
	[TestFixture]
	public class ModelTests : ModelTestHelper
	{
		[Test]
		public void SetButtonWidth()
		{
			DesignItem button = CreateCanvasContext("<Button/>");
			button.Properties["Width"].SetValue(100.0);
			AssertCanvasDesignerOutput(@"<Button Width=""100"" />", button.Context);
			AssertLog("");
		}
		
		[Test]
		public void SetButtonWidthElementSyntax()
		{
			DesignItem button = CreateCanvasContext("<Button><Button.Width>50</Button.Width></Button>");
			button.Properties["Width"].SetValue(100.0);
			AssertCanvasDesignerOutput("<Button Width=\"100\" />", button.Context);
			AssertLog("");
		}
		
		[Test]
		public void UndoRedoTest()
		{
			DesignItem button = CreateCanvasContext("<Button/>");
			UndoService s = button.Context.Services.GetService<UndoService>();
			Assert.IsFalse(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			button.Properties["Width"].SetValue(100.0);
			Assert.IsTrue(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			AssertCanvasDesignerOutput(@"<Button Width=""100"" />", button.Context);
			s.Undo();
			Assert.IsFalse(s.CanUndo);
			Assert.IsTrue(s.CanRedo);
			AssertCanvasDesignerOutput(@"<Button />", button.Context);
			s.Redo();
			Assert.IsTrue(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			AssertCanvasDesignerOutput(@"<Button Width=""100"" />", button.Context);
			AssertLog("");
		}
		
		[Test]
		public void ButtonClickEventHandler()
		{
			DesignItem button = CreateCanvasContext("<Button Click='OnClick'/>");
			Assert.AreEqual("OnClick", button.Properties["Click"].ValueOnInstance);
			
			button.Properties["Click"].Reset();
			button.Properties["KeyDown"].SetValue("ButtonKeyDown");
			
			AssertCanvasDesignerOutput(@"<Button KeyDown=""ButtonKeyDown"" />", button.Context);
			AssertLog("");
		}
		
		[Test]
		public void ButtonClickEventHandlerUndoRedo()
		{
			DesignItem button = CreateCanvasContext("<Button/>");
			UndoService s = button.Context.Services.GetService<UndoService>();
			Assert.IsFalse(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			button.Properties["Click"].SetValue("OnClick");
			Assert.IsTrue(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			AssertCanvasDesignerOutput(@"<Button Click=""OnClick"" />", button.Context);
			
			button.Properties["Click"].SetValue("OnClick2");
			Assert.IsTrue(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			AssertCanvasDesignerOutput(@"<Button Click=""OnClick2"" />", button.Context);
			
			s.Undo();
			Assert.IsTrue(s.CanUndo);
			Assert.IsTrue(s.CanRedo);
			AssertCanvasDesignerOutput(@"<Button Click=""OnClick"" />", button.Context);
			
			s.Undo();
			Assert.IsFalse(s.CanUndo);
			Assert.IsTrue(s.CanRedo);
			AssertCanvasDesignerOutput(@"<Button />", button.Context);
			
			s.Redo();
			Assert.IsTrue(s.CanUndo);
			Assert.IsTrue(s.CanRedo);
			AssertCanvasDesignerOutput(@"<Button Click=""OnClick"" />", button.Context);
			AssertLog("");
		}
		
		[Test]
		public void UndoRedoChangeGroupTest()
		{
			DesignItem button = CreateCanvasContext("<Button/>");
			UndoService s = button.Context.Services.GetService<UndoService>();
			Assert.IsFalse(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			using (ChangeGroup g = button.OpenGroup("Resize")) {
				button.Properties["Width"].SetValue(100.0);
				button.Properties["Height"].SetValue(200.0);
				g.Commit();
			}
			Assert.IsTrue(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			AssertCanvasDesignerOutput(@"<Button Width=""100"" Height=""200"" />", button.Context);
			s.Undo();
			Assert.IsFalse(s.CanUndo);
			Assert.IsTrue(s.CanRedo);
			AssertCanvasDesignerOutput(@"<Button />", button.Context);
			s.Redo();
			Assert.IsTrue(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			AssertCanvasDesignerOutput(@"<Button Width=""100"" Height=""200"" />", button.Context);
			AssertLog("");
		}
		
		[Test]
		public void UndoRedoImplicitList()
		{
			UndoRedoListInternal(false);
		}
		
		[Test]
		public void UndoRedoExplicitList()
		{
			UndoRedoListInternal(true);
		}

		void UndoRedoListInternal(bool useExplicitList)
		{
			const string originalXaml = "<Button />";
			DesignItem button = CreateCanvasContext(originalXaml);
			UndoService s = button.Context.Services.GetService<UndoService>();
			IComponentService component = button.Context.Services.Component;
			string expectedXamlWithList;
			DesignItemProperty otherListProp;
			
			Assert.IsFalse(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			
			using (ChangeGroup g = button.OpenGroup("UndoRedoListInternal test")) {
				DesignItem containerItem = component.RegisterComponentForDesigner(new ICSharpCode.WpfDesign.Tests.XamlDom.ExampleClassContainer());
				
				otherListProp = containerItem.Properties["OtherList"];
				
				if(useExplicitList) {
					otherListProp.SetValue(new ICSharpCode.WpfDesign.Tests.XamlDom.ExampleClassList());
					
					expectedXamlWithList = @"<Button>
  <Button.Tag>
    <Controls0:ExampleClassContainer>
      <Controls0:ExampleClassContainer.OtherList>
        <Controls0:ExampleClassList>
          <Controls0:ExampleClass StringProp=""String value"" />
        </Controls0:ExampleClassList>
      </Controls0:ExampleClassContainer.OtherList>
    </Controls0:ExampleClassContainer>
  </Button.Tag>
</Button>";
				} else {
					expectedXamlWithList = @"<Button>
  <Button.Tag>
    <Controls0:ExampleClassContainer>
      <Controls0:ExampleClassContainer.OtherList>
        <Controls0:ExampleClass StringProp=""String value"" />
      </Controls0:ExampleClassContainer.OtherList>
    </Controls0:ExampleClassContainer>
  </Button.Tag>
</Button>";
				}
				
				DesignItem exampleClassItem = component.RegisterComponentForDesigner(new ICSharpCode.WpfDesign.Tests.XamlDom.ExampleClass());
				exampleClassItem.Properties["StringProp"].SetValue("String value");
				otherListProp.CollectionElements.Add(exampleClassItem);
				
				button.Properties["Tag"].SetValue(containerItem);
				g.Commit();
			}
			
			Assert.IsTrue(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			AssertCanvasDesignerOutput(expectedXamlWithList, button.Context, "xmlns:Controls0=\"" + ICSharpCode.WpfDesign.Tests.XamlDom.XamlTypeFinderTests.XamlDomTestsNamespace + "\"");
			
			otherListProp = button.Properties["Tag"].Value.Properties["OtherList"];
			Assert.IsTrue(((ICSharpCode.WpfDesign.Tests.XamlDom.ExampleClassList)otherListProp.ValueOnInstance).Count == otherListProp.CollectionElements.Count);
			
			s.Undo();
			Assert.IsFalse(s.CanUndo);
			Assert.IsTrue(s.CanRedo);
			AssertCanvasDesignerOutput(originalXaml, button.Context, "xmlns:Controls0=\"" + ICSharpCode.WpfDesign.Tests.XamlDom.XamlTypeFinderTests.XamlDomTestsNamespace + "\"");
			
			s.Redo();
			Assert.IsTrue(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			AssertCanvasDesignerOutput(expectedXamlWithList, button.Context, "xmlns:Controls0=\"" + ICSharpCode.WpfDesign.Tests.XamlDom.XamlTypeFinderTests.XamlDomTestsNamespace + "\"");
			
			otherListProp = button.Properties["Tag"].Value.Properties["OtherList"];
			Assert.IsTrue(((ICSharpCode.WpfDesign.Tests.XamlDom.ExampleClassList)otherListProp.ValueOnInstance).Count == otherListProp.CollectionElements.Count);
			
			AssertLog("");
		}
		
		[Test]
		public void UndoRedoImplicitDictionary()
		{
			UndoRedoDictionaryInternal(false);
		}
		
		[Test]
		public void UndoRedoExplicitDictionary()
		{
			UndoRedoDictionaryInternal(true);
		}

		void UndoRedoDictionaryInternal(bool useExplicitDictionary)
		{
			const string originalXaml = "<Button />";
			DesignItem button = CreateCanvasContext(originalXaml);
			UndoService s = button.Context.Services.GetService<UndoService>();
			IComponentService component = button.Context.Services.Component;
			string expectedXamlWithDictionary;
			DesignItemProperty dictionaryProp;
			
			Assert.IsFalse(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			
			using (ChangeGroup g = button.OpenGroup("UndoRedoDictionaryInternal test")) {
				DesignItem containerItem = component.RegisterComponentForDesigner(new ICSharpCode.WpfDesign.Tests.XamlDom.ExampleClassContainer());
				
				dictionaryProp = containerItem.Properties["Dictionary"];
				
				if(useExplicitDictionary) {
					dictionaryProp.SetValue(new ICSharpCode.WpfDesign.Tests.XamlDom.ExampleClassDictionary());
					
					expectedXamlWithDictionary = @"<Button>
  <Button.Tag>
    <Controls0:ExampleClassContainer>
      <Controls0:ExampleClassContainer.Dictionary>
        <Controls0:ExampleClassDictionary>
          <Controls0:ExampleClass x:Key=""testKey"" StringProp=""String value"" />
        </Controls0:ExampleClassDictionary>
      </Controls0:ExampleClassContainer.Dictionary>
    </Controls0:ExampleClassContainer>
  </Button.Tag>
</Button>";
				} else {
					expectedXamlWithDictionary = @"<Button>
  <Button.Tag>
    <Controls0:ExampleClassContainer>
      <Controls0:ExampleClassContainer.Dictionary>
        <Controls0:ExampleClass x:Key=""testKey"" StringProp=""String value"" />
      </Controls0:ExampleClassContainer.Dictionary>
    </Controls0:ExampleClassContainer>
  </Button.Tag>
</Button>";
				}
				
				DesignItem exampleClassItem = component.RegisterComponentForDesigner(new ICSharpCode.WpfDesign.Tests.XamlDom.ExampleClass());
				exampleClassItem.Key = "testKey";
				exampleClassItem.Properties["StringProp"].SetValue("String value");
				dictionaryProp.CollectionElements.Add(exampleClassItem);
				
				button.Properties["Tag"].SetValue(containerItem);
				g.Commit();
			}
			
			Assert.IsTrue(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			AssertCanvasDesignerOutput(expectedXamlWithDictionary, button.Context, "xmlns:Controls0=\"" + ICSharpCode.WpfDesign.Tests.XamlDom.XamlTypeFinderTests.XamlDomTestsNamespace + "\"");
			
			dictionaryProp = button.Properties["Tag"].Value.Properties["Dictionary"];
			Assert.IsTrue(((ICSharpCode.WpfDesign.Tests.XamlDom.ExampleClassDictionary)dictionaryProp.ValueOnInstance).Count == dictionaryProp.CollectionElements.Count);
			
			s.Undo();
			Assert.IsFalse(s.CanUndo);
			Assert.IsTrue(s.CanRedo);
			AssertCanvasDesignerOutput(originalXaml, button.Context, "xmlns:Controls0=\"" + ICSharpCode.WpfDesign.Tests.XamlDom.XamlTypeFinderTests.XamlDomTestsNamespace + "\"");
			
			s.Redo();
			Assert.IsTrue(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			AssertCanvasDesignerOutput(expectedXamlWithDictionary, button.Context, "xmlns:Controls0=\"" + ICSharpCode.WpfDesign.Tests.XamlDom.XamlTypeFinderTests.XamlDomTestsNamespace + "\"");
			
			dictionaryProp = button.Properties["Tag"].Value.Properties["Dictionary"];
			Assert.IsTrue(((ICSharpCode.WpfDesign.Tests.XamlDom.ExampleClassDictionary)dictionaryProp.ValueOnInstance).Count == dictionaryProp.CollectionElements.Count);
			
			AssertLog("");
		}
		
		[Test]
		public void UndoRedoInputBindings()
		{
			const string originalXaml =  "<TextBlock Text=\"My text\" />";
			
			DesignItem textBlock = CreateCanvasContext(originalXaml);
			UndoService s = textBlock.Context.Services.GetService<UndoService>();
			IComponentService component = textBlock.Context.Services.Component;
			
			Assert.IsFalse(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			
			DesignItemProperty inputbinding = textBlock.Properties["InputBindings"];
			Assert.IsTrue(inputbinding.IsCollection);
			
			const string expectedXaml = @"<TextBlock Text=""My text"">
  <TextBlock.InputBindings>
    <MouseBinding Gesture=""LeftDoubleClick"" Command=""ApplicationCommands.New"" />
  </TextBlock.InputBindings>
</TextBlock>";
			
			using (ChangeGroup changeGroup = textBlock.Context.OpenGroup("", new[] { textBlock }))
			{
				DesignItem di = component.RegisterComponentForDesigner(new System.Windows.Input.MouseBinding());
				di.Properties["Gesture"].SetValue(System.Windows.Input.MouseAction.LeftDoubleClick);
				di.Properties["Command"].SetValue("ApplicationCommands.New");

				inputbinding.CollectionElements.Add(di);

				changeGroup.Commit();
			}
			
			Assert.IsTrue(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			AssertCanvasDesignerOutput(expectedXaml, textBlock.Context);
			
			inputbinding = textBlock.Properties["InputBindings"];
			Assert.IsTrue(((System.Windows.Input.InputBindingCollection)inputbinding.ValueOnInstance).Count == inputbinding.CollectionElements.Count);
			
			s.Undo();
			Assert.IsFalse(s.CanUndo);
			Assert.IsTrue(s.CanRedo);
			AssertCanvasDesignerOutput(originalXaml, textBlock.Context);
			
			s.Redo();
			Assert.IsTrue(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			AssertCanvasDesignerOutput(expectedXaml, textBlock.Context);
			
			Assert.IsTrue(((System.Windows.Input.InputBindingCollection)inputbinding.ValueOnInstance).Count == inputbinding.CollectionElements.Count);
			
			AssertLog("");
		}
		
		void UndoRedoInputBindingsRemoveClearResetInternal(bool remove, bool clear, bool reset)
		{
			const string originalXaml =  "<TextBlock Text=\"My text\" />";
			
			DesignItem textBlock = CreateCanvasContext(originalXaml);
			UndoService s = textBlock.Context.Services.GetService<UndoService>();
			IComponentService component = textBlock.Context.Services.Component;
			
			Assert.IsFalse(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			
			DesignItemProperty inputbinding = textBlock.Properties["InputBindings"];
			Assert.IsTrue(inputbinding.IsCollection);
			
			const string expectedXaml = @"<TextBlock Text=""My text"" Cursor=""Hand"">
  <TextBlock.InputBindings>
    <MouseBinding Gesture=""LeftDoubleClick"" Command=""ApplicationCommands.New"" />
  </TextBlock.InputBindings>
</TextBlock>";
			
			using (ChangeGroup changeGroup = textBlock.Context.OpenGroup("", new[] { textBlock }))
			{
				DesignItem di = component.RegisterComponentForDesigner(new System.Windows.Input.MouseBinding());
				di.Properties["Gesture"].SetValue(System.Windows.Input.MouseAction.LeftDoubleClick);
				di.Properties["Command"].SetValue("ApplicationCommands.New");

				inputbinding.CollectionElements.Add(di);
				
				textBlock.Properties["Cursor"].SetValue(System.Windows.Input.Cursors.Hand);

				changeGroup.Commit();
			}
			
			Assert.IsTrue(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			AssertCanvasDesignerOutput(expectedXaml, textBlock.Context);
			
			using (ChangeGroup changeGroup = textBlock.Context.OpenGroup("", new[] { textBlock }))
			{
				DesignItem di = inputbinding.CollectionElements.First();
				
				// Remove, Clear, Reset combination caused exception at first Undo after this group commit before the issue was fixed
				if (remove)
					inputbinding.CollectionElements.Remove(di);
				if (clear)
					inputbinding.CollectionElements.Clear();
				if (reset)
					inputbinding.Reset();
				
				di = component.RegisterComponentForDesigner(new System.Windows.Input.MouseBinding());
				di.Properties["Gesture"].SetValue(System.Windows.Input.MouseAction.LeftDoubleClick);
				di.Properties["Command"].SetValue("ApplicationCommands.New");

				inputbinding.CollectionElements.Add(di);
				
				textBlock.Properties["Cursor"].SetValue(System.Windows.Input.Cursors.Hand);

				changeGroup.Commit(); 
			}
			
			s.Undo();
			s.Undo();
			Assert.IsFalse(s.CanUndo);
			Assert.IsTrue(s.CanRedo);
			AssertCanvasDesignerOutput(originalXaml, textBlock.Context);
			
			s.Redo();
			s.Redo();
			Assert.IsTrue(s.CanUndo);
			Assert.IsFalse(s.CanRedo);
			AssertCanvasDesignerOutput(expectedXaml, textBlock.Context);
			
			Assert.IsTrue(((System.Windows.Input.InputBindingCollection)inputbinding.ValueOnInstance).Count == inputbinding.CollectionElements.Count);
			
			AssertLog("");
		}
		
		[Test]
		public void UndoRedoInputBindingsRemoveClearReset()
		{
			UndoRedoInputBindingsRemoveClearResetInternal(true, true, true);
		}
		
		[Test]
		public void UndoRedoInputBindingsRemove()
		{
			UndoRedoInputBindingsRemoveClearResetInternal(true, false, false);
		}
		
		[Test]
		public void UndoRedoInputBindingsClear()
		{
			UndoRedoInputBindingsRemoveClearResetInternal(false, true, false);
		}
		
		[Test]
		public void UndoRedoInputBindingsReset()
		{
			UndoRedoInputBindingsRemoveClearResetInternal(false, false, true);
		}
		
		[Test]
		public void UndoRedoInputBindingsRemoveClear()
		{
			UndoRedoInputBindingsRemoveClearResetInternal(true, true, false);
		}
		
		[Test]
		public void UndoRedoInputBindingsRemoveReset()
		{
			UndoRedoInputBindingsRemoveClearResetInternal(true, false, true);
		}
		
		[Test]
		public void UndoRedoInputBindingsClearReset()
		{
			UndoRedoInputBindingsRemoveClearResetInternal(false, true, true);
		}
		
		[Test]
		public void AddTextBoxToCanvas()
		{
			DesignItem button = CreateCanvasContext("<Button/>");
			DesignItem canvas = button.Parent;
			DesignItem textBox = canvas.Services.Component.RegisterComponentForDesigner(new TextBox());
			canvas.Properties["Children"].CollectionElements.Add(textBox);
			AssertCanvasDesignerOutput("<Button />\n<TextBox />", button.Context);
			AssertLog("");
		}
		
		[Test]
		public void AddTextBoxToCanvasExplicitChildrenCollection()
		{
			DesignItem button = CreateCanvasContext("<Canvas.Children><Button/></Canvas.Children>");
			DesignItem canvas = button.Parent;
			DesignItem textBox = canvas.Services.Component.RegisterComponentForDesigner(new TextBox());
			canvas.Properties["Children"].CollectionElements.Add(textBox);
			AssertCanvasDesignerOutput("<Canvas.Children>\n  <Button />\n  <TextBox />\n</Canvas.Children>", button.Context);
			AssertLog("");
		}
		
		[Test]
		public void AddTextBoxToCanvasEmptyImplicitPanelChildrenCollection()
		{
			DesignItem canvas = CreateCanvasContext("<Canvas></Canvas>");
			DesignItem textBox = canvas.Services.Component.RegisterComponentForDesigner(new TextBox());
			canvas.Properties["Children"].CollectionElements.Add(textBox);
			AssertCanvasDesignerOutput("<Canvas>\n" +
			                           "  <TextBox />\n" +
			                           "</Canvas>", canvas.Context);
			AssertLog("");
		}
		
		[Test]
		public void AddTextBoxToCanvasEmptyImplicitPanelChildrenCollectionEmptyTag()
		{
			DesignItem canvas = CreateCanvasContext("<Canvas/>");
			DesignItem textBox = canvas.Services.Component.RegisterComponentForDesigner(new TextBox());
			canvas.Properties["Children"].CollectionElements.Add(textBox);
			AssertCanvasDesignerOutput("<Canvas>\n" +
			                           "  <TextBox />\n" +
			                           "</Canvas>", canvas.Context);
			AssertLog("");
		}
		
		[Test]
		public void AddTextBoxToCanvasEmptyExplicitPanelChildrenCollection()
		{
			DesignItem canvas = CreateCanvasContext("<Canvas><Panel.Children></Panel.Children></Canvas>");
			DesignItem textBox = canvas.Services.Component.RegisterComponentForDesigner(new TextBox());
			canvas.Properties["Children"].CollectionElements.Add(textBox);
			AssertCanvasDesignerOutput("<Canvas>\n" +
			                           "  <Panel.Children>\n" +
			                           "    <TextBox />\n" +
			                           "  </Panel.Children>\n" +
			                           "</Canvas>", canvas.Context);
			AssertLog("");
		}
		
		[Test]
		public void AddTextBoxToCanvasEmptyExplicitCanvasChildrenCollection()
		{
			DesignItem canvas = CreateCanvasContext("<Canvas><Canvas.Children></Canvas.Children></Canvas>");
			DesignItem textBox = canvas.Services.Component.RegisterComponentForDesigner(new TextBox());
			canvas.Properties["Children"].CollectionElements.Add(textBox);
			AssertCanvasDesignerOutput("<Canvas>\n" +
			                           "  <Canvas.Children>\n" +
			                           "    <TextBox />\n" +
			                           "  </Canvas.Children>\n" +
			                           "</Canvas>", canvas.Context);
			AssertLog("");
		}
		
		[Test]
		public void InsertTextBoxInCanvas()
		{
			DesignItem button = CreateCanvasContext("<Canvas.Width>50</Canvas.Width><Button/><Canvas.Height>60</Canvas.Height>");
			DesignItem canvas = button.Parent;
			DesignItem textBox = canvas.Services.Component.RegisterComponentForDesigner(new TextBox());
			canvas.Properties["Children"].CollectionElements.Add(textBox);
			
			DesignItem checkBox = canvas.Services.Component.RegisterComponentForDesigner(new CheckBox());
			canvas.Properties["Children"].CollectionElements.Insert(0, checkBox);
			
			AssertCanvasDesignerOutput("<Canvas.Width>50</Canvas.Width>\n" +
			                           "<CheckBox />\n" +
			                           "<Button />\n" +
			                           "<TextBox />\n" +
			                           "<Canvas.Height>60</Canvas.Height>", button.Context);
			AssertLog("");
		}
		
		[Test]
		public void ClearImplicitChildCollection()
		{
			DesignItem canvas = CreateCanvasContext("<Canvas><Button/><TextBox/></Canvas>");
			DesignItem textBox = canvas.Services.Component.RegisterComponentForDesigner(new TextBox());
			canvas.Properties["Children"].CollectionElements.Clear();
			AssertCanvasDesignerOutput("<Canvas>\n" +
			                           "</Canvas>", canvas.Context);
			AssertLog("");
		}
		
		[Test]
		public void ClearExplicitList()
		{
			DesignItem button = CreateCanvasContext("<Button/>");
			
			button.Properties["Tag"].SetValue(new ICSharpCode.WpfDesign.Tests.XamlDom.ExampleClassContainer());
			var containerItem = button.Properties["Tag"].Value;
			var otherListProp = containerItem.Properties["OtherList"];
			otherListProp.SetValue(new ICSharpCode.WpfDesign.Tests.XamlDom.ExampleClassList());
			
			DesignItem exampleClassItem = button.Context.Services.Component.RegisterComponentForDesigner(new ICSharpCode.WpfDesign.Tests.XamlDom.ExampleClass());
			exampleClassItem.Properties["StringProp"].SetValue("String value");
			otherListProp.CollectionElements.Add(exampleClassItem);
			
			var listInstance = (ICSharpCode.WpfDesign.Tests.XamlDom.ExampleClassList)otherListProp.ValueOnInstance;
			Assert.AreEqual(1, listInstance.Count);
			Assert.AreEqual(1, otherListProp.CollectionElements.Count);
			
			button.Properties["Tag"].Value.Properties["OtherList"].CollectionElements.Clear();
			
			Assert.AreEqual(0, listInstance.Count);
			Assert.AreEqual(0, otherListProp.CollectionElements.Count);

			AssertCanvasDesignerOutput(@"<Button>
  <Button.Tag>
    <Controls0:ExampleClassContainer>
      <Controls0:ExampleClassContainer.OtherList>
        <Controls0:ExampleClassList>
        </Controls0:ExampleClassList>
      </Controls0:ExampleClassContainer.OtherList>
    </Controls0:ExampleClassContainer>
  </Button.Tag>
</Button>", button.Context, "xmlns:Controls0=\"" + ICSharpCode.WpfDesign.Tests.XamlDom.XamlTypeFinderTests.XamlDomTestsNamespace + "\"");
			AssertLog("");
		}
		
		[Test]
		public void ClearExplicitDictionary()
		{
			DesignItem button = CreateCanvasContext("<Button/>");
			
			button.Properties["Tag"].SetValue(new ICSharpCode.WpfDesign.Tests.XamlDom.ExampleClassContainer());
			var containerItem = button.Properties["Tag"].Value;
			var dictionaryProp = containerItem.Properties["Dictionary"];
			dictionaryProp.SetValue(new ICSharpCode.WpfDesign.Tests.XamlDom.ExampleClassDictionary());
			
			DesignItem exampleClassItem = button.Context.Services.Component.RegisterComponentForDesigner(new ICSharpCode.WpfDesign.Tests.XamlDom.ExampleClass());
			exampleClassItem.Key = "testKey";
			exampleClassItem.Properties["StringProp"].SetValue("String value");
			dictionaryProp.CollectionElements.Add(exampleClassItem);
			
			var dictionaryInstance = (ICSharpCode.WpfDesign.Tests.XamlDom.ExampleClassDictionary)dictionaryProp.ValueOnInstance;
			Assert.AreEqual(1, dictionaryInstance.Count);
			Assert.AreEqual(1, dictionaryProp.CollectionElements.Count);
			
			button.Properties["Tag"].Value.Properties["Dictionary"].CollectionElements.Clear();
			
			Assert.AreEqual(0, dictionaryInstance.Count);
			Assert.AreEqual(0, dictionaryProp.CollectionElements.Count);
			
			dictionaryProp.CollectionElements.Add(exampleClassItem);
			
			Assert.AreEqual(1, dictionaryInstance.Count);
			Assert.AreEqual(1, dictionaryProp.CollectionElements.Count);
			
			button.Properties["Tag"].Value.Properties["Dictionary"].CollectionElements.Clear();
			
			Assert.AreEqual(0, dictionaryInstance.Count);
			Assert.AreEqual(0, dictionaryProp.CollectionElements.Count);

			AssertCanvasDesignerOutput(@"<Button>
  <Button.Tag>
    <Controls0:ExampleClassContainer>
      <Controls0:ExampleClassContainer.Dictionary>
        <Controls0:ExampleClassDictionary>
        </Controls0:ExampleClassDictionary>
      </Controls0:ExampleClassContainer.Dictionary>
    </Controls0:ExampleClassContainer>
  </Button.Tag>
</Button>", button.Context, "xmlns:Controls0=\"" + ICSharpCode.WpfDesign.Tests.XamlDom.XamlTypeFinderTests.XamlDomTestsNamespace + "\"");
			AssertLog("");
		}
		
		[Test]
		public void AddMultiBindingToTextBox()
		{
			DesignItem button = CreateCanvasContext("<Button/>");
			DesignItem canvas = button.Parent;
			DesignItem textBox = canvas.Services.Component.RegisterComponentForDesigner(new TextBox());
			canvas.Properties["Children"].CollectionElements.Add(textBox);
			
			textBox.Properties[TextBox.TextProperty].SetValue(new MultiBinding());
			DesignItem multiBindingItem = textBox.Properties[TextBox.TextProperty].Value;
			multiBindingItem.Properties["Converter"].SetValue(new MyMultiConverter());

			DesignItemProperty bindingsProp = multiBindingItem.ContentProperty;
			Assert.IsTrue(bindingsProp.IsCollection);
			Assert.AreEqual(bindingsProp.Name, "Bindings");
			
			DesignItem bindingItem = canvas.Services.Component.RegisterComponentForDesigner(new Binding());
			bindingItem.Properties["Path"].SetValue("SomeProperty");
			bindingsProp.CollectionElements.Add(bindingItem);
			
			string expectedXaml = "<Button />\n" +
			                      "<TextBox>\n" +
			                      "  <MultiBinding>\n" +
			                      "    <MultiBinding.Converter>\n" +
			                      "      <t:MyMultiConverter />\n" +
			                      "    </MultiBinding.Converter>\n" +
			                      "    <Binding Path=\"SomeProperty\" />\n" +
			                      "  </MultiBinding>\n" +
			                      "</TextBox>";
			
			AssertCanvasDesignerOutput(expectedXaml, button.Context);
			AssertLog("");
		}
		
		[Test]
		public void AddSimpleBinding()
		{
			DesignItem button = CreateCanvasContext("<Button/>");
			DesignItem canvas = button.Parent;
			DesignItem textBox = canvas.Services.Component.RegisterComponentForDesigner(new TextBox());
			canvas.Properties["Children"].CollectionElements.Add(textBox);
			
			textBox.Properties[TextBox.TextProperty].SetValue(new Binding());
			textBox.Properties[TextBox.TextProperty].Value.Properties["Path"].SetValue("SomeProperty");
			
			string expectedXaml = "<Button />\n" +
								  "<TextBox Text=\"{Binding SomeProperty}\" />\n";
			
			AssertCanvasDesignerOutput(expectedXaml, button.Context);
			AssertLog("");
		}
		
		[Test]
		public void AddBindingWithStaticResource()
		{
			DesignItem button = CreateCanvasContext("<Button/>");
			DesignItem canvas = button.Parent;
			DesignItem textBox = canvas.Services.Component.RegisterComponentForDesigner(new TextBox());
			canvas.Properties["Children"].CollectionElements.Add(textBox);
			
			DesignItemProperty resProp = canvas.Properties.GetProperty("Resources");
			Assert.IsTrue(resProp.IsCollection);
			DesignItem exampleClassItem = canvas.Services.Component.RegisterComponentForDesigner(new ExampleClass());
			exampleClassItem.Key = "bindingSource";
			resProp.CollectionElements.Add(exampleClassItem);
			
			DesignItem bindingItem = canvas.Services.Component.RegisterComponentForDesigner(new Binding());
			textBox.Properties[TextBox.TextProperty].SetValue(bindingItem);
			bindingItem.Properties["Path"].SetValue("StringProp");
			bindingItem.Properties["Source"].SetValue(new StaticResourceExtension());
			bindingItem.Properties["Source"].Value.Properties["ResourceKey"].SetValue("bindingSource");
			
			string expectedXaml = "<Canvas.Resources>\n" +
								  "  <t:ExampleClass x:Key=\"bindingSource\" />\n" +
								  "</Canvas.Resources>\n" +
								  "<Button />\n" +
								  "<TextBox Text=\"{Binding StringProp, Source={StaticResource bindingSource}}\" />";
			
			AssertCanvasDesignerOutput(expectedXaml, button.Context);
			AssertLog("");
		}
		
		void AddBindingWithStaticResourceWhereResourceOnSameElement(bool setBindingPropertiesAfterSet)
		{
			DesignItem button = CreateCanvasContext("<Button/>");
			DesignItem canvas = button.Parent;
			DesignItem textBox = canvas.Services.Component.RegisterComponentForDesigner(new TextBox());
			canvas.Properties["Children"].CollectionElements.Add(textBox);
			
			DesignItemProperty resProp = button.Properties.GetProperty("Resources");
			Assert.IsTrue(resProp.IsCollection);
			DesignItem dummyItem = canvas.Services.Component.RegisterComponentForDesigner(new ExampleClass());
			dummyItem.Key = "dummy";
			resProp.CollectionElements.Add(dummyItem);
			
			resProp = textBox.Properties.GetProperty("Resources");
			Assert.IsTrue(resProp.IsCollection);
			DesignItem exampleClassItem = canvas.Services.Component.RegisterComponentForDesigner(new ExampleClass());
			exampleClassItem.Key = "bindingSource";
			resProp.CollectionElements.Add(exampleClassItem);
			
			DesignItem bindingItem = canvas.Services.Component.RegisterComponentForDesigner(new Binding());
			if (!setBindingPropertiesAfterSet) {
				bindingItem.Properties["Path"].SetValue("StringProp");
				// Using resource "dummy" before "bindingSource" to enable test where not the first set property will require element-style print of the binding
				bindingItem.Properties["ConverterParameter"].SetValue(new StaticResourceExtension());
				bindingItem.Properties["ConverterParameter"].Value.Properties["ResourceKey"].SetValue("dummy");
				bindingItem.Properties["Source"].SetValue(new StaticResourceExtension());
				bindingItem.Properties["Source"].Value.Properties["ResourceKey"].SetValue("bindingSource");
			}
			textBox.Properties[TextBox.TextProperty].SetValue(bindingItem);
			if (setBindingPropertiesAfterSet) {
				bindingItem.Properties["Path"].SetValue("StringProp");
				// Using resource "dummy" before "bindingSource" to enable test where not the first set property will require element-style print of the binding
				bindingItem.Properties["ConverterParameter"].SetValue(new StaticResourceExtension());
				bindingItem.Properties["ConverterParameter"].Value.Properties["ResourceKey"].SetValue("dummy");
				bindingItem.Properties["Source"].SetValue(new StaticResourceExtension());
				bindingItem.Properties["Source"].Value.Properties["ResourceKey"].SetValue("bindingSource");
			}
			
			var binding = bindingItem.Component as Binding;
			Assert.IsNotNull(binding);
			Assert.IsNotNull(binding.Source);
			Assert.IsNotNull(exampleClassItem.Component);
			Assert.AreSame(exampleClassItem.Component, binding.Source);
			
			const string expectedXaml = "<Button>\n" +
										"  <Button.Resources>\n" +
										"    <t:ExampleClass x:Key=\"dummy\" />\n" +
										"  </Button.Resources>\n" +
										"</Button>\n" +
										"<TextBox>\n" +
										"  <TextBox.Resources>\n" +
										"    <t:ExampleClass x:Key=\"bindingSource\" />\n" +
										"  </TextBox.Resources>\n" +
										"  <Binding Path=\"StringProp\" ConverterParameter=\"{StaticResource dummy}\" Source=\"{StaticResource bindingSource}\" />\n" +
										"</TextBox>";
			
			AssertCanvasDesignerOutput(expectedXaml, button.Context);
			AssertLog("");
		}
		
		[Test]
		public void AddBindingWithStaticResourceWhereResourceOnSameElement()
		{
			AddBindingWithStaticResourceWhereResourceOnSameElement(false);
		}
		
		[Test]
		public void AddBindingWithStaticResourceWhereResourceOnSameElementAlt()
		{
			AddBindingWithStaticResourceWhereResourceOnSameElement(true);
		}
		
		[Test]
		public void AddStaticResourceWhereResourceOnSameElement()
		{
			DesignItem button = CreateCanvasContext("<Button/>");
			DesignItem canvas = button.Parent;
			
			DesignItemProperty resProp = button.Properties.GetProperty("Resources");
			Assert.IsTrue(resProp.IsCollection);
			DesignItem exampleClassItem = canvas.Services.Component.RegisterComponentForDesigner(new ExampleClass());
			exampleClassItem.Key = "res1";
			resProp.CollectionElements.Add(exampleClassItem);
			
			button.Properties["Tag"].SetValue(new StaticResourceExtension());
			button.Properties["Tag"].Value.Properties["ResourceKey"].SetValue("res1");
			
			string expectedXaml = "<Button>\n" +
								  "  <Button.Resources>\n" +
								  "    <t:ExampleClass x:Key=\"res1\" />\n" +
								  "  </Button.Resources>\n" +
								  "  <Button.Tag>\n" +
								  "    <StaticResourceExtension ResourceKey=\"res1\" />\n" +
								  "  </Button.Tag>\n" +
								  "</Button>";
			
			AssertCanvasDesignerOutput(expectedXaml, button.Context);
			AssertLog("");
		}
		
		[Test]
		public void AddMarkupExtensionWithoutWrapperToCollection()
		{
			DesignItem textBox = CreateCanvasContext("<TextBox/>");
			
			DesignItem multiBindingItem = textBox.Context.Services.Component.RegisterComponentForDesigner(new System.Windows.Data.MultiBinding());
			multiBindingItem.Properties["Converter"].SetValue(new ICSharpCode.WpfDesign.Tests.XamlDom.MyMultiConverter());
			DesignItemProperty bindingsProp = multiBindingItem.ContentProperty;
			
			// MyBindingExtension is a markup extension that will not use a wrapper. 
			DesignItem myBindingExtension = textBox.Context.Services.Component.RegisterComponentForDesigner(new ICSharpCode.WpfDesign.Tests.XamlDom.MyBindingExtension());
			
			// Adding it to MultiBinding "Bindings" collection.
			bindingsProp.CollectionElements.Add(myBindingExtension);
			
			textBox.ContentProperty.SetValue(multiBindingItem);
			
			const string expectedXaml = "<TextBox>\n" +
									    "  <MultiBinding>\n" +
									    "    <MultiBinding.Converter>\n" +
									    "      <Controls0:MyMultiConverter />\n" +
									    "    </MultiBinding.Converter>\n" +
									    "    <Controls0:MyBindingExtension />\n" +
									    "  </MultiBinding>\n" +
										"</TextBox>";
			
			AssertCanvasDesignerOutput(expectedXaml, textBox.Context, "xmlns:Controls0=\"" + ICSharpCode.WpfDesign.Tests.XamlDom.XamlTypeFinderTests.XamlDomTestsNamespace + "\"");
			AssertLog("");
		}
		
		[Test]
		public void AddBrushAsResource()
		{
			DesignItem checkBox = CreateCanvasContext("<CheckBox/>");
			DesignItem canvas = checkBox.Parent;
			
			DesignItemProperty canvasResources = canvas.Properties.GetProperty("Resources");
			
			DesignItem brush = canvas.Services.Component.RegisterComponentForDesigner(new SolidColorBrush());
			brush.Key = "testBrush";
			brush.Properties[SolidColorBrush.ColorProperty].SetValue(Colors.Fuchsia);
			
			Assert.IsTrue(canvasResources.IsCollection);
			canvasResources.CollectionElements.Add(brush);
			
			checkBox.Properties[CheckBox.ForegroundProperty].SetValue(new StaticResourceExtension());
			DesignItemProperty prop = checkBox.Properties[CheckBox.ForegroundProperty];
			prop.Value.Properties["ResourceKey"].SetValue("testBrush");
			
			string expectedXaml = "<Canvas.Resources>\n" +
								  "  <SolidColorBrush x:Key=\"testBrush\" Color=\"#FFFF00FF\" />\n" +
								  "</Canvas.Resources>\n" +
								  "<CheckBox Foreground=\"{StaticResource testBrush}\" />";
			
			AssertCanvasDesignerOutput(expectedXaml, checkBox.Context);
			AssertLog("");
		}
		
		public void AddNativeTypeAsResource(object component, string expectedXamlValue)
		{
			AddTypeAsResource(component, expectedXamlValue, "Controls0:", new string[] { "xmlns:Controls0=\"clr-namespace:System;assembly=mscorlib\""} );
		}
		
		public void AddTypeAsResource(object component, string expectedXamlValue, string typePrefix, String[] additionalXmlns)
		{
			DesignItem textBlock = CreateCanvasContext("<TextBlock/>");
			DesignItem canvas = textBlock.Parent;
			
			DesignItemProperty canvasResources = canvas.Properties.GetProperty("Resources");
			
			DesignItem componentItem = canvas.Services.Component.RegisterComponentForDesigner(component);
			componentItem.Key = "res1";
			
			Assert.IsTrue(canvasResources.IsCollection);
			canvasResources.CollectionElements.Add(componentItem);
			
			DesignItemProperty prop = textBlock.Properties[TextBlock.TagProperty];
			prop.SetValue(new StaticResourceExtension());
			prop.Value.Properties["ResourceKey"].SetValue("res1");
			
			string typeName = component.GetType().Name;
			
			string expectedXaml = "<Canvas.Resources>\n" +
								  "  <" + typePrefix + typeName + " x:Key=\"res1\">" + expectedXamlValue + "</" + typePrefix + typeName + ">\n" +
								  "</Canvas.Resources>\n" +
								  "<TextBlock Tag=\"{StaticResource res1}\" />";
			
			AssertCanvasDesignerOutput(expectedXaml, textBlock.Context, additionalXmlns);
			AssertLog("");
		}
		
		[Test]
		public void TestTextValue()
		{
			// An invalid path (in this case containing a question mark), or a path to a file that does not exist, will give the same result.
			// It will cause the typeconverter to fail and no value can be get from neither ValueOnInstance nor Value from the Image.Source DesignItemProperty.
			// TextValue was added to have a way of getting the xaml value.
			string sourceTextValue = "file:///C:/Folder/image?.bmp";
			
			string xaml = "<Image Source=\"" + sourceTextValue + "\" />";
			DesignItem image = CreateCanvasContext(xaml);
			
			var sourceProp = image.Properties[Image.SourceProperty];
			
			Assert.IsNull(sourceProp.ValueOnInstance);
			Assert.IsNull(sourceProp.Value);
			Assert.IsNotNull(sourceProp.TextValue);
			Assert.AreEqual(sourceTextValue, sourceProp.TextValue);
			
			string expectedXaml = xaml;
			AssertCanvasDesignerOutput(expectedXaml, image.Context);
			AssertLog("");
		}
		
		[Test]
		public void AddStringAsResource()
		{
			AddNativeTypeAsResource("stringresource 1", "stringresource 1");
		}
		
		[Test]
		public void AddDoubleAsResource()
		{
			AddNativeTypeAsResource(0.0123456789d, "0.0123456789");
		}
		
		[Test]
		public void AddInt32AsResource()
		{
			const int i = 123;
			AddNativeTypeAsResource(i, "123");
		}
		
		[Test]
		public void AddWpfEnumAsResource()
		{
			AddTypeAsResource(VerticalAlignment.Center, "Center", "", new string[0]);
		}
		
		[Test]
		public void AddCustomEnumAsResource()
		{
			AddTypeAsResource(MyEnum.One, "One", "t:", new string[0]);
		}
	}
	
	public class MyMultiConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
		
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			return new object[targetTypes.Length];
		}
	}
	
	public class ExampleClass
	{
		string stringProp;
		
		public string StringProp {
			get { return stringProp; }
			set { stringProp = value; }
		}
	}
	
	public enum MyEnum
	{
		One, Two
	}
}
