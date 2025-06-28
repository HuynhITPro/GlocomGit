using Newtonsoft.Json;
using System.Data;

namespace NFCWebBlazor.App_ModelClass
{
    public class JsonMsgAndroid
    {
        public string topic { get; set; }
        public string clientid { get; set; }
        public string message { get; set; }
        public string usersend { get; set; }
        public string typemsg { get; set; }
        public string msgJson { get; set; } //Class MsgJson
        public List<string> lstuserrecive { get; set; } = new List<string>();
        public string ToJson()
        {
            string json = JsonConvert.SerializeObject(this);
            return json;
        }
        public JsonMsgAndroid GetJson(string json)
        {
            JsonMsgAndroid jsonMsgAndroid = JsonConvert.DeserializeObject<JsonMsgAndroid>(json);
            return jsonMsgAndroid;
        }
        public void sendmsgtogroup(string _topic,string _message)
        {
            this.topic = _topic;
            this.message= _message;
            typemsg=TypemsgAPI.sendmsgtogroup.ToString();
        }
        public void sendmsgtogroupexceptme(string _topic, string _message)
        {
            this.topic = _topic;
            this.message = _message;
            typemsg = TypemsgAPI.sendmsgtogroupexceptme.ToString();
        }
        public void sendmsgall(string _message)
        {
            
            this.message = _message;
            typemsg = TypemsgAPI.sendmsgall.ToString();
        }
        public void sendmsgtoclient(string _clientid, string _message)
        {
            this.clientid = _clientid;
            this.message = _message;
            typemsg = TypemsgAPI.sendmsgtoclient.ToString();
        }
        public void sendmsggetconnectionid()
        {
           
            typemsg = TypemsgAPI.getconnectionid.ToString();
        }
        public void sendmsgjoingroup(string _toppic)
        {
            this.topic = _toppic;
            
            typemsg = TypemsgAPI.joingroup.ToString();
        }
        public void sendmsgjoingrouplist(string _topic)//Topic ngăn cách nhau bởi ;
        {
            this.topic = _topic;
            typemsg = TypemsgAPI.joingrouplist.ToString();
        }
        public void sendmsgleavegroup(string _topic)
        {
            this.topic = _topic;
            typemsg = TypemsgAPI.leavegroup.ToString();
        }
        public void sendmsgdisconnect(string _clientid)
        {
            this.clientid = clientid;
            typemsg = TypemsgAPI.disconnect.ToString();
        }
    }
    public class InPhieuJson
    {
        public string id { get; set; }
        public string topic { get; set; }
        public string tablename { get; set; }
        public string msg { get; set; }
        public string typename { get; set; }
        public string ketqua { get; set; }
        public string ToJson()
        {
            string json = JsonConvert.SerializeObject(this);
            return json;
        }

        public DataTable dtsource { get; set; }

    }
    public enum TypemsgAPI
    {
        sendmsgall,
        sendmsgtogroup,
        sendmsgtogroupexceptme,
        sendmsgtoclient,
        getconnectionid,
        joingroup,
        joingrouplist,
        leavegroup,
        disconnect

    }
}
