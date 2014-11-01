using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor
{
    public class UndoRedo
    {
        public enum Type
        {
            Plaats,
            Verwijder
        }

        private Type t;
        private SchetsObject s;

        public UndoRedo(Type t, SchetsObject s)
        {
            this.t = t;
            this.s = s;
        }
    }
}
