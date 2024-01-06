using System;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using System.IO;
using System.Drawing;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace webiconsetter
{
    class MainWindow : Window
    {
        [UI] private Label _label1 = null;
        [UI] private Button _button1 = null;
        [UI] private Entry emailran = null;
        [UI] private ComboBoxText iconsource = null;
        [UI] private Image _thum = null;
        private string account = "";

        public MainWindow() : this(new Builder("MainWindow.glade")) { }
        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);
            DeleteEvent += Window_DeleteEvent;
            _button1.Clicked += Button1_Clicked;
            _button1.Label = "設定";
            iconsource.Active = 0;
           // byte[] epreview = File.WriteAllBytes(@"~/.face");
           // var image = new Gdk.Pixbuf(epreview);
           // _thum.Pixbuf = image;

        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void Button1_Clicked(object sender, EventArgs a)
        {
            //もし選択肢が０だったら
            if(iconsource.Active == 0){
                iconset_gravater(account);
            }

        }
        private void changed(object sender, EventArgs a)
        {
            account = emailran.Text;
        }
        private async void iconset_gravater(string account)
        {
            _label1.Text = "Gravater アイコンを同期中...";
  
            // メールアドレスをmd5ハッシュ化
            account = account.ToLower();
            Console.WriteLine(account);
            string md5ed = CalculateMD5Hash(account);
            // http://www.gravatar.com/avatar/{ここにmd5ハッシュ}をget
            var getavatar = new HttpClient();
            byte[] kekka = await getavatar.GetByteArrayAsync("https://www.gravatar.com/avatar/" + md5ed + "?s=256");
            File.WriteAllBytes(".face", kekka);
            await Task.Delay(1000);
            //プレビュー用に 196x196 の pixbuf も生成し表示
            byte[] preview = await getavatar.GetByteArrayAsync("https://www.gravatar.com/avatar/" + md5ed + "?s=196");
            var image = new Gdk.Pixbuf(preview);
            _thum.Pixbuf = image;
            _label1.Text = "同期しました！";
        }
        public static string CalculateMD5Hash(string inputstring)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(inputstring);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
