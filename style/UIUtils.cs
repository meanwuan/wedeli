using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public static class UIUtils
{
    private const int EM_SETMARGINS = 0xD3;
    private const int EC_LEFTMARGIN = 0x1;
    private const int EC_RIGHTMARGIN = 0x2;

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Thiết lập style cho TextBox với chiều cao tùy chỉnh và một giá trị padding duy nhất.
    /// </summary>
    /// <remarks>
    /// Padding trên/dưới được giả lập bằng cách cung cấp một chiều cao (height) lớn hơn chiều cao của font chữ.
    /// Padding trái/phải được thiết lập tường minh bằng giá trị padding.
    /// </remarks>
    /// <param name="textBox">TextBox cần chỉnh.</param>
    /// <param name="height">Chiều cao tùy chỉnh mong muốn.</param>
    /// <param name="padding">Khoảng đệm cho trái và phải.</param>
    public static void SetStyle(TextBox textBox, int height, int padding)
    {
        if (textBox == null) return;

        // 1. Bắt buộc phải là Multiline để có thể tùy chỉnh chiều cao
        textBox.Multiline = true;

        // 2. Thiết lập chiều cao tùy chỉnh do người dùng cung cấp
        textBox.Height = height;

        // 3. Đặt padding cho trái và phải bằng giá trị padding được cung cấp
        SetHorizontalPadding(textBox, padding, padding);
    }

    /// <summary>
    /// Phương thức riêng để đặt padding ngang, đảm bảo được gọi sau khi handle được tạo.
    /// </summary>
    private static void SetHorizontalPadding(TextBox textBox, int left, int right)
    {
        if (textBox.IsHandleCreated)
        {
            int lParam = (right << 16) | (left & 0xFFFF);
            SendMessage(textBox.Handle, EM_SETMARGINS, (IntPtr)(EC_LEFTMARGIN | EC_RIGHTMARGIN), (IntPtr)lParam);
        }
        else
        {
            textBox.HandleCreated += (sender, e) => {
                int lParam = (right << 16) | (left & 0xFFFF);
                SendMessage(textBox.Handle, EM_SETMARGINS, (IntPtr)(EC_LEFTMARGIN | EC_RIGHTMARGIN), (IntPtr)lParam);
            };
        }
    }
}