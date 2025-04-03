using System;
using System.Windows.Forms;

namespace SimpleCalculator
{
    public class Calculator : Form
    {
        private TextBox txtNum1, txtNum2, txtResult;
        private Button btnAdd, btnSubtract, btnMultiply, btnDivide;

        public Calculator()
        {
            this.Text = "Simple Calculator";
            this.Size = new System.Drawing.Size(300, 250);

            Label lblNum1 = new Label() { Text = "Number 1:", Left = 10, Top = 20 };
            txtNum1 = new TextBox() { Left = 100, Top = 20, Width = 150 };
            
            Label lblNum2 = new Label() { Text = "Number 2:", Left = 10, Top = 50 };
            txtNum2 = new TextBox() { Left = 100, Top = 50, Width = 150 };
            
            btnAdd = new Button() { Text = "+", Left = 10, Top = 80, Width = 50 };
            btnSubtract = new Button() { Text = "-", Left = 70, Top = 80, Width = 50 };
            btnMultiply = new Button() { Text = "*", Left = 130, Top = 80, Width = 50 };
            btnDivide = new Button() { Text = "/", Left = 190, Top = 80, Width = 50 };
            
            txtResult = new TextBox() { Left = 100, Top = 120, Width = 150, ReadOnly = true };
            Label lblResult = new Label() { Text = "Result:", Left = 10, Top = 120 };
            
            btnAdd.Click += (sender, e) => Calculate("+");
            btnSubtract.Click += (sender, e) => Calculate("-");
            btnMultiply.Click += (sender, e) => Calculate("*");
            btnDivide.Click += (sender, e) => Calculate("/");

            Controls.Add(lblNum1);
            Controls.Add(txtNum1);
            Controls.Add(lblNum2);
            Controls.Add(txtNum2);
            Controls.Add(btnAdd);
            Controls.Add(btnSubtract);
            Controls.Add(btnMultiply);
            Controls.Add(btnDivide);
            Controls.Add(lblResult);
            Controls.Add(txtResult);
        }

        private void Calculate(string operation)
        {
            if (double.TryParse(txtNum1.Text, out double num1) && double.TryParse(txtNum2.Text, out double num2))
            {
                double result = operation switch
                {
                    "+" => num1 + num2,
                    "-" => num1 - num2,
                    "*" => num1 * num2,
                    "/" => num2 != 0 ? num1 / num2 : double.NaN,
                    _ => 0
                };
                txtResult.Text = result.ToString();
            }
            else
            {
                MessageBox.Show("Please enter valid numbers", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new Calculator());
        }
    }
}
