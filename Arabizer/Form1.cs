using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Arabizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static string filename = "";
        long vLineNumber = 0;
        long vLineNumberE = 0;
        int isFinished = 0;
        FileStream bytes;
        FileStream outputStream;
        void NewThread()
        {
            /*byte[] bytes = File.ReadAllBytes(filename);
            string img = System.Text.Encoding.UTF8.GetString(bytes);
            img.Replace(";zh_CN;", ";ar_SA;");

            FileStream fs = new FileStream(filename, FileMode.Create);
            StreamWriter w = new StreamWriter(fs, Encoding.UTF8);
            w.WriteLine(img);
            w.Flush();
            w.Close();
            fs.Close();*/
            outputStream = File.OpenWrite(filename+"AR.img");
            bytes = File.OpenRead(filename);
            ReplaceFile(bytes, outputStream);
            isFinished = 1;
    }
        string g = "0";
        protected void ReplaceFile(FileStream FilePath, FileStream NewFilePath)
        {
            using (BinaryReader vReader = new BinaryReader(FilePath))
            {
                using (BinaryWriter vWriter = new BinaryWriter (NewFilePath))
                {
                    vLineNumberE = vReader.BaseStream.Length;
                    while (vReader.BaseStream.Position != vReader.BaseStream.Length)
                    {
                        byte[] vLine = vReader.ReadBytes(20480);
                        string result = BitConverter.ToString(vLine);
                        if (result.Contains("7A-68-5F-43-4E"))
                        {
                            g = result;
                            //byte[] b1 = Encoding.GetEncoding("iso-8859-1").GetBytes(ReplaceLine(result));
                            byte[] b1 = HexStringToBytes(ReplaceLine(result));
                            vWriter.Write(b1);
                            vLineNumber = vReader.BaseStream.Position;
                        }
                        else
                            vWriter.Write(vLine);
                    }
                }
            }
        }
        protected string ReplaceLine(string Line)
        {
                g = Line;
            Line.Replace("7A-68-5F-43-4E", "61-72-5F-53-41");
            return Line;
        }


        public static byte[] HexStringToBytes(string s)
        {
            const string HEX_CHARS = "0123456789ABCDEF";

            if (s.Length == 0)
                return new byte[0];

            if ((s.Length + 1) % 3 != 0)
                throw new FormatException();

            byte[] bytes = new byte[(s.Length + 1) / 3];

            int state = 0; // 0 = expect first digit, 1 = expect second digit, 2 = expect hyphen
            int currentByte = 0;
            int x;
            int value = 0;

            foreach (char c in s)
            {
                switch (state)
                {
                    case 0:
                        x = HEX_CHARS.IndexOf(Char.ToUpperInvariant(c));
                        if (x == -1)
                            throw new FormatException();
                        value = x << 4;
                        state = 1;
                        break;
                    case 1:
                        x = HEX_CHARS.IndexOf(Char.ToUpperInvariant(c));
                        if (x == -1)
                            throw new FormatException();
                        bytes[currentByte++] = (byte)(value + x);
                        state = 2;
                        break;
                    case 2:
                        if (c != '-')
                            throw new FormatException();
                        state = 0;
                        break;
                }
            }

            return bytes;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = @"C:\";
            openFileDialog1.Title = "Browse System.img File";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.DefaultExt = "IMG";
            openFileDialog1.Filter = "IMG files (*.img)|*.img";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.ReadOnlyChecked = true;
            openFileDialog1.ShowReadOnly = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
             {
                filename = openFileDialog1.FileName;
                Thread th = new Thread(NewThread);
                th.Start();
                while(isFinished == 0)
                {
                    label1.Text = "Arabizing ..." + (vLineNumberE-vLineNumber) + " blocks remainig";
                    if (g != "0")
                    {
                        textBox1.AppendText(g);
                        textBox1.AppendText(Environment.NewLine);
                    }
                    Application.DoEvents();
                    g = "0";
                }
                label1.Text = "Finished :)";
            }
            }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.ScrollBars = ScrollBars.Vertical;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
