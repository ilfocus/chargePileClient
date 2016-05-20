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
        private string _cpStartupState = "";
        private bool _isSetupCpStateData = false;
        private bool _isSetupCpCurInfoData = false;
        
        // 忙状态判断
        private bool _isHeartFrameBusy = false;
        private bool _isSetTimeBusy = false;
        private bool _isSetRateBusy = false;
        private bool _isCpStartBusy = false;
        private bool _isGetCpStateBusy = false;
        private bool _isGetCpCurInfoBusy = false;

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
        public string cpStartupState {
            get { return _cpStartupState; }
            set { _cpStartupState = value; }
        }
        public bool isSetupCpStateData {
            get { return _isSetupCpStateData; }
            set { _isSetupCpStateData = value; }
        }
        public bool isSetupCpCurInfoData {
            get { return _isSetupCpCurInfoData; }
            set { _isSetupCpCurInfoData = value; }
        }


        public bool isHeartFrameBusy {
            get { return _isHeartFrameBusy; }
            set { _isHeartFrameBusy = value; }
        }
        public bool isSetTimeBusy {
            get { return _isSetTimeBusy; }
            set { _isSetTimeBusy = value; }
        }
        public bool isSetRateBusy {
            get { return _isSetRateBusy; }
            set { _isSetRateBusy = value; }
        }
        public bool isCpStartBusy {
            get { return _isCpStartBusy; }
            set { _isCpStartBusy = value; }
        }
        public bool isGetCpStateBusy {
            get { return _isGetCpStateBusy; }
            set { _isGetCpStateBusy = value; }
        }
        public bool isGetCpCurInfoBusy {
            get { return _isGetCpCurInfoBusy; }
            set { _isGetCpCurInfoBusy = value; }
        }
    }
}
