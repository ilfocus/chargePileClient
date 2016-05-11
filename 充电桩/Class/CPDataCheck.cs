using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChargingPile.Class
{
    public class CPDataCheck
    {
        public UInt64 CHARGING_PILE_ADDRESS = 0x1122334455667788;
        // BCC:异或校验法(Block Check Character)
        // startNum 以开始点，从帧长度开始
        // endNum arr.length - 2 
        public byte GetBCC_Check(byte[] temp, int startNum, int endNum)
        {
            byte A = 0;
            for (int i = startNum; i < endNum; i++) {
                A ^= temp[i];
            }
            return A;
        }
        // 检测数据帧长度是否正确,正确返回ture,出错返回false
        public bool DataLengthCheck(byte[] array,int lenNum)
        {
            if (array.Length < lenNum) return false;
            if ((array[lenNum] == (array.Length - lenNum - 1))) return true;
            return false;
        }
        public bool AddressCheck(byte[] array)
        {
            // array[2] ~ array[9] 为充电桩地址
            UInt32 addressH = (UInt32)((array[2] << 24) | (array[3] << 16) 
                                    | (array[4] << 8) | (array[5]));
            UInt32 addressL = (UInt32)((array[6] << 24) | (array[7] << 16) 
                                    | (array[8] << 8) | (array[9]));
            UInt64 LocalAddress = CHARGING_PILE_ADDRESS;
            if ( (addressH == (LocalAddress >> 32))
              && (addressL == ((UInt32)LocalAddress))) return true;
            return false;
        }

        //private UInt64[] addRessArray = 
        private static List<UInt64> addressList = new List<UInt64>();
        public bool AddressCheck(byte[] array, UInt64 address) {
            // array[2] ~ array[9] 为充电桩地址
            UInt32 addressH = (UInt32)((array[2] << 24) | (array[3] << 16)
                                    | (array[4] << 8) | (array[5]));
            UInt32 addressL = (UInt32)((array[6] << 24) | (array[7] << 16)
                                    | (array[8] << 8) | (array[9]));
            UInt64 LocalAddress = address;

            if (false == addAddressToList(address)) {
                addressList.Add(address);
            }
            Console.WriteLine("addressList:" + addressList.Count);
            for (int i = 0; i < addressList.Count; i++) {
                if ((addressH == (addressList[i] >> 32))
                 && (addressL == ((UInt32)addressList[i]))) return true;
            }

            //if ((addressH == (LocalAddress >> 32))
            //    && (addressL == ((UInt32)LocalAddress))) return true;
            return false;
        }

        private bool addAddressToList(UInt64 address) {
            for (int i = 0; i < addressList.Count; i++) {
                if (address == addressList[i]) return true;
            }
            return false;
        }
    }
}
