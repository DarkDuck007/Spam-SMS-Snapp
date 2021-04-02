using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SnappSpamSMS
{
    class SendMSGArgs
    {
        private string _Server;
        public string Server
        {
            get { return _Server; }
            set { _Server = value; }
        }
        private string _Data;
        public string Data
        {
            get { return _Data; }
            set { _Data = value; }
        }

        private string _Proxy;

        public string Proxy
        {
            get { return _Proxy; }
            set { _Proxy = value; }
        }

    }
}
