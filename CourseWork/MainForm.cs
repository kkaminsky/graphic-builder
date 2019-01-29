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
    public partial class MainForm : Form
    {
        Dictionary<CurveItem, HelpForm> MyD = new Dictionary<CurveItem, HelpForm>();

        public MainForm()
        {
            InitializeComponent();
            //включим зум на колесико мыши
            zedGraphControl1.IsZoomOnMouseCenter = true;
            // отключим стандартное контекстное меню
            zedGraphControl1.IsShowContextMenu = false;
            // Разрешим выбор кривых
            zedGraphControl1.IsEnableSelection = true;
           
            // Выбирать кривые будем с помощью правой кнопки мыши
            zedGraphControl1.SelectButtons = MouseButtons.Right;
            // При это никакие другие кнопки нам не нужны
            zedGraphControl1.SelectModifierKeys = Keys.None;
           

            // Установим масштаб по умолчанию
            GraphPane pane = zedGraphControl1.GraphPane;
            pane.XAxis.Scale.Min = -2;
            pane.XAxis.Scale.Max = 2;
            pane.YAxis.Scale.Min = -2;
            pane.YAxis.Scale.Max = 2;

            // заголовок
            pane.Title.Text = "Построение графика функции";
            // Ось X будет пересекаться с осью Y на уровне Y = 0
            pane.XAxis.Cross = 0.0;

            // Ось Y будет пересекаться с осью X на уровне X = 0
            pane.YAxis.Cross = 0.0;

            // Отключим отображение первых и последних меток по осям
            pane.XAxis.Scale.IsSkipFirstLabel = true;
            pane.XAxis.Scale.IsSkipLastLabel = true;

            // Отключим отображение меток в точке пересечения с другой осью
            pane.XAxis.Scale.IsSkipCrossLabel = true;

            // Отключим отображение первых и последних меток по осям
            pane.YAxis.Scale.IsSkipFirstLabel = true;

            // Отключим отображение меток в точке пересечения с другой осью
            pane.YAxis.Scale.IsSkipLastLabel = true;
            pane.YAxis.Scale.IsSkipCrossLabel = true;

            // Спрячем заголовки осей
            pane.XAxis.Title.IsVisible = false;
            pane.YAxis.Title.IsVisible = false;
            // включи отображение сетки
            pane.XAxis.MajorGrid.IsVisible = true;

            // Задаем вид пунктирной линии для крупных рисок по оси X:
            // Длина штрихов равна 10 пикселям, ... 
            pane.XAxis.MajorGrid.DashOn = 10;

            // затем 5 пикселей - пропуск
            pane.XAxis.MajorGrid.DashOff = 5;


            // Включаем отображение сетки напротив крупных рисок по оси Y
            pane.YAxis.MajorGrid.IsVisible = true;

            // Аналогично задаем вид пунктирной линии для крупных рисок по оси Y
            pane.YAxis.MajorGrid.DashOn = 10;
            pane.YAxis.MajorGrid.DashOff = 5;


            // Включаем отображение сетки напротив мелких рисок по оси X
            pane.YAxis.MinorGrid.IsVisible = true;

            // Задаем вид пунктирной линии для крупных рисок по оси Y: 
            // Длина штрихов равна одному пикселю, ... 
            pane.YAxis.MinorGrid.DashOn = 1;

            // затем 2 пикселя - пропуск
            pane.YAxis.MinorGrid.DashOff = 2;

            // Включаем отображение сетки напротив мелких рисок по оси Y
            pane.XAxis.MinorGrid.IsVisible = true;

            // Аналогично задаем вид пунктирной линии для крупных рисок по оси Y
            pane.XAxis.MinorGrid.DashOn = 1;
            pane.XAxis.MinorGrid.DashOff = 2;

            // задаим вид осей
            pane.IsFontsScaled = false;
            pane.XAxis.Scale.FontSpec.Size = 15;
            pane.YAxis.Scale.FontSpec.Size = 15;
            pane.XAxis.Color = Color.RoyalBlue;
            pane.YAxis.Color = Color.RoyalBlue;
            

            // обновим оси
            zedGraphControl1.AxisChange();
            // обновим график
            zedGraphControl1.Invalidate();

            // Включим показ всплывающих подсказок при наведении курсора на график
            zedGraphControl1.IsShowPointValues = true;

            // Будем обрабатывать событие PointValueEvent, чтобы изменить формат представления координат
            zedGraphControl1.PointValueEvent +=
                new ZedGraphControl.PointValueHandler(zedGraph_PointValueEvent);

        }


        string zedGraph_PointValueEvent(ZedGraphControl sender,
        GraphPane pane,
        CurveItem curve,
        int iPt)
        {
            // Получим точку, около которой находимся
            PointPair point = curve[iPt];

            // Сформируем строку
            string result = string.Format("X: {0:F3}\nY: {1:F3}", point.X, point.Y);

            return result;
        }

        private void добавитьФункциюToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                // создадим вспомагательную форму
                HelpForm helpForm = new HelpForm(); 

                if (helpForm.ShowDialog() == DialogResult.OK)
                {
                    // Получим панель для рисования
                    GraphPane pane = zedGraphControl1.GraphPane;

                    Random r = new Random();
                    // создадим линию на основе полученных данных из вспомогательной формы
                    LineItem myCurve = pane.AddCurve(helpForm.expression, helpForm.list, Color.FromArgb(r.Next(255), r.Next(255), r.Next(100)), SymbolType.None);
                    // Зададим ее тощину
                    myCurve.Line.Width = 2;
                    // добавим линию и ее вспомогатльную форму в словарь
                    MyD.Add(myCurve, helpForm);
                    // Обновляем график
                    zedGraphControl1.Invalidate();
                };
            }
            catch(Exception er)
            {
                MessageBox.Show(er.Message, "Ошикба", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void масштабПоУмолчаниюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // получим панель
            GraphPane pane = zedGraphControl1.GraphPane;
            // зададим масштаб
            pane.XAxis.Scale.Min = -2;
            pane.XAxis.Scale.Max = 2;

            pane.YAxis.Scale.Min = -2;
            pane.YAxis.Scale.Max = 2;

            zedGraphControl1.AxisChange();

            zedGraphControl1.Invalidate();
        }

        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraphPane pane = zedGraphControl1.GraphPane;

            // Если есть что удалять
            if (pane.CurveList.Count > 0)
            {
                foreach (CurveItem a in pane.CurveList)
                {
                    MyD.Remove(a);
                }

                pane.CurveList.RemoveRange(0, pane.CurveList.Count);


                zedGraphControl1.Invalidate();

            }
            else
            {
                MessageBox.Show("Удалять нечего.", "Ошикба", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы точно хотите выйти?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dialogResult == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (zedGraphControl1.Selection.Count > 0)
            {
                GraphPane pane = zedGraphControl1.GraphPane;

                pane.CurveList.Remove(zedGraphControl1.Selection[0]);

                MyD.Remove(zedGraphControl1.Selection[0]);
               
                zedGraphControl1.Invalidate();
            }
            else
            {
                MessageBox.Show("Нет выделеного графика. Чтобы выдлеить график нажмите правую кнопку мыши на графике.", "Ошикба", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (zedGraphControl1.Selection.Count > 0 && MyD.ContainsKey(zedGraphControl1.Selection[0]))
            {
               
                    if (MyD[zedGraphControl1.Selection[0]].ShowDialog() == DialogResult.OK)
                    {
                        // Получим панель для рисования
                        GraphPane pane = zedGraphControl1.GraphPane;

                        LineItem myCurve =
                            pane.AddCurve
                            (
                                MyD[zedGraphControl1.Selection[0]].expression,
                                MyD[zedGraphControl1.Selection[0]].list,
                                zedGraphControl1.Selection[0].Color,
                                SymbolType.None
                            );

                        pane.CurveList.Remove(zedGraphControl1.Selection[0]);

                        MyD.Add(myCurve, MyD[zedGraphControl1.Selection[0]]);

                        MyD.Remove(zedGraphControl1.Selection[0]);

                        myCurve.Line.Width = 2;

                        // Обновляем график
                        zedGraphControl1.Invalidate();
                    };
                
                
            }
            else
            {
                MessageBox.Show("Нет выделеного графика. Чтобы выдлеить график нажмите правую кнопку мыши на графике.", "Ошикба", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
