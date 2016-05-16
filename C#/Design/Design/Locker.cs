using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Design
{
    class Locker
    {

        public enum Status
        {
            unset,
            set
        };

        public enum LockerStatus
        { 
            LOCK,
            UNLOCL
        }
        private LockerStatus lockerStatus = LockerStatus.LOCK;


        private String password = null;
        private Status passwordStatus = Status.unset;
        private ulong bardCode = 0;
        private Status bardCodeStatus = Status.unset;
        private String rfid = null;
        private Status rfidStatus = Status.unset;

        public Locker() 
        {
        }
        public void setPassword(String password)
        {
            this.passwordStatus = Status.set;
            this.password = password;
        }
        public String getPassword()
        {
            return this.password;
        }
        public void cleanPassword()
        {
            this.passwordStatus = Status.unset;
            this.password = "";
        }
        public void setBardCode(ulong bardCode)
        {
            this.bardCode = bardCode;
        }
        public ulong getBardCode()
        {
            return this.bardCode;
        }
        public void setRfid(String rfid)
        {
            this.rfid = rfid;
        }
        public String getRfid()
        {
            return this.rfid;
        }
        public LockerStatus getLockerStatus()
        {
            return this.lockerStatus;
        }
        public void setLockerStatus(LockerStatus status)
        {         

            this.lockerStatus = status;
        }













    }
}
