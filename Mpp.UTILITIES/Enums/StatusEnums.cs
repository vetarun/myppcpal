using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.UTILITIES.Enums
{
    public class StatusEnums
    {
        public enum DownLoadStatus
        {
            NotSet=0,Set=1,Downloaded=2,Imported=3
        }
        public enum NegativeDownLoadStatus
        {
            Notfound=0, Set = 1, Downloaded = 2, Imported = 3
        }
        public enum UploadStatus
        {
           NA=0, CSVCreated = 1, Uploaded = 2
        }
        public enum AccessStatus
        {
            Notfound = 0, Granted = 1, Verified = 2
        }
        public enum RefreshStatus
        {
            NotSet=0,Set=1,Downloaded=2
        }
       

    }
}
