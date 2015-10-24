using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace MapEdittor
{
    public partial class Form1 : Form
    {      
        public Form1()
        {   
            InitializeComponent();
            loadItem();           
        }

        private void btnLoadMap_Click(object sender, EventArgs e)
        {
            //Bitmap tileSet = Image.FromFile("tileset.png");
            OpenFileDialog f = new OpenFileDialog();
            f.Filter = "txt(*.txt) | *.txt";

            if (f.ShowDialog() == DialogResult.OK)
            {
                using (XmlReader reader = XmlReader.Create(f.FileName.ToString()))
                {
                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            // Get element name and switch on it.
                            switch (reader.Name)
                            {
                                case "map":
                                    // Detect this element.
                                    string attribute1 = reader["w"];
                                    if (attribute1 != null)
                                    {
                                        _mapWidth = int.Parse(attribute1);
                                    }

                                    string attribute3 = reader["h"];
                                    if (attribute1 != null)
                                    {
                                        _mapHeight = int.Parse(attribute3);
                                    }

                                    string attribute2 = reader["s"];

                                    if (attribute2 != null)
                                    {
                                        _mapSize = int.Parse(attribute2);
                                    }
                                    break;
                                case "tile":
                                    string attribute = reader["t"];
                                    if (attribute != null)
                                    {
                                        _listTileMap.Add(int.Parse(attribute));
                                        _set.Add(int.Parse(attribute));
                                    }
                                    break;
                            }
                        }
                    }
                }  //end using
                _listUniqueTile = _set.ToList();
             

                //load map           
                for (int j = 0; j <= _mapSize; j++)
                {

                    for (int i = 0; i < _listTileMap.Count(); i++)
                    {
                        int x = ((i % _mapWidth) * _tileSize) + (_mapWidth * _tileSize) * j;
                        int y = (i / _mapWidth) * _tileSize;
                        int index = _listUniqueTile.FindIndex(u => u == _listTileMap.ElementAt(i));

                        Rectangle r = new Rectangle { X = index * _tileSize, Y = 0, Width = _tileSize, Height = _tileSize };
                        Bitmap clone = _tileSet.Clone(r, _tileSet.PixelFormat);
                        _listTile4Drawing.Add(new Tile { _x = x, _y = y, _texture = clone });
                    }
                }


                //draw on screen
                //foreach (var item in _listTile4Drawing)
                //{
                //    PictureBox tile = new PictureBox();
                //    tile.Location = new System.Drawing.Point(item._x, item._y);
                //    tile.Size = new System.Drawing.Size(64, 64);
                //    tile.Image = item._texture;
                //    panelDesign.Controls.Add(tile);

                //}

                Image bg = makeMapBackground(_listTile4Drawing);
                pictureBox1.Image = bg;
            } //end if
        }

        private void btnLoadObject_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            if (f.ShowDialog() == DialogResult.OK)
            {
                _tileSet = new Bitmap(f.FileName.ToString());
            }
        }

        private void loadItem()
        {
            for (int i = 1; i <= 7; i++)
            {
                string tilePath = "tile/ground" + i + ".png";
                _listObject.Add(tilePath);
                PictureBox tile = new PictureBox();
                tile.Tag = i - 1;
                tile.SizeMode = PictureBoxSizeMode.AutoSize;
                tile.Image = new Bitmap(tilePath);
                tile.Click += Tile_Click;
                flowLayoutPanel1.Controls.Add(tile);
                
            }

        }

        private void Tile_Click(object sender, EventArgs e)
        {
            PictureBox tile = (PictureBox)sender;
            if (tile.BorderStyle == BorderStyle.None)
            {
                tile.BorderStyle = BorderStyle.FixedSingle;
                _editMode = true;
            }
            else
            {
                tile.BorderStyle = BorderStyle.None;
                _editMode = false;
            }
            _tileWidth = tile.Width - 2;
            _tileHeight = tile.Height - 2;
            _selectedObjectTag=(int)(((PictureBox)sender).Tag);
        }

        private void panelDesign_MouseMove(object sender, MouseEventArgs e)
        {
            //panelDesign.Invalidate();
            //_eX = e.X;
            //_eY = e.Y;

        }

        private void panelDesign_MouseLeave(object sender, EventArgs e)
        {
            //_mouseEdit = false;
            //panelDesign.Invalidate();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            
            
        }

        private void panelDesign_MouseEnter(object sender, EventArgs e)
        {
            //_mouseEdit = true;
        }

        private void panelDesign_Paint(object sender, PaintEventArgs e)
        {
            //if (_mouseEdit)
            //{
            //    Graphics g = e.Graphics;
            //    g.DrawRectangle(Pens.Red, _eX - 15, _eY - 15, _tileWidth, _tileHeight);
            //}
        }

        private Bitmap MergeTwoImages(Image firstImage, Image secondImage)
        {
            if (firstImage == null)
            {
                throw new ArgumentNullException("firstImage");
            }

            if (secondImage == null)
            {
                throw new ArgumentNullException("secondImage");
            }

            int outputImageWidth = firstImage.Width + secondImage.Width;

            int outputImageHeight = _tileSize;

            Bitmap outputImage = new Bitmap(outputImageWidth, outputImageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(outputImage))
            {
                graphics.DrawImage(firstImage, new Rectangle(new Point(), firstImage.Size),
                    new Rectangle(new Point(), firstImage.Size), GraphicsUnit.Pixel);
                graphics.DrawImage(secondImage, new Rectangle(new Point(firstImage.Width, 0), secondImage.Size),
                    new Rectangle(new Point(), secondImage.Size), GraphicsUnit.Pixel);
            }

            return outputImage;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            float[] dashValues = { 1, 1, 1, 1 };
            Pen pen = new Pen(Color.Red, 1);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            pen.DashPattern = dashValues;

            if (_mouseEdit && _editMode) 
            {
                Graphics g = e.Graphics;

                pen.Color = Color.Red;
                g.DrawRectangle(pen, _eX - 15, _eY - 15, _tileWidth, _tileHeight);
            }

            if (_enableGrid)
            {
                Graphics g = e.Graphics;
                int nRow = pictureBox1.Height / _gridSize;
                int nColumn = pictureBox1.Width / _gridSize;
                pen.Color = Color.Black;

                for (int i = 1; i <= nRow; i++)
                {
                    Point start = new Point(0, i * _gridSize);
                    Point end = new Point(pictureBox1.Width, i * _gridSize);
                    g.DrawLine(pen, start, end);
                }

                for (int i = 1; i <= nColumn; i++)
                {
                    Point start = new Point(i * _gridSize, 0);
                    Point end = new Point(i * _gridSize, pictureBox1.Height);
                    g.DrawLine(pen, start, end);
                }
            }
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            _mouseEdit = true;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            _mouseEdit = false;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            pictureBox1.Invalidate();
            _eX = e.X;
            _eY = e.Y;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                if (_mouseEdit && _editMode)
                {
                    _shiftKeyPressed = true;
                }
            }

            //delete placed tile object

            if (e.KeyCode == Keys.Delete)
            {
                if (_selectedTileToDelete != null)
                {
                    pictureBox1.Controls.Remove(_selectedTileToDelete);
                    _selectedTileToDelete = null;
                }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                if (_mouseEdit && _editMode)
                {
                    _shiftKeyPressed = false;
                }
            }
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!gridToolStripMenuItem.Checked) 
            {
                gridToolStripMenuItem.Checked = true;
                _enableGrid = true;
                pictureBox1.Invalidate();
            }
            else
            {
                gridToolStripMenuItem.Checked = false;
                _enableGrid = false;
                pictureBox1.Invalidate();
            }
        }

        private void value32toolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!value32toolStripMenuItem.Checked)
            {
                value32toolStripMenuItem.Checked= true;
                value64toolStripMenuItem.Checked = false;

                _gridSize = 32;
                pictureBox1.Invalidate();
            }
        }

        private void value64toolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!value64toolStripMenuItem.Checked)
            {
                value32toolStripMenuItem.Checked = false;
                value64toolStripMenuItem.Checked = true;

                _gridSize = 64;
                pictureBox1.Invalidate();
            }
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs ea = (MouseEventArgs)e;
            _posClickedX = ea.X;
            _posClickedY = ea.Y;

            if (_mouseEdit && _editMode)
            {

                PictureBox placedObject = new PictureBox();
                placedObject.SizeMode = PictureBoxSizeMode.AutoSize;
                Bitmap image = new Bitmap(_listObject.ElementAt(_selectedObjectTag));
                placedObject.Image = image;
                placedObject.Location = new Point(_posClickedX - image.Width / 2+1, _posClickedY - image.Height / 2+1);
                placedObject.Click += (object s, EventArgs ee) =>
                {
                    PictureBox p = (PictureBox)s;
                    _selectedTileToDelete = p;
                    if (p.BorderStyle == BorderStyle.None)
                    {
                        p.BorderStyle = BorderStyle.FixedSingle;
                    }
                    else
                    {
                        p.BorderStyle = BorderStyle.None;
                    }
                };
                _listPictureBox.Add(placedObject);
                pictureBox1.Controls.Add(placedObject);

                if (_shiftKeyPressed)
                {                    
                    var p = pictureBox1.PointToScreen(new Point(_posClickedX+_tileWidth, _posClickedY));
                    Cursor.Position = p;
                }
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var item in _listPictureBox)
            {
                pictureBox1.Controls.Remove(item);
            }
        }

        private void PlacedObject_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private Image makeMapBackground(List<Tile> listTile)
        {
            Image background = new Bitmap(_mapWidth * _mapSize * _tileSize,
                                          _mapHeight * _tileSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int singleBgWidth = listTile.Count / (_mapSize + 1);
            int temp = singleBgWidth;

            int count = 0, i;
           
            using (Graphics graphics = Graphics.FromImage(background))
            {
                for (int j = 0; j <= _mapSize; ++j) 
                { 
                    for (i = count; i < singleBgWidth; ++i) 
                    {
                        Tile _tile = listTile.ElementAt(i);
                        graphics.DrawImage(_tile._texture,
                                            new Rectangle(_tile._x, _tile._y, _tileSize, _tileSize),
                                            new Rectangle(new Point(), _tile._texture.Size),
                                            GraphicsUnit.Pixel);
                    }

                    count = i;
                    singleBgWidth += temp;
                }
            }

            return background;
        }
    }
}