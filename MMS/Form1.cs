using System.Drawing;
namespace MMS
{
    public partial class Form1 : Form
    {
        Obrada? slika;
        string? filename;
        public Form1()
        {
            InitializeComponent();
            this.AutoSize = true;
            pictureBox1.Width = this.Width;
            pictureBox1.Height = this.Height;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void ucitajToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "Image(.bmp,.jpg)|*.bmp;*.jpg;*.photo";

            if (DialogResult.OK == d.ShowDialog())
            {
                String url = d.FileName;


                slika = new Obrada(url);
                Bitmap bmp = slika.GetOriginal();
                pictureBox1.Image = bmp;
                var height = Math.Min(bmp.Height, 1024);
                var width = Math.Min(bmp.Width, 1024);
                pictureBox1.Height = height;
                pictureBox1.Width = width;
                this.Height = height;
                this.Width = width;

            }
            d.Dispose();

        }

        private void kompresujToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (slika != null)
            {
                slika.CompressFile(false);
            }
        }

        private void invertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (slika != null)
            {
                Bitmap img = slika.GetOriginal();
                img = Filter.Invert(img);
                pictureBox1.Image = img;
                slika.SetOriginal(img);
            }
        }

        private void burkesDitheringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (slika != null)
            {
                Bitmap img = slika.GetOriginal();
                img = Filter.BurkesDithering(img);
                pictureBox1.Image = img;
                slika.SetOriginal(img);

            }
            TestFilter.Test_BurkesOperator();
        }

        private void kuwaharaFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (slika != null)
            {
                Bitmap img = slika.GetOriginal();
                img = Filter.KuwaharaFilter(img);
                pictureBox1.Image = img;
                slika.SetOriginal(img);
            }

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            String folder_out = Environment.CurrentDirectory + "\\photos";
            if (!Directory.Exists(folder_out))
                Directory.CreateDirectory(folder_out);
        }
    }
}
