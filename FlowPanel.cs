using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Thothi
{
    class FlowPanel : FlowLayoutPanel
    {
        public FlowPanel()
        {
            DoubleBuffered = true;
        }
        protected override void OnScroll(ScrollEventArgs se)
        {
            Invalidate();
            base.OnScroll(se);
        }
    }
}
