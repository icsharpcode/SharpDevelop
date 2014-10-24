using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace ICSharpCode.WpfDesign.Designer.PropertyGrid.Editors.FormatedTextEditor
{
    /// <summary>
    /// Interaktionslogik für FormatedTextEditor.xaml
    /// </summary>
    public partial class FormatedTextEditor
    {
        private DesignItem designItem;

        public FormatedTextEditor(DesignItem designItem)
        {
            InitializeComponent();

            this.designItem = designItem;            
        }

        private void GetDesignItems(TextElementCollection<Block> blocks, List<DesignItem> list)
        {
            bool first = true;

            foreach (var block in blocks)
            {
                if (block is Paragraph)
                {
                    if (!first)
                    {
                        list.Add(designItem.Services.Component.RegisterComponentForDesigner(new LineBreak()));
                        list.Add(designItem.Services.Component.RegisterComponentForDesigner(new LineBreak()));
                    }

                    foreach (var inline in ((Paragraph) block).Inlines)
                    {
                        //yield return inline;
                        list.Add(CloneInline(inline));
                    }
                }
                else if (block is Section)
                {
                    GetDesignItems(((Section)block).Blocks, list);                    
                }

                first = false;
            }
        }

        private DesignItem CloneInline(Inline inline)
        {
            DesignItem d = d = designItem.Services.Component.RegisterComponentForDesigner(inline);
            if (inline is Run)
            {
                var run = inline as Run;

                if (run.ReadLocalValue(Run.TextProperty) != DependencyProperty.UnsetValue)
                {
                    d.Properties.GetProperty(Run.TextProperty).SetValue(run.Text);
                }                
            }
            else if (inline is Span)
            { }
            else if (inline is LineBreak)
            { }
            else
            {
                return null;
            }

            if (inline.ReadLocalValue(TextElement.BackgroundProperty) != DependencyProperty.UnsetValue)
                d.Properties.GetProperty(TextElement.BackgroundProperty).SetValue(inline.Background);
            if (inline.ReadLocalValue(TextElement.ForegroundProperty) != DependencyProperty.UnsetValue)
                d.Properties.GetProperty(TextElement.ForegroundProperty).SetValue(inline.Foreground);
            if (inline.ReadLocalValue(TextElement.FontFamilyProperty) != DependencyProperty.UnsetValue)
                d.Properties.GetProperty(TextElement.FontFamilyProperty).SetValue(inline.FontFamily);
            if (inline.ReadLocalValue(TextElement.FontSizeProperty) != DependencyProperty.UnsetValue)
                d.Properties.GetProperty(TextElement.FontSizeProperty).SetValue(inline.FontSize);
            if (inline.ReadLocalValue(TextElement.FontStretchProperty) != DependencyProperty.UnsetValue)
                d.Properties.GetProperty(TextElement.FontStretchProperty).SetValue(inline.FontStretch);
            if (inline.ReadLocalValue(TextElement.FontStyleProperty) != DependencyProperty.UnsetValue)
                d.Properties.GetProperty(TextElement.FontStyleProperty).SetValue(inline.FontStyle);
            if (inline.ReadLocalValue(TextElement.FontWeightProperty) != DependencyProperty.UnsetValue)
                d.Properties.GetProperty(TextElement.FontWeightProperty).SetValue(inline.FontWeight);
            if (inline.ReadLocalValue(TextElement.TextEffectsProperty) != DependencyProperty.UnsetValue)
                d.Properties.GetProperty(TextElement.TextEffectsProperty).SetValue(inline.TextEffects);

            return d;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            var changeGroup = designItem.OpenGroup("Formated Text");

            designItem.Properties.GetProperty(TextBlock.TextProperty).Reset();

            var inlinesProperty = designItem.Properties.GetProperty("Inlines");
            inlinesProperty.CollectionElements.Clear();

            var doc = richTextBox.Document;
            richTextBox.Document = new FlowDocument();

            var inlines = new List<DesignItem>();
            GetDesignItems(doc.Blocks, inlines);
            
            foreach (var inline in inlines)
            {

                inlinesProperty.CollectionElements.Add(inline);
            }

            changeGroup.Commit();

            this.TryFindParent<Window>().Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.TryFindParent<Window>().Close();
        }
    }
}
