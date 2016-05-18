using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChargingPile.Class {
    class CPHeartFrameDeal {
        private UInt64 _address;
        private int _count;

        public UInt64 address {
            get { return _address;}
            set { _address = value;}
        }
        public int count {
            get { return _count; }
            set { _count = value; }
        }
    }
}
