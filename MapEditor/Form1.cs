using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapEditor.HelperForms;
using MapEditor.TileMap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Threading;
using MapEditor.Display;
using Microsoft.Xna.Framework.Input;
namespace MapEditor
{
    public partial class Form1 : Form
    {
        int mapWidth;
        int mapHeight;
        NewMapForm mapform = new NewMapForm();

        public SpriteBatch spriteBatch;
        public Texture2D cursor;
        TileMap.TileMap tileMap = null;
        public Texture2D texture;
        public Texture2D tilemaptexture;
        Camera camera = new Camera();
        Tileset tileset = null;
        Vector2 position = new Vector2();

        public Rectangle selectedrectangle;

        TilePalette tilepalette;
        public string TileSetImagepath;

        bool trackMouse = false;
        bool isMouseDown = false;
        bool isChanged = false;
        TileMapLayer currentLayer = null;
        bool showGrid = true;


         int mouseX;
        int mouseY;

        public GraphicsDevice GraphicsDevice
        {
            get { return tileMapDisplay1.GraphicsDevice; }
        }

        public Form1()
        {
            InitializeComponent();
            BtnShowGrid.CheckOnClick = true;
            tileMapDisplay1.OnInitialize += new EventHandler(tileDisplay1_OnInitialize);
            tileMapDisplay1.OnDraw += new EventHandler(tileDisplay1_OnDraw);
            tileMapDisplay1.MouseEnter += 
                new EventHandler(tileDisplay1_MouseEnter);

            tileMapDisplay1.MouseLeave += 
                new EventHandler(tileDisplay1_MouseLeave);

                tileMapDisplay1.MouseMove += 
                new MouseEventHandler(tileDisplay1_MouseMove);

            tileMapDisplay1.MouseDown += 
                new MouseEventHandler(tileDisplay1_MouseDown);

            tileMapDisplay1.MouseUp += 
                new MouseEventHandler(tileDisplay1_MouseUp);
        }


        void tileDisplay1_OnInitialize(object sender, EventArgs e)
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //tileMap = new TileMap.TileMap(32, 32);
            GraphicsDevice.PresentationParameters.PresentationInterval = PresentInterval.Default;
            FileStream stream = new FileStream("./Content/cursor.png", FileMode.Open);
            cursor = Texture2D.FromStream(GraphicsDevice, stream);
            stream.Close();
        }

        void tileDisplay1_OnDraw(object sender, EventArgs e)
        {
            long ticks = DateTime.UtcNow.Ticks; ;
            Logic();
            Render();
            long runtime = (DateTime.UtcNow.Ticks - ticks) / 10000;
            if (runtime < 1000)
            {
                int sleeptime = (1000 - (int)runtime) / 60;
                Thread.Sleep(sleeptime);
            }
           
            
        }


        private void Logic()
        {
            if (tileMap != null)
            {
                if (trackMouse)
                {
                    position.X = mouseX + camera.Position.X;
                    position.Y = mouseY + camera.Position.Y;
                }

                int tileX = (int)position.X / Engine.TileWidth;
                int tileY = (int)position.Y / Engine.TileHeight;
                

                // Movement Keyboard
                KeyboardState ks = Keyboard.GetState();
                if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                {
                    camera.Position.X = MathHelper.Clamp(camera.Position.X - 2, 0, (tileMap.MapWidth - (tileMapDisplay1.Width / Engine.TileWidth)) * Engine.TileWidth);
                }

                if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                {
                    camera.Position.X = MathHelper.Clamp(camera.Position.X + 2, 0, (tileMap.MapWidth - (tileMapDisplay1.Width / Engine.TileWidth)) * Engine.TileWidth);
                }

                if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                {
                    camera.Position.Y = MathHelper.Clamp(camera.Position.Y - 2, 0, (tileMap.MapHeight - (tileMapDisplay1.Height / Engine.TileHeight)) * Engine.TileHeight);
                }

                if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                {
                    camera.Position.Y = MathHelper.Clamp(camera.Position.Y + 2, 0, (tileMap.MapHeight - (tileMapDisplay1.Height / Engine.TileHeight)) * Engine.TileHeight);
                }

                // DRAW TEXTURE
                if (isMouseDown)
                {

                    //Texture ID opvragen
                    int TileID = 0;
                    for (int y = 0; y < selectedrectangle.Height / Engine.TileHeight; y++)
                    {
                        for (int x = 0; x < selectedrectangle.Width / Engine.TileWidth; x++)
                        {
                            currentLayer.SetTile(
                            tileX + x,
                            tileY + y,
                            tileset.GetTileID(new Rectangle(selectedrectangle.X + (x * Engine.TileWidth),selectedrectangle.Y + (y * Engine.TileHeight),Engine.TileWidth,Engine.TileHeight),tilemaptexture));
                           
                        }
                    }

                        isChanged = true;
                    if (true)
                    {
                        
                    }
                    //if (rbErase.Checked)
                    //{
                    //    currentLayer.SetTile(
                    //        tileX,
                    //        tileY,
                    //        -1);
                    //}
                }
                //tbCursorPosition.Text = "( " + tileX.ToString() + ", ";
                //tbCursorPosition.Text += tileY.ToString() + " )";
                //tbCursorPosition.Invalidate();
            }
        }

         private void Render()
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            if (tileMap != null)
            {
                DrawLayer(0);
                DrawDisplay();
            }
        }

        private void DrawLayer(int layer)
        {
            int tile;
            Rectangle tileRect = new Rectangle(
                0,
                0,
                Engine.TileWidth,
                Engine.TileHeight);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            for (int y = 0; y < tileMap.MapHeight; y++)
                for (int x = 0; x < tileMap.MapWidth; x++)
                {
                    tile = tileMap.layers[layer].GetTile(x, y);
                    if (tile != -1)
                    {
                        tileRect.X = x * Engine.TileWidth
                            - (int)camera.Position.X;
                        tileRect.Y = y * Engine.TileHeight
                            - (int)camera.Position.Y;
                        spriteBatch.Draw(tilemaptexture ,
                            tileRect,
                                tileset.tiles[tile],
                                Color.White);
                    }
                }
            spriteBatch.End();
        }

        private void DrawDisplay()
        {
            Vector2 FirstSquare = new Vector2(camera.Position.X / Engine.TileWidth, camera.Position.Y / Engine.TileHeight);
            Vector2 SquareOffSet = new Vector2(camera.Position.X % Engine.TileWidth, camera.Position.Y % Engine.TileHeight);




            //GRID Tekenen met SquareOffset !
            if (BtnShowGrid.Checked )
            {
            
            spriteBatch.Begin();
            for (int x = 0; x < (tileMapDisplay1.Width + Engine.TileWidth)  / Engine.TileWidth ; x++)
                for (int y = 0; y < (tileMapDisplay1.Height + Engine.TileHeight) / Engine.TileHeight; y++)
                    spriteBatch.Draw(cursor,
                        new Rectangle(x * Engine.TileWidth - (int)SquareOffSet.X,
                                      y * Engine.TileHeight - (int)SquareOffSet.Y,
                        Engine.TileWidth,
                        Engine.TileHeight),
                        Color.White);
            spriteBatch.End();
            }
            ////Selected TILE Draw
            Rectangle dest = new Rectangle((int)(position.X / Engine.TileWidth) * Engine.TileWidth - (int)camera.Position.X,
                                           (int)(position.Y / Engine.TileHeight) * Engine.TileHeight - (int)camera.Position.Y,
                        selectedrectangle.Width,
                        selectedrectangle.Height);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            if (tilemaptexture != null)
            {
                spriteBatch.Draw(tilemaptexture, dest, selectedrectangle, new Color(255, 255, 255, 200));
            }

            //SELECTION CURSOR RED
            spriteBatch.Draw(cursor,
                new Rectangle(
                    (int)(position.X / Engine.TileWidth) * Engine.TileWidth
                     - (int)camera.Position.X,
                    (int)(position.Y / Engine.TileHeight) * Engine.TileHeight
                     - (int)camera.Position.Y,
                    Engine.TileWidth,
                    Engine.TileHeight),
                    Color.Red);
            spriteBatch.End();
        }


        private List<Texture2D> getTexturesFromRectangle(Rectangle src)
        {
            List<Texture2D> textures = new List<Texture2D>();
            return textures;
        }
        // New map menuitem clicked
        private void newMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapform.ShowDialog();
            createNewMap();
        }

        
        private void createNewMap()
        {
            //get values from newmapform
            mapWidth = mapform.MapWidth;
            mapHeight = mapform.MapHeight;
            tileMap = new TileMap.TileMap(mapWidth, mapHeight);
            currentLayer = tileMap.layers[0];
        }

        private void openTilesetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            //load_tiles_dialog.InitialDirectory = Environment.CurrentDirectory + @"\Tiles";
            file.Filter = "Image files (*.img *.jpg *.png *.bmp)|*.img; *.jpg; *.png; *.bmp|All Files (*.*)| *.*";
            file.Title = "Load tiles";

            if (file.ShowDialog() == DialogResult.OK)
            {
                TileSetImagepath = file.FileName;

                //Texture Texture2D

                FileStream stream = new FileStream(TileSetImagepath, FileMode.Open);
                tilemaptexture = Texture2D.FromStream(GraphicsDevice, stream);
                stream.Close();

                tileset = new Tileset(tilemaptexture);

                //Image inladen in panel / palette
                tilepalette = new TilePalette(this);
                PnlPaletteContainer.Controls.Add(tilepalette);
                tilepalette.SetImage(TileSetImagepath);

                
                
                
            }

           

        }

        

         void tileDisplay1_MouseEnter(object sender, EventArgs e)
        {
            trackMouse = true;
        }

        void tileDisplay1_MouseLeave(object sender, EventArgs e)
        {
            trackMouse = false;
        }
        
        void tileDisplay1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;
        }

        void tileDisplay1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                isMouseDown = true;
        }

        void tileDisplay1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                isMouseDown = false;
        }


        

  

       

    }
}
