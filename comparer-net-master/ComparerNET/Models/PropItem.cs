using System;
using System.Collections.Generic;
using System.Text;

namespace ComparerNET.Models
{
    public class PropItem
    {
        public string PropIndex { get; set; }
        public string PropMedId { get; set; }
        public string MedId { get; set; }
        public string MedTitle { get; set; }
        public string PropTitle { get; set; }
        public string PropId { get; set; }
        public string PropStr { get; set; }
        public string PropVal { get; set; }
        public string DateVal { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as PropItem);
        }

        public bool Equals(PropItem propItem)
        {
            return propItem != null
                && propItem.MedId == MedId
                && propItem.PropTitle == PropTitle
                && propItem.PropId == PropId
                && propItem.PropStr == PropStr
                && propItem.PropVal == PropVal
                && propItem.DateVal == DateVal;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}