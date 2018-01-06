using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLabeler
{
    public class DefItem : IEquatable<DefItem>
    {
        public string KeyName;
        public string DispName;
        public string ClassName;

        public bool Equals(DefItem item)
        {
            if (this.KeyName == item.KeyName || this.DispName == item.DispName || this.ClassName == item.ClassName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return 1;

        }
    }


    static class DefinitionParser
    {


    }
}
