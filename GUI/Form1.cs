using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ImpMove.Core;
using Styx;
using Styx.Common;
using Styx.Pathing;
using Styx.WoWInternals;

namespace ImpMove.GUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Close();
         
           
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ImpMovePlugin.NeedMove = checkBox1.Checked;

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            ImpMovePlugin.Los = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var point = new WoWPoint((float)numericUpDown1.Value, (float)numericUpDown2.Value, (float)numericUpDown3.Value);
           Logging.Write("Точка: {0}",point);

            var rayCast = point.RayCast(StyxWoW.Me.Rotation, 20);
            Logging.Write("Точка RayCast: {0}",rayCast);
            if (!Navigator.CanNavigateFully(StyxWoW.Me.Location, rayCast))
            {
                Logging.Write("Неудается найти путь от {0} до {1}", StyxWoW.Me.Location, rayCast);
                return;
            }
            Logging.Write("Путь найден. Устанавливаю значение для метода передвижения.");
            ImpMovePlugin.PointTo = rayCast; 
            ImpMovePlugin.NeedMove = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
         
            checkBox1.Checked = ImpMovePlugin.NeedMove;
            checkBox2.Checked = ImpMovePlugin.Los;
            var point = StyxWoW.Me.Location;
            numericUpDown1.Value = (decimal) point.X;
            numericUpDown2.Value = (decimal) point.Y;
            numericUpDown3.Value = (decimal) point.Z;
            PerSecond.Value = ImpMovePlugin.PerSecond;
            label2.Text = StyxWoW.Me.RotationDegrees.ToString(CultureInfo.InvariantCulture)+" Дегрес";
            label1.Text = StyxWoW.Me.Rotation.ToString(CultureInfo.InvariantCulture)+" Радиан";
        

        }

/*
        private void GetUpdateAll()
        {
            var degres =(int) Math.Round(StyxWoW.Me.RotationDegrees);
           // trackBar1.Value = degres;
            checkBox1.Checked = ImpMovePlugin.NeedMove;
            checkBox2.Checked = ImpMovePlugin.Los;
            var point = StyxWoW.Me.Location;
            numericUpDown1.Value = (decimal)point.X;
            numericUpDown2.Value = (decimal)point.Y;
            numericUpDown3.Value = (decimal)point.Z;
            PerSecond.Value = ImpMovePlugin.PerSecond;
            label2.Text = degres.ToString(CultureInfo.InvariantCulture) + " Дегрес";
            label1.Text = StyxWoW.Me.Rotation.ToString(CultureInfo.InvariantCulture) + " Радиан";
           
        }
*/

        private void label2_Click(object sender, EventArgs e)
        {
            label2.Text = StyxWoW.Me.RotationDegrees.ToString(CultureInfo.InvariantCulture) + " Дегрес";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var target = StyxWoW.Me.CurrentTarget;
            if(target==null)return;
            var point = target.Location;
            ImpMovePlugin.PointTo = point;
            ImpMovePlugin.NeedMove = true;
            label5.Text = point.ToString();
            label6.Text = target.Name;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FilesMethod.SaveIdList("MyPoints.xml",ImpMovePlugin.MyPointsList);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            ImpMovePlugin.MyPointsList = FilesMethod.ReadIdList("MyPoints.xml");
            foreach (var point in ImpMovePlugin.MyPointsList)
            {
                comboBox1.Items.Add(point.Name);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var point = ImpMovePlugin.MyPointsList[comboBox1.SelectedIndex];
            ImpMovePlugin.PointTo = point.Point;
            ImpMovePlugin.NeedMove = true;
            

        }

        private void button7_Click(object sender, EventArgs e)
        {
            var point = StyxWoW.Me.Location;
            var name = StyxWoW.Me.ZoneText;
            var pi = new ImpPoint(name, point);
            ImpMovePlugin.MyPointsList.Add(pi);
            comboBox1.Items.Add(pi.Name);

        }

        private void PerSecond_ValueChanged(object sender, EventArgs e)
        {
            ImpMovePlugin.PerSecond = (int) PerSecond.Value;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            label1.Text = StyxWoW.Me.Rotation.ToString(CultureInfo.InvariantCulture) + " Радиан";
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            Navigator.Clear();
            ImpMovePlugin.PointTo = WoWPoint.Empty;
            ImpMovePlugin.PathNav=new List<WoWPoint>();
            ImpMovePlugin.NeedMove = false;
            WoWMovement.MoveStop(WoWMovement.MovementDirection.All);
            
          
        }

        private static void MoveDirection( int xOffset,int yOffset)
        {
            var oldpoint = StyxWoW.Me.Location;
            var point=oldpoint.Add(xOffset, yOffset, 0);
            float  z;
            if (Navigator.FindHeight(point.X, point.Y, out z)) point.Z = z;
            Logging.Write(point.ToString());
            if (Navigator.CanNavigateFully(oldpoint, point))
            {
                Logging.Write("Маршрут известен");
                ImpMovePlugin.PointTo = point;
                ImpMovePlugin.NeedMove = true;
            }
             else if (!Navigator.CanNavigateFully(StyxWoW.Me.Location, point))
             {
                 Logging.Write("Негенерит путь");
                 WoWMovement.ClickToMove(point);
             }
        }

        private void YPlus_Click(object sender, EventArgs e)
        {
            MoveDirection(0, (int)numericUpDown4.Value);
        }

        private void YMinus_Click(object sender, EventArgs e)
        {
            MoveDirection(0, -(int)numericUpDown4.Value);
        }

        private void XMinus_Click(object sender, EventArgs e)
        {
            MoveDirection(-(int)numericUpDown4.Value, 0);
        }

        private void XPlus_Click(object sender, EventArgs e)
        {
            MoveDirection((int)numericUpDown4.Value, 0);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var point = ImpMovePlugin.MyPointsList[comboBox1.SelectedIndex];
            if (ImpMovePlugin.MyPointsList.Contains(point))
            {
                ImpMovePlugin.MyPointsList.Remove(point);
                comboBox1.Items.RemoveAt(comboBox1.SelectedIndex);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            var list = ImpMovePlugin.MyPointsList.Select(impPoint => impPoint.Point).ToList();
            ImpMovePlugin.PathNav = list.ToList();
        }
    }
}
