using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace MapEditor.TileMap
{
    public class Tileset
    {
        public string textureName;
        public int tileWidth, tileHeight;
        public List<Rectangle> tiles;
        public string TilesetTextureName;


        public Tileset(string tileSetName)
        {
            textureName = "";
            tileWidth = 0;
            tileHeight = 0;
            tiles = new List<Rectangle>();
            ProcessTileSet(tileSetName);
        }

        void ProcessTileSet(string filePath)
        {
            XmlDocument input = new XmlDocument();

            input.Load(filePath);
            foreach (XmlNode node in input.DocumentElement.ChildNodes)
            {
                if (node.Name == "TextureElement")
                {
                    textureName = node.Attributes["TextureName"].Value;
                }
                if (node.Name == "TilesetDefinitions")
                {
                    tileWidth = Int32.Parse(node.Attributes["TileWidth"].Value);
                    tileHeight = Int32.Parse(node.Attributes["TileHeight"].Value);
                }

                if (node.Name == "TilesetRectangles")
                {
                    List<Rectangle> rectangles = new List<Rectangle>();

                    foreach (XmlNode rectNode in node.ChildNodes)
                    {
                        if (rectNode.Name == "Rectangle")
                        {
                            Rectangle rect;
                            rect = new Rectangle(
                                Int32.Parse(rectNode.Attributes["X"].Value),
                                Int32.Parse(rectNode.Attributes["Y"].Value),
                                Int32.Parse(rectNode.Attributes["Width"].Value),
                                Int32.Parse(rectNode.Attributes["Height"].Value));
                            rectangles.Add(rect);
                        }
                    }

                    tiles = rectangles;
                }
            }

            string fileName = Path.GetDirectoryName(filePath);
            fileName += @"\" + textureName;

            string[] extensions = { ".png", ".jpg", ".tga", ".bmp", ".gif" };
            bool found = false;

            foreach (string extension in extensions)
            {
                if (File.Exists(fileName + extension))
                {
                    found = true;
                    fileName += extension;
                    break;
                }
            }

            if (found)
            {
                TilesetTextureName = fileName;
            }
            else
            {
                MessageBox.Show("Unrecognized image format in the tile set.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                throw new Exception("Unable to load the tile set.");
            }
        }

    }
}
