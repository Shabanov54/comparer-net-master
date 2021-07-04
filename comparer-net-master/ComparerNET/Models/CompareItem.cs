using System;
using System.Text;

namespace ComparerNET.Models
{
    public class CompareItem: IEquatable<CompareItem>
    {
        public string HostId { get; set; }
        public string HostStandartId { get; set; }
        public string MedId { get; set; }
        public string MedTitle { get; set; }
        public string PropTitle { get; set; }
        public string PropStandartStr { get; set; }
        public string PropComparetStr { get; set; }
        public string PropStandartVal { get; set; }
        public string PropCompareVal { get; set; }
        public string DateStandart { get; set; }
        public string DateCompare { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(MedTitle);
            stringBuilder.Append(PropTitle);
            stringBuilder.Append(PropStandartStr);
            stringBuilder.Append(PropComparetStr);
            stringBuilder.Append(PropStandartVal);
            stringBuilder.Append(PropCompareVal);
            stringBuilder.Append(DateStandart);
            stringBuilder.Append(DateCompare);
            return stringBuilder.ToString();
        }

        public bool Equals(CompareItem other)
        {
            if (other == null)
            {
                return false;
            }
            else
            {
                return ToString() == other.ToString();
            }
        }

        public override int GetHashCode()
        {
            return base.ToString().GetHashCode();
        }
    }
}
