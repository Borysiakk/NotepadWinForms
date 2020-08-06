using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private int _countTabPage;
        private Image _closeImage;
        private Point _imageLocation = new Point(20, 4);
        private Point _imgHitArea = new Point(20, 4);
       

        public Form1()
        {
            InitializeComponent();
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Image img = new Bitmap(_closeImage);
            Rectangle r = e.Bounds;
            r = this.tabControl1.GetTabRect(e.Index);
            r.Offset(2, 2);
            Brush TitleBrush = new SolidBrush(Color.Black);
            Font f = this.Font;
            string title = this.tabControl1.TabPages[e.Index].Text;
            e.Graphics.DrawString(title, f, TitleBrush, new PointF(r.X, r.Y));
            e.Graphics.DrawImage(img, new Point(r.X + (this.tabControl1.GetTabRect(e.Index).Width - _imageLocation.X), _imageLocation.Y));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _countTabPage = 0;
            tabControl1.Padding = new Point(20, 4);
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;

            _closeImage = WindowsFormsApp1.Properties.Resources.Close;
        }

        private void nowyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage page = new TabPage();
            page.Text = "Nowy " + _countTabPage.ToString();
            tabControl1.Controls.Add(page);


            RichTextBox richTextBox = new RichTextBox();
            richTextBox.Dock = DockStyle.Fill;
            page.Controls.Add(richTextBox);

            _countTabPage++;

            tabControl1.SelectTab(page);
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            TabControl tabControl = (TabControl)sender;
            Point p = e.Location;
            int _tabWidth = 0;
            _tabWidth = this.tabControl1.GetTabRect(tabControl.SelectedIndex).Width - (_imgHitArea.X);
            Rectangle r = this.tabControl1.GetTabRect(tabControl.SelectedIndex);
            r.Offset(_tabWidth, _imgHitArea.Y);
            r.Width = 16;
            r.Height = 16;
            if (tabControl1.SelectedIndex >= 0)
            {
                if (r.Contains(p))
                {
                    TabPage tabPage = (TabPage)tabControl.TabPages[tabControl.SelectedIndex];
                    tabControl.TabPages.Remove(tabPage);
                }
            }
        }

        private void wyjśćieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void zapiszToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(tabControl1.TabCount >= 0)
            {
                TabPage tabPage = (TabPage)tabControl1.TabPages[tabControl1.SelectedIndex];
                RichTextBox richTextBox = (RichTextBox)tabPage.Controls[0];
                   
                if(tabPage.Tag != null)
                {
                    using (FileStream fileStream = new FileStream((string)tabPage.Tag,FileMode.Open))
                    {
                        var bs = Encoding.UTF8.GetBytes(richTextBox.Text);
                        await fileStream.WriteAsync(bs, 0, bs.Length);
                        fileStream.Close();
                    }
                }
                else
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Plik Tekstowy (*.txt)|*.txt|All files (*.*)|*.*";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        using(FileStream fileStream = (FileStream)saveFileDialog.OpenFile())
                        {
                            var bs = Encoding.UTF8.GetBytes(richTextBox.Text);
                            await fileStream.WriteAsync(bs, 0, bs.Length);
                            fileStream.Close();
                        }

                        tabPage.Tag = saveFileDialog.FileName;
                        tabPage.Text = Path.GetFileName((string)tabPage.Tag);
                       
                    }
                }
            }
            else
            {
                MessageBox.Show("Prosze utworzyć plik ");
            }


        }

        private async void otwórzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[] result;
            TabPage tabPage = new TabPage();
            tabControl1.Controls.Add(tabPage);


            RichTextBox richTextBox = new RichTextBox();
            richTextBox.Dock = DockStyle.Fill;
            tabPage.Controls.Add(richTextBox);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Plik Tekstowy (*.txt)|*.txt|All files (*.*)|*.*";

            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using(FileStream fileStream = (FileStream)openFileDialog.OpenFile())
                {
                    result = new byte[fileStream.Length];
                    await fileStream.ReadAsync(result, 0, (int)fileStream.Length);
                }
                richTextBox.Text = Encoding.UTF8.GetString(result);
            }

            tabPage.Tag = openFileDialog.FileName;
            tabPage.Text = Path.GetFileName((string)tabPage.Tag);
            tabControl1.SelectTab(tabPage);
        }
    }
}
