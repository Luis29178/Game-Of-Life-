using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Luis_Bazaldua_GOL
{
    public partial class Form1 : Form
    {
        //ints for universe settings
        int hight = 30;
        int width = 30;


        // The universe array
        bool[,] universe = new bool[30, 30];

        // Drawing colors
        Color gridColor = Color.Black;
        Color gridx10 = Color.Black;// implement
        Color cellColor = Color.Gray;
        Color live = Color.Green;
        Color dead = Color.Red;
        Color bacgrnd = Color.White;//implament

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;


        //determins Universe mode
        bool torodial = false;

        //Enable/Disable HUD
        bool HUD = false;

        // enable\ desable Nabour count
        bool Ncout = true;
        //enable disable x10grid
        bool tenxgrid = true;
        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            bool[,] sratchpad = new bool[universe.GetLength(0), universe.GetLength(1)];
            for (int y = 0; y < universe.GetLength(1); y++)
            {

                for (int x = 0; x < universe.GetLength(0); x++)
                {


                    int cout;
                    if (torodial)
                    {
                        cout = CountNeighborsToroidal(x, y);
                    }
                    else
                    {
                        cout = CountNeighborsFinite(x, y);
                    }

                    //apply the rules
                    if (universe[x, y] == true && cout < 2)
                    {
                        sratchpad[x, y] = false;
                    }
                    else if (universe[x, y] == true && cout > 3)
                    {
                        sratchpad[x, y] = false;
                    }
                    else if (universe[x, y] == true)
                    {
                        if (cout == 2 || cout == 3)
                        {
                            sratchpad[x, y] = true;
                        }
                    }
                    else if (universe[x, y] == false && cout == 3)
                    {
                        sratchpad[x, y] = true;
                    }
                    else
                    {
                        sratchpad[x, y] = universe[x, y];
                    }


                    // turn on/off in scratchpad[] for new gen. 


                }


            }

            //copy everything from scratchpad to universe
            universe = sratchpad;



            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            Pen gridx10pen = new Pen(gridx10, 4);
            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }
            if (tenxgrid)
            {
                for (int y = 0; y < universe.GetLength(1); y += 10)
                {
                    for (int x = 0; x < universe.GetLength(0); x += 10)
                    {
                        Rectangle cellx10rect = Rectangle.Empty;
                        cellx10rect.X = x * cellWidth;
                        cellx10rect.Y = y * cellHeight;
                        cellx10rect.Width = cellWidth * 10;
                        cellx10rect.Height = cellHeight * 10;

                        if (x % 10 == 0)
                        {

                            e.Graphics.DrawRectangle(gridx10pen, cellx10rect.X, cellx10rect.Y, cellx10rect.Width, cellx10rect.Height);
                        }
                    }
                }
            }
            if (Ncout)
            {
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {

                        Rectangle cellRect = Rectangle.Empty;
                        cellRect.X = x * cellWidth;
                        cellRect.Y = y * cellHeight;
                        cellRect.Width = cellWidth;
                        cellRect.Height = cellHeight;

                        if (torodial)
                        {
                            if (CountNeighborsToroidal(x, y) > 0)
                            {
                                Font font = new Font("Arial", 12f);
                                StringFormat stringFormat = new StringFormat();
                                stringFormat.Alignment = StringAlignment.Center;
                                stringFormat.LineAlignment = StringAlignment.Center;


                                int neighbors = CountNeighborsToroidal(x, y);
                                if (neighbors >= 2)
                                {
                                    if (universe[x, y] == true || neighbors >= 3)
                                    {

                                        e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat);

                                    }
                                    else
                                    {
                                        e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat);
                                    }


                                }
                                else
                                {
                                    e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat);
                                }

                            }
                        }
                        else
                        {
                            if (CountNeighborsFinite(x, y) > 0)
                            {
                                Font font = new Font("Arial", 12f);
                                StringFormat stringFormat = new StringFormat();
                                stringFormat.Alignment = StringAlignment.Center;
                                stringFormat.LineAlignment = StringAlignment.Center;


                                int neighbors = CountNeighborsFinite(x, y);

                                if (neighbors >= 2)
                                {
                                    if (universe[x, y] == true || neighbors >= 3)
                                    {

                                        e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat);

                                    }
                                    else
                                    {
                                        e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat);
                                    }

                                }
                                else
                                {
                                    e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat);
                                }

                            }
                        }

                        graphicsPanel1.Invalidate();

                    }

                }
            }   
            

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {

                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;

                }
            }
            graphicsPanel1.Invalidate();
        }

        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            torodial = false;
        }

        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            torodial = true;
        }
        private int CountNeighborsToroidal(int x, int y)

        {

            int count = 0;

            int xLen = universe.GetLength(0);

            int yLen = universe.GetLength(1);


            for (int yOffset = -1; yOffset <= 1; yOffset++)

            {

                for (int xOffset = -1; xOffset <= 1; xOffset++)

                {

                    int xCheck = x + xOffset;

                    int yCheck = y + yOffset;


                    // if xOffset and yOffset are both equal to 0 then continue
                    if (!(xOffset == 0 && yOffset == 0))
                    {
                        // if xCheck is less than 0 then set to xLen - 1

                        if (xCheck < 0)
                        {
                            xCheck = xLen - 1;


                        }
                        // if yCheck is less than 0 then set to yLen - 1

                        if (yCheck < 0)
                        {
                            yCheck = yLen - 1;

                        }
                        // if xCheck is greater than or equal too xLen then set to 0

                        if (xCheck >= xLen)
                        {
                            xCheck = 0;
                        }
                        // if yCheck is greater than or equal too yLen then set to 0

                        if (yCheck >= yLen)
                        {
                            yCheck = 0;
                        }


                        if (universe[xCheck, yCheck] == true) count++;

                    }










                }

            }


            return count;

        }
        private int CountNeighborsFinite(int x, int y)
        {

            int count = 0;

            int xLen = universe.GetLength(0);

            int yLen = universe.GetLength(1);


            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {

                for (int xOffset = -1; xOffset <= 1; xOffset++)

                {

                    int xCheck = x + xOffset;

                    int yCheck = y + yOffset;

                    // if xOffset and yOffset are both equal to 0 then continue
                    if (!(xOffset == 0 && yOffset == 0))
                    {
                        if (!(xCheck < 0))
                        {
                            if (!(yCheck < 0))
                            {
                                if (!(xCheck >= xLen))
                                {
                                    if (!(yCheck >= yLen))
                                    {

                                        if (universe[xCheck, yCheck] == true) count++;

                                    }

                                }


                            }

                        }



                    }
                    // if xCheck is less than 0 then continue

                    // if yCheck is less than 0 then continue

                    // if xCheck is greater than or equal too xLen then continue

                    // if yCheck is greater than or equal too yLen then continue









                }

            }


            return count;

        }
    }
}
