namespace MMS
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            menuStrip1 = new MenuStrip();
            slikaToolStripMenuItem = new ToolStripMenuItem();
            ucitajToolStripMenuItem = new ToolStripMenuItem();
            kompresijaToolStripMenuItem = new ToolStripMenuItem();
            kompresujToolStripMenuItem = new ToolStripMenuItem();
            filterToolStripMenuItem = new ToolStripMenuItem();
            burkesDitheringToolStripMenuItem = new ToolStripMenuItem();
            invertToolStripMenuItem = new ToolStripMenuItem();
            kuwaharaFilterToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(12, 31);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(291, 254);
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { slikaToolStripMenuItem, kompresijaToolStripMenuItem, filterToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 28);
            menuStrip1.TabIndex = 3;
            menuStrip1.Text = "menuStrip1";
            // 
            // slikaToolStripMenuItem
            // 
            slikaToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { ucitajToolStripMenuItem });
            slikaToolStripMenuItem.Name = "slikaToolStripMenuItem";
            slikaToolStripMenuItem.Size = new Size(54, 24);
            slikaToolStripMenuItem.Text = "Slika";
            // 
            // ucitajToolStripMenuItem
            // 
            ucitajToolStripMenuItem.Name = "ucitajToolStripMenuItem";
            ucitajToolStripMenuItem.Size = new Size(224, 26);
            ucitajToolStripMenuItem.Text = "Ucitaj";
            ucitajToolStripMenuItem.Click += ucitajToolStripMenuItem_Click;
            // 
            // kompresijaToolStripMenuItem
            // 
            kompresijaToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { kompresujToolStripMenuItem });
            kompresijaToolStripMenuItem.Name = "kompresijaToolStripMenuItem";
            kompresijaToolStripMenuItem.Size = new Size(98, 24);
            kompresijaToolStripMenuItem.Text = "Kompresija";
            // 
            // kompresujToolStripMenuItem
            // 
            kompresujToolStripMenuItem.Name = "kompresujToolStripMenuItem";
            kompresujToolStripMenuItem.Size = new Size(163, 26);
            kompresujToolStripMenuItem.Text = "Kompresuj";
            kompresujToolStripMenuItem.Click += kompresujToolStripMenuItem_Click;
            // 
            // filterToolStripMenuItem
            // 
            filterToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { burkesDitheringToolStripMenuItem, invertToolStripMenuItem, kuwaharaFilterToolStripMenuItem });
            filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            filterToolStripMenuItem.Size = new Size(56, 24);
            filterToolStripMenuItem.Text = "Filter";
            // 
            // burkesDitheringToolStripMenuItem
            // 
            burkesDitheringToolStripMenuItem.Name = "burkesDitheringToolStripMenuItem";
            burkesDitheringToolStripMenuItem.Size = new Size(201, 26);
            burkesDitheringToolStripMenuItem.Text = "Burkes Dithering";
            burkesDitheringToolStripMenuItem.Click += burkesDitheringToolStripMenuItem_Click;
            // 
            // invertToolStripMenuItem
            // 
            invertToolStripMenuItem.Name = "invertToolStripMenuItem";
            invertToolStripMenuItem.Size = new Size(201, 26);
            invertToolStripMenuItem.Text = "Invert";
            invertToolStripMenuItem.Click += invertToolStripMenuItem_Click;
            // 
            // kuwaharaFilterToolStripMenuItem
            // 
            kuwaharaFilterToolStripMenuItem.Name = "kuwaharaFilterToolStripMenuItem";
            kuwaharaFilterToolStripMenuItem.Size = new Size(201, 26);
            kuwaharaFilterToolStripMenuItem.Text = "Kuwahara filter";
            kuwaharaFilterToolStripMenuItem.Click += kuwaharaFilterToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pictureBox1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Obrada slike";
            Load += Form1_Load_1;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem slikaToolStripMenuItem;
        private ToolStripMenuItem ucitajToolStripMenuItem;
        private ToolStripMenuItem sacuvajToolStripMenuItem;
        private ToolStripMenuItem kompresijaToolStripMenuItem;
        private ToolStripMenuItem kompresujToolStripMenuItem;
        private ToolStripMenuItem filterToolStripMenuItem;
        private ToolStripMenuItem burkesDitheringToolStripMenuItem;
        private ToolStripMenuItem invertToolStripMenuItem;
        private ToolStripMenuItem kuwaharaFilterToolStripMenuItem;
    }
}
