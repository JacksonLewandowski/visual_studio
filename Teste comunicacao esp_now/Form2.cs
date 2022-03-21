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
        string RxString2;

        byte auto = 0;
        int contador = 0;
        int contador_2 = 0;

        byte ID;
        byte tam;
        static SerialPort SerialPortMaster;
        public Form1()
        {
            InitializeComponent();
            timer1.Enabled = true;
            SerialPortMaster = new SerialPort();
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

            //adiciona todas as COM diponíveis na lista
            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(s);
            }
        }

        private void serialPortMaster_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            RxString = "";
            int error_cont = 0;
            ID = Convert.ToByte(SerialPortMaster.ReadByte());
            tam = Convert.ToByte(SerialPortMaster.ReadByte());
            for(int i = 0 ;i<tam; i++)
            {
                RxString = RxString + Convert.ToString(SerialPortMaster.ReadByte()) + ' ';
            }

            this.Invoke(new EventHandler(trataDadoRecebido));
        }
        private void trataDadoRecebido(object sender, EventArgs e)
        {
            contador_2++;
            textBox1.AppendText("ID: " + Convert.ToString(ID) + " Tamanho: " +  Convert.ToString(tam) 
                                + " Mensagem-> " + RxString + Environment.NewLine);

        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            if ((SerialPortMaster.IsOpen == false))
            {
                try
                {

                    SerialPortMaster.PortName = comboBox1.Items[comboBox1.SelectedIndex].ToString();
                    SerialPortMaster.BaudRate = Convert.ToInt32(comboBox2.Items[comboBox2.SelectedIndex]);
                    SerialPortMaster.DataReceived += new SerialDataReceivedEventHandler(serialPortMaster_DataReceived);
                    SerialPortMaster.Open();
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
                }
            }
            else
            {
                try
                {
                    SerialPortMaster.Close();
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    button1.Text = "Conectar";
                }
                catch
                {
                    return;
                }

            }
        }
        private static byte[] GetByteArrayFromHexString(string input)
        {
            return input
                .Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(i => Convert.ToByte(i, 10))
                .ToArray();
                /*.Select(i => i.Trim().Replace("0x", ""))*/
                /*.Select(i => Convert.ToByte(i, 10))*/
                /*.ToArray();*/
        }
        private void button2_Click(object sender, EventArgs e)
        {
            escreve();
        }
        void escreve()
        {
            byte[] Data = GetByteArrayFromHexString(textBox2.Text);

            if (SerialPortMaster.IsOpen == true)          //porta está aberta
                SerialPortMaster.Write(Data, 0, Data.Length);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            AtCOM();
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (SerialPortMaster.IsOpen == true)  // se porta aberta
                SerialPortMaster.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(auto == 0)
            {
                auto = 1;
                timer2.Enabled = true;
            }
            else if(auto == 1)
            {
                auto = 0;
                timer2.Enabled = false;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox4.Clear();
            textBox5.Clear();
            escreve();
            contador++;
            textBox4.AppendText(contador.ToString());
            textBox5.AppendText(contador_2.ToString());
        }
    }
}
