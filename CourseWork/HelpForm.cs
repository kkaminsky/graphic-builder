using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;


namespace CourseWork
{
    public partial class HelpForm : Form
    {
        
        
        public string expression { get; set; }
        public PointPairList list { get; set; }
        public HelpForm()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // создаем обьект своего парсера
                Parser MyParser = new Parser();
                // переводим выражение в обратную польскую запись
                string RPNexpression = MyParser.Parse(textBox1.Text);
                // запоминаем текующее выражение
                expression = textBox1.Text;

                
                double x1 = double.Parse(textBox2.Text);// начало промежутка
                double x2 = double.Parse(textBox3.Text);// конец промежутка
                double dx = double.Parse(textBox4.Text);// шаг изменения

                if (x1 >= x2) throw new ArgumentException("x1 должен быть меньше х2.");
                list = new PointPairList();
                // Заполняем список точек
                for (double x = x1; x <= x2; x += dx)
                {
                    list.Add(x, MyParser.Calculate(RPNexpression, x));
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            
            catch(Exception er)
            {
                MessageBox.Show(er.Message,"Ошикба", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
