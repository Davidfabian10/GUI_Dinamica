using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GUI_Dinamica
{
    public partial class Form1 : Form
    {
        private Button btnAddImages;
        private Label lblFolderPath;
        private TextBox txtFolderPath;
        private List<PictureBox> dynamicPictureBoxes = new List<PictureBox>();
        private int imageCounter = 0;
        private Panel panelImages;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Configuracion del Form
            this.Text = "Galeria";
            this.WindowState = FormWindowState.Maximized;

            // Crear Label para introducir la ruta 
            lblFolderPath = new Label { Text = "Ruta de la carpeta:", Location = new Point(20, 20), AutoSize = true };
            this.Controls.Add(lblFolderPath);

            // Crear TextBox para ingresar la ruta de la carpeta
            txtFolderPath = new TextBox { Size = new Size(400, 20), Location = new Point(20, 50) };
            this.Controls.Add(txtFolderPath);

            // Crear Button 
            this.Controls.Add(CreateButton("Agregar Imagen", new Point(500, 25), AddImages));
            this.Controls.Add(CreateButton("Eliminar Imagen", new Point(700, 25), RemoveImages));
            this.Controls.Add(CreateButton("Limpiar Galeria", new Point(900, 25), ClearImages));
            this.Controls.Add(CreateButton("Cerrar Aplicacion", new Point(1100, 25), (s, ev) => this.Close()));

            // Crear Panel para contener las images
            panelImages = new Panel { AutoScroll = true, Size = new Size(this.ClientSize.Width - 40, this.ClientSize.Height - 160), Location = new Point(20, 100), BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(panelImages);

        }

        private Button CreateButton(string text, Point location, EventHandler clickHandler)
        {
            var button = new Button
            {
                Text = text,
                Location = location,
                Size = new Size(150, 50),
                Font = new Font("Verdana", 9, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            button.Click += clickHandler;
            return button;
        }

        private void AddImages(object sender, EventArgs e)
        {
            // Obtener la ruta ingresada
            string imageFolderPath = txtFolderPath.Text.Trim();
            if (string.IsNullOrWhiteSpace(imageFolderPath) || !Directory.Exists(imageFolderPath))
            {
                MessageBox.Show("Ingrese una ruta valida", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Obtener archivos de imagen en la carpeta
            string[] imageFiles = Directory.GetFiles(imageFolderPath, "*.jpg")
                .Concat(Directory.GetFiles(imageFolderPath, "*.jpeg"))
                .Concat(Directory.GetFiles(imageFolderPath, "*.png"))
                .Concat(Directory.GetFiles(imageFolderPath, "*.bmp"))
                .ToArray();

            if (imageCounter >= imageFiles.Length)
            {
                MessageBox.Show("No hay mas imagenes en la carpeta", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener la siguiente imagen
            string imagePath = imageFiles[imageCounter];
            int imageSize = 130;
            int padding = 10;
            int rows = panelImages.Height / (imageSize + padding);
            if (rows < 1) rows = 1;

            int col = dynamicPictureBoxes.Count / rows;
            int row = dynamicPictureBoxes.Count % rows;

            // Crear PictureBox 
            PictureBox newPictureBox = new PictureBox
            {
                Image = new Bitmap(imagePath),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(imageSize, imageSize),
                Location = new Point(col * (imageSize + padding), row * (imageSize + padding)),
                Tag = imagePath
            };
            newPictureBox.Click += OpenFullScreenImage;

            // Agregar el PictureBox al Panel
            dynamicPictureBoxes.Add(newPictureBox);
            panelImages.Controls.Add(newPictureBox);
            imageCounter++;
        }

        private void OpenFullScreenImage(object sender, EventArgs e)
        {
            PictureBox clickedPictureBox = sender as PictureBox;
            if (clickedPictureBox == null || clickedPictureBox.Tag == null) return;

            // Obtener la ruta y el nombre de la imagen
            string imagePath = clickedPictureBox.Tag.ToString();
            string imageName = Path.GetFileName(imagePath); 

            // Crear Form para la imagen
            Form imageForm = new Form
            {
                Text = imageName, 
                Size = new Size(800, 600), 
                StartPosition = FormStartPosition.CenterScreen, 
                FormBorderStyle = FormBorderStyle.FixedDialog,
                BackColor = Color.Black
            };

            // Crear PictureBox para mostrar la imagen en la ventana
            PictureBox fullScreenPictureBox = new PictureBox
            {
                Image = new Bitmap(imagePath),
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Fill
            };
            fullScreenPictureBox.Click += (s, ev) => imageForm.Close();

            // Agregar PictureBox al formulario y mostrarlo
            imageForm.Controls.Add(fullScreenPictureBox);
            imageForm.ShowDialog();
        }

        private void RemoveImages(object sender, EventArgs e)
        {
            if (dynamicPictureBoxes.Count == 0)
            {
                MessageBox.Show("No hay mas imagenes para eliminar", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Eliminar la imagen del Panel
            PictureBox lastPictureBox = dynamicPictureBoxes[^1];
            panelImages.Controls.Remove(lastPictureBox);
            dynamicPictureBoxes.RemoveAt(dynamicPictureBoxes.Count - 1);
            lastPictureBox.Dispose();
            if (imageCounter > 0) imageCounter--;
        }

        private void ClearImages(object sender, EventArgs e)
        {
            // Verificar si esta vacia
            if (dynamicPictureBoxes.Count == 0)
            {
                MessageBox.Show("La galeria esta vacia", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (PictureBox pictureBox in dynamicPictureBoxes)
            {
                panelImages.Controls.Remove(pictureBox);
                pictureBox.Dispose();
            }
            dynamicPictureBoxes.Clear();
            imageCounter = 0;
        }
    }
}