using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Practice15_KeyboardCalculator;

public class MainForm : Form
{
    private const string BaseTitle = "Практична робота №15";
    private readonly Label lblDisplay = new();
    private double firstNumber;
    private string operation = "";
    private bool isOperationSelected;

    public MainForm()
    {
        Text = BaseTitle;
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(340, 460);
        KeyPreview = true;

        Panel pnlDisplay = new()
        {
            Dock = DockStyle.Top,
            Height = 86,
            Padding = new Padding(10)
        };

        lblDisplay.Text = "0";
        lblDisplay.Dock = DockStyle.Fill;
        lblDisplay.BackColor = Color.White;
        lblDisplay.TextAlign = ContentAlignment.MiddleRight;
        lblDisplay.Font = new Font("Segoe UI", 24, FontStyle.Bold);
        lblDisplay.BorderStyle = BorderStyle.FixedSingle;
        pnlDisplay.Controls.Add(lblDisplay);

        TableLayoutPanel table = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 5,
            Padding = new Padding(10)
        };

        for (int i = 0; i < 4; i++)
        {
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        }

        for (int i = 0; i < 5; i++)
        {
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
        }

        AddButton(table, "CE", 0, 0, Clear_Click);
        AddButton(table, "C", 1, 0, Clear_Click);
        AddButton(table, "/", 2, 0, Operation_Click);
        AddButton(table, "*", 3, 0, Operation_Click);
        AddButton(table, "7", 0, 1, Number_Click);
        AddButton(table, "8", 1, 1, Number_Click);
        AddButton(table, "9", 2, 1, Number_Click);
        AddButton(table, "-", 3, 1, Operation_Click);
        AddButton(table, "4", 0, 2, Number_Click);
        AddButton(table, "5", 1, 2, Number_Click);
        AddButton(table, "6", 2, 2, Number_Click);
        AddButton(table, "+", 3, 2, Operation_Click);
        AddButton(table, "1", 0, 3, Number_Click);
        AddButton(table, "2", 1, 3, Number_Click);
        AddButton(table, "3", 2, 3, Number_Click);
        AddButton(table, "=", 3, 3, Equal_Click);
        AddButton(table, "0", 0, 4, Number_Click, 2);
        AddButton(table, ".", 2, 4, Number_Click);
        AddButton(table, "+/-", 3, 4, ChangeSign_Click);

        Controls.Add(table);
        Controls.Add(pnlDisplay);
        MouseMove += Form_MouseMove;
        MouseClick += Form_MouseClick;
        KeyDown += Form_KeyDown;
    }

    private static void AddButton(TableLayoutPanel table, string text, int column, int row, EventHandler handler, int columnSpan = 1)
    {
        Button button = new()
        {
            Text = text,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Margin = new Padding(4)
        };
        button.Click += handler;
        table.Controls.Add(button, column, row);
        if (columnSpan > 1)
        {
            table.SetColumnSpan(button, columnSpan);
        }
    }

    private void Number_Click(object? sender, EventArgs e)
    {
        string symbol = ((Button)sender!).Text;

        if (lblDisplay.Text == "0" || isOperationSelected)
        {
            lblDisplay.Text = "";
            isOperationSelected = false;
        }

        if (symbol == "." && lblDisplay.Text.Contains('.'))
        {
            return;
        }

        lblDisplay.Text += symbol;
    }

    private void Operation_Click(object? sender, EventArgs e)
    {
        firstNumber = GetDisplayNumber();
        operation = ((Button)sender!).Text;
        isOperationSelected = true;
    }

    private void Equal_Click(object? sender, EventArgs e)
    {
        double secondNumber = GetDisplayNumber();
        double result;

        switch (operation)
        {
            case "+":
                result = firstNumber + secondNumber;
                break;
            case "-":
                result = firstNumber - secondNumber;
                break;
            case "*":
                result = firstNumber * secondNumber;
                break;
            case "/":
                if (Math.Abs(secondNumber) < double.Epsilon)
                {
                    MessageBox.Show("Ділення на нуль заборонено.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                result = firstNumber / secondNumber;
                break;
            default:
                return;
        }

        lblDisplay.Text = result.ToString(CultureInfo.InvariantCulture);
        operation = "";
        isOperationSelected = true;
    }

    private void Clear_Click(object? sender, EventArgs e)
    {
        lblDisplay.Text = "0";
        firstNumber = 0;
        operation = "";
        isOperationSelected = false;
    }

    private void ChangeSign_Click(object? sender, EventArgs e)
    {
        double value = GetDisplayNumber();
        lblDisplay.Text = (-value).ToString(CultureInfo.InvariantCulture);
    }

    private void Form_MouseMove(object? sender, MouseEventArgs e)
    {
        Text = $"{BaseTitle} X={e.X}, Y={e.Y}";
    }

    private void Form_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            MessageBox.Show($"Координати: ({e.X}; {e.Y})\nКнопка: {e.Button}", "Подія миші");
        }
    }

    private void Form_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
        {
            AppendKeyboardNumber((char)('0' + e.KeyCode - Keys.D0));
        }
        else if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)
        {
            AppendKeyboardNumber((char)('0' + e.KeyCode - Keys.NumPad0));
        }
        else if (e.KeyCode == Keys.Decimal || e.KeyCode == Keys.OemPeriod)
        {
            AppendKeyboardNumber('.');
        }
        else if (e.KeyCode == Keys.Add || (e.Shift && e.KeyCode == Keys.Oemplus))
        {
            SelectKeyboardOperation("+");
        }
        else if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus)
        {
            SelectKeyboardOperation("-");
        }
        else if (e.KeyCode == Keys.Multiply)
        {
            SelectKeyboardOperation("*");
        }
        else if (e.KeyCode == Keys.Divide || e.KeyCode == Keys.OemQuestion)
        {
            SelectKeyboardOperation("/");
        }
        else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Oemplus)
        {
            Equal_Click(this, EventArgs.Empty);
        }
        else if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Escape)
        {
            Clear_Click(this, EventArgs.Empty);
        }
    }

    private void AppendKeyboardNumber(char symbol)
    {
        if (lblDisplay.Text == "0" || isOperationSelected)
        {
            lblDisplay.Text = "";
            isOperationSelected = false;
        }

        if (symbol == '.' && lblDisplay.Text.Contains('.'))
        {
            return;
        }

        lblDisplay.Text += symbol;
    }

    private void SelectKeyboardOperation(string selectedOperation)
    {
        firstNumber = GetDisplayNumber();
        operation = selectedOperation;
        isOperationSelected = true;
    }

    private double GetDisplayNumber()
    {
        return double.TryParse(lblDisplay.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
            ? value
            : 0;
    }
}
