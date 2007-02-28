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
using NUnit.Framework;
using ICSharpCode.WpfDesign.Designer.Xaml;
using ICSharpCode.WpfDesign.Designer.Services;

namespace ICSharpCode.WpfDesign.Designer.Tests
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
	}
}
