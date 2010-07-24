using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
    public class MatrixControl<TValue> : Grid
    {
        public Matrix<TValue> Matrix { get; set; }

        /// <summary>
        /// TODO: Needs to be reworked for DataBinding and to XAML
        /// </summary>
        public void DrawMatrix()
        {
            DrawHeaders();

            for (int i = 0; i < Matrix.HeaderRows.Count; i++) {
                RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });

                for (int j = 0; j < Matrix.HeaderColumns.Count; j++) {
                    ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                    var val = Matrix.EvaluateCell(Matrix.HeaderRows[i], Matrix.HeaderColumns[j]);

                    var label = new Label { Content = val };

                    var style = new Style();
                    
                    style.Setters.Add(new Setter
                    {
                        Property = Grid.RowProperty,
                        Value = i + 1
                    });

                    style.Setters.Add(new Setter
                    {
                        Property = Grid.ColumnProperty,
                        Value = j + 1
                    });

                    label.Style = style;

                    Children.Add(label);
                }
            }
        }

        protected void DrawHeaders()
        {
            RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

            for (int i = 0; i < Matrix.HeaderRows.Count; i++) {
                var label = new Label { Content = Matrix.HeaderRows[i].Value.ToString() };

                var style = new Style();

                style.Setters.Add(new Setter
                {
                    Property = Grid.RowProperty,
                    Value = i + 1
                });

                style.Setters.Add(new Setter
                {
                    Property = Grid.ColumnProperty,
                    Value = 0
                });

                label.Style = style;

                Children.Add(label);
            }

            for (int i = 0; i < Matrix.HeaderColumns.Count; i++) {
                var label = new Label { Content = Matrix.HeaderColumns[i].Value.ToString() };

                var style = new Style();

                style.Setters.Add(new Setter
                {
                    Property = Grid.RowProperty,
                    Value = 0
                });

                style.Setters.Add(new Setter
                {
                    Property = Grid.ColumnProperty,
                    Value = i + 1
                });

                label.Style = style;
                /*label.RenderTransform = new RotateTransform() // need some tweaking
                                            {
                                                Angle = -90,
                                                CenterX = -10,
                                                CenterY = 40
                                            };*/ 

                Children.Add(label);
            }
        }
    }
}
