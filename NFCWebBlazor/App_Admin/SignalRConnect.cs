using BlazorBootstrap;
using Microsoft.AspNetCore.SignalR.Client;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System.ServiceModel.Channels;

namespace NFCWebBlazor.App_Admin
{
    public  class SignalRConnect
    {
        private HubConnection? _hubConnection;
        private readonly string _hubUrl;
        public string ConnectionID { get; set; } = "";

        public List<string>lstgroupsubcribe=new List<string>();
        public string PrinterID { get; set; } = "";
        public Action<string> OnMessageReceived { get; set; }
        
        public SignalRConnect()
        {
            _hubUrl = ModelAdmin.urlAPI + "/chathub";
            _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl)
            .WithAutomaticReconnect(new[]
            {
                  TimeSpan.Zero,             // Thử kết nối lại ngay lập tức (lần 1)
                    TimeSpan.FromSeconds(60),  // Thử lại sau 60 giây (lần 2)
                    TimeSpan.FromSeconds(60),  // Thử lại sau thêm 60 giây (lần 3)
                    TimeSpan.FromSeconds(60),
                     TimeSpan.FromSeconds(300)
                     

            })
            .Build();
            //Trả về Json
            _hubConnection.On<string>("ReceiveMessage", (receivedMessage) =>
            {
                JsonMsgAndroid jsonMsgAndroid = new JsonMsgAndroid();
                jsonMsgAndroid = jsonMsgAndroid.GetJson(receivedMessage);
                if (jsonMsgAndroid.typemsg == TypemsgAPI.getconnectionid.ToString())
                {
                    ModelAdmin.ConnectionID = jsonMsgAndroid.message;
                    ConnectionID=jsonMsgAndroid.message;
                }
                OnMessageReceived(receivedMessage);
                //toastService.Notify(new ToastMessage(ToastType.Success, $"Tin nhắn: {jsonMsgAndroid.message}"));
                //messages.Add(receivedMessage); // Lưu tin nhắn vào danh sách
                //StateHasChanged(); // Cập nhật giao diện
            });

            _hubConnection.Closed += OnClosedAsync;
            // Lắng nghe tin nhắn từ server
            //Trả về Json

            //Bắt đầu kết nối
            // await _hubConnection.StartAsync();

            //await getConnectionID();
        }
        // Hàm kiểm tra trạng thái kết nối
        public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;
        public async Task ConnectAsync()
        {
            if (!IsConnected)
            {
                try
                {
                    await _hubConnection.StartAsync();
                    
                    Console.WriteLine("Kết nối thành công đến SignalR.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Kết nối thất bại: {ex.Message}");
                }
            }
        }

        // Ngắt kết nối SignalR
        public async Task DisconnectAsync()
        {
            if (_hubConnection.State != HubConnectionState.Disconnected)
            {
                await _hubConnection.StopAsync();
            }
        }

        // Hàm xử lý khi mất kết nối
        private async Task OnClosedAsync(Exception? error)
        {
            Console.WriteLine($"Kết nối bị mất: {error?.Message}. Đang thử lại sau 60 giây...");

            // Chờ 60 giây trước khi thử kết nối lại
            await Task.Delay(60000);

            try
            {
                await ConnectAsync();
                Console.WriteLine("Đã kết nối lại thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kết nối lại thất bại: {ex.Message}");
            }
        }
        public  async Task<bool> SendMsg(JsonMsgAndroid jsonMsgAndroid)
        {
            try
            {
                jsonMsgAndroid.clientid = ConnectionID;
                jsonMsgAndroid.usersend=ModelAdmin.users.UsersName;
               
                await _hubConnection.SendAsync("SendMsgJsonHub", jsonMsgAndroid.ToJson());
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return false;
            }
           
          
        }
        public  async Task getConnectionID()
        {
            JsonMsgAndroid jsonMsgAndroid = new JsonMsgAndroid();
            jsonMsgAndroid.typemsg = TypemsgAPI.getconnectionid.ToString();
            await _hubConnection.SendAsync("SendMsgJsonHub", jsonMsgAndroid.ToJson());
        }
    }
}
