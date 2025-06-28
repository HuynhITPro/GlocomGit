namespace NFCWebBlazor.App_ClassDefine
{
    public class PopupService
    {

        private readonly Stack<string> _popupStack = new(); // Quản lý popup theo thứ tự mở

        public event Action<string>? OnClosePopup; // Sự kiện để thông báo đóng popup
        public event Action<string>? OnOpenPopup;  // Sự kiện khi mở popup

        // Đăng ký mở popup
        public void OpenPopup(string popupId)
        {
            _popupStack.Push(popupId);
            OnOpenPopup?.Invoke(popupId);
        }

        // Đóng popup gần nhất
        public void CloseTopPopup()
        {
            if (_popupStack.Any())
            {
                var popupId = _popupStack.Pop();
                OnClosePopup?.Invoke(popupId);
            }
        }

        // Kiểm tra nếu còn popup mở
        public bool HasPopup() => _popupStack.Any();

        // Đóng tất cả popup
        public void CloseAllPopups()
        {
            while (_popupStack.Any())
            {
                CloseTopPopup();
            }
        }
    }
}
