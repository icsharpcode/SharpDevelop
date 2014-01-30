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
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
//using ICSharpCode.Reports.Addin.Designer;
using ICSharpCode.Reports.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
//using SharpQuery.Collections;
//using SharpQuery.SchemaClass;

using System.ComponentModel;
using System.Windows.Forms.VisualStyles;


/// <summary>
/// //http://www.codeproject.com/useritems/CheckBoxHeaderCell.asp
/// </summary>
namespace ICSharpCode.Reports.Addin.ReportWizard
{
    public class DataGridViewColumnHeaderCheckBoxCell : DataGridViewColumnHeaderCell
    {
        #region Fields

        private bool _Checked;
        private Size _CheckBoxSize;
        private Point _CheckBoxLocation;
        private Rectangle _CheckBoxBounds;
        private Point _CellLocation = new Point();
        private HorizontalAlignment _CheckBoxAlignment = HorizontalAlignment.Center;

        #endregion

        #region Events

        public event DataGridViewCheckBoxHeaderCellEvenHandler CheckBoxClicked;

        #endregion

        #region Constructor

        public DataGridViewColumnHeaderCheckBoxCell()
            : base()
        {
        }

        #endregion

        #region Properties

        public HorizontalAlignment CheckBoxAlignment
        {
            get { return _CheckBoxAlignment; }
            set
            {
                if (!Enum.IsDefined(typeof(HorizontalAlignment), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(HorizontalAlignment));

                }
                if (_CheckBoxAlignment != value)
                {
                    _CheckBoxAlignment = value;
                    if (this.DataGridView != null)
                    {
                        this.DataGridView.InvalidateCell(this);
                    }
                }
            }
        }

        public bool Checked
        {
            get { return _Checked; }
            set
            {
                if (_Checked != value)
                {
                    _Checked = value;
                    if (this.DataGridView != null)
                    {
                        this.DataGridView.InvalidateCell(this);
                    }
                }
            }
        }

        #endregion

        #region Methods

        protected virtual void OnCheckBoxClicked(DataGridViewCheckBoxHeaderCellEventArgs e)
        {
            if (this.CheckBoxClicked != null)
            {
                this.CheckBoxClicked(this, e);
            }
        }

        #endregion

        #region Override

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds,
                    int rowIndex, DataGridViewElementStates dataGridViewElementState, object value,
                    object formattedValue, string errorText, DataGridViewCellStyle cellStyle,
                    DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            //checkbox bounds
            CheckBoxState state = this.CheckBoxState;
            _CellLocation = cellBounds.Location;
            _CheckBoxSize = CheckBoxRenderer.GetGlyphSize(graphics, state);
            Point p = new Point();
            p.Y = cellBounds.Location.Y + (cellBounds.Height / 2) - (_CheckBoxSize.Height / 2);
            switch (_CheckBoxAlignment)
            {
                case HorizontalAlignment.Center:
                    p.X = cellBounds.Location.X + (cellBounds.Width / 2) - (_CheckBoxSize.Width / 2) - 1;
                    break;
                case HorizontalAlignment.Left:
                    p.X = cellBounds.Location.X + 2;
                    break;
                case HorizontalAlignment.Right:
                    p.X = cellBounds.Right - _CheckBoxSize.Width - 4;
                    break;
            }
            _CheckBoxLocation = p;
            _CheckBoxBounds = new Rectangle(_CheckBoxLocation, _CheckBoxSize);

            //paint background
            paintParts &= ~DataGridViewPaintParts.ContentForeground;
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, value,
                formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            //paint foreground
            switch (_CheckBoxAlignment)
            {
                case HorizontalAlignment.Center:
                    cellBounds.Width = _CheckBoxLocation.X - cellBounds.X - 2;
                    break;
                case HorizontalAlignment.Left:
                    cellBounds.X += _CheckBoxSize.Width + 2;
                    cellBounds.Width -= _CheckBoxSize.Width + 2;
                    break;
                case HorizontalAlignment.Right:
                    cellBounds.Width -= _CheckBoxSize.Width + 4;
                    break;
            }
            paintParts = DataGridViewPaintParts.ContentForeground;
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, value,
                formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            //paint check box           
            CheckBoxRenderer.DrawCheckBox(graphics, _CheckBoxLocation, state);
        }

        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //click on check box ?
                Point p = new Point(_CellLocation.X + e.X, _CellLocation.Y + e.Y);
                if (_CheckBoxBounds.Contains(p))
                {
                    //raise event
                    RaiseCheckBoxClicked();
                }
            }
            base.OnMouseClick(e);
        }

        protected override void OnKeyDown(KeyEventArgs e, int rowIndex)
        {
            if (e.KeyCode == Keys.Space)
            {
                //raise event
                RaiseCheckBoxClicked();
            }
            base.OnKeyDown(e, rowIndex);
        }

        public override object Clone()
        {
            DataGridViewColumnHeaderCheckBoxCell cell = base.Clone() as DataGridViewColumnHeaderCheckBoxCell;
            if (cell != null)
            {
                cell.Checked = this.Checked;
            }
            return cell;
        }
        #endregion

        #region Private

        private void RaiseCheckBoxClicked()
        {
            //raise event
            DataGridViewCheckBoxHeaderCellEventArgs e = new DataGridViewCheckBoxHeaderCellEventArgs(!_Checked);
            this.OnCheckBoxClicked(e);
            if (!e.Cancel)
            {
                this.Checked = e.Checked;
                this.DataGridView.InvalidateCell(this);
            }
        }
        private CheckBoxState CheckBoxState
        {
            get
            {
                bool enabled = true;
                if (base.DataGridView != null && !base.DataGridView.Enabled)
                {
                    enabled = false;
                }
                if (enabled)
                {
                    return (_Checked) ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal;
                }
                return (_Checked) ? CheckBoxState.CheckedDisabled : CheckBoxState.UncheckedDisabled;
            }
        }

        #endregion
    }

    #region Events and Delegates

    public delegate void DataGridViewCheckBoxHeaderCellEvenHandler(object sender, DataGridViewCheckBoxHeaderCellEventArgs e);

    public class DataGridViewCheckBoxHeaderCellEventArgs : CancelEventArgs
    {
        bool _Checked;

        public DataGridViewCheckBoxHeaderCellEventArgs(bool checkedValue)
            : base()
        {
            _Checked = checkedValue;
        }
        public DataGridViewCheckBoxHeaderCellEventArgs(bool checkedValue, bool cancel)
            : base(cancel)
        {
            _Checked = checkedValue;
        }

        public bool Checked
        {
            get { return _Checked; }
        }
    }

    #endregion
}
