using QuanLyBanHang.DAL;
using QuanLyBanHang.GUI;

namespace QuanLyBanHang
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            try
            {
            
                DBConnection.KhoiTaoCSDL();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không kết nối được SQL Server:\n" + ex.Message +
                    "\n\nKiểm tra chuỗi kết nối trong App.config.", "Lỗi CSDL",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.Run(new FrmMain());
        }
    }
}
