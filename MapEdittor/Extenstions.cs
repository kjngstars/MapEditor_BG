using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapEdittor
{
    public partial class Form1 : Form
    {
        HashSet<int> _set = new HashSet<int>();
        List<int> _listUniqueTile = new List<int>();
        List<int> _listTileMap = new List<int>();
        List<Tile> _listTile4Drawing = new List<Tile>();
        List<string> _listObject = new List<string>();
        List<PictureBox> _listPictureBox = new List<PictureBox>();

        int _mapWidth;
        int _mapHeight;
        int _mapSize;
        int _tileSize = 64;
        Bitmap _tileSet;

        int _tileWidth;
        int _tileHeight;
        int _gridSize = 32;
        int _selectedObjectTag;

        int _eX, _eY;
        int _posClickedX, _posClickedY;

        bool _mouseEdit = false;
        bool _editMode = false;

        bool _shiftKeyPressed = false;
        bool _enableGrid = false;

        PictureBox _selectedTileToDelete = null;
    }
}
