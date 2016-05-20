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

        //  充电状态
        private UInt32 _cpVoltage = 0;
        public UInt32 CpVoltage {
            get { return _cpVoltage; }
            set { _cpVoltage = value; }
        }
        private UInt32 _cpCurrent = 0;
        public UInt32 CpCurrent {
            get { return _cpCurrent; }
            set { _cpCurrent = value; }
        }
        private UInt32 _cpTotalElect = 0;
        public UInt32 CpTotalElect {
            get { return _cpTotalElect; }
            set { _cpTotalElect = value; }
        }
        private UInt32 _cpPointElect = 0;
        public UInt32 CpPointElect {
            get { return _cpPointElect; }
            set { _cpPointElect = value; }
        }
        private UInt32 _cpPeakElect = 0;
        public UInt32 CpPeakElect {
            get { return _cpPeakElect; }
            set { _cpPeakElect = value; }
        }
        private UInt32 _cpFlatElect = 0;
        public UInt32 CpFlatElect {
            get { return _cpFlatElect; }
            set { _cpFlatElect = value; }
        }
        private UInt32 _cpValleyElect = 0;
        public UInt32 CpValleyElect {
            get { return _cpValleyElect; }
            set { _cpValleyElect = value; }
        }
        private byte _emergencyStopButton = 0;
        public byte EmergencyStopButton {
            get { return _emergencyStopButton; }
            set { _emergencyStopButton = value; }
        }
        private byte _electMeter = 0;
        public byte ElectMeter {
            get { return _electMeter; }
            set { _electMeter = value; }
        }
        private byte _chargePlug = 0;
        public byte ChargePlug {
            get { return _chargePlug; }
            set { _chargePlug = value; }
        }
        private byte _cpOutState = 0;
        public byte CpOutState {
            get { return _cpOutState; }
            set { _cpOutState = value; }
        }
        private UInt16 _faultState = 0;
        public UInt16 FaultState {
            get { return _faultState; }
            set { _faultState = value; }
        }
        private byte _currentState = 0;
        public byte CurrentState {
            get { return _currentState; }
            set { _currentState = value; }
        }

        //////////// 充电桩当前状态 ////////////////////////
        private UInt32 _chargeTotalElect = 0;
        public UInt32 ChargeTotalElect {
            get { return _chargeTotalElect; }
            set { _chargeTotalElect = value; }
        }
        private UInt32 _chargeTotalPrice = 0;
        public UInt32 ChargeTotalPrice {
            get { return _chargeTotalPrice; }
            set { _chargeTotalPrice = value; }
        }
        private UInt32 _chargePointElect = 0;

        public UInt32 ChargePointElect {
            get { return _chargePointElect; }
            set { _chargePointElect = value; }
        }
        private UInt32 _chargePeakElect = 0;
        public UInt32 ChargePeakElect {
            get { return _chargePeakElect; }
            set { _chargePeakElect = value; }
        }
        private UInt32 _chargeFlatElect = 0;
        public UInt32 ChargeFlatElect {
            get { return _chargeFlatElect; }
            set { _chargeFlatElect = value; }
        }
        private UInt32 _chargeValleyElect = 0;
        public UInt32 ChargeValleyElect {
            get { return _chargeValleyElect; }
            set { _chargeValleyElect = value; }
        }
        private UInt32 _chargePointPrice = 0;
        public UInt32 ChargePointPrice {
            get { return _chargePointPrice; }
            set { _chargePointPrice = value; }
        }
        private UInt32 _chargePeakPrice = 0;
        public UInt32 ChargePeakPrice {
            get { return _chargePeakPrice; }
            set { _chargePeakPrice = value; }
        }
        private UInt32 _chargeFlatPrice = 0;
        public UInt32 ChargeFlatPrice {
            get { return _chargeFlatPrice; }
            set { _chargeFlatPrice = value; }
        }
        private UInt32 _chargeValleyPrice = 0;
        public UInt32 ChargeValleyPrice {
            get { return _chargeValleyPrice; }
            set { _chargeValleyPrice = value; }
        }
        private UInt32 _chargePointCost = 0;
        public UInt32 ChargePointCost {
            get { return _chargePointCost; }
            set { _chargePointCost = value; }
        }
        private UInt32 _chargePeakCost = 0;
        public UInt32 ChargePeakCost {
            get { return _chargePeakCost; }
            set { _chargePeakCost = value; }
        }
        private UInt32 _chargeFlatCost = 0;
        public UInt32 ChargeFlatCost {
            get { return _chargeFlatCost; }
            set { _chargeFlatCost = value; }
        }
        private UInt32 _chargeValleyCost = 0;
        public UInt32 ChargeValleyCost {
            get { return _chargeValleyCost; }
            set { _chargeValleyCost = value; }
        }
        // 设置时间
        private string _year = "";

        public string Year {
            get { return _year; }
            set { _year = value; }
        }

        private string _month = "";

        public string Month {
            get { return _month; }
            set { _month = value; }
        }
        private string _day = "";

        public string Day {
            get { return _day; }
            set { _day = value; }
        }
        private string _hour = "";

        public string Hour {
            get { return _hour; }
            set { _hour = value; }
        }
        private string _minute = "";

        public string Minute {
            get { return _minute; }
            set { _minute = value; }
        }
        private string _second = "";

        public string Second {
            get { return _second; }
            set { _second = value; }
        }
        // 设置费率
        private UInt32 _pointPrice = 0;

        public UInt32 PointPrice {
            get { return _pointPrice; }
            set { _pointPrice = value; }
        }
        private UInt32 _peakPrice = 0;

        public UInt32 PeakPrice {
            get { return _peakPrice; }
            set { _peakPrice = value; }
        }
        private UInt32 _flatPrice = 0;

        public UInt32 FlatPrice {
            get { return _flatPrice; }
            set { _flatPrice = value; }
        }
        private UInt32 _valleyPrice = 0;

        public UInt32 ValleyPrice {
            get { return _valleyPrice; }
            set { _valleyPrice = value; }
        }

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
