using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugins;
using System.Windows.Forms;
using DataMaintain;
namespace DataMaintain
{
    public class DataMaintain : ICustom
    {
        private List<StructInfo> _listStructInfo;
        public List<StructInfo> ListStructInfo
        {
            get
            {
                return _listStructInfo;
            }
            set
            {
                //throw new Exception("The method or operation is not implemented.");
                _listStructInfo = value;
            }
        }

        public System.Windows.Forms.Form Execute(int menuId)
        {
            Form f = new Form();
            foreach (StructInfo si in ListStructInfo)
            {
                if (si.MenuId == menuId)
                {
                    return ExecuteFunctions(si);
                }
            }
            return f;
        }
        private Form ExecuteFunctions(StructInfo si)
        {
            Form f = new Form();
            switch (si.MenuId)
            {
                case 1010:
                    Form1 frm = new Form1();
                    frm.Text = si.MenuName;
                    //frm.ShowDialog();
                    return frm;
                    break;
            }
            return f;
        }
    }
}
