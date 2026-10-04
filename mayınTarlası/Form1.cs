using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mayınTarlası
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            OyunuBaslat();
        }
        // Sabitler
        private const int SATIR = 10;
        private const int SUTUN = 10;
        private const int MAYIN_SAYISI = 10;
        private const int BUTON_BOYUTU = 40;

        // Oyun Değişkenleri
        private bool[,] bayrakKoyuldu = new bool[SATIR, SUTUN];
        private Button[,] butonlar = new Button[SATIR, SUTUN];
        private bool[,] mayinlar = new bool[SATIR, SUTUN];
        private bool[,] acildi = new bool[SATIR, SUTUN];
        private int acilanKutuSayisi = 0;
        private void OyunuBaslat()
        {
            // Form Boyutlarını Ayarla
            this.ClientSize = new Size(SUTUN * BUTON_BOYUTU, SATIR * BUTON_BOYUTU);
            this.Text = "Mayın Tarlası (10x10)";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // 1. Grid (10x10 Butonlar) Oluşturma
            for (int r = 0; r < SATIR; r++)
            {
                for (int c = 0; c < SUTUN; c++)
                {
                    Button btn = new Button
                    {
                        Size = new Size(BUTON_BOYUTU, BUTON_BOYUTU),
                        Location = new Point(c * BUTON_BOYUTU, r * BUTON_BOYUTU),
                        Tag = new Point(r, c), // Butonun matristeki konumunu tutuyoruz
                        Font = new Font("Arial", 10, FontStyle.Bold),
                        BackColor = Color.LightGray
                    };

                    //btn.Click += Buton_Click;
                    btn.MouseDown += Buton_MouseDown;
                    this.Controls.Add(btn);
                    butonlar[r, c] = btn;
                }
            }

            // 2. Rastgele 10 Mayın Yerleştirme
            MayinlariYerlestir();
        }
        private void MayinlariYerlestir()
        {
            Random rnd = new Random();
            int yerlestirilen = 0;

            while (yerlestirilen < MAYIN_SAYISI)
            {
                int r = rnd.Next(SATIR);
                int c = rnd.Next(SUTUN);

                if (!mayinlar[r, c])
                {
                    mayinlar[r, c] = true;
                    yerlestirilen++;
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void Buton_Click(object sender, EventArgs e)
        {
            Button tiklanan = (Button)sender;
            Point p = (Point)tiklanan.Tag;
            int r = p.X;
            int c = p.Y;

            // Zaten açılmış bir kutuya tekrar tıklanırsa işlem yapma
            if (acildi[r, c]) return;

            // Mayına tıklandıysa
            if (mayinlar[r, c])
            {
                tiklanan.BackColor = Color.Red;
                tiklanan.Text = "💣";
                TumMayinlariGoster();
                MessageBox.Show("Kaybettiniz! Mayına bastınız.", "Oyun Bitti", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Restart(); // Oyunu yeniden başlat
                return;
            }

            // Mayın değilse kutuyu aç
            KutuyuAc(r, c);

            // Kazanma kontrolü (Mayın olmayan tüm alanlar açıldı mı?)
            if (acilanKutuSayisi == (SATIR * SUTUN) - MAYIN_SAYISI)
            {
                MessageBox.Show("Tebrikler! Tüm mayınsız alanları buldunuz.", "Oyun Kazanıldı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Restart();
            }
        }
        private void Buton_MouseDown(object sender, MouseEventArgs e)
        {
            Button tiklanan = (Button)sender;
            Point p = (Point)tiklanan.Tag;
            int r = p.X;
            int c = p.Y;

            if (acildi[r, c]) return;

            // SAĞ TIK: Bayrak Koy / Kaldır
            if (e.Button == MouseButtons.Right)
            {
                if (!bayrakKoyuldu[r, c])
                {
                    bayrakKoyuldu[r, c] = true;
                    tiklanan.Text = "🚩";
                    tiklanan.ForeColor = Color.DarkRed;
                }
                else
                {
                    bayrakKoyuldu[r, c] = false;
                    tiklanan.Text = "";
                }
                return;
            }

            // SOL TIK: Kutuyu Aç
            if (e.Button == MouseButtons.Left)
            {
                // Bayrak koyulmuş kutunun açılmasını engelle
                if (bayrakKoyuldu[r, c]) return;

                if (mayinlar[r, c])
                {
                    tiklanan.BackColor = Color.Red;
                    tiklanan.Text = "💣";
                    TumMayinlariGoster();
                    MessageBox.Show("Kaybettiniz! Mayına bastınız.", "Oyun Bitti", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Restart();
                    return;
                }

                KutuyuAc(r, c);

                if (acilanKutuSayisi == (SATIR * SUTUN) - MAYIN_SAYISI)
                {
                    MessageBox.Show("Tebrikler! Tüm mayınsız alanları buldunuz.", "Oyun Kazanıldı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Restart();
                }
            }
        }

        private void KutuyuAc(int r, int c)
        {
            if (r < 0 || r >= SATIR || c < 0 || c >= SUTUN || acildi[r, c] || bayrakKoyuldu[r, c]) return;

            acildi[r, c] = true;
            acilanKutuSayisi++;

            butonlar[r, c].BackColor = Color.White;
            butonlar[r, c].Enabled = false;

            int komsuMayin = KomsuMayinSayisi(r, c);

            if (komsuMayin > 0)
            {
                butonlar[r, c].Text = komsuMayin.ToString();
                butonlar[r, c].ForeColor = SayiRengiAl(komsuMayin);
            }
            else
            {
                for (int dr = -1; dr <= 1; dr++)
                {
                    for (int dc = -1; dc <= 1; dc++)
                    {
                        if (dr == 0 && dc == 0) continue;
                        KutuyuAc(r + dr, c + dc);
                    }
                }
            }
        }

        private int KomsuMayinSayisi(int r, int c)
        {
            int sayac = 0;

            // 8 komşu yönü kontrol et
            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    int nr = r + dr;
                    int nc = c + dc;

                    if (nr >= 0 && nr < SATIR && nc >= 0 && nc < SUTUN)
                    {
                        if (mayinlar[nr, nc]) sayac++;
                    }
                }
            }

            return sayac;
        }

        private Color SayiRengiAl(int sayi)
        {
            switch (sayi)
            {
                case 1: return Color.Blue;
                case 2: return Color.Green;
                case 3: return Color.Red;
                case 4: return Color.DarkBlue;
                default: return Color.DarkRed;
            }
        }

        private void TumMayinlariGoster()
        {
            for (int r = 0; r < SATIR; r++)
            {
                for (int c = 0; c < SUTUN; c++)
                {
                    if (mayinlar[r, c])
                    {
                        butonlar[r, c].BackColor = Color.Red;
                        butonlar[r, c].Text = "💣";
                    }
                }
            }
        }
    }
}

