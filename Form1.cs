using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace LANSpeedChecker {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        Stopwatch sw;

        byte[] bin = new byte[65536];

        class Ent {
            public int r;
            public long t;

            public Ent(int r, long t) {
                this.r = r;
                this.t = t;
            }
        }

        List<Ent> al = new List<Ent>(100);

        private void bCheck_Click(object sender, EventArgs e) {
            HttpWebRequest r1 = (HttpWebRequest)HttpWebRequest.Create(tbURL.Text);
            r1.Method = "GET";
            r1.UserAgent = "LANSpeedChecker/0.1";

            al.Clear();

            int tot = 0;

            sw = new Stopwatch();
            sw.Start();
            using (HttpWebResponse r2 = (HttpWebResponse)r1.GetResponse()) {
                Stream si = r2.GetResponseStream();
                while (true) {
                    int r = si.Read(bin, 0, bin.Length);
                    if (r < 1) break;
                    al.Add(new Ent(r, sw.ElapsedMilliseconds));
                    tot += r;
                }
            }
            sw.Stop();

            StringWriter wr = new StringWriter();
            foreach (Ent ent in al) {
                wr.WriteLine(ent.t + " " + ent.r);
            }
            tbLOG.Text = wr.ToString();

            {
                int d = 10;
                int cx = (int)(sw.ElapsedMilliseconds / d);

                int sy = 100;

                int cy = 200;
                Bitmap pic = new Bitmap(cx + 1, cy + 1);
                using (Graphics cv = Graphics.FromImage(pic)) {
                    foreach (Ent ent in al) {
                        int x0 = (int)(ent.t / d);
                        int v = ent.r / sy;

                        cv.DrawLine(Pens.BlueViolet, x0, cy - v, x0, cy);
                    }
                    cv.DrawLine(Pens.Green, 0, cy, cx + 1, cy);
                }

                pictureBox1.Image = pic;
            }

            lStat.Text = String.Format("{0:#,##0}ミリ秒、{1:#,##0}バイト", sw.ElapsedMilliseconds, tot);
        }
    }
}
