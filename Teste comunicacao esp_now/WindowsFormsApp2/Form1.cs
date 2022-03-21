using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        string RxString;
        static SerialPort SerialPortMaster;
        static SerialPort SerialPortSlave;
        public Form1()
        {
            InitializeComponent();
            timer1.Enabled = true;
            SerialPortMaster = new SerialPort();
            SerialPortSlave = new SerialPort();
        }
        private void AtCOM()
        {

            //combox1 e 4
            int i;
            bool quantDiferente;    //flag para sinalizar que a quantidade de portas mudou

            i = 0;
            quantDiferente = false;

            if (comboBox1.Items.Count == SerialPort.GetPortNames().Length)
            {
                foreach (string s in SerialPort.GetPortNames())
                {
                    if (comboBox1.Items[i++].Equals(s) == false)
                    {
                        quantDiferente = true;
                    }
                }
            }
            else
            {
                quantDiferente = true;
            }

            //Se não foi detectado diferença
            if (quantDiferente == false)
            {
                return;                     //retorna
            }

            //limpa comboBox
            comboBox1.Items.Clear();
            comboBox4.Items.Clear();

            //adiciona todas as COM diponíveis na lista
            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(s);
                comboBox4.Items.Add(s);
            }
        }

        private void serialPortMaster_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            RxString = Convert.ToString(SerialPortMaster.ReadExisting());              //le o dado disponível na serial
            this.Invoke(new EventHandler(trataDadoRecebido));
        }
        private void serialPortSlave_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            RxString = Convert.ToString(SerialPortSlave.ReadExisting());              //le o dado disponível na serial
            this.Invoke(new EventHandler(trataDadoRecebido));
        }

        private void trataDadoRecebido(object sender, EventArgs e)
        {
            textBox1.AppendText(RxString);
            textBox1.AppendText("\n");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            if ((SerialPortMaster.IsOpen == false) /*&& (serialPort1.IsOpen == false)*/)
            {
                try
                {

                    SerialPortMaster.PortName = comboBox1.Items[comboBox1.SelectedIndex].ToString();
                    SerialPortMaster.BaudRate = Convert.ToInt32(comboBox2.Items[comboBox2.SelectedIndex]);
                    SerialPortMaster.DataReceived += new SerialDataReceivedEventHandler(serialPortMaster_DataReceived);
                    SerialPortMaster.Open();
                    /*SerialPortSlave.PortName = comboBox4.Items[comboBox4.SelectedIndex].ToString();
                    SerialPortSlave.BaudRate = Convert.ToInt32(comboBox5.Items[comboBox5.SelectedIndex]);
                    SerialPortSlave.DataReceived += new SerialDataReceivedEventHandler(serialPortSlave_DataReceived);
                    SerialPortSlave.Open();*/
                }
                catch
                {
                    return;

                }
                if (SerialPortMaster.IsOpen)
                {
                    button1.Text = "Desconectar";
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox4.Enabled = false;
                    comboBox5.Enabled = false;
                }
            }
            else
            {
                try
                {
                    SerialPortMaster.Close();
                    SerialPortSlave.Close();
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    comboBox4.Enabled = true;
                    comboBox5.Enabled = true;
                    button1.Text = "Conectar";
                }
                catch
                {
                    return;
                }

            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            byte[] Data = { Convert.ToByte(comboBox3.Text) }; 
           
            if (SerialPortMaster.IsOpen == true)          //porta está aberta
                SerialPortMaster.Write(Data, 0, 1);
            //SerialPortMaster.Write();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            AtCOM();
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (SerialPortMaster.IsOpen == true)  // se porta aberta
                SerialPortMaster.Close();
            if (SerialPortSlave.IsOpen == true)  // se porta aberta
                SerialPortSlave.Close();  //fecha a porta
        }
    }
}
