using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows;
using System.Windows.Documents;
using System.Xml.Linq;
using System.Windows.Markup;
using ICSharpCode.WpfDesign.Designer.XamlBackend;

namespace ICSharpCode.Xaml.Tests
{
	[TestFixture]
	public class EditingTests
	{
		DefaultWpfProject project = new DefaultWpfProject();
		XamlDocument document;

		void Check(string s)
		{
			var doc1 = XDocument.Parse(EasyDocument(s));
			foreach (var e in doc1.Root.DescendantsAndSelf()) {
				e.Name = XamlConstants.Presentation2007Namespace + e.Name.LocalName;
			}

			Assert.IsTrue(XNode.DeepEquals(doc1, document.XmlDocument));

			// some tests incorrect from instance point of view
			object officialInstance = null;
			try {
				officialInstance = XamlReader.Parse(document.XmlDocument.ToString());
			}
			catch {
				return;
			}

			var myResult = XamlWriter.Save(document.Root.Instance);
			var officialResult = XamlWriter.Save(officialInstance);

			Assert.AreEqual(myResult, officialResult);
		}

		string EasyDocument(string s)
		{
			var index = s.IndexOfAny(new[] { ' ', '/', '>' });
			return s.Insert(index, string.Format(" xmlns='{0}' xmlns:x='{1}' ",
				XamlConstants.Presentation2007Namespace, XamlConstants.XamlNamespace));
		}

		[Test]
		public void Test1()
		{
			document = project.CreateDocument(new Button());
			ObjectNode button = document.Root;

			Check("<Button/>");

			button.Property("Width").Set(100);
			Check("<Button Width='100'/>");

			button.Property("Width").Reset();
			Check("<Button/>");

			button.Property(Button.WidthProperty).Set(100);
			Check("<Button Width='100'/>");

			button.Property(Button.WidthProperty).Reset();
			Check("<Button/>");

			button.Content.Set("Label");
			Check("<Button Content='Label'/>");

			button.Content.Set(new Border());
			Check("<Button><Border/></Button>");
			button.Content.Reset();

			button.Property(Button.BackgroundProperty).Set(new LinearGradientBrush());
			Check("<Button><Button.Background><LinearGradientBrush/></Button.Background></Button>");
			button.Property(Button.BackgroundProperty).Reset();

			button.Property(Canvas.LeftProperty).Set(100);
			Check("<Button Canvas.Left='100'/>");
			button.Property(Canvas.LeftProperty).Reset();

			var binding = button.Content.SetObject(new Binding());
			Check("<Button Content='{Binding}'/>");

			binding.Property("ElementName").Set("e1");
			Check("<Button Content='{Binding ElementName=e1}'/>");

			binding.Property("Mode").Set(BindingMode.TwoWay);
			Check("<Button Content='{Binding ElementName=e1, Mode=TwoWay}'/>");

			var staticResource = binding.Property("Converter").SetObject(new StaticResourceExtension());
			staticResource.Property("ResourceKey").Set("Converter1");
			Check("<Button Content='{Binding ElementName=e1, Mode=TwoWay, Converter={StaticResource Converter1}}'/>");
			button.Content.Reset();

			binding = button.Content.SetObject("{Binding Some1}");
			Check("<Button Content='{Binding Some1}'/>");

			binding.Property("Source").Set(new Button());
			Check("<Button><Binding Path='Some1'><Binding.Source><Button/></Binding.Source></Binding></Button>");

			binding.Property("Source").Reset();
			Check("<Button Content='{Binding Some1}'/>");

			binding.Remove();
			Check("<Button/>");

			button.Property("Resources").Add("brush1", Brushes.Red);
			Check("<Button><Button.Resources><Brush x:Key='brush1'>Red</Brush></Button.Resources></Button>");

			button.Property("Resources").Reset();
			Check("<Button/>");
		}

		[Test]
		public void Test2()
		{
			document = project.CreateDocument(new TextBlock());
			ObjectNode textBlock = document.Root;

			Check("<TextBlock/>");

			textBlock.Content.Add("text1");
			Check("<TextBlock>text1</TextBlock>");

			textBlock.Content.Add("text2");
			Check("<TextBlock>text1text2</TextBlock>");

			var bold = textBlock.Content.AddObject(new Bold());
			bold.Content.Set("bold1");
			textBlock.Content.Add("text3");
			Check("<TextBlock>text1text2<Bold>bold1</Bold>text3</TextBlock>");

			//var bold = textBlock.Content.AddObject(new Bold());
			//bold.Content.Set("bold1");
			//textBlock.Content.Add(" text3 ");
			//Check("<TextBlock xml:space='preserve'>text1text2<Bold>bold1</Bold> text3 </TextBlock>");

			//bold.Remove();
			//Check("<TextBlock>text1text2 text3 </TextBlock>");

			//textBlock.Content.Reset();
			//Check("<TextBlock/>");
		}

		[Test]
		public void Test3()
		{
			document = project.CreateDocument(new CheckBox());
			var checkBox = document.Root;
			document.Root = null;
			Assert.IsTrue(document.XmlDocument.Root == null);

			document.Root = checkBox;
			Check("<CheckBox/>");

			document.Parse(EasyDocument("<Slider/>"));
			Check("<Slider/>");

			var canvas = document.CreateObject(new Canvas());
			canvas.Property("Width").Set(100);
			document.Root = canvas;
			Check("<Canvas Width='100'/>");
			canvas.Property("Width").Reset();

			var button1 = canvas.Content.AddObject(new Button());
			canvas.Content.Add(new Button());
			button1.Remove();
			Check("<Canvas><Button/></Canvas>");
		}

		//[Test]
		//public void Test4()
		//{
		//    document = project.CreateDocument(new Style());
		//    var style = document.Root;
		//    var setter = style.Content.AddObject(new Setter());
		//    setter.Property("Property").Set("Background");
		//}

		[Test]
		public void Test5()
		{
			document = project.CreateDocument(new Button());
			var button = document.Root;
			Assert.AreEqual(button.Property(Button.HorizontalAlignmentProperty).ValueOnInstance,
				new Button().HorizontalAlignment);
		}
	}
}
