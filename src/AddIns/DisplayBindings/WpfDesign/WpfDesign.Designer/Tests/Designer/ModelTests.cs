// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using NUnit.Framework;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.Xaml;
using ICSharpCode.WpfDesign.Designer.Services;

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
			AssertCanvasDesignerOutput("<Button Width=\"100\">\n</Button>", button.Context);
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
	}
}
