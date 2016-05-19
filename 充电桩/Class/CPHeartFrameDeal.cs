using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChargingPile.Class {
    class CPHeartFrameDeal {
        private UInt64 _address;
        private int _count;
        private bool _comLedFlg = false;
        private heartFrameState _heartFrameLedState;


        public enum heartFrameState {
            comBreakGray,
            heartFrameNomalGreen,
            heartFrameBusyYellow
        }
        public UInt64 address {
            get { return _address; }
            set { _address = value;}
        }
        public int count {
            get { return _count; }
            set { _count = value; }
        }
        public bool comLedFlg {
            get { return _comLedFlg; }
            set { _comLedFlg = value; }
        }
        public heartFrameState heartFrameLedState {
            get {
                if (false == _comLedFlg) {
                    _heartFrameLedState = heartFrameState.comBreakGray;
                }
                return _heartFrameLedState; 
            }
            set { _heartFrameLedState = value; }
        }
    }
}
