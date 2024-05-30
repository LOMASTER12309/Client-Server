using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Runtime.Serialization;
using System.Text.Json;
using Newtonsoft.Json;

namespace ClientForm
{
    public partial class Form1 : Form
    {
        // адрес и порт сервера, к которому будем подключаться
        static int port = 8005; // порт сервера
        static string address = "127.0.0.1"; // адрес сервера
        Socket socket;
        Thread thread;
        object locker = new();
        bool connected = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                //создаем сокет
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);
                connected = true;
                thread = new Thread(ReceivingMessages);
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                        ex.Message,
                        "Ошибка!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                connected = false;
                listBox1.Items.Clear();
                //this.Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextAlignChanged(object sender, EventArgs e)
        {
            //textBox2.ReadOnly = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendMessage(textBox1.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //textBox2.ReadOnly = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!connected)
                try
                {
                    IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                    //создаем сокет
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    // подключаемся к удаленному хосту
                    socket.Connect(ipPoint);
                    connected = true;
                    thread = new Thread(ReceivingMessages);
                    thread.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                            ex.Message,
                            "Ошибка!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    connected = false;
                    listBox1.Items.Clear();
                    //this.Close();
                }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                string text_message = "/Close";
                byte[] data = Encoding.Unicode.GetBytes(text_message);
                try
                {
                    socket.Send(data);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                                ex.Message,
                                "Ошибка!",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                }
                listBox1.Items.Clear();
                connected = false;
            }
        }
        private void ReceivingMessages()
        {
            lock (locker)
            {
                byte[] mes = new byte[256];
                int bytes = 0;
                string message = "";
                StringBuilder builder = new StringBuilder();
                while (connected)
                {
                    do
                    {
                        try
                        {
                            bytes = socket.Receive(mes);  // получаем сообщение
                            builder.Append(Encoding.Unicode.GetString(mes, 0, bytes));
                        }
                        catch
                        {
                            return;
                        }
                    }
                    while (socket.Available > 0);
                    listBox1.Invoke(() => listBox1.Items.Add(builder.ToString()));
                    builder.Remove(0, builder.Length);
                    Array.Clear(mes);
                }
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
                SendMessage(textBox1.Text);
        }
        private void SendMessage(string mes)
        {
            try
            {
                if (textBox2.Text.Length > 0)
                {
                    string text_message = textBox2.Text + ": " + mes;
                    byte[] data = Encoding.Unicode.GetBytes(text_message);
                    socket.Send(data);
                    textBox1.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                        ex.Message,
                        "Ошибка!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                connected = false;
                listBox1.Items.Clear();
                //this.Close();
            }
        }
    }
}
