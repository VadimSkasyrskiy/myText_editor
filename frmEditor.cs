using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace Text_Editor
{
    public partial class frmEditor : Form
    {
        List<string> colorList = new List<string>();
        string filenamee;    // фаил, открытый внутри RTB
        const int MIDDLE = 382;    // средняя сумма RGB - максимальная 765
        int sumRGB;    // сумма выбранных цветов RGB
        int pos, line, column;    // для определения номеров строк и столбцов

        public frmEditor()
        {
            InitializeComponent();
        }

        private void frmEditor_Load(object sender, EventArgs e)
        {
            myTextBox1.AllowDrop = true;     // чтобы разрешить перетаскивание в MyTextBox
            myTextBox1.AcceptsTab = true;    // вкладка разрешить
            myTextBox1.WordWrap = false;    // отключить перенос слов при запуске
            myTextBox1.ShortcutsEnabled = true;    // разрешить ярлыки
            myTextBox1.DetectUrls = true; 
            fontDialog1.ShowColor = true;
            fontDialog1.ShowApply = true;
            fontDialog1.ShowHelp = true;
            colorDialog1.AllowFullOpen = true;
            colorDialog1.AnyColor = true;
            colorDialog1.SolidColorOnly = false;
            colorDialog1.ShowHelp = true;
            colorDialog1.AnyColor = true;
            leftAlignStripButton.Checked = true;
            centerAlignStripButton.Checked = false;
            rightAlignStripButton.Checked = false;
            boldStripButton3.Checked = false;
            italicStripButton.Checked = false;
            rightAlignStripButton.Checked = false;
            bulletListStripButton.Checked = false;
            wordWrapToolStripMenuItem.Image = null;
            MinimizeBox = false;
            MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // заполнить список элементов zoomDropDownButton
            zoomDropDownButton.DropDown.Items.Add("20%");
            zoomDropDownButton.DropDown.Items.Add("50%");
            zoomDropDownButton.DropDown.Items.Add("70%");
            zoomDropDownButton.DropDown.Items.Add("100%");
            zoomDropDownButton.DropDown.Items.Add("150%");
            zoomDropDownButton.DropDown.Items.Add("200%");
            zoomDropDownButton.DropDown.Items.Add("300%");
            zoomDropDownButton.DropDown.Items.Add("400%");
            zoomDropDownButton.DropDown.Items.Add("500%");

            // размеры шрифта в поле со списком
            for (int i = 8; i < 80; i += 2)
            {
                fontSizeComboBox.Items.Add(i);
            }

            // заливка цветов в раскрывающемся списке "Color"
            foreach (System.Reflection.PropertyInfo prop in typeof(Color).GetProperties())
            {
                if (prop.PropertyType.FullName == "System.Drawing.Color")
                {
                    colorList.Add(prop.Name);     
                }
            }

            // раскрывающийся список элементов
            foreach (string color in colorList)
            {
                colorStripDropDownButton.DropDownItems.Add(color);
            }

            //  "BackColor" для каждого цвета в раскрывающемся списке элементов
            for (int i = 0; i < colorStripDropDownButton.DropDownItems.Count; i++)
            {
                // Создание объекта KnownColor
                KnownColor selectedColor;
                selectedColor = (KnownColor)System.Enum.Parse(typeof(KnownColor), colorList[i]);    // разбор на известный цвет
                colorStripDropDownButton.DropDownItems[i].BackColor = Color.FromKnownColor(selectedColor);    // присвоение "BackColor" в соответствующий элемент списка

                // Установление цвета текста в зависимости от того, темнее или светлее "background"
                Color col = Color.FromName(colorList[i]);

 
                sumRGB = ConvertToRGB(col);    // получить сумму цветовых объектов значения RGB
                if (sumRGB <= MIDDLE)    // Темный "Background"
                {
                    colorStripDropDownButton.DropDownItems[i].ForeColor = Color.White;    // установить белый текст
                }
                else if (sumRGB > MIDDLE)    // Светлый "Background"
                {
                    colorStripDropDownButton.DropDownItems[i].ForeColor = Color.Black;    // установить темный текст
                }
            }

            // шрифты в поле со списком шрифтов
            InstalledFontCollection fonts = new InstalledFontCollection();
            foreach (FontFamily family in fonts.Families)
            {
                fontStripComboBox.Items.Add(family.Name);
            }

            // определяет номера строк и столбцов позиции мыши в RichTextBox
            int pos = myTextBox1.SelectionStart;
            int line = myTextBox1.GetLineFromCharIndex(pos);
            int column = myTextBox1.SelectionStart - myTextBox1.GetFirstCharIndexFromLine(line);
            lineColumnStatusLabel.Text = "Line " + (line + 1) + ", Column " + (column + 1);
        }

        //******************************************************************************************************************************
        // ConvertToRGB - Принимает объект цвета в качестве параметра. Получает значения RGB переданного ему объекта и вычисляет сумму.*
        //******************************************************************************************************************************
        private int ConvertToRGB(System.Drawing.Color c)
        {
            int r = c.R, // Значение "RED" красного компонента
                g = c.G, // Значение "GREEN" зеленого компонента
                b = c.B; // Значение "BLUE" синего компонента
            int sum = 0;

            // вычисление суммы RGB
            sum = r + g + b;

            return sum;
        }
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myTextBox1.SelectAll();     // выделить весь текст
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // очистить 
            myTextBox1.Clear();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myTextBox1.Paste();     // вставить текст
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myTextBox1.Copy();      // копировать текст
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myTextBox1.Cut();     // вырезать текст
        }

        private void boldStripButton3_Click(object sender, EventArgs e)
        {
           
            if (boldStripButton3.Checked == false)
            {
                boldStripButton3.Checked = true; // значение "BOLD" is true
            }
            else if (boldStripButton3.Checked == true)
            {
                boldStripButton3.Checked = false;    // значение "BOLD" is false
            }

            if (myTextBox1.SelectionFont == null)
            {
                return;
            }

            // создание объекта FontStyle
            FontStyle style = myTextBox1.SelectionFont.Style;

            // определяет стиль шрифта
            if (myTextBox1.SelectionFont.Bold)
            {
                style &= ~FontStyle.Bold; 
            }
            else
            {
                style |= FontStyle.Bold;

            }
            myTextBox1.SelectionFont = new Font(myTextBox1.SelectionFont, style);     // задает стиль шрифта
        }

        private void underlineStripButton_Click(object sender, EventArgs e)
        {
            if (underlineStripButton.Checked == false)
            {
                underlineStripButton.Checked = true;     // значение "UNDERLINE" is active
            }
            else if (underlineStripButton.Checked == true)
            {
                underlineStripButton.Checked = false;    // значение UNDERLINE is not active
            }

            if (myTextBox1.SelectionFont == null)
            {
                return;
            }

            
            FontStyle style = myTextBox1.SelectionFont.Style;

            
            if (myTextBox1.SelectionFont.Underline)
            {
                style &= ~FontStyle.Underline;
            }
            else
            {
                style |= FontStyle.Underline;
            }
            myTextBox1.SelectionFont = new Font(myTextBox1.SelectionFont, style);
        }

        private void italicStripButton_Click(object sender, EventArgs e)
        {
            if (italicStripButton.Checked == false)
            {
                italicStripButton.Checked = true;    // значение ITALICS is active
            }
            else if (italicStripButton.Checked == true)
            {
                italicStripButton.Checked = false;    // значение ITALICS is not active
            }

            if (myTextBox1.SelectionFont == null)
            {
                return;
            }
            
            FontStyle style = myTextBox1.SelectionFont.Style;

            
            if (myTextBox1.SelectionFont.Italic)
            {
                style &= ~FontStyle.Italic;
            }
            else
            {
                style |= FontStyle.Italic;
            }
            myTextBox1.SelectionFont = new Font(myTextBox1.SelectionFont, style);
        }

        private void fontSizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (myTextBox1.SelectionFont == null)
            {
                return;
            }
            // устанавливает размер шрифта при изменении
            myTextBox1.SelectionFont = new Font(myTextBox1.SelectionFont.FontFamily,Convert.ToInt32(fontSizeComboBox.Text),myTextBox1.SelectionFont.Style);
        }

        private void fontStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (myTextBox1.SelectionFont == null)
            {
                // задает стиль семейства шрифтов
                myTextBox1.SelectionFont = new Font(fontStripComboBox.Text, myTextBox1.Font.Size);
            }
            // устанавливает выбранный стиль шрифта
            myTextBox1.SelectionFont = new Font(fontStripComboBox.Text, myTextBox1.SelectionFont.Size);
        }

        private void saveStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.ShowDialog();    // показывает диалоговое окно
                string file;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string filename = saveFileDialog1.FileName;
                    // сохраняет содержимое поля форматированного текста
                    myTextBox1.SaveFile(filename, RichTextBoxStreamType.PlainText);
                    file = Path.GetFileName(filename);    // получает имя файла
                    MessageBox.Show("Файл " + file + " был успешно сохранен.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void openFileStripButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();     // показывает диалоговое окно
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filenamee = openFileDialog1.FileName;
                // загружает файл в RichTextBox
                myTextBox1.LoadFile(filenamee, RichTextBoxStreamType.PlainText);    // загружает его в обычном текстовом формате
                // myTextBox1.LoadFile(filename, RichTextBoxStreamType.RichText);    // загружает его в формате RTB
            }
        }

        private void colorStripDropDownButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // // создает объект "KnownColor"
            KnownColor selectedColor;
            selectedColor = (KnownColor)System.Enum.Parse(typeof(KnownColor), e.ClickedItem.Text);    // преобразует его в Цветовую структуру
            myTextBox1.SelectionColor = Color.FromKnownColor(selectedColor);    // устанавливает выбранный цвет
        }

        private void myTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            // выделяет границу кнопки, когда кнопки имеют значение true
            if (myTextBox1.SelectionFont != null)
            {
                boldStripButton3.Checked = myTextBox1.SelectionFont.Bold;
                italicStripButton.Checked = myTextBox1.SelectionFont.Italic;
                underlineStripButton.Checked = myTextBox1.SelectionFont.Underline;
            }
        }

        private void leftAlignStripButton_Click(object sender, EventArgs e)
        {
            // устанавливает свойства
            centerAlignStripButton.Checked = false;
            rightAlignStripButton.Checked = false;
            if(leftAlignStripButton.Checked == false)
            {
                leftAlignStripButton.Checked = true;    // LEFT ALIGN is active выравнивание по левому краю
            }
            else if(leftAlignStripButton.Checked == true)
            {
                leftAlignStripButton.Checked = false;    // LEFT ALIGN is not active
            }
            myTextBox1.SelectionAlignment = HorizontalAlignment.Left;    // выбор выравнивания по левому краю
        }

        private void centerAlignStripButton_Click(object sender, EventArgs e)
        {
            leftAlignStripButton.Checked = false;
            rightAlignStripButton.Checked = false;
            if (centerAlignStripButton.Checked == false)
            {
                centerAlignStripButton.Checked = true;    // CENTER ALIGN is active выравнивание по центру
            }
            else if (centerAlignStripButton.Checked == true)
            {
                centerAlignStripButton.Checked = false;    // CENTER ALIGN is not active
            }
            myTextBox1.SelectionAlignment = HorizontalAlignment.Center;     // выбор выравнивания по центру
        }

        private void rightAlignStripButton_Click(object sender, EventArgs e)
        {
            leftAlignStripButton.Checked = false;
            centerAlignStripButton.Checked = false;

            if (rightAlignStripButton.Checked == false)
            {
                rightAlignStripButton.Checked = true;    // RIGHT ALIGN is active выравнивание по правому краю
            }
            else if (rightAlignStripButton.Checked == true)
            {
                rightAlignStripButton.Checked = false;    // RIGHT ALIGN is not active
            }
            myTextBox1.SelectionAlignment = HorizontalAlignment.Right;    // выбор выравнивания по правому краю
        }

        private void bulletListStripButton_Click(object sender, EventArgs e)
        {
            if (bulletListStripButton.Checked == false)
            {
                bulletListStripButton.Checked = true;
                myTextBox1.SelectionBullet = true;    // BULLET LIST is active маркированный список
            }
            else if (bulletListStripButton.Checked == true)
            {
                bulletListStripButton.Checked = false;
                myTextBox1.SelectionBullet = false;    // BULLET LIST is not active
            }
        }

        private void increaseStripButton_Click(object sender, EventArgs e)
        {
            string fontSizeNum = fontSizeComboBox.Text;    // переменная для хранения выбранного размера        
            try
            {
                int size = Convert.ToInt32(fontSizeNum) + 1;    // конвертирует (fontSizeNum + 1)
                if (myTextBox1.SelectionFont == null)
                {
                    return;
                }
                // устанавливает обновленный размер шрифта
                myTextBox1.SelectionFont = new Font(myTextBox1.SelectionFont.FontFamily,size,myTextBox1.SelectionFont.Style);
                fontSizeComboBox.Text = size.ToString();    // обновляет размер шрифта
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void decreaseStripButton_Click(object sender, EventArgs e)
        {
            string fontSizeNum = fontSizeComboBox.Text;    // переменная для хранения выбранного размера          
            try
            {
                int size = Convert.ToInt32(fontSizeNum) - 1;    // конвертирует (fontSizeNum - 1)
                if (myTextBox1.SelectionFont == null)
                {
                    return;
                }
                myTextBox1.SelectionFont = new Font(myTextBox1.SelectionFont.FontFamily,size,myTextBox1.SelectionFont.Style);
                fontSizeComboBox.Text = size.ToString();    // обновляет размер шрифта
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Information", MessageBoxButtons.OK, MessageBoxIcon.Warning); // показывает сообщение об ошибке
            }
        }

        //*********************************************************************************************
        // myTextBox1_DragEnter - Пользовательское событие. Копирует текст, перетаскиваемый в "RichTextBox" *
        //*********************************************************************************************
        private void myTextBox1_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;    // копирует данные в RTB
            else
                e.Effect = DragDropEffects.None;    // не принимает данные в RTB
        }
        //***************************************************************************************************
        // myTextBox1_DragEnter - Пользовательское событие. Отбрасывает скопированный текст, перетаскиваемый в "RichTextBox" *
        //***************************************************************************************************
        private void myTextBox1_DragDrop(object sender,System.Windows.Forms.DragEventArgs e)
        {
            // переменные
            int i;
            String s;

            // Получает начальную позицию, чтобы удалить текст.
            i = myTextBox1.SelectionStart;
            s = myTextBox1.Text.Substring(i);
            myTextBox1.Text = myTextBox1.Text.Substring(0, i);

            // Помещяет текст в поле "RichTextBox".
            myTextBox1.Text += e.Data.GetData(DataFormats.Text).ToString();
            myTextBox1.Text += s;
        }

        private void undoStripButton_Click(object sender, EventArgs e)
        {           
            myTextBox1.Undo();     // отменить действие
        }

        private void redoStripButton_Click(object sender, EventArgs e)
        {            
            myTextBox1.Redo();    // повторить действие
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            this.Close();     // закрыть форму
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            myTextBox1.Undo();     // отменить действие
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            myTextBox1.Redo();     // повторить действие
        }

        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {            
            myTextBox1.Cut();     // вырезать текст
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            
            myTextBox1.Copy();     // копировать текст
        }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {           
            myTextBox1.Paste();    // вставить текст
        }

        private void selectAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {            
            myTextBox1.SelectAll();    // выделить весь текст
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // очищает поле форматированного текста
            myTextBox1.Clear();
            myTextBox1.Focus();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // удаляет выделенный текст
            myTextBox1.SelectedText = "";
            myTextBox1.Focus();
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                myTextBox1.LoadFile(openFileDialog1.FileName, RichTextBoxStreamType.PlainText);
                // myTextBox1.LoadFile(openFileDialog1.FileName, RichTextBoxStreamType.RichText);  // загружает файл в формате RTB
            }
        }

        private void newMenuItem_Click(object sender, EventArgs e)
        {
            
            if (myTextBox1.Text != string.Empty) 
            {
               
               DialogResult result =  MessageBox.Show("Хотите сохранить свои изменения? Редактор не пуст.", "Save Changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                
                if(result == DialogResult.Yes)
                {
                    
                    saveFileDialog1.ShowDialog();    
                    string file;
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string filename = saveFileDialog1.FileName;
                        
                        myTextBox1.SaveFile(filename, RichTextBoxStreamType.PlainText);
                        file = Path.GetFileName(filename); 
                        MessageBox.Show("File " + file + " was saved successfully.", "Save Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    
                    myTextBox1.ResetText();
                    myTextBox1.Focus();
                }
                else if(result == DialogResult.No)
                {
                    
                    myTextBox1.ResetText();
                    myTextBox1.Focus();
                }               
            }
            else 
            {
                
                myTextBox1.ResetText();
                myTextBox1.Focus();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();    // показывает диалоговое окно
            string file; 

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog1.FileName;
                // сохраняет содержимое поле форматированного текста
                myTextBox1.SaveFile(filename, RichTextBoxStreamType.PlainText);
            }
            file = Path.GetFileName(filenamee);    // получает имя файла
            MessageBox.Show("File " + file + " was saved successfully.", "Save Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void zoomDropDownButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            float zoomPercent = Convert.ToSingle(e.ClickedItem.Text.Trim('%')); 
            myTextBox1.ZoomFactor = zoomPercent / 100;    

            if(e.ClickedItem.Image == null)
            {
                
                for (int i = 0; i < zoomDropDownButton.DropDownItems.Count; i++)
                {
                    zoomDropDownButton.DropDownItems[i].Image = null;
                }

                
                Bitmap bmp = new Bitmap(5, 5);
                using (Graphics gfx = Graphics.FromImage(bmp))
                {
                    gfx.FillEllipse(Brushes.Black, 1, 1, 3, 3);
                }
                e.ClickedItem.Image = bmp; 
            }
            else
            {
                e.ClickedItem.Image = null;
                myTextBox1.ZoomFactor = 1.0f;    // устанавливает обратно значение в "NO ZOOM"
            }
        }

        private void uppercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myTextBox1.SelectedText = myTextBox1.SelectedText.ToUpper();    // текст в верхнем регистре
        }

        private void lowercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myTextBox1.SelectedText = myTextBox1.SelectedText.ToLower();    // текст в нижнем регистре
        }

        private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            Bitmap bmp = new Bitmap(5, 5);
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.FillEllipse(Brushes.Black, 1, 1, 3, 3);
            }

            if (myTextBox1.WordWrap == false)
            {
                myTextBox1.WordWrap = true;   
                wordWrapToolStripMenuItem.Image = bmp;  
            }
            else if(myTextBox1.WordWrap == true)
            {
                myTextBox1.WordWrap = false; 
                wordWrapToolStripMenuItem.Image = null; 
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                fontDialog1.ShowDialog();    // показывает диалоговое окно шрифта
                System.Drawing.Font oldFont = this.Font;    // получает текущий шрифт

                if (fontDialog1.ShowDialog() == DialogResult.OK)
                {
                    fontDialog1_Apply(myTextBox1, new System.EventArgs());
                }
                // возвращает к недавнему шрифту
                else if (fontDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    // меняет текущий шрифт обратно на старый шрифт
                    this.Font = oldFont;

                    // устанавливает старый шрифт для элементов управления внутри myTextBox1
                    foreach (Control containedControl in myTextBox1.Controls)
                    {
                        containedControl.Font = oldFont;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Information", MessageBoxButtons.OK, MessageBoxIcon.Warning); // error
            }
        }

        private void fontDialog1_HelpRequest(object sender, EventArgs e)
        {
            // отображение сообщения "HelpReques"
            MessageBox.Show("Выберите шрифт и любые другие атрибуты, затем нажмите Apply и ОК.", "Font Dialog Help Request", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void fontDialog1_Apply(object sender, EventArgs e)
        {
            fontDialog1.FontMustExist = true;    // ошибка, если шрифт не существует

            myTextBox1.Font = fontDialog1.Font;    // устанавливает выбранный шрифт (включает в себя: FontFamily, Size и Effect. Нет необходимости устанавливать их отдельно)
            myTextBox1.ForeColor = fontDialog1.Color;    // устанавливает выбранный цвет шрифта

            // устанавливает шрифт для элементов управления внутри myTextBox1
            foreach (Control containedControl in myTextBox1.Controls)
            {
                containedControl.Font = fontDialog1.Font;
            }
        }

        private void deleteStripMenuItem_Click(object sender, EventArgs e)
        {
            myTextBox1.SelectedText = ""; // удаляет выделенный текст
        }

        private void clearFormattingStripButton_Click(object sender, EventArgs e)
        {
            fontStripComboBox.Text = "Font Family";
            fontSizeComboBox.Text = "Font Size";
            string pureText = myTextBox1.Text;    // // получить текущий обычный текст     
            myTextBox1.Clear();    
            myTextBox1.ForeColor = Color.Black;
            myTextBox1.Font = default(Font);    // устанавливает шрифт по умолчанию
            myTextBox1.Text = pureText; 
            rightAlignStripButton.Checked = false;
            centerAlignStripButton.Checked = false;
            leftAlignStripButton.Checked = true;           
        }

        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // рисует строку на печатном документе
            e.Graphics.DrawString(myTextBox1.Text, myTextBox1.Font, Brushes.Black, 100, 20);
            e.Graphics.PageUnit = GraphicsUnit.Inch; 
        }

        private void printStripButton_Click(object sender, EventArgs e)
        {
            // PrintDialog ассоциируется с PrintDocument
            printDialog.Document = printDocument;
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print(); // Печать документа
            }
        }

        private void printPreviewStripButton_Click(object sender, EventArgs e)
        {
            printPreviewDialog.Document = printDocument;
            // Показывает диалоговое окно "PrintPreview"
            printPreviewDialog.ShowDialog();
        }

        private void printStripMenuItem_Click(object sender, EventArgs e)
        {
            // PrintDialog ассоциируется с PrintDocument
            printDialog.Document = printDocument;
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }

        private void printPreviewStripMenuItem_Click(object sender, EventArgs e)
        {
            printPreviewDialog.Document = printDocument;
            // Показывает диалоговое окно "PrintPreview"
            printPreviewDialog.ShowDialog();
        }

        private void colorDialog1_HelpRequest(object sender, EventArgs e)
        {
            // отображение сообщения "HelpRequest"
            MessageBox.Show("Please select a color by clicking it. This will change the text color.", "Color Dialog Help Request", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void colorOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();

            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                // устанавливает выбранный цвет
                myTextBox1.ForeColor = colorDialog1.Color;
            }
        }

        //****************************************************************************************************************************************
        // myTextBox1_KeyUp - Определяет, какая клавиша была отпущена, и получает номера строк и столбцов текущей позиции курсора *
        //**************************************************************************************************************************************** 
        private void myTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            // определяет "KeyCode"
            switch (e.KeyCode)
            {
                case Keys.Down:
                    pos = myTextBox1.SelectionStart;    // получает отправную точку
                    line = myTextBox1.GetLineFromCharIndex(pos);    // получает номер строки
                    column = myTextBox1.SelectionStart - myTextBox1.GetFirstCharIndexFromLine(line);
                    lineColumnStatusLabel.Text = "Line " + (line + 1) + ", Column " + (column + 1);
                    break;
                case Keys.Right:
                    pos = myTextBox1.SelectionStart; // получает отправную точку
                    line = myTextBox1.GetLineFromCharIndex(pos); // получает номер строки
                    column = myTextBox1.SelectionStart - myTextBox1.GetFirstCharIndexFromLine(line);
                    lineColumnStatusLabel.Text = "Line " + (line + 1) + ", Column " + (column + 1);
                    break;
                case Keys.Up:
                    pos = myTextBox1.SelectionStart; // получает отправную точку
                    line = myTextBox1.GetLineFromCharIndex(pos); // получает номер строки
                    column = myTextBox1.SelectionStart - myTextBox1.GetFirstCharIndexFromLine(line);    // получает номер столбца
                    lineColumnStatusLabel.Text = "Line " + (line + 1) + ", Column " + (column + 1);
                    break;
                case Keys.Left:
                    pos = myTextBox1.SelectionStart; // получает отправную точку
                    line = myTextBox1.GetLineFromCharIndex(pos); // получает номер строки
                    column = myTextBox1.SelectionStart - myTextBox1.GetFirstCharIndexFromLine(line);    // получает номер столбца
                    lineColumnStatusLabel.Text = "Line " + (line + 1) + ", Column " + (column + 1);
                    break;                
            }
        }

        //****************************************************************************************************************************
        // myTextBox1_MouseDown - Получает номера строк и столбцов позиции курсора при щелчке мыши по области *
        //****************************************************************************************************************************
        private void myTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int pos = myTextBox1.SelectionStart;    // получает отправную точку
            int line = myTextBox1.GetLineFromCharIndex(pos);    // получает номер строки
            int column = myTextBox1.SelectionStart - myTextBox1.GetFirstCharIndexFromLine(line);    // получает номер столбца
            lineColumnStatusLabel.Text = "Line " + (line + 1) + ", Column " + (column + 1);
        }
        
    }
}
