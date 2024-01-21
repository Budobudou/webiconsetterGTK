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
        [UI] private Label _label2 = null;
        [UI] private Button _button1 = null;
        [UI] private Entry emailran = null;
        [UI] private ComboBoxText iconsource = null;
        [UI] private Image _thum = null;
        private string account = "";
        private string version = "1.0";
        private License license = License.Gpl30;

        private string homedir = "/home/" + Environment.UserName;
        public MainWindow() : this(new Builder("MainWindow.glade")) { }
        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);
            DeleteEvent += Window_DeleteEvent;
            _button1.Clicked += Button1_Clicked;
            _button1.Label = "設定";
            iconsource.Active = 0;
            // ホームディレクトリ✅
            if(Environment.UserName == "root")
            {
                homedir = "/root/";
            }
            var combined = System.IO.Path.Combine(homedir);
            //ホームディレクトリ直下に .face があるか
            try{
            byte[] epreview = File.ReadAllBytes(@combined + "/.face");
            var image = new Gdk.Pixbuf(epreview,196,196);
            _thum.Pixbuf = image;
            } catch(Exception e){
            byte[] epreview = File.ReadAllBytes("/usr/share/webiconsetter/black196.png");
            var image = new Gdk.Pixbuf(epreview,196,196);
            _thum.Pixbuf = image;
            }


        }
        // 修了関連
        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }
        private void Quit_activate(object sender, EventArgs a)
        {
            Application.Quit();
        }
        private void iconsource_changed_cb(object sender, EventArgs a)
        {
            if(iconsource.Active == 0)
            {
                Console.WriteLine("gravatarに変更");
               _label1.Text = "オンライン上のシステムからアイコンを設定します";
               // _label2.Text = "Gravatar アカウントのメールアドレス";
               // emailran.PlaceholderText = "example@example.com";
                _button1.Visible = true;
                _label2.Visible = true;
                emailran.Visible = true;

            }else if(iconsource.Active == 1){
                Console.WriteLine("ローカル参照に変更");
                _label1.Text = "ローカルからアイコンを設定します";
                //_label2.Text = "アイコン画像へのファイルパス";
                //emailran.PlaceholderText = "/home/hoge/ドキュメント/hoge.png";
                _button1.Visible = false;
                _label2.Visible = false;
                emailran.Visible = false;
                var iconchoose = new FileChooserDialog("正方形のアイコン画像を選択",null,FileChooserAction.Open,"やめる", ResponseType.Cancel,"開く", ResponseType.Accept);
                // 画像しか使えないように
                var fileFilter = new FileFilter();
                fileFilter.Name = "JPEG/PNG/GIF の画像";
                fileFilter.AddMimeType("image/png");
                fileFilter.AddMimeType("image/jpeg");
                fileFilter.AddMimeType("image/gif");
                fileFilter.AddMimeType("image/svg+xml");
                iconchoose.AddFilter(fileFilter);
                var response = (ResponseType)iconchoose.Run();
                if (response == ResponseType.Accept)
                {
                    var account = iconchoose.Filename;
                    Console.WriteLine(account);
                    iconset_local(account);
                }
                else if (response == ResponseType.Cancel)
                {
                }
                iconchoose.Destroy();

            }
        }
        private void Button1_Clicked(object sender, EventArgs a)
        {
            //もし選択肢が０だったら
            if(account == ""){
                _label1.Text = "まず最初に画像のソースを設定します。";
            }
            else{
            if(iconsource.Active == 0){
                iconset_gravatar(account);
            }
            else if(iconsource.Active == 1){
                iconset_local(account);
            }
            }
        }
        private async void iconset_local(string account)
        {
            var combined = System.IO.Path.Combine(homedir);
            File.Delete(@combined + "/.face");
            File.Copy(@account,@combined + "/.face");
            _label1.Text = "ローカルアイコンを設定しました。";
            try{
                byte[] epreview = File.ReadAllBytes(@combined + "/.face");
                var image = new Gdk.Pixbuf(epreview,196,196);
                _thum.Pixbuf = image;
            } 
            catch(Exception e)
            {
                byte[] epreview = File.ReadAllBytes("/usr/share/webiconsetter/black196.png");
                var image = new Gdk.Pixbuf(epreview,196,196);
                _thum.Pixbuf = image;
            }
        }
        private void changed(object sender, EventArgs a)
        {
            account = emailran.Text;
        }
        private async void opendialog(object sender, EventArgs a)
        {
            // about ダイアログ表示
            AboutDialog aboutdialog = new AboutDialog();
            aboutdialog.ProgramName = "WebIconSetter";
            aboutdialog.Version = version;
            byte[] epreview = File.ReadAllBytes("/usr/share/webiconsetter/logo.png");
            var image = new Gdk.Pixbuf(epreview,128,128);
            aboutdialog.Logo = image;
            aboutdialog.Title = "WebIconSetter";
            aboutdialog.Comments = ".face アイコンをウェブ上のシステムから設定します";
            aboutdialog.Copyright = "(C)2024 Budobudou";
            aboutdialog.Website = "https://github.com/Budobudou/webiconsetterGTK";
            aboutdialog.WebsiteLabel = "リポジトリ";
            aboutdialog.LicenseType = license;
            aboutdialog.Run();
            aboutdialog.Destroy();
        }
        private async void iconset_gravatar(string account)
        {
            _label1.Text = "Gravatar アイコンを同期中...";
  
            // メールアドレスをmd5ハッシュ化
            account = account.ToLower();
            Console.WriteLine(account);
            string md5ed = CalculateMD5Hash(account);
            // http://www.gravatar.com/avatar/{ここにmd5ハッシュ}をget
            var getavatar = new HttpClient();
            try{
            byte[] kekka = await getavatar.GetByteArrayAsync("https://www.gravatar.com/avatar/" + md5ed + "?s=256");
            var combined = System.IO.Path.Combine(homedir);
            File.WriteAllBytes(@combined + "/.face", kekka);
            await Task.Delay(250);
            //プレビュー用に 196x196 の pixbuf も生成し表示
            var image = new Gdk.Pixbuf(kekka,196,196);
            _thum.Pixbuf = image;
            _label1.Text = "同期しました！";
            }catch(System.Net.Http.HttpRequestException){
            _label1.Text = "同期に失敗しました。";
            }

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
