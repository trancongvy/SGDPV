using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
namespace DevControls
{
    public partial class CDTBandGridColumn : BandedGridColumn
    {
        public CDTBandGridColumn()
        {
            InitializeComponent();
        }

        DataRow _masterRow;
        string _refFilter;
        bool _isExCol = false;
        int _indexvisible = 1;
        public int IndexVisible
        {
            get { return _indexvisible; }
            set { _indexvisible = value; }
        }
        public DataRow MasterRow
        {
            get { return _masterRow; }
            set { _masterRow = value; }
        }
        public bool isExCol
        {
            get { return _isExCol; }
            set { _isExCol = value; }
        }
        public string refFilter
        {
            get { return _refFilter; }
            set { _refFilter = value; }
        }
    }
    
}
