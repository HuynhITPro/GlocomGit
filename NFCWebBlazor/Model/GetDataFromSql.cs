using System.Data;

namespace NFCWebBlazor.Model
{
    public class GetDataFromSql
    {
        public GetDataFromSql()
        {

        }
        public string json { get; set; } = "";
        //public SqlParameter[] arrparameter { get; set; }
        public string sql { get; set; }
        //Sử dụng để gửi tin nhắn
        public string topic { get; set; }
        public string id { get; set; }
        public string reportname { get; set; }
        public string dtparameter { get; set; }
    }
   
    public class ParameterDefine
    {
        public string ParameterName { get; set; }
        public object ParameterValue { get; set; }
        public string Type { get; set; }
        public ParameterDefine(string name, object value)
        {
            ParameterName = name;
            ParameterValue = value;
        }

        public ParameterDefine(string name, object value,string type)
        {
            ParameterName = name;
            ParameterValue = value;
            Type= type;
        }
        public ParameterDefine()
        {

        }
    }
}
