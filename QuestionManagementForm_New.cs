using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Comp1551_ApplicationDev_CW
{
    public partial class QuestionManagementFormNew : Form
    {
        private QuestionManagementController _controller;
        private ListBox _questionListBox = null!;
        private Button _addButton = null!, _editButton = null!, _deleteButton = null!, _closeButton = null!;

        public QuestionManagementFormNew(QuizGame quizGame)
        {
            _controller = new QuestionManagementController(quizGame);
            SetupUI();
            RefreshQuestionList();
        }

        private void SetupUI()
        {
            this.Text = "📝 Question Management";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 248, 255);

            Label titleLabel = new Label
            {
                Text = "📝 Question Management Center",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 118, 210),
                AutoSize = true,
                Location = new Point(200, 20)
            };

            _questionListBox = new ListBox
            {
                Size = new Size(450, 300),
                Location = new Point(30, 60),
                Font = new Font("Arial", 10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                SelectionMode = SelectionMode.One
            };

            _addButton = CreateStyledButton("➕ Add Question", new Point(520, 60), new Size(130, 40), 
                Color.FromArgb(46, 125, 50), Color.FromArgb(76, 175, 80));
            _addButton.Click += AddButton_Click;

            _editButton = CreateStyledButton("✏️ Edit Question", new Point(520, 120), new Size(130, 40),
                Color.FromArgb(255, 152, 0), Color.FromArgb(255, 193, 7));
            _editButton.Click += EditButton_Click;

            _deleteButton = CreateStyledButton("🗑️ Delete Question", new Point(520, 180), new Size(130, 40),
                Color.FromArgb(183, 28, 28), Color.FromArgb(244, 67, 54));
            _deleteButton.Click += DeleteButton_Click;

            _closeButton = CreateStyledButton("🚪 Close", new Point(520, 400), new Size(130, 40),
                Color.FromArgb(96, 125, 139), Color.FromArgb(120, 144, 156));
            _closeButton.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { titleLabel, _questionListBox, _addButton, _editButton, _deleteButton, _closeButton });
        }

        private Button CreateStyledButton(string text, Point location, Size size, Color darkColor, Color lightColor)
        {
            Button btn = new Button
            {
                Text = text,
                Location = location,
                Size = size,
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = darkColor,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };

            btn.MouseEnter += (s, e) => btn.BackColor = lightColor;
            btn.MouseLeave += (s, e) => btn.BackColor = darkColor;

            return btn;
        }

        private void RefreshQuestionList()
        {
            _questionListBox.Items.Clear();
            var questions = _controller.GetQuestionDisplayList();
            foreach (var question in questions)
            {
                _questionListBox.Items.Add(question);
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var form = new AddEditQuestionFormNew();
            if (form.ShowDialog() == DialogResult.OK && form.CreatedQuestion != null)
            {
                if (_controller.AddQuestion(form.CreatedQuestion))
                {
                    RefreshQuestionList();
                    MessageBox.Show("Question added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to add question.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (_questionListBox.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a question to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var existingQuestion = _controller.GetQuestion(_questionListBox.SelectedIndex);
            if (existingQuestion == null)
            {
                MessageBox.Show("Selected question not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var form = new AddEditQuestionFormNew(existingQuestion);
            if (form.ShowDialog() == DialogResult.OK && form.CreatedQuestion != null)
            {
                if (_controller.UpdateQuestion(_questionListBox.SelectedIndex, form.CreatedQuestion))
                {
                    RefreshQuestionList();
                    MessageBox.Show("Question updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to update question.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (_questionListBox.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a question to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete this question?", "Confirm Delete", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (_controller.DeleteQuestion(_questionListBox.SelectedIndex))
                {
                    RefreshQuestionList();
                    MessageBox.Show("Question deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to delete question.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    public partial class AddEditQuestionFormNew : Form
    {
        private QuestionFormController _controller;
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Question? CreatedQuestion { get; private set; }

        private ComboBox _typeCombo = null!;
        private TextBox _questionText = null!;
        private Panel _answerPanel = null!;

        public AddEditQuestionFormNew(Question? existingQuestion = null)        
        {
            _controller = new QuestionFormController();
            SetupUI();
            if (existingQuestion != null) LoadQuestion(existingQuestion);
        }

        private void SetupUI()
        {
            this.Text = "✏️ Add/Edit Question";
            this.Size = new Size(600, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(250, 250, 250);

            Label titleLabel = new Label
            {
                Text = "✏️ Create Your Geography Question",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 118, 210),
                AutoSize = true,
                Location = new Point(150, 20)
            };

            Label lblType = new Label
            {
                Text = "📋 Question Type:",
                Location = new Point(30, 70),
                AutoSize = true,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33)
            };

            _typeCombo = new ComboBox
            {
                Location = new Point(160, 68),
                Size = new Size(180, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 10),
                BackColor = Color.White
            };
            _typeCombo.Items.AddRange(_controller.GetQuestionTypes().ToArray());
            _typeCombo.SelectedIndexChanged += TypeCombo_SelectedIndexChanged;
 
            Label lblQuestion = new Label
            {
                Text = "❓ Question Text:",
                Location = new Point(30, 110),
                AutoSize = true,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33)
            };

            _questionText = new TextBox
            {
                Location = new Point(30, 135),
                Size = new Size(520, 80),
                Multiline = true,
                Font = new Font("Arial", 10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ScrollBars = ScrollBars.Vertical
            };

            Label lblAnswer = new Label
            {
                Text = "💡 Answer Configuration:",
                Location = new Point(30, 230),
                AutoSize = true,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33)
            };

            _answerPanel = new Panel
            {
                Location = new Point(30, 255),
                Size = new Size(520, 150),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            Button btnOK = CreateStyledButton("✅ Save Question", new Point(350, 430), new Size(120, 40),
                Color.FromArgb(46, 125, 50), Color.FromArgb(76, 175, 80));
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Click += BtnOK_Click;

            Button btnCancel = CreateStyledButton("❌ Cancel", new Point(480, 430), new Size(100, 40),
                Color.FromArgb(183, 28, 28), Color.FromArgb(244, 67, 54));
            btnCancel.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] { titleLabel, lblType, _typeCombo, lblQuestion, _questionText, lblAnswer, _answerPanel, btnOK, btnCancel });
        }

        private Button CreateStyledButton(string text, Point location, Size size, Color darkColor, Color lightColor)
        {
            Button btn = new Button
            {
                Text = text,
                Location = location,
                Size = size,
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = darkColor,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand
            };

            btn.MouseEnter += (s, e) => btn.BackColor = lightColor;
            btn.MouseLeave += (s, e) => btn.BackColor = darkColor;

            return btn;
        }

        private void TypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            _answerPanel.Controls.Clear();

            if (_typeCombo.SelectedItem == null) return;

            var questionType = _controller.ParseQuestionType(_typeCombo.SelectedItem.ToString()!);

            switch (questionType)
            {
                case QuestionFormController.QuestionType.MultipleChoice:
                    SetupMultipleChoicePanel();
                    break;
                case QuestionFormController.QuestionType.OpenEnded:
                    SetupOpenEndedPanel();
                    break;
                case QuestionFormController.QuestionType.TrueFalse:
                    SetupTrueFalsePanel();
                    break;
            }
        }

        private void SetupMultipleChoicePanel()
        {
            for (int i = 0; i < 4; i++)
            {
                Label lbl = new Label { Text = $"Option {i + 1}:", Location = new Point(10, 10 + i * 30), AutoSize = true };
                TextBox txt = new TextBox { Name = $"option{i}", Location = new Point(80, 8 + i * 30), Size = new Size(200, 20) };
                RadioButton rb = new RadioButton { Name = $"correct{i}", Location = new Point(290, 10 + i * 30), Text = "Correct" };

                _answerPanel.Controls.AddRange(new Control[] { lbl, txt, rb });
            }
        }

        private void SetupOpenEndedPanel()
        {
            Label lbl = new Label { Text = "Correct Answers (separate with ;):", Location = new Point(10, 10), AutoSize = true };
            TextBox txt = new TextBox { Name = "openAnswers", Location = new Point(10, 35), Size = new Size(400, 60), Multiline = true };
            Label hint = new Label { Text = "Example: London;UK;United Kingdom", Location = new Point(10, 105), AutoSize = true, ForeColor = Color.Gray };

            _answerPanel.Controls.AddRange(new Control[] { lbl, txt, hint });
        }

        private void SetupTrueFalsePanel()
        {
            RadioButton trueRb = new RadioButton { Name = "tfTrue", Text = "True", Location = new Point(10, 20), Checked = true };
            RadioButton falseRb = new RadioButton { Name = "tfFalse", Text = "False", Location = new Point(10, 50) };

            _answerPanel.Controls.AddRange(new Control[] { trueRb, falseRb });
        }

        private void LoadQuestion(Question question)
        {
            _questionText.Text = question.QuestionText;

            var data = _controller.ExtractQuestionData(question);
            _typeCombo.SelectedItem = data.Type switch
            {
                QuestionFormController.QuestionType.MultipleChoice => "Multiple Choice",
                QuestionFormController.QuestionType.OpenEnded => "Open-Ended",
                QuestionFormController.QuestionType.TrueFalse => "True/False",
                _ => null
            };

            // Load specific data based on question type
            switch (data.Type)
            {
                case QuestionFormController.QuestionType.MultipleChoice:
                    LoadMultipleChoiceData(data);
                    break;
                case QuestionFormController.QuestionType.OpenEnded:
                    LoadOpenEndedData(data);
                    break;
                case QuestionFormController.QuestionType.TrueFalse:
                    LoadTrueFalseData(data);
                    break;
            }
        }

        private void LoadMultipleChoiceData(QuestionFormController.QuestionData data)
        {
            for (int i = 0; i < 4; i++)
            {
                var textBox = _answerPanel.Controls.Find($"option{i}", false).FirstOrDefault() as TextBox;
                var radioButton = _answerPanel.Controls.Find($"correct{i}", false).FirstOrDefault() as RadioButton;

                if (textBox != null && i < data.Options.Count)
                    textBox.Text = data.Options[i];

                if (radioButton != null)
                    radioButton.Checked = (i == data.CorrectOptionIndex);
            }
        }

        private void LoadOpenEndedData(QuestionFormController.QuestionData data)
        {
            var textBox = _answerPanel.Controls.Find("openAnswers", false).FirstOrDefault() as TextBox;
            if (textBox != null)
                textBox.Text = string.Join(";", data.CorrectAnswers);
        }

        private void LoadTrueFalseData(QuestionFormController.QuestionData data)
        {
            var trueRb = _answerPanel.Controls.Find("tfTrue", false).FirstOrDefault() as RadioButton;
            var falseRb = _answerPanel.Controls.Find("tfFalse", false).FirstOrDefault() as RadioButton;

            if (trueRb != null) trueRb.Checked = data.TrueFalseAnswer;
            if (falseRb != null) falseRb.Checked = !data.TrueFalseAnswer;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                var data = CollectQuestionData();
                CreatedQuestion = _controller.CreateQuestion(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating question: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }

        private QuestionFormController.QuestionData CollectQuestionData()
        {
            var data = new QuestionFormController.QuestionData
            {
                QuestionText = _questionText.Text,
                Type = _controller.ParseQuestionType(_typeCombo.SelectedItem?.ToString() ?? "")
            };

            switch (data.Type)
            {
                case QuestionFormController.QuestionType.MultipleChoice:
                    CollectMultipleChoiceData(data);
                    break;
                case QuestionFormController.QuestionType.OpenEnded:
                    CollectOpenEndedData(data);
                    break;
                case QuestionFormController.QuestionType.TrueFalse:
                    CollectTrueFalseData(data);
                    break;
            }

            return data;
        }

        private void CollectMultipleChoiceData(QuestionFormController.QuestionData data)
        {
            for (int i = 0; i < 4; i++)
            {
                var textBox = _answerPanel.Controls.Find($"option{i}", false).FirstOrDefault() as TextBox;
                var radioButton = _answerPanel.Controls.Find($"correct{i}", false).FirstOrDefault() as RadioButton;

                data.Options.Add(textBox?.Text ?? "");
                if (radioButton?.Checked == true)
                    data.CorrectOptionIndex = i;
            }
        }

        private void CollectOpenEndedData(QuestionFormController.QuestionData data)
        {
            var textBox = _answerPanel.Controls.Find("openAnswers", false).FirstOrDefault() as TextBox;
            if (textBox != null && !string.IsNullOrWhiteSpace(textBox.Text))
            {
                data.CorrectAnswers = textBox.Text.Split(';').Select(a => a.Trim()).Where(a => !string.IsNullOrEmpty(a)).ToList();
            }
        }

        private void CollectTrueFalseData(QuestionFormController.QuestionData data)
        {
            var trueRb = _answerPanel.Controls.Find("tfTrue", false).FirstOrDefault() as RadioButton;
            data.TrueFalseAnswer = trueRb?.Checked == true;
        }
    }
}
