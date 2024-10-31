using LiftDemo_A;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lift_Project
{
    public partial class Form1 : Form
    {
        public bool isClosing;
        public bool isOpening;
        public bool isOpen;


        int doorMaxOpenWidth;
        int doorSpeed = 5;
        int liftSpeed = 5;
        private int currentFloor = 0; // 0 represents the ground floor, 1 represents the first floor.
        private Lift lift;
        DataTable dt=new DataTable();
        Db_context db_Context = new Db_context();
        

        public Form1(bool isOpening)
        {
            InitializeComponent();
            this.isOpening = isOpening;
            lift = new Lift(mainElevator, btn_1, btn_G, this.ClientSize.Height, liftSpeed, liftTimerUp, liftTimerDown, fFloorBack, gFloorBack,doorTimer);


            doorMaxOpenWidth = mainElevator.Width / 2;

            dataGridView.ColumnCount= 2;
            dataGridView.Columns[0].Name = "Time";
            dataGridView.Columns[1].Name = "Events";

            dt.Columns.Add("Time");
            dt.Columns.Add("Events");

            pictureBox1.Visible = false;

        }

        private void logEvents(string message)
        {
            string currentTime = DateTime.Now.ToString("hh:mm:ss");

            dt.Rows.Add(currentTime, message);
            dataGridView.Rows.Add(currentTime,message);

            db_Context.InsertLogsIntoDB(dt); 

        }
        

        private void Form1_Load(object sender, EventArgs e)
        {
            db_Context.LoadLogsfromDB(dt, dataGridView);
            pictureBox1.Image = Image.FromFile("\"C:\\Users\\Ashmi Chaudhary\\OneDrive\\Desktop\\Lift Assignment\\upArrow.gif\"");
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            // Adding GIF for down arrow screen
            //downArrowScreen.Image = Image.FromFile("C:\\Users\\user\\Desktop\\down.gif");
            //downArrowScreen.SizeMode = PictureBoxSizeMode.Zoom;

            //// Adding GIF for emergency alert
            //emergencyBellScreen.Image = Image.FromFile("C:\\Users\\user\\Desktop\\emergency.gif");
            //emergencyBellScreen.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void btn_1_Click(object sender, EventArgs e)
        {
            if (isOpen)
            {
                btn_Close_Click(null, EventArgs.Empty);
                lift.SetState(new MovingUpState());
                lift.LiftTimerUp.Start();
                logEvents("lift first floor");

            }
            else
            {
                lift.SetState(new MovingUpState());
                lift.LiftTimerUp.Start();
                
                logEvents("lift first floor");
            }

        }

        private void btn_G_Click(object sender, EventArgs e)
        {
            if (isOpen)
            {
                btn_Close_Click(null, EventArgs.Empty);
                lift.SetState(new MovingDownState());
                lift.LiftTimerDown.Start();
                logEvents("lift ground floor" );
            }
            else
            {
                lift.SetState(new MovingDownState());
                lift.LiftTimerDown.Start();
                logEvents("lift ground floor");
            }


        }

        public void liftTimerUp_Tick(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            currentFloor = 1;
            lift.MovingUp();
            lblFloorStatus.Text = "Floor: " + (currentFloor == 0 ? "Ground" : "First");
        }

        public void liftTimerDown_Tick(object sender, EventArgs e)
        {
            currentFloor = 0;
            lift.MovingDown();
            lblFloorStatus.Text = "Floor: " + (currentFloor == 0 ? "Ground" : "First");
        }

        private void up_button_Click(object sender, EventArgs e)
        {
            if (currentFloor == 0)
            {
                lift.SetState(new MovingUpState());
                lift.LiftTimerUp.Start();
                logEvents("Reached first floor");
            }
            else
            {
                MessageBox.Show("Lift is already on the first floor");
            }
        }

        private void down_button_Click(object sender, EventArgs e)
        {
            if (currentFloor == 1)
            {
                lift.SetState(new MovingDownState());
                lift.LiftTimerDown.Start();
                logEvents("Reached ground floor");
            }
            else
            {
                MessageBox.Show("lift is Already on the ground floor ");
            }

        }

        private void btn_open_Click(object sender, EventArgs e)
        {
            isOpening = true;
            isOpen = true;
            isClosing = false;
            doorTimer.Start();
            btn_Close.Enabled = false;
            
            logEvents("Lift open");
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            isOpening = false;
            isOpen = false;
            isClosing = true;
            doorTimer.Start();
            logEvents("Lift closed");
        }

        private void door_Timer_Tick(object sender, EventArgs e)
        {
            // To Check the current floor and open/close the appropriate doors
            if (currentFloor == 0) // Ground floor
            {
                if (this.isOpening)
                {
                    if (doorLeft_G.Left > doorMaxOpenWidth / 2 + 65)
                    {
                        doorLeft_G.Left -= doorSpeed;
                        doorRight_G.Left += doorSpeed;
                    }
                    else
                    {
                        doorTimer.Stop();
                        btn_Close.Enabled = true;
                    }
                }

                if (this.isClosing)
                {
                    if (doorLeft_G.Right < mainElevator.Width + doorMaxOpenWidth / 2 + 65)
                    {
                        doorLeft_G.Left += doorSpeed;
                        doorRight_G.Left -= doorSpeed;
                    }
                    else
                    {
                        doorTimer.Stop();
                    }
                }
            }
            else if (currentFloor == 1) // First floor
            {
                if (this.isOpening)
                {
                    if (doorLeft_1.Left > doorMaxOpenWidth / 2 + 65)
                    {
                        doorLeft_1.Left -= doorSpeed;
                        doorRight_1.Left += doorSpeed;
                    }
                    else
                    {
                        doorTimer.Stop();
                        btn_Close.Enabled = true;
                    }
                }

                if (this.isClosing)
                {
                    if (doorLeft_1.Right < mainElevator.Width + doorMaxOpenWidth / 2 + 65)
                    {
                        doorLeft_1.Left += doorSpeed;
                        doorRight_1.Left -= doorSpeed;
                    }
                    else
                    {
                        doorTimer.Stop();
                    }
                }
            }
        }
      private void btn_Delete_Click(object sender, EventArgs e)
        {
         db_Context.DeleteAllLogs(dt, dataGridView);
        }


        private void groundFloor_Click(object sender, EventArgs e)
        {

        }
    }
}
