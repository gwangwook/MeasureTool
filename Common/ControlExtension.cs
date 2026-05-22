using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;

namespace MeasureTool
{
    public static class ControlExtension
    {
        public static void DoubleBuffered(this Control control, bool enable)
        {
            if (control == null)
                return;

            PropertyInfo property = typeof(Control).GetProperty(
                "DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (property != null)
                property.SetValue(control, enable, null);
        }
    }
}
